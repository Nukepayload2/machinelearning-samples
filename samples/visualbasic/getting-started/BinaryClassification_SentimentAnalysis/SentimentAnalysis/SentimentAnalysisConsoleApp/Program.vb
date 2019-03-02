Imports System.IO
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML
Imports Microsoft.ML.Data

Imports SentimentAnalysisConsoleApp.DataStructures
Imports Common
Imports Microsoft.Data.DataView

Namespace SentimentAnalysisConsoleApp
    Friend Module Program
        Private ReadOnly Property AppPath As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private ReadOnly BaseDatasetsRelativePath As String = "../../../../Data"
        Private ReadOnly TrainDataRelativePath As String = $"{BaseDatasetsRelativePath}/wikipedia-detox-250-line-data.tsv"
        Private ReadOnly TestDataRelativePath As String = $"{BaseDatasetsRelativePath}/wikipedia-detox-250-line-test.tsv"

        Private TrainDataPath As String = GetAbsolutePath(TrainDataRelativePath)
        Private TestDataPath As String = GetAbsolutePath(TestDataRelativePath)

        Private ReadOnly BaseModelsRelativePath As String = "../../../../MLModels"
        Private ReadOnly ModelRelativePath As String = $"{BaseModelsRelativePath}/SentimentModel.zip"

        Private ModelPath As String = GetAbsolutePath(ModelRelativePath)

        Sub Main(args() As String)
            'Create MLContext to be shared across the model creation workflow objects 
            'Set a random seed for repeatable/deterministic results across multiple trainings.
            Dim mlContext = New MLContext(seed:=1)

            ' Create, Train, Evaluate and Save a model
            BuildTrainEvaluateAndSaveModel(mlContext)
            Common.ConsoleHelper.ConsoleWriteHeader("=============== End of training process ===============")

            ' Make a single test prediction loding the model from .ZIP file
            TestSinglePrediction(mlContext)

            Common.ConsoleHelper.ConsoleWriteHeader("=============== End of process, hit any key to finish ===============")
            Console.ReadKey()

        End Sub

        Private Function BuildTrainEvaluateAndSaveModel(mlContext As MLContext) As ITransformer
            ' STEP 1: Common data loading configuration
            Dim trainingDataView As IDataView = mlContext.Data.ReadFromTextFile(Of SentimentIssue)(TrainDataPath, hasHeader:=True)
            Dim testDataView As IDataView = mlContext.Data.ReadFromTextFile(Of SentimentIssue)(TestDataPath, hasHeader:=True)

            ' STEP 2: Common data process configuration with pipeline data transformations          
            Dim dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName:=DefaultColumnNames.Features, inputColumnName:=NameOf(SentimentIssue.Text))

            ' (OPTIONAL) Peek data (such as 2 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            ConsoleHelper.PeekDataViewInConsole(mlContext, trainingDataView, dataProcessPipeline, 2)
            ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, DefaultColumnNames.Features, trainingDataView, dataProcessPipeline, 1)

            ' STEP 3: Set the training algorithm, then create and config the modelBuilder                            
            Dim trainer = mlContext.BinaryClassification.Trainers.FastTree(labelColumn:=DefaultColumnNames.Label, featureColumn:=DefaultColumnNames.Features)
            Dim trainingPipeline = dataProcessPipeline.Append(trainer)

            ' STEP 4: Train the model fitting to the DataSet
            Console.WriteLine("=============== Training the model ===============")
            Dim trainedModel As ITransformer = trainingPipeline.Fit(trainingDataView)

            ' STEP 5: Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
            Dim predictions = trainedModel.Transform(testDataView)
            Dim metrics = mlContext.BinaryClassification.Evaluate(data:=predictions, label:=DefaultColumnNames.Label, score:=DefaultColumnNames.Score)

            ConsoleHelper.PrintBinaryClassificationMetrics(trainer.ToString(), metrics)

            ' STEP 6: Save/persist the trained model to a .ZIP file

            Using fs = New FileStream(ModelPath, FileMode.Create, FileAccess.Write, FileShare.Write)
                mlContext.Model.Save(trainedModel, fs)
            End Using

            Console.WriteLine("The model is saved to {0}", ModelPath)

            Return trainedModel
        End Function

        ' (OPTIONAL) Try/test a single prediction by loding the model from the file, first.
        Private Sub TestSinglePrediction(mlContext As MLContext)
            Dim sampleStatement As SentimentIssue = New SentimentIssue With {.Text = "This is a very rude movie"}

            Dim trainedModel As ITransformer
            Using stream = New FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                trainedModel = mlContext.Model.Load(stream)
            End Using

            ' Create prediction engine related to the loaded trained model
            Dim predEngine = trainedModel.CreatePredictionEngine(Of SentimentIssue, SentimentPrediction)(mlContext)

            'Score
            Dim resultprediction = predEngine.Predict(sampleStatement)

            Console.WriteLine($"=============== Single Prediction  ===============")
            Console.WriteLine($"Text: {sampleStatement.Text} | Prediction: {(If(Convert.ToBoolean(resultprediction.Prediction), "Negative", "Nice"))} sentiment | Probability: {resultprediction.Probability} ")
            Console.WriteLine($"==================================================")
        End Sub

        Public Function GetAbsolutePath(relativePath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

            Return fullPath
        End Function
    End Module
End Namespace
