Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Threading

Namespace OnnxObjectDetection
    Public Class OnnxOutputParser
        Private Class BoundingBoxPrediction
            Inherits BoundingBoxDimensions

            Public Property Confidence As Single
        End Class

        ' The number of rows and columns in the grid the image is divided into.
        Public Const rowCount As Integer = 13, columnCount As Integer = 13

        ' The number of features contained within a box (x, y, height, width, confidence).
        Public Const featuresPerBox As Integer = 5

        ' Labels corresponding to the classes the onnx model can predict. For example, the 
        ' Tiny YOLOv2 model included with this sample is trained to predict 20 different classes.
        Private ReadOnly classLabels() As String

        ' Predetermined anchor offsets for the bounding boxes in a cell.
        Private ReadOnly boxAnchors As (x As Single, y As Single)()


        'INSTANT VB WARNING: The following constructor is declared outside of its associated class:
        'ORIGINAL LINE: public OnnxOutputParser(IOnnxModel onnxModel)
        Public Sub New(onnxModel As IOnnxModel)
            classLabels = onnxModel.Labels
            boxAnchors = onnxModel.Anchors
        End Sub

        ' Applies the sigmoid function that outputs a number between 0 and 1.
        Private Function Sigmoid(value As Single) As Single
            Dim k = MathF.Exp(value)
            Return k / (1.0F + k)
        End Function

        ' Normalizes an input vector into a probability distribution.
        Private Function Softmax(classProbabilities() As Single) As Single()
            Dim max = classProbabilities.Max()
            Dim exp = classProbabilities.Select(Function(v) MathF.Exp(v - max))
            Dim sum = exp.Sum()
            'INSTANT VB WARNING: Instant VB cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
            Return exp.Select(Function(v) v / sum).ToArray()
        End Function

        ' Onnx outputst a tensor that has a shape of (for Tiny YOLOv2) 125x13x13. ML.NET flattens
        ' this multi-dimensional into a one-dimensional array. This method allows us to access a 
        ' specific channel for a givin (x,y) cell position by calculating the offset into the array.
        Private Function GetOffset(row As Integer, column As Integer, channel As Integer) As Integer
            Const channelStride As Integer = rowCount * columnCount
            Return (channel * channelStride) + (column * columnCount) + row
        End Function

        ' Extracts the bounding box features (x, y, height, width, confidence) method from the model
        ' output. The confidence value states how sure the model is that it has detected an object. 
        ' We use the Sigmoid function to turn it that confidence into a percentage.
        Private Function ExtractBoundingBoxPrediction(modelOutput() As Single, row As Integer, column As Integer, channel As Integer) As BoundingBoxPrediction
            'INSTANT VB TODO TASK: The following line contains an assignment within expression that was not extracted by Instant VB:
            'ORIGINAL LINE: return new BoundingBoxPrediction { X = modelOutput[GetOffset(row, column, channel++)], Y = modelOutput[GetOffset(row, column, channel++)], Width = modelOutput[GetOffset(row, column, channel++)], Height = modelOutput[GetOffset(row, column, channel++)], Confidence = Sigmoid(modelOutput[GetOffset(row, column, channel++)]) };
            'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
            Dim tempVar = New BoundingBoxPrediction With {
                .X = modelOutput(GetOffset(row, column, Interlocked.Increment(channel))),
                .Y = modelOutput(GetOffset(row, column, Interlocked.Increment(channel))),
                .Width = modelOutput(GetOffset(row, column, Interlocked.Increment(channel))),
                .Height = modelOutput(GetOffset(row, column, Interlocked.Increment(channel))),
                .Confidence = Sigmoid(modelOutput(GetOffset(row, column, channel)))
            }
            Return tempVar
        End Function

        ' The predicted x and y coordinates are relative to the location of the grid cell; we use 
        ' the logistic sigmoid to constrain these coordinates to the range 0 - 1. Then we add the
        ' cell coordinates (0-12) and multiply by the number of pixels per grid cell (32).
        ' Now x/y represent the center of the bounding box in the original 416x416 image space.
        ' Additionally, the size (width, height) of the bounding box is predicted relative to the
        ' size of an "anchor" box. So we transform the width/weight into the original 416x416 image space.
        Private Function MapBoundingBoxToCell(row As Integer, column As Integer, box As Integer, boxDimensions As BoundingBoxPrediction) As BoundingBoxDimensions
            'INSTANT VB WARNING: Instant VB cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
            Const cellWidth As Single = ImageSettings.imageWidth / columnCount
            'INSTANT VB WARNING: Instant VB cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
            Const cellHeight As Single = ImageSettings.imageHeight / rowCount

            Dim mappedBox = New BoundingBoxDimensions With {
                .X = (row + Sigmoid(boxDimensions.X)) * cellWidth,
                .Y = (column + Sigmoid(boxDimensions.Y)) * cellHeight,
                .Width = MathF.Exp(boxDimensions.Width) * cellWidth * boxAnchors(box).x,
                .Height = MathF.Exp(boxDimensions.Height) * cellHeight * boxAnchors(box).y
            }

            ' The x,y coordinates from the (mapped) bounding box prediction represent the center
            ' of the bounding box. We adjust them here to represent the top left corner.
            mappedBox.X -= mappedBox.Width \ 2
            mappedBox.Y -= mappedBox.Height \ 2

            Return mappedBox
        End Function

        ' Extracts the class predictions for the bounding box from the model output using the
        ' GetOffset method and turns them into a probability distribution using the Softmax method.
        Public Function ExtractClassProbabilities(modelOutput() As Single, row As Integer, column As Integer, channel As Integer, confidence As Single) As Single()
            Dim classProbabilitiesOffset = channel + featuresPerBox
            Dim classProbabilities(classLabels.Length - 1) As Single
            For classProbability As Integer = 0 To classLabels.Length - 1
                classProbabilities(classProbability) = modelOutput(GetOffset(row, column, classProbability + classProbabilitiesOffset))
            Next classProbability
            Return Softmax(classProbabilities).Select(Function(p) p * confidence).ToArray()
        End Function

        ' IoU (Intersection over union) measures the overlap between 2 boundaries. We use that to
        ' measure how much our predicted boundary overlaps with the ground truth (the real object
        ' boundary). In some datasets, we predefine an IoU threshold (say 0.5) in classifying
        ' whether the prediction is a true positive or a false positive. This method filters
        ' overlapping bounding boxes with lower probabilities.
        Private Function IntersectionOverUnion(boundingBoxA As RectangleF, boundingBoxB As RectangleF) As Single
            Dim areaA = boundingBoxA.Width * boundingBoxA.Height
            Dim areaB = boundingBoxB.Width * boundingBoxB.Height

            If areaA <= 0 OrElse areaB <= 0 Then
                Return 0
            End If

            Dim minX = MathF.Max(boundingBoxA.Left, boundingBoxB.Left)
            Dim minY = MathF.Max(boundingBoxA.Top, boundingBoxB.Top)
            Dim maxX = MathF.Min(boundingBoxA.Right, boundingBoxB.Right)
            Dim maxY = MathF.Min(boundingBoxA.Bottom, boundingBoxB.Bottom)

            Dim intersectionArea = MathF.Max(maxY - minY, 0) * MathF.Max(maxX - minX, 0)

            Return intersectionArea / (areaA + areaB - intersectionArea)
        End Function

        Public Function ParseOutputs(modelOutput() As Single, Optional probabilityThreshold As Single = 0.3F) As List(Of BoundingBox)
            Dim boxes = New List(Of BoundingBox)

            For row As Integer = 0 To rowCount - 1
                For column As Integer = 0 To columnCount - 1
                    For box As Integer = 0 To boxAnchors.Length - 1
                        Dim channel = box * (classLabels.Length + featuresPerBox)

                        Dim boundingBoxPrediction = ExtractBoundingBoxPrediction(modelOutput, row, column, channel)

                        Dim mappedBoundingBox = MapBoundingBoxToCell(row, column, box, boundingBoxPrediction)

                        If boundingBoxPrediction.Confidence < probabilityThreshold Then
                            Continue For
                        End If

                        Dim classProbabilities() As Single = ExtractClassProbabilities(modelOutput, row, column, channel, boundingBoxPrediction.Confidence)

                        'INSTANT VB TODO TASK: VB has no equivalent to C# deconstruction declarations:
                        With classProbabilities.Select(Function(probability, index) (Score:=probability, index:=index)).Max()
                            Dim topProbability = .Score, topIndex = .index
                            If topProbability < probabilityThreshold Then
                                Continue For
                            End If

                            boxes.Add(New BoundingBox With {
                                .Dimensions = mappedBoundingBox,
                                .Confidence = topProbability,
                                .Label = classLabels(topIndex),
                                .BoxColor = BoundingBox.GetColor(topIndex)
                            })
                        End With
                    Next box
                Next column
            Next row
            Return boxes
        End Function

        Public Function FilterBoundingBoxes(boxes As List(Of BoundingBox), limit As Integer, iouThreshold As Single) As List(Of BoundingBox)
            Dim results = New List(Of BoundingBox)
            Dim filteredBoxes = New Boolean(boxes.Count - 1) {}
            Dim sortedBoxes = boxes.OrderByDescending(Function(b) b.Confidence).ToArray()

            For i As Integer = 0 To boxes.Count - 1
                If filteredBoxes(i) Then
                    Continue For
                End If

                results.Add(sortedBoxes(i))

                If results.Count >= limit Then
                    Exit For
                End If

                For j = i + 1 To boxes.Count - 1
                    If filteredBoxes(j) Then
                        Continue For
                    End If

                    If IntersectionOverUnion(sortedBoxes(i).Rect, sortedBoxes(j).Rect) > iouThreshold Then
                        filteredBoxes(j) = True
                    End If

                    If filteredBoxes.Count(Function(b) b) <= 0 Then
                        Exit For
                    End If
                Next j
            Next i
            Return results
        End Function

    End Class
End Namespace