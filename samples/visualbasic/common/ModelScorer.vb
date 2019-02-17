Imports System.IO
Imports Microsoft.Data.DataView
Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Data

Namespace Common
    Public Class ModelScorer(Of TObservation As Class, TPrediction As {Class, New})
        Private _mlContext As MLContext
        Private privateTrainedModel As ITransformer
        Public Property TrainedModel() As ITransformer
            Get
                Return privateTrainedModel
            End Get
            Private Set(value As ITransformer)
                privateTrainedModel = value
            End Set
        End Property
        Public PredictionEngine As PredictionEngine(Of TObservation, TPrediction)

        Public Sub New(mlContext As MLContext, Optional trainedModel As ITransformer = Nothing)
            _mlContext = mlContext

            If trainedModel IsNot Nothing Then
                'Keep the trainedModel passed through the constructor
                Me.TrainedModel = trainedModel

                ' Create prediction engine related to the passed trained model
                PredictionEngine = Me.TrainedModel.CreatePredictionEngine(Of TObservation, TPrediction)(_mlContext)
            End If
        End Sub

        Public Function PredictSingle(input As TObservation) As TPrediction
            CheckTrainedModelIsLoaded()
            Return PredictionEngine.Predict(input)
        End Function

        Public Function PredictBatch(inputDataView As IDataView) As IEnumerable(Of TPrediction)
            CheckTrainedModelIsLoaded()
            Dim predictions = TrainedModel.Transform(inputDataView)
            Return _mlContext.CreateEnumerable(Of TPrediction)(predictions, reuseRowObject:=False)
        End Function

        Public Function LoadModelFromZipFile(modelPath As String) As ITransformer
            Using stream = New FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                TrainedModel = _mlContext.Model.Load(stream)
            End Using

            ' Create prediction engine related to the loaded trained model
            PredictionEngine = TrainedModel.CreatePredictionEngine(Of TObservation, TPrediction)(_mlContext)

            Return TrainedModel
        End Function

        Private Sub CheckTrainedModelIsLoaded()
            If TrainedModel Is Nothing Then
                Throw New InvalidOperationException("Need to have a model before scoring. Call LoadModelFromZipFile(modelPath) first or provided a model through the constructor.")
            End If
        End Sub
    End Class

End Namespace
