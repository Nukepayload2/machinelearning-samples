Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Linq
Imports Microsoft.ML

Namespace ObjectDetection
	Friend Class OnnxModelScorer
		Private ReadOnly imagesFolder As String
		Private ReadOnly modelLocation As String
		Private ReadOnly mlContext As MLContext

		Private _boundingBoxes As IList(Of YoloBoundingBox) = New List(Of YoloBoundingBox)
		Private ReadOnly _parser As YoloWinMlParser = New YoloWinMlParser

		Public Sub New(imagesFolder As String, modelLocation As String)
			Me.imagesFolder = imagesFolder
			Me.modelLocation = modelLocation
			mlContext = New MLContext
		End Sub

		Public Structure ImageNetSettings
			Public Const imageHeight As Integer = 416
			Public Const imageWidth As Integer = 416
		End Structure

		Public Structure TinyYoloModelSettings
			' for checking TIny yolo2 Model input and  output  parameter names,
			'you can use tools like Netron, 
			' which is installed by Visual Studio AI Tools

			' input tensor name
			Public Const ModelInput As String = "image"

			' output tensor name
			Public Const ModelOutput As String = "grid"
		End Structure

		Public Sub Score()
			Dim model = LoadModel(modelLocation)

			PredictDataUsingModel(imagesFolder, model)
		End Sub

		Private Function LoadModel(modelLocation As String) As PredictionEngine(Of ImageNetData, ImageNetPrediction)
			Console.WriteLine("Read model")
			Console.WriteLine($"Model location: {modelLocation}")
			Console.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight})")

			Dim data = CreateEmptyDataView()

			Dim pipeline = mlContext.Transforms.LoadImages(outputColumnName:= "image", imageFolder:= "", inputColumnName:= NameOf(ImageNetData.ImagePath)).Append(mlContext.Transforms.ResizeImages(outputColumnName:= "image", imageWidth:= ImageNetSettings.imageWidth, imageHeight:= ImageNetSettings.imageHeight, inputColumnName:= "image")).Append(mlContext.Transforms.ExtractPixels(outputColumnName:= "image")).Append(mlContext.Transforms.ApplyOnnxModel(modelFile:= modelLocation, outputColumnNames:= { TinyYoloModelSettings.ModelOutput }, inputColumnNames:= { TinyYoloModelSettings.ModelInput }))

			Dim model = pipeline.Fit(data)

			Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of ImageNetData, ImageNetPrediction)(model)

			Return predictionEngine
		End Function

		Protected Sub PredictDataUsingModel(imagesFolder As String, model As PredictionEngine(Of ImageNetData, ImageNetPrediction))
			Console.WriteLine($"Images location: {imagesFolder}")
			Console.WriteLine("")
			Console.WriteLine("=====Identify the objects in the images=====")
			Console.WriteLine("")

			Dim testData = GetImagesData(imagesFolder)

			For Each sample In testData
				Dim probs = model.Predict(sample).PredictedLabels
				_boundingBoxes = _parser.ParseOutputs(probs)
				Dim filteredBoxes = _parser.FilterBoundingBoxes(_boundingBoxes, 5, .5F)


				Dim outputDirectory = Path.Combine(Directory.GetParent(sample.ImagePath).FullName, "output")
				Dim filename = (New FileInfo(sample.ImagePath)).Name

				DrawBoundingBox(imagesFolder, outputDirectory, filename, filteredBoxes)

				Console.WriteLine(".....The objects in the image {0} are detected as below....", sample.Label)
				For Each box In filteredBoxes
					Console.WriteLine(box.Label & " and its Confidence score: " & box.Confidence)
				Next box
				Console.WriteLine("")
			Next sample
		End Sub

		Private Shared Function GetImagesData(folder As String) As IEnumerable(Of ImageNetData)
			Dim imagesList As New List(Of ImageNetData)
			Dim filePaths() As String = Directory.GetFiles(folder).Where(Function(filePath) Path.GetExtension(filePath) <> ".md").ToArray()
			For Each filePath In filePaths
				Dim imagedata As ImageNetData = New ImageNetData With {
					.ImagePath = filePath,
					.Label = Path.GetFileName(filePath)
				}
				imagesList.Add(imagedata)
			Next filePath
			Return imagesList
		End Function

		Private Function CreateEmptyDataView() As IDataView
			'Create empty DataView. We just need the schema to call fit()
			Dim list As New List(Of ImageNetData)
			Dim enumerableData As IEnumerable(Of ImageNetData) = list
			Dim dv = mlContext.Data.LoadFromEnumerable(enumerableData)
			Return dv
		End Function

		Public Sub DrawBoundingBox(inputImageLocation As String, outputImageLocation As String, imageName As String, filteredBoundingBoxes As IList(Of YoloBoundingBox))
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
				x = CUInt(originalImageWidth) * x \ 416
				y = CUInt(originalImageHeight) * y \ 416
				width = CUInt(originalImageWidth) * width \ 416
				height = CUInt(originalImageHeight) * height \ 416

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
	End Class
End Namespace

