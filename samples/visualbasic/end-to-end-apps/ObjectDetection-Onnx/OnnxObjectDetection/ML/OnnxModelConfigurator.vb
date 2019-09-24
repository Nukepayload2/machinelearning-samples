Imports Microsoft.ML
Imports Microsoft.ML.Transforms.Image
Imports System.Collections.Generic
Imports System.Linq

Namespace OnnxObjectDetection
	Public Class OnnxModelConfigurator
		Private ReadOnly mlContext As MLContext
		Private ReadOnly mlModel As ITransformer

		Public Sub New(onnxModel As IOnnxModel)
			mlContext = New MLContext
			' Model creation and pipeline definition for images needs to run just once,
			' so calling it from the constructor:
			mlModel = SetupMlNetModel(onnxModel)
		End Sub

		Private Function SetupMlNetModel(onnxModel As IOnnxModel) As ITransformer
			Dim dataView = mlContext.Data.LoadFromEnumerable(New List(Of ImageInputData))

			Dim pipeline = mlContext.Transforms.ResizeImages(resizing:= ImageResizingEstimator.ResizingKind.Fill, outputColumnName:= onnxModel.ModelInput, imageWidth:= ImageSettings.imageWidth, imageHeight:= ImageSettings.imageHeight, inputColumnName:= NameOf(ImageInputData.Image)).Append(mlContext.Transforms.ExtractPixels(outputColumnName:= onnxModel.ModelInput)).Append(mlContext.Transforms.ApplyOnnxModel(modelFile:= onnxModel.ModelPath, outputColumnName:= onnxModel.ModelOutput, inputColumnName:= onnxModel.ModelInput))

			Dim mlNetModel = pipeline.Fit(dataView)

			Return mlNetModel
		End Function

		Public Function GetMlNetPredictionEngine(Of T As {Class, IOnnxObjectPrediction, New})() As PredictionEngine(Of ImageInputData, T)
			Return mlContext.Model.CreatePredictionEngine(Of ImageInputData, T)(mlModel)
		End Function

		Public Sub SaveMLNetModel(mlnetModelFilePath As String)
			' Save/persist the model to a .ZIP file to be loaded by the PredictionEnginePool
			mlContext.Model.Save(mlModel, Nothing, mlnetModelFilePath)
		End Sub
	End Class
End Namespace
