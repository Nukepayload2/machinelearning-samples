Imports Microsoft.ML
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Threading.Tasks
Imports TensorFlowImageClassification.ML.DataModels

Namespace TensorFlowImageClassification.ML
	Public Class TensorFlowModelConfigurator
		Private ReadOnly _mlContext As MLContext
		Private ReadOnly _mlModel As ITransformer

		Public Sub New(tensorFlowModelFilePath As String)
			_mlContext = New MLContext

			' Model creation and pipeline definition for images needs to run just once, so calling it from the constructor:
			_mlModel = SetupMlnetModel(tensorFlowModelFilePath)
		End Sub

		Public Structure ImageSettings
			Public Const imageHeight As Integer = 227
			Public Const imageWidth As Integer = 227
			Public Const mean As Single = 117 'offsetImage
			Public Const channelsLast As Boolean = True 'interleavePixelColors
		End Structure

		' For checking tensor names, you can open the TF model .pb file with tools like Netron: https://github.com/lutzroeder/netron
		Public Structure TensorFlowModelSettings
			' input tensor name
			Public Const inputTensorName As String = "Placeholder"

			' output tensor name
			Public Const outputTensorName As String = "loss"
		End Structure

		Private Function SetupMlnetModel(tensorFlowModelFilePath As String) As ITransformer
			Dim pipeline = _mlContext.Transforms.ResizeImages(outputColumnName:= TensorFlowModelSettings.inputTensorName, imageWidth:= ImageSettings.imageWidth, imageHeight:= ImageSettings.imageHeight, inputColumnName:= NameOf(ImageInputData.Image)).Append(_mlContext.Transforms.ExtractPixels(outputColumnName:= TensorFlowModelSettings.inputTensorName, interleavePixelColors:= ImageSettings.channelsLast, offsetImage:= ImageSettings.mean)).Append(_mlContext.Model.LoadTensorFlowModel(tensorFlowModelFilePath).ScoreTensorFlowModel(outputColumnNames:= { TensorFlowModelSettings.outputTensorName }, inputColumnNames:= { TensorFlowModelSettings.inputTensorName }, addBatchDimensionInput:= False))

			Dim mlModel As ITransformer = pipeline.Fit(CreateEmptyDataView())

			Return mlModel
		End Function
		Private Function CreateEmptyDataView() As IDataView
			'Create empty DataView ot Images. We just need the schema to call fit()
			Dim list As New List(Of ImageInputData)
			list.Add(New ImageInputData With {.Image = New System.Drawing.Bitmap(ImageSettings.imageWidth, ImageSettings.imageHeight)}) 'Test: Might not need to create the Bitmap.. = null; ?
			Dim enumerableData As IEnumerable(Of ImageInputData) = list

			Dim dv = _mlContext.Data.LoadFromEnumerable(Of ImageInputData)(list)
			Return dv
		End Function

		Public Sub SaveMLNetModel(mlnetModelFilePath As String)
			' Save/persist the model to a .ZIP file to be loaded by the PredictionEnginePool
			_mlContext.Model.Save(_mlModel, Nothing, mlnetModelFilePath)
		End Sub
	End Class
End Namespace
