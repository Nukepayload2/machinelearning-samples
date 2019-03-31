Imports Microsoft.ML
Imports Microsoft.Extensions.ObjectPool
Imports System.IO
Imports Microsoft.Data.DataView

Namespace Scalable.Model.Engine
	Public Class MLModelEngine(Of TData As Class, TPrediction As {Class, New})
		Private ReadOnly _mlContext As MLContext
		Private ReadOnly _mlModel As ITransformer
		Private ReadOnly _predictionEnginePool As ObjectPool(Of PredictionEngine(Of TData, TPrediction))
		Private ReadOnly _maxObjectsRetained As Integer

		''' <summary>
		''' Exposing the ML model allowing additional ITransformer operations such as Bulk predictions', etc.
		''' </summary>
		Public ReadOnly Property MLModel As ITransformer
			Get
				Return _mlModel
			End Get
		End Property

		''' <summary>
		''' Constructor with modelFilePathName to load from
		''' </summary>
		Public Sub New(modelFilePathName As String, Optional maxObjectsRetained As Integer = -1)
			'Create the MLContext object to use under the scope of this class 
			_mlContext = New MLContext

			'Load the ProductSalesForecast model from the .ZIP file
			Using fileStream = File.OpenRead(modelFilePathName)
				_mlModel = _mlContext.Model.Load(fileStream)
			End Using

			_maxObjectsRetained = maxObjectsRetained

			'Create PredictionEngine Object Pool
			_predictionEnginePool = CreatePredictionEngineObjectPool()
		End Sub

		' Create the Object Pool based on the PooledPredictionEnginePolicy.
		' This method is only used once, from the cosntructor.
		Private Function CreatePredictionEngineObjectPool() As ObjectPool(Of PredictionEngine(Of TData, TPrediction))
			Dim predEnginePolicy = New PooledPredictionEnginePolicy(Of TData, TPrediction)(_mlContext, _mlModel)

			Dim pool As DefaultObjectPool(Of PredictionEngine(Of TData, TPrediction))

			If _maxObjectsRetained <> -1 Then
				pool = New DefaultObjectPool(Of PredictionEngine(Of TData, TPrediction))(predEnginePolicy, _maxObjectsRetained)
			Else
				'default maximumRetained is Environment.ProcessorCount * 2, if not explicitly provided
				pool = New DefaultObjectPool(Of PredictionEngine(Of TData, TPrediction))(predEnginePolicy)
			End If

			Return pool
		End Function

		''' <summary>
		''' The Predict() method performs a single prediction based on sample data provided (dataSample) and returning the Prediction.
		''' This implementation uses an object pool internally so it is optimized for scalable and multi-threaded apps.
		''' </summary>
		''' <param name="dataSample"></param>
		''' <returns></returns>
		Public Function Predict(dataSample As TData) As TPrediction
			'Get PredictionEngine object from the Object Pool
			Dim predictionEngine As PredictionEngine(Of TData, TPrediction) = _predictionEnginePool.Get()

			Try
				'Predict
				Dim prediction As TPrediction = predictionEngine.Predict(dataSample)
				Return prediction
			Finally
				'Release used PredictionEngine object into the Object Pool
				_predictionEnginePool.Return(predictionEngine)
			End Try
		End Function

	End Class

End Namespace
