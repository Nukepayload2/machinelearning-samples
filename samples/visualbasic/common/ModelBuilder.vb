Imports System.IO
Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Data

Namespace Common
    Public Class ModelBuilder(Of TObservation As Class, TPrediction As {Class, New})
        Private _mlcontext As MLContext
        Private privateTrainingPipeline As IEstimator(Of ITransformer)
        Public Property TrainingPipeline() As IEstimator(Of ITransformer)
            Get
                Return privateTrainingPipeline
            End Get
            Private Set(value As IEstimator(Of ITransformer))
                privateTrainingPipeline = value
            End Set
        End Property
        Private privateTrainedModel As ITransformer
        Public Property TrainedModel() As ITransformer
            Get
                Return privateTrainedModel
            End Get
            Private Set(value As ITransformer)
                privateTrainedModel = value
            End Set
        End Property

        Public Sub New(mlContext As MLContext, dataProcessPipeline As IEstimator(Of ITransformer))
            _mlcontext = mlContext
            TrainingPipeline = dataProcessPipeline
        End Sub

        Public Sub AddTrainer(trainer As IEstimator(Of ITransformer))
            Me.AddEstimator(trainer)
        End Sub

        Public Sub AddEstimator(estimator As IEstimator(Of ITransformer))
            TrainingPipeline = TrainingPipeline.Append(estimator)
        End Sub

        Public Function Train(trainingData As IDataView) As ITransformer
            TrainedModel = TrainingPipeline.Fit(trainingData)
            Return TrainedModel
        End Function

        Public Function EvaluateRegressionModel(testData As IDataView, label As String, score As String) As RegressionMetrics
            CheckTrained()
            Dim predictions = TrainedModel.Transform(testData)
            Dim metrics = _mlcontext.Regression.Evaluate(predictions, label:=label, score:=score)
            Return metrics
        End Function

        Public Function EvaluateBinaryClassificationModel(testData As IDataView, label As String, score As String) As CalibratedBinaryClassificationMetrics
            CheckTrained()
            Dim predictions = TrainedModel.Transform(testData)
            Dim metrics = _mlcontext.BinaryClassification.Evaluate(predictions, label:=label, score:=score)
            Return metrics
        End Function

        Public Function EvaluateMultiClassClassificationModel(testData As IDataView, Optional label As String = "Label", Optional score As String = "Score") As MultiClassClassifierMetrics
            CheckTrained()
            Dim predictions = TrainedModel.Transform(testData)
            Dim metrics = _mlcontext.MulticlassClassification.Evaluate(predictions, label:=label, score:=score)
            Return metrics
        End Function

        Public Function CrossValidateAndEvaluateMulticlassClassificationModel(data As IDataView, Optional numFolds As Integer = 5, Optional labelColumn As String = "Label", Optional stratificationColumn As String = Nothing) As (metrics As MultiClassClassifierMetrics, model As ITransformer, scoredTestData As IDataView)()
            'CrossValidation happens actually before training, so no check here.

            'Cross validate
            Dim crossValidationResults = _mlcontext.MulticlassClassification.CrossValidate(data, TrainingPipeline, numFolds, labelColumn, stratificationColumn)

            'Another way to do it:
            'var context = new MulticlassClassificationContext(_mlcontext);
            'var crossValidationResults = context.CrossValidate(data, TrainingPipeline, numFolds, labelColumn, stratificationColumn);

            Return crossValidationResults
        End Function

        Public Function EvaluateClusteringModel(dataView As IDataView) As ClusteringMetrics
            CheckTrained()
            Dim predictions = TrainedModel.Transform(dataView)

            Dim metrics = _mlcontext.Clustering.Evaluate(predictions, score:="Score", features:="Features")
            Return metrics
        End Function

        Public Sub SaveModelAsFile(persistedModelPath As String)
            CheckTrained()

            Using fs = New FileStream(persistedModelPath, FileMode.Create, FileAccess.Write, FileShare.Write)
                _mlcontext.Model.Save(TrainedModel, fs)
            End Using
            Console.WriteLine("The model is saved to {0}", persistedModelPath)
        End Sub

        Private Sub CheckTrained()
            If TrainedModel Is Nothing Then
                Throw New InvalidOperationException("Cannot test before training. Call Train() first.")
            End If
        End Sub

    End Class
End Namespace
