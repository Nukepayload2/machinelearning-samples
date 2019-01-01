Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports System.IO
Imports Microsoft.ML

Namespace Common
	Public Class MLModelEngine(Of TData As Class, TPrediction As {Class, New})
		Private ReadOnly _mlContext As MLContext
		Private ReadOnly _model As ITransformer
		Private ReadOnly _predictionEnginePool As ObjectPool(Of PredictionFunction(Of TData, TPrediction))
		Private ReadOnly _minPredictionEngineObjectsInPool As Integer
		Private ReadOnly _maxPredictionEngineObjectsInPool As Integer
		Private ReadOnly _expirationTime As Double

		Public ReadOnly Property CurrentPredictionEnginePoolSize() As Integer
			Get
				Return _predictionEnginePool.CurrentPoolSize
			End Get
		End Property

        ''' <summary>
        ''' Constructor with modelFilePathName to load
        ''' </summary>
        ''' <param name="mlContext">MLContext to use</param>
        ''' <param name="modelFilePathName">Model .ZIP file path name</param>
        ''' <param name="minPredictionEngineObjectsInPool">Minimum number of PredictionEngineObjects in pool, as goal. Could be less but eventually it'll tend to that number</param>
        ''' <param name="maxPredictionEngineObjectsInPool">Maximum number of PredictionEngineObjects in pool</param>
        ''' <param name="expirationTime">Expiration Time (mlSecs) of PredictionEngineObject since added to the pool</param>
        Public Sub New(mlContext As MLContext, modelFilePathName As String,
                       Optional minPredictionEngineObjectsInPool As Integer = 5,
                       Optional maxPredictionEngineObjectsInPool As Integer = 1000,
                       Optional expirationTime As Double = 30000)
            _mlContext = mlContext

            'Load the ProductSalesForecast model from the .ZIP file
            Using fileStream = File.OpenRead(modelFilePathName)
                _model = mlContext.Model.Load(fileStream)
            End Using

            _minPredictionEngineObjectsInPool = minPredictionEngineObjectsInPool
            _maxPredictionEngineObjectsInPool = maxPredictionEngineObjectsInPool
            _expirationTime = expirationTime

            'Create PredictionEngine Object Pool
            _predictionEnginePool = CreatePredictionEngineObjectPool()
        End Sub

        ''' <summary>
        ''' Constructor with ITransformer model already created
        ''' </summary>
        ''' <param name="mlContext">MLContext to use</param>
        ''' <param name="model">Model/Transformer to use, already created</param>
        ''' <param name="minPredictionEngineObjectsInPool">Minimum number of PredictionEngineObjects in pool, as goal. Could be less but eventually it'll tend to that number</param>
        ''' <param name="maxPredictionEngineObjectsInPool">Maximum number of PredictionEngineObjects in pool</param>
        ''' <param name="expirationTime">Expiration Time (mlSecs) of PredictionEngineObject since added to the pool</param>
        Public Sub New(mlContext As MLContext, model As ITransformer,
                       Optional minPredictionEngineObjectsInPool As Integer = 5,
                       Optional maxPredictionEngineObjectsInPool As Integer = 1000,
                       Optional expirationTime As Double = 30000)
            _mlContext = mlContext
            _model = model
            _minPredictionEngineObjectsInPool = minPredictionEngineObjectsInPool
            _maxPredictionEngineObjectsInPool = maxPredictionEngineObjectsInPool
            _expirationTime = expirationTime

            'Create PredictionEngine Object Pool
            _predictionEnginePool = CreatePredictionEngineObjectPool()
        End Sub

        Private Function CreatePredictionEngineObjectPool() As ObjectPool(Of PredictionFunction(Of TData, TPrediction))
            Return New ObjectPool(Of PredictionFunction(Of TData, TPrediction))(
                objectGenerator:=Function()
                                     'Measure PredictionEngine creation
                                     Dim watch = System.Diagnostics.Stopwatch.StartNew()

                                     'Make PredictionEngine
                                     Dim predEngine = _model.MakePredictionFunction(Of TData, TPrediction)(_mlContext)

                                     'Stop measuring time
                                     watch.Stop()
                                     Dim elapsedMs As Long = watch.ElapsedMilliseconds

                                     Return predEngine
                                 End Function,
                minPoolSize:=_minPredictionEngineObjectsInPool,
                maxPoolSize:=_maxPredictionEngineObjectsInPool,
                expirationTime:=_expirationTime)
        End Function

        Public Function Predict(dataSample As TData) As TPrediction
            'Get PredictionEngine object from the Object Pool
            Dim predictionEngine As PredictionFunction(Of TData, TPrediction) = _predictionEnginePool.GetObject()

            'Measure Predict() execution time
            Dim watch = System.Diagnostics.Stopwatch.StartNew()

            'Predict
            Dim prediction As TPrediction = predictionEngine.Predict(dataSample)

            'Stop measuring time
            watch.Stop()
            Dim elapsedMs As Long = watch.ElapsedMilliseconds

            'Release used PredictionEngine object into the Object Pool
            _predictionEnginePool.PutObject(predictionEngine)

            Return prediction
        End Function

    End Class
End Namespace
