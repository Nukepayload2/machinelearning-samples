Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.ML
Imports Microsoft.ML.Transforms.Image
Imports OnnxObjectDetectionE2EAPP.Utilities

Namespace OnnxObjectDetectionE2EAPP.MLModel
	Public Class OnnxModelConfigurator
		Private ReadOnly _mlContext As MLContext
		Private ReadOnly _mlModel As ITransformer

		Public Sub New(onnxModelFilePath As String)
			_mlContext = New MLContext
			' Model creation and pipeline definition for images needs to run just once, so calling it from the constructor:
			_mlModel = SetupMlNetModel(onnxModelFilePath)
		End Sub

		Public Structure ImageSettings
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

		Public Function SetupMlNetModel(onnxModelFilePath As String) As ITransformer
			Dim dataView = CreateEmptyDataView()

			Dim pipeline = _mlContext.Transforms.ResizeImages(resizing:= ImageResizingEstimator.ResizingKind.Fill, outputColumnName:= "image", imageWidth:= ImageSettings.imageWidth, imageHeight:= ImageSettings.imageHeight, inputColumnName:= NameOf(ImageInputData.Image)).Append(_mlContext.Transforms.ExtractPixels(outputColumnName:= "image")).Append(_mlContext.Transforms.ApplyOnnxModel(modelFile:= onnxModelFilePath, outputColumnNames:= { TinyYoloModelSettings.ModelOutput }, inputColumnNames:= { TinyYoloModelSettings.ModelInput }))

			Dim mlNetModel = pipeline.Fit(dataView)

			Return mlNetModel
		End Function

		Public Sub SaveMLNetModel(mlnetModelFilePath As String)
			' Save/persist the model to a .ZIP file to be loaded by the PredictionEnginePool
			_mlContext.Model.Save(_mlModel, Nothing, mlnetModelFilePath)
		End Sub

		Private Function CreateEmptyDataView() As IDataView
			'Create empty DataView ot Images. We just need the schema to call fit()
			Dim list As New List(Of ImageInputData)
			list.Add(New ImageInputData With {.Image = New System.Drawing.Bitmap(ImageSettings.imageWidth, ImageSettings.imageHeight)}) 'Test: Might not need to create the Bitmap.. = null; ?
			Dim enumerableData As IEnumerable(Of ImageInputData) = list

			Dim dv = _mlContext.Data.LoadFromEnumerable(Of ImageInputData)(list)
			Return dv
		End Function
	End Class
End Namespace

