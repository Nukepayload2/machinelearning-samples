Imports System.IO
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML

Imports SentimentAnalysisConsoleApp.DataStructures
Imports Common
Imports Microsoft.ML.Data

Namespace SentimentAnalysisConsoleApp

    Friend Module Program
        Private ReadOnly Property AppPath() As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private BaseDatasetsLocation As String = "../../../../Data"
        Private TrainDataPath As String = $"{BaseDatasetsLocation}/wikipedia-detox-250-line-data.tsv"
        Private TestDataPath As String = $"{BaseDatasetsLocation}/wikipedia-detox-250-line-test.tsv"

        Private BaseModelsPath As String = "../../../../MLModels"
        Private ModelPath As String = $"{BaseModelsPath}/SentimentModel.zip"

        Sub Main(args() As String)
            'Create MLContext to be shared across the model creation workflow objects 
            'Set a random seed for repeatable/deterministic results across multiple trainings.
            Dim mlContext = New MLContext(seed:=1)

            ' Create, Train, Evaluate and Save a model
            BuildTrainEvaluateAndSaveModel(mlContext)
            Common.ConsoleHelper.ConsoleWriteHeader("=============== End of training processh ===============")

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
            Dim dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText("Text", "Features")

            ' (OPTIONAL) Peek data (such as 2 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            ConsoleHelper.PeekDataViewInConsole(Of SentimentIssue)(mlContext, trainingDataView, dataProcessPipeline, 2)
            ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", trainingDataView, dataProcessPipeline, 1)

            ' STEP 3: Set the training algorithm, then create and config the modelBuilder                            
            Dim trainer = mlContext.BinaryClassification.Trainers.FastTree(labelColumn:="Label", featureColumn:="Features")
            Dim trainingPipeline = dataProcessPipeline.Append(trainer)

            ' STEP 4: Train the model fitting to the DataSet
            Console.WriteLine("=============== Training the model ===============")
            Dim trainedModel As ITransformer = trainingPipeline.Fit(trainingDataView)

            ' STEP 5: Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
            Dim predictions = trainedModel.Transform(testDataView)
            Dim metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label", "Score")

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

            ' Using a Model Scorer helper class --> 3 lines, including the object creation, and a single object to deal with
            ' var modelScorer = new ModelScorer<SentimentIssue, SentimentPrediction>(mlContext);
            ' modelScorer.LoadModelFromZipFile(ModelPath);
            ' var resultprediction = modelScorer.PredictSingle(sampleStatement);

            Console.WriteLine($"=============== Single Prediction  ===============")
            Console.WriteLine($"Text: {sampleStatement.Text} | Prediction: {(If(Convert.ToBoolean(resultprediction.Prediction), "Toxic", "Nice"))} sentiment | Probability: {resultprediction.Probability} ")
            Console.WriteLine($"==================================================")
            '
        End Sub
    End Module
End Namespace