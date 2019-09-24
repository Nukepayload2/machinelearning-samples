Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports ObjectDetection.DataStructures
Imports ObjectDetection.YoloParser

Namespace ObjectDetection
	Friend Class OnnxModelScorer
		Private ReadOnly imagesFolder As String
		Private ReadOnly modelLocation As String
		Private ReadOnly mlContext As MLContext

		Private _boundingBoxes As IList(Of YoloBoundingBox) = New List(Of YoloBoundingBox)

		Public Sub New(imagesFolder As String, modelLocation As String, mlContext As MLContext)
			Me.imagesFolder = imagesFolder
			Me.modelLocation = modelLocation
			Me.mlContext = mlContext
		End Sub

		Public Structure ImageNetSettings
			Public Const imageHeight As Integer = 416
			Public Const imageWidth As Integer = 416
		End Structure

		Public Structure TinyYoloModelSettings
			' for checking Tiny yolo2 Model input and  output  parameter names,
			'you can use tools like Netron, 
			' which is installed by Visual Studio AI Tools

			' input tensor name
			Public Const ModelInput As String = "image"

			' output tensor name
			Public Const ModelOutput As String = "grid"
		End Structure

		Private Function LoadModel(modelLocation As String) As ITransformer
			Console.WriteLine("Read model")
			Console.WriteLine($"Model location: {modelLocation}")
			Console.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight})")

			' Create IDataView from empty list to obtain input data schema
			Dim data = mlContext.Data.LoadFromEnumerable(New List(Of ImageNetData))

			' Define scoring pipeline
			Dim pipeline = mlContext.Transforms.LoadImages(outputColumnName:= "image", imageFolder:= "", inputColumnName:= NameOf(ImageNetData.ImagePath)).Append(mlContext.Transforms.ResizeImages(outputColumnName:= "image", imageWidth:= ImageNetSettings.imageWidth, imageHeight:= ImageNetSettings.imageHeight, inputColumnName:= "image")).Append(mlContext.Transforms.ExtractPixels(outputColumnName:= "image")).Append(mlContext.Transforms.ApplyOnnxModel(modelFile:= modelLocation, outputColumnNames:= { TinyYoloModelSettings.ModelOutput }, inputColumnNames:= { TinyYoloModelSettings.ModelInput }))

			' Fit scoring pipeline
			Dim model = pipeline.Fit(data)

			Return model
		End Function

		Private Function PredictDataUsingModel(testData As IDataView, model As ITransformer) As IEnumerable(Of Single())
			Console.WriteLine($"Images location: {imagesFolder}")
			Console.WriteLine("")
			Console.WriteLine("=====Identify the objects in the images=====")
			Console.WriteLine("")

			Dim scoredData As IDataView = model.Transform(testData)

			Dim probabilities As IEnumerable(Of Single()) = scoredData.GetColumn(Of Single())(TinyYoloModelSettings.ModelOutput)

			Return probabilities
		End Function

		Public Function Score(data As IDataView) As IEnumerable(Of Single())
			Dim model = LoadModel(modelLocation)

			Return PredictDataUsingModel(data, model)
		End Function
	End Class
End Namespace

