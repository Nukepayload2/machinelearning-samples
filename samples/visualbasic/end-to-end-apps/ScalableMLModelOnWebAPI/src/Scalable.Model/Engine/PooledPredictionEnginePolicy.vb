Imports Microsoft.Extensions.ObjectPool
Imports Microsoft.ML

Namespace Scalable.Model.Engine
	Public Class PooledPredictionEnginePolicy(Of TData As Class, TPrediction As {Class, New})
		Implements IPooledObjectPolicy(Of PredictionEngine(Of TData, TPrediction))

		Private ReadOnly _mlContext As MLContext
		Private ReadOnly _model As ITransformer
		Public Sub New(mlContext As MLContext, model As ITransformer)
			_mlContext = mlContext
			_model = model
		End Sub

        Public Function Create() As PredictionEngine(Of TData, TPrediction) Implements IPooledObjectPolicy(Of PredictionEngine(Of TData, TPrediction)).Create
            ' Measuring CreatePredictionengine() time
            Dim watch = System.Diagnostics.Stopwatch.StartNew()

            Dim predictionEngine = _mlContext.Model.CreatePredictionEngine(Of TData, TPrediction)(_model)

            watch.Stop()
            Dim elapsedMs As Long = watch.ElapsedMilliseconds

            Return predictionEngine
        End Function

        Public Function [Return](obj As PredictionEngine(Of TData, TPrediction)) As Boolean Implements IPooledObjectPolicy(Of PredictionEngine(Of TData, TPrediction)).Return
            If obj Is Nothing Then
                Return False
            End If

            Return True
        End Function
    End Class
End Namespace
