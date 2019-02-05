Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Data
Imports MulticlassClassification_HeartDisease.DataStructure
Imports System.IO

Imports Common

Namespace MulticlassClassification_HeartDisease
    Module Program
        Private ReadOnly Property AppPath As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private BaseDatasetsLocation As String = "../../../../Data"
        Private TrainDataPath As String = $"{BaseDatasetsLocation}/HeartTraining.csv"
        Private TestDataPath As String = $"{BaseDatasetsLocation}/HeartTest.csv"

        Private BaseModelsPath As String = "../../../../MLModels"
        Private ModelPath As String = $"{BaseModelsPath}/HeartClassification.zip"

        Sub Main(args() As String)
            Dim mlContext = New MLContext()
            BuildTrainEvaluateAndSaveModel(mlContext)

            TestPrediction(mlContext)

            Console.WriteLine("=============== End of process, hit any key to finish ===============")
            Console.ReadKey()
        End Sub

        Private Sub BuildTrainEvaluateAndSaveModel(mlContext As MLContext)

            Dim trainingDataView = mlContext.Data.ReadFromTextFile(Of HeartDataImport)(TrainDataPath, hasHeader:=True, separatorChar:=","c)
            Dim testDataView = mlContext.Data.ReadFromTextFile(Of HeartDataImport)(TestDataPath, hasHeader:=True, separatorChar:=","c)

            Dim dataProcessPipeline = mlContext.Transforms.Concatenate("Features", "Age", "Sex", "Cp", "TrestBps", "Chol", "Fbs", "RestEcg", "Thalac", "Exang", "OldPeak", "Slope", "Ca", "Thal").AppendCacheCheckpoint(mlContext)

            ' (OPTIONAL) Peek data (such as 5 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            ConsoleHelper.PeekDataViewInConsole(Of HeartData)(mlContext, trainingDataView, dataProcessPipeline, 5)
            ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", trainingDataView, dataProcessPipeline, 5)

            Dim trainer = mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(labelColumn:=DefaultColumnNames.Label, featureColumn:=DefaultColumnNames.Features)
            Dim trainingPipeline = dataProcessPipeline.Append(trainer)

            Console.WriteLine("=============== Training the model ===============")
            Dim trainedModel = trainingPipeline.Fit(trainingDataView)
            Console.WriteLine("=============== Finish the train model.===============")

            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
            Dim predictions = trainedModel.Transform(testDataView)
            Dim metrics = mlContext.MulticlassClassification.Evaluate(predictions, "Label", "Score", "PredictedLabel", 0)

            Common.ConsoleHelper.PrintMultiClassClassificationMetrics(trainer.ToString(), metrics)

            Console.WriteLine("=============== Saving the model to a file ===============")
            Using fs = New FileStream(ModelPath, FileMode.Create, FileAccess.Write, FileShare.Write)
                mlContext.Model.Save(trainedModel, fs)
            End Using

            Console.WriteLine("=============== Model Saved ============= ")
        End Sub

        Private Sub TestPrediction(mlContext As MLContext)
            Dim trainedModel As ITransformer

            Using stream = New FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                trainedModel = mlContext.Model.Load(stream)
            End Using
            Dim predictionEngine = trainedModel.CreatePredictionEngine(Of HeartData, HeartPrediction)(mlContext)

            For Each heartData In HeartSampleData.heartDatas
                Dim prediction = predictionEngine.Predict(heartData)

                Console.WriteLine($" 0: {prediction.Score(0):0.###}")
                Console.WriteLine($" 1: {prediction.Score(1):0.###}")
                Console.WriteLine($" 2: {prediction.Score(2):0.###}")
                Console.WriteLine($" 3: {prediction.Score(3):0.###}")
                Console.WriteLine($" 4: {prediction.Score(4):0.###}")
                Console.WriteLine()
            Next heartData

        End Sub
    End Module

End Namespace
