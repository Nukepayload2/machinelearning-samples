Imports System.IO
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.ML
Imports ObjectDetection.YoloParser
Imports ObjectDetection.DataStructures

Namespace ObjectDetection
    Friend Class Program
        Public Shared Sub Main()
            Dim assetsRelativePath = "../../../assets"
            Dim assetsPath As String = GetAbsolutePath(assetsRelativePath)
            Dim modelFilePath = Path.Combine(assetsPath, "Model", "TinyYolo2_model.onnx")
            Dim imagesFolder = Path.Combine(assetsPath, "images")
            Dim outputFolder = Path.Combine(assetsPath, "images", "output")

            ' Initialize MLContext
            Dim mlContext As MLContext = New MLContext

            Try
                ' Load Data
                Dim images As IEnumerable(Of ImageNetData) = ImageNetData.ReadFromFile(imagesFolder)
                Dim imageDataView As IDataView = mlContext.Data.LoadFromEnumerable(images)

                ' Create instance of model scorer
                Dim modelScorer = New OnnxModelScorer(imagesFolder, modelFilePath, mlContext)

                ' Use model to score data
                Dim probabilities As IEnumerable(Of Single()) = modelScorer.Score(imageDataView)

                ' Post-process model output
                Dim parser As YoloOutputParser = New YoloOutputParser

                Dim boundingBoxes = probabilities.Select(Function(probability) parser.ParseOutputs(probability)).Select(Function(boxes) parser.FilterBoundingBoxes(boxes, 5, .5F))

                ' Draw bounding boxes for detected objects in each of the images
                For i = 0 To images.Count() - 1
                    Dim imageFileName As String = images.ElementAt(i).Label
                    Dim detectedObjects As IList(Of YoloBoundingBox) = boundingBoxes.ElementAt(i)

                    DrawBoundingBox(imagesFolder, outputFolder, imageFileName, detectedObjects)

                    LogDetectedObjects(imageFileName, detectedObjects)
                Next i
            Catch ex As Exception
                Console.WriteLine(ex.ToString())
            End Try

            Console.WriteLine("========= End of Process..Hit any Key ========")
            Console.ReadLine()
        End Sub

        Public Shared Function GetAbsolutePath(relativePath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

            Return fullPath
        End Function

        Private Shared Sub DrawBoundingBox(inputImageLocation As String, outputImageLocation As String, imageName As String, filteredBoundingBoxes As IList(Of YoloBoundingBox))
            Dim image As Image = Image.FromFile(Path.Combine(inputImageLocation, imageName))

            Dim originalImageHeight = image.Height
            Dim originalImageWidth = image.Width

            For Each box In filteredBoundingBoxes
                ' Get Bounding Box Dimensions
                Dim x = CUInt(Math.Truncate(Math.Max(box.Dimensions.X, 0)))
                Dim y = CUInt(Math.Truncate(Math.Max(box.Dimensions.Y, 0)))
                Dim width = CUInt(Math.Min(originalImageWidth - x, box.Dimensions.Width))
                Dim height = CUInt(Math.Min(originalImageHeight - y, box.Dimensions.Height))

                ' Resize To Image
                x = CUInt(originalImageWidth) * x \ OnnxModelScorer.ImageNetSettings.imageWidth
                y = CUInt(originalImageHeight) * y \ OnnxModelScorer.ImageNetSettings.imageHeight
                width = CUInt(originalImageWidth) * width \ OnnxModelScorer.ImageNetSettings.imageWidth
                height = CUInt(originalImageHeight) * height \ OnnxModelScorer.ImageNetSettings.imageHeight

                ' Bounding Box Text
                Dim text As String = $"{box.Label} ({(box.Confidence * 100).ToString("0")}%)"

                Using thumbnailGraphic As Graphics = Graphics.FromImage(image)
                    thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality
                    thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality
                    thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic

                    ' Define Text Options
                    Dim drawFont As New Font("Arial", 12, FontStyle.Bold)
                    Dim size As SizeF = thumbnailGraphic.MeasureString(text, drawFont)
                    Dim fontBrush As New SolidBrush(Color.Black)
                    Dim atPoint As New Point(CInt(x), CInt(y) - CInt(size.Height) - 1)

                    ' Define BoundingBox options
                    Dim pen As New Pen(box.BoxColor, 3.2F)
                    Dim colorBrush As New SolidBrush(box.BoxColor)

                    ' Draw text on image 
                    thumbnailGraphic.FillRectangle(colorBrush, CInt(x), CInt(y - size.Height - 1), CInt(size.Width), CInt(size.Height))
                    thumbnailGraphic.DrawString(text, drawFont, fontBrush, atPoint)

                    ' Draw bounding box on image
                    thumbnailGraphic.DrawRectangle(pen, x, y, width, height)
                End Using
            Next box

            If Not Directory.Exists(outputImageLocation) Then
                Directory.CreateDirectory(outputImageLocation)
            End If

            image.Save(Path.Combine(outputImageLocation, imageName))
        End Sub

        Private Shared Sub LogDetectedObjects(imageName As String, boundingBoxes As IList(Of YoloBoundingBox))
            Console.WriteLine($".....The objects in the image {imageName} are detected as below....")

            For Each box In boundingBoxes
                Console.WriteLine($"{box.Label} and its Confidence score: {box.Confidence}")
            Next box

            Console.WriteLine("")
        End Sub
    End Class
End Namespace



