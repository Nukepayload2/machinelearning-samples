Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.ML

Namespace ObjectDetection
	Friend Class OnnxModelScorer
		Private ReadOnly imagesLocation As String
		Private ReadOnly imagesFolder As String
		Private ReadOnly modelLocation As String
		Private ReadOnly mlContext As MLContext

		Private _boxes As IList(Of YoloBoundingBox) = New List(Of YoloBoundingBox)
		Private ReadOnly _parser As YoloWinMlParser = New YoloWinMlParser

		Public Sub New(imagesLocation As String, imagesFolder As String, modelLocation As String)
			Me.imagesLocation = imagesLocation
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
			Dim model = LoadModel(imagesFolder, modelLocation)

			PredictDataUsingModel(imagesLocation, imagesFolder, model)
		End Sub

		Private Function LoadModel(imagesFolder As String, modelLocation As String) As PredictionEngine(Of ImageNetData, ImageNetPrediction)
			Console.WriteLine("Read model")
			Console.WriteLine($"Model location: {modelLocation}")
			Console.WriteLine($"Images folder: {imagesFolder}")
			Console.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight})")

			Dim data = mlContext.Data.LoadFromTextFile(Of ImageNetData)(imagesLocation, hasHeader:= True)

			Dim pipeline = mlContext.Transforms.LoadImages(outputColumnName:= "image", imageFolder:= imagesFolder, inputColumnName:= NameOf(ImageNetData.ImagePath)).Append(mlContext.Transforms.ResizeImages(outputColumnName:= "image", imageWidth:= ImageNetSettings.imageWidth, imageHeight:= ImageNetSettings.imageHeight, inputColumnName:= "image")).Append(mlContext.Transforms.ExtractPixels(outputColumnName:= "image")).Append(mlContext.Transforms.ApplyOnnxModel(modelFile:= modelLocation, outputColumnNames:= { TinyYoloModelSettings.ModelOutput }, inputColumnNames:= { TinyYoloModelSettings.ModelInput }))

			Dim model = pipeline.Fit(data)

			Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of ImageNetData, ImageNetPrediction)(model)

			Return predictionEngine
		End Function

		Protected Sub PredictDataUsingModel(imagesLocation As String, imagesFolder As String, model As PredictionEngine(Of ImageNetData, ImageNetPrediction))
			Console.WriteLine($"Tags file location: {imagesLocation}")
			Console.WriteLine("")
			Console.WriteLine("=====Identify the objects in the images=====")
			Console.WriteLine("")

			Dim testData = ImageNetData.ReadFromCsv(imagesLocation, imagesFolder)

			For Each sample In testData
				Dim probs = model.Predict(sample).PredictedLabels
				Dim boundingBoxes As IList(Of YoloBoundingBox) = _parser.ParseOutputs(probs)
				Dim filteredBoxes = _parser.NonMaxSuppress(boundingBoxes, 5, .5F)

				Console.WriteLine(".....The objects in the image {0} are detected as below....", sample.Label)
				For Each box In filteredBoxes
					Console.WriteLine(box.Label & " and its Confidence score: " & box.Confidence)
				Next box
				Console.WriteLine("")
			Next sample
		End Sub
	End Class
End Namespace

