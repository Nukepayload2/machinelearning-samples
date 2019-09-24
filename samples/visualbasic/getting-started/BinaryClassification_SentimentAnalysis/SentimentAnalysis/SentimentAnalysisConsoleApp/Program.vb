Imports System.IO
Imports Microsoft.ML
Imports SentimentAnalysisConsoleApp.DataStructures
Imports Common
Imports Microsoft.ML.DataOperationsCatalog

Namespace SentimentAnalysisConsoleApp
    Friend Module Program
        Private ReadOnly BaseDatasetsRelativePath As String = "../../../../Data"
        Private ReadOnly DataRelativePath As String = $"{BaseDatasetsRelativePath}/wikiDetoxAnnotated40kRows.tsv"

        Private ReadOnly DataPath As String = GetAbsolutePath(DataRelativePath)

        Private ReadOnly BaseModelsRelativePath As String = "../../../../MLModels"
        Private ReadOnly ModelRelativePath As String = $"{BaseModelsRelativePath}/SentimentModel.zip"

        Private ReadOnly ModelPath As String = GetAbsolutePath(ModelRelativePath)

        Sub Main(args() As String)
            ' Create MLContext to be shared across the model creation workflow objects 
            ' Set a random seed for repeatable/deterministic results across multiple trainings.
            Dim mlContext = New MLContext(seed:=1)

            ' STEP 1: Common data loading configuration
            Dim dataView As IDataView = mlContext.Data.LoadFromTextFile(Of SentimentIssue)(DataPath, hasHeader:=True)

            Dim trainTestSplit As TrainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction:=0.2)
            Dim trainingData As IDataView = trainTestSplit.TrainSet
            Dim testData As IDataView = trainTestSplit.TestSet

            ' STEP 2: Common data process configuration with pipeline data transformations          
            Dim dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName:="Features", inputColumnName:=NameOf(SentimentIssue.Text))

            ' STEP 3: Set the training algorithm, then create and config the modelBuilder                            
            Dim trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName:="Label", featureColumnName:="Features")
            Dim trainingPipeline = dataProcessPipeline.Append(trainer)

            ' STEP 4: Train the model fitting to the DataSet
            Dim trainedModel As ITransformer = trainingPipeline.Fit(trainingData)

            ' STEP 5: Evaluate the model and show accuracy stats
            Dim predictions = trainedModel.Transform(testData)
            Dim metrics = mlContext.BinaryClassification.Evaluate(data:=predictions, labelColumnName:="Label", scoreColumnName:="Score")

            ConsoleHelper.PrintBinaryClassificationMetrics(trainer.ToString(), metrics)

            ' STEP 6: Save/persist the trained model to a .ZIP file
            mlContext.Model.Save(trainedModel, trainingData.Schema, ModelPath)

            Console.WriteLine("The model is saved to {0}", ModelPath)

            ' TRY IT: Make a single test prediction, loading the model from .ZIP file
            Dim sampleStatement As SentimentIssue = New SentimentIssue With {.Text = "I love this movie!"}

            ' Create prediction engine related to the loaded trained model
            Dim predEngine = mlContext.Model.CreatePredictionEngine(Of SentimentIssue, SentimentPrediction)(trainedModel)

            ' Score
            Dim resultprediction = predEngine.Predict(sampleStatement)

            Console.WriteLine($"=============== Single Prediction  ===============")
            Console.WriteLine($"Text: {sampleStatement.Text} | Prediction: {(If(Convert.ToBoolean(resultprediction.Prediction), "Toxic", "Non Toxic"))} sentiment | Probability of being toxic: {resultprediction.Probability} ")
            Console.WriteLine($"================End of Process.Hit any key to exit==================================")
            Console.ReadLine()
        End Sub

        Public Function GetAbsolutePath(relativePath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

            Return fullPath
        End Function
    End Module
End Namespace