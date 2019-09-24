Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ObjectDetection.YoloParser
	Friend Class YoloOutputParser
		Private Class CellDimensions
			Inherits DimensionsBase

		End Class

		Public Const ROW_COUNT As Integer = 13
		Public Const COL_COUNT As Integer = 13
		Public Const CHANNEL_COUNT As Integer = 125
		Public Const BOXES_PER_CELL As Integer = 5
		Public Const BOX_INFO_FEATURE_COUNT As Integer = 5
		Public Const CLASS_COUNT As Integer = 20
		Public Const CELL_WIDTH As Single = 32
		Public Const CELL_HEIGHT As Single = 32

		Private channelStride As Integer = ROW_COUNT * COL_COUNT

		Private anchors() As Single = { 1.08F, 1.19F, 3.42F, 4.41F, 6.63F, 11.38F, 9.42F, 5.11F, 16.62F, 10.52F }

		Private labels() As String = { "aeroplane", "bicycle", "bird", "boat", "bottle", "bus", "car", "cat", "chair", "cow", "diningtable", "dog", "horse", "motorbike", "person", "pottedplant", "sheep", "sofa", "train", "tvmonitor" }

		Private Shared classColors() As Color = { Color.Khaki, Color.Fuchsia, Color.Silver, Color.RoyalBlue, Color.Green, Color.DarkOrange, Color.Purple, Color.Gold, Color.Red, Color.Aquamarine, Color.Lime, Color.AliceBlue, Color.Sienna, Color.Orchid, Color.Tan, Color.LightPink, Color.Yellow, Color.HotPink, Color.OliveDrab, Color.SandyBrown, Color.DarkTurquoise }

		Private Function Sigmoid(value As Single) As Single
			Dim k = CSng(Math.Exp(value))
			Return k / (1.0F + k)
		End Function

		Private Function Softmax(values() As Single) As Single()
			Dim maxVal = values.Max()
			Dim exp = values.Select(Function(v) Math.Exp(v - maxVal))
			Dim sumExp = exp.Sum()

'INSTANT VB WARNING: Instant VB cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Return exp.Select(Function(v) CSng(v / sumExp)).ToArray()
		End Function

		Private Function GetOffset(x As Integer, y As Integer, channel As Integer) As Integer
			' YOLO outputs a tensor that has a shape of 125x13x13, which 
			' WinML flattens into a 1D array.  To access a specific channel 
			' for a given (x,y) cell position, we need to calculate an offset
			' into the array
			Return (channel * Me.channelStride) + (y * COL_COUNT) + x
		End Function

		Private Function ExtractBoundingBoxDimensions(modelOutput() As Single, x As Integer, y As Integer, channel As Integer) As BoundingBoxDimensions
			Return New BoundingBoxDimensions With {
				.X = modelOutput(GetOffset(x, y, channel)),
				.Y = modelOutput(GetOffset(x, y, channel + 1)),
				.Width = modelOutput(GetOffset(x, y, channel + 2)),
				.Height = modelOutput(GetOffset(x, y, channel + 3))
			}
		End Function

		Private Function GetConfidence(modelOutput() As Single, x As Integer, y As Integer, channel As Integer) As Single
			Return Sigmoid(modelOutput(GetOffset(x, y, channel + 4)))
		End Function

		Private Function MapBoundingBoxToCell(x As Integer, y As Integer, box As Integer, boxDimensions As BoundingBoxDimensions) As CellDimensions
			Return New CellDimensions With {
				.X = (CSng(x) + Sigmoid(boxDimensions.X)) * CELL_WIDTH,
				.Y = (CSng(y) + Sigmoid(boxDimensions.Y)) * CELL_HEIGHT,
				.Width = CSng(Math.Exp(boxDimensions.Width)) * CELL_WIDTH * anchors(box * 2),
				.Height = CSng(Math.Exp(boxDimensions.Height)) * CELL_HEIGHT * anchors(box * 2 + 1)
			}
		End Function

		Public Function ExtractClasses(modelOutput() As Single, x As Integer, y As Integer, channel As Integer) As Single()
			Dim predictedClasses(CLASS_COUNT - 1) As Single
			Dim predictedClassOffset As Integer = channel + BOX_INFO_FEATURE_COUNT
			For predictedClass As Integer = 0 To CLASS_COUNT - 1
				predictedClasses(predictedClass) = modelOutput(GetOffset(x, y, predictedClass + predictedClassOffset))
			Next predictedClass
			Return Softmax(predictedClasses)
		End Function

		Private Function GetTopResult(predictedClasses() As Single) As ValueTuple(Of Integer, Single)
			Return predictedClasses.Select(Function(predictedClass, index) (Index:= index, Value:= predictedClass)).OrderByDescending(Function(result) result.Value).First()
		End Function

		Private Function IntersectionOverUnion(boundingBoxA As RectangleF, boundingBoxB As RectangleF) As Single
			Dim areaA = boundingBoxA.Width * boundingBoxA.Height

			If areaA <= 0 Then
				Return 0
			End If

			Dim areaB = boundingBoxB.Width * boundingBoxB.Height

			If areaB <= 0 Then
				Return 0
			End If

			Dim minX = Math.Max(boundingBoxA.Left, boundingBoxB.Left)
			Dim minY = Math.Max(boundingBoxA.Top, boundingBoxB.Top)
			Dim maxX = Math.Min(boundingBoxA.Right, boundingBoxB.Right)
			Dim maxY = Math.Min(boundingBoxA.Bottom, boundingBoxB.Bottom)

			Dim intersectionArea = Math.Max(maxY - minY, 0) * Math.Max(maxX - minX, 0)

			Return intersectionArea / (areaA + areaB - intersectionArea)
		End Function

		Public Function ParseOutputs(yoloModelOutputs() As Single, Optional threshold As Single = .3F) As IList(Of YoloBoundingBox)
			Dim boxes = New List(Of YoloBoundingBox)

			For row As Integer = 0 To ROW_COUNT - 1
				For column As Integer = 0 To COL_COUNT - 1
					For box As Integer = 0 To BOXES_PER_CELL - 1
						Dim channel = (box * (CLASS_COUNT + BOX_INFO_FEATURE_COUNT))

						Dim boundingBoxDimensions As BoundingBoxDimensions = ExtractBoundingBoxDimensions(yoloModelOutputs, row, column, channel)

						Dim confidence As Single = GetConfidence(yoloModelOutputs, row, column, channel)

						Dim mappedBoundingBox As CellDimensions = MapBoundingBoxToCell(row, column, box, boundingBoxDimensions)

						If confidence < threshold Then
							Continue For
						End If

						Dim predictedClasses() As Single = ExtractClasses(yoloModelOutputs, row, column, channel)

'INSTANT VB TODO TASK: VB has no equivalent to C# deconstruction declarations:
						var(topResultIndex, topResultScore) = GetTopResult(predictedClasses)
						Dim topScore = topResultScore * confidence

						If topScore < threshold Then
							Continue For
						End If

						boxes.Add(New YoloBoundingBox With {
							.Dimensions = New BoundingBoxDimensions With {
								.X = (mappedBoundingBox.X - mappedBoundingBox.Width \ 2),
								.Y = (mappedBoundingBox.Y - mappedBoundingBox.Height \ 2),
								.Width = mappedBoundingBox.Width,
								.Height = mappedBoundingBox.Height
							},
							.Confidence = topScore,
							.Label = labels(topResultIndex),
							.BoxColor = classColors(topResultIndex)
						})
					Next box
				Next column
			Next row
			Return boxes
		End Function

		Public Function FilterBoundingBoxes(boxes As IList(Of YoloBoundingBox), limit As Integer, threshold As Single) As IList(Of YoloBoundingBox)
			Dim activeCount = boxes.Count
			Dim isActiveBoxes = New Boolean(boxes.Count - 1){}

			For i As Integer = 0 To isActiveBoxes.Length - 1
				isActiveBoxes(i) = True
			Next i

			Dim sortedBoxes = boxes.Select(Function(b, i) New With {
				Key .Box = b,
				Key .Index = i
			}).OrderByDescending(Function(b) b.Box.Confidence).ToList()

			Dim results = New List(Of YoloBoundingBox)

			For i As Integer = 0 To boxes.Count - 1
				If isActiveBoxes(i) Then
					Dim boxA = sortedBoxes(i).Box
					results.Add(boxA)

					If results.Count >= limit Then
						Exit For
					End If

					For j = i + 1 To boxes.Count - 1
						If isActiveBoxes(j) Then
							Dim boxB = sortedBoxes(j).Box

							If IntersectionOverUnion(boxA.Rect, boxB.Rect) > threshold Then
								isActiveBoxes(j) = False
								activeCount -= 1

								If activeCount <= 0 Then
									Exit For
								End If
							End If
						End If
					Next j

					If activeCount <= 0 Then
						Exit For
					End If
				End If
			Next i
			Return results
		End Function

	End Class
End Namespace