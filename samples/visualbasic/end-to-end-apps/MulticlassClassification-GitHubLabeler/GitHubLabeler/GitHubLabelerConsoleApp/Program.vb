Option Infer On

Imports Microsoft.VisualBasic
Imports System
Imports System.Threading.Tasks
Imports System.IO
' Requires following NuGet packages
' NuGet package -> Microsoft.Extensions.Configuration
' NuGet package -> Microsoft.Extensions.Configuration.Json
Imports Microsoft.Extensions.Configuration
Imports Microsoft.ML
Imports Common
Imports GitHubLabeler.DataStructures
Imports Microsoft.ML.Data
Imports Microsoft.ML.TrainCatalogBase

Namespace GitHubLabeler
    Friend Module Program
        Private ReadOnly Property AppPath As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private BaseDatasetsRelativePath As String = "../../../../Data"
        Private DataSetRelativePath As String = $"{BaseDatasetsRelativePath}/corefx-issues-train.tsv"
        Private DataSetLocation As String = GetAbsolutePath(DataSetRelativePath)

        Private BaseModelsRelativePath As String = "../../../../MLModels"
        Private ModelRelativePath As String = $"{BaseModelsRelativePath}/GitHubLabelerModel.zip"
        Private ModelPath As String = GetAbsolutePath(ModelRelativePath)


        Public Enum MyTrainerStrategy As Integer
            SdcaMultiClassTrainer = 1
            OVAAveragedPerceptronTrainer = 2
        End Enum

        Public Property Configuration As IConfiguration

        Sub Main(args() As String)
            MainAsync(args).GetAwaiter.GetResult()
        End Sub

        Public Async Function MainAsync(args() As String) As Task
            SetupAppConfiguration()

            '1. ChainedBuilderExtensions and Train the model
            BuildAndTrainModel(DataSetLocation, ModelPath, MyTrainerStrategy.OVAAveragedPerceptronTrainer)

            '2. Try/test to predict a label for a single hard-coded Issue
            TestSingleLabelPrediction(ModelPath)

            '3. Predict Issue Labels and apply into a real GitHub repo
            ' (Comment the next line if no real access to GitHub repo) 
            Await PredictLabelsAndUpdateGitHub(ModelPath)

            Common.ConsoleHelper.ConsolePressAnyKey()
        End Function

        Public Sub BuildAndTrainModel(DataSetLocation As String, ModelPath As String, selectedStrategy As MyTrainerStrategy)
            ' Create MLContext to be shared across the model creation workflow objects 
            ' Set a random seed for repeatable/deterministic results across multiple trainings.
            Dim mlContext = New MLContext(seed:=1)

            ' STEP 1: Common data loading configuration
            Dim trainingDataView = mlContext.Data.LoadFromTextFile(Of GitHubIssue)(DataSetLocation, hasHeader:=True, separatorChar:=vbTab, allowSparse:=False)

            ' STEP 2: Common data process configuration with pipeline data transformations
            Dim dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:="Label", inputColumnName:=NameOf(GitHubIssue.Area)).Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName:="TitleFeaturized", inputColumnName:=NameOf(GitHubIssue.Title))).Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName:="DescriptionFeaturized", inputColumnName:=NameOf(GitHubIssue.Description))).Append(mlContext.Transforms.Concatenate(outputColumnName:="Features", "TitleFeaturized", "DescriptionFeaturized")).AppendCacheCheckpoint(mlContext)
            ' Use in-memory cache for small/medium datasets to lower training time. 
            ' Do NOT use it (remove .AppendCacheCheckpoint()) when handling very large datasets.

            ' (OPTIONAL) Peek data (such as 2 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            Common.ConsoleHelper.PeekDataViewInConsole(mlContext, trainingDataView, dataProcessPipeline, 2)

            ' STEP 3: Create the selected training algorithm/trainer
            Dim trainer As IEstimator(Of ITransformer) = Nothing
            Select Case selectedStrategy
                Case MyTrainerStrategy.SdcaMultiClassTrainer
                    trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features")
                Case MyTrainerStrategy.OVAAveragedPerceptronTrainer
                    ' Create a binary classification trainer.
                    Dim averagedPerceptronBinaryTrainer = mlContext.BinaryClassification.Trainers.AveragedPerceptron("Label", "Features", numberOfIterations:=10)
                    ' Compose an OVA (One-Versus-All) trainer with the BinaryTrainer.
                    ' In this strategy, a binary classification algorithm is used to train one classifier for each class, "
                    ' which distinguishes that class from all other classes. Prediction is then performed by running these binary classifiers, "
                    ' and choosing the prediction with the highest confidence score.
                    trainer = mlContext.MulticlassClassification.Trainers.OneVersusAll(averagedPerceptronBinaryTrainer)

                    Exit Select
                Case Else
            End Select

            'Set the trainer/algorithm and map label to value (original readable state)
            Dim trainingPipeline = dataProcessPipeline.Append(trainer).Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"))

            ' STEP 4: Cross-Validate with single dataset (since we don't have two datasets, one for training and for evaluate)
            ' in order to evaluate and get the model's accuracy metrics

            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============")

            'Measure cross-validation time
            Dim watchCrossValTime = System.Diagnostics.Stopwatch.StartNew()

            Dim crossValidationResults = mlContext.MulticlassClassification.CrossValidate(data:=trainingDataView, estimator:=trainingPipeline, numberOfFolds:=6, labelColumnName:="Label")

            'Stop measuring time
            watchCrossValTime.Stop()
            Dim elapsedMs As Long = watchCrossValTime.ElapsedMilliseconds
            Console.WriteLine($"Time Cross-Validating: {elapsedMs} miliSecs")

            ConsoleHelper.PrintMulticlassClassificationFoldsAverageMetrics(DirectCast(trainer, Object).ToString(), crossValidationResults)

            ' STEP 5: Train the model fitting to the DataSet
            Console.WriteLine("=============== Training the model ===============")

            'Measure training time
            Dim watch = System.Diagnostics.Stopwatch.StartNew()

            Dim trainedModel = trainingPipeline.Fit(trainingDataView)

            'Stop measuring time
            watch.Stop()
            Dim elapsedCrossValMs As Long = watch.ElapsedMilliseconds

            Console.WriteLine($"Time Training the model: {elapsedCrossValMs} miliSecs")

            ' (OPTIONAL) Try/test a single prediction with the "just-trained model" (Before saving the model)
            Dim issue As GitHubIssue = New GitHubIssue With {
                .ID = "Any-ID",
                .Title = "WebSockets communication is slow in my machine",
                .Description = "The WebSockets communication used under the covers by SignalR looks like is going slow in my development machine.."
            }
            ' Create prediction engine related to the loaded trained model
            Dim predEngine = mlContext.Model.CreatePredictionEngine(Of GitHubIssue, GitHubIssuePrediction)(trainedModel)
            'Score
            Dim prediction = predEngine.Predict(issue)
            Console.WriteLine($"=============== Single Prediction just-trained-model - Result: {prediction.Area} ===============")
            '

            ' STEP 6: Save/persist the trained model to a .ZIP file
            Console.WriteLine("=============== Saving the model to a file ===============")
            mlContext.Model.Save(trainedModel, trainingDataView.Schema, ModelPath)

            Common.ConsoleHelper.ConsoleWriteHeader("Training process finalized")
        End Sub

        Private Sub TestSingleLabelPrediction(modelFilePathName As String)
            Dim labeler = New Labeler(modelPath:=ModelPath)
            labeler.TestPredictionForSingleIssue()
        End Sub

        Private Async Function PredictLabelsAndUpdateGitHub(ModelPath As String) As Task
            Console.WriteLine(".............Retrieving Issues from GITHUB repo, predicting label/s and assigning predicted label/s......")

            Dim token = Configuration("GitHubToken")
            Dim repoOwner = Configuration("GitHubRepoOwner") 'IMPORTANT: This can be a GitHub User or a GitHub Organization
            Dim repoName = Configuration("GitHubRepoName")

            If String.IsNullOrEmpty(token) OrElse token Is "YOUR - GUID - GITHUB - TOKEN" OrElse String.IsNullOrEmpty(repoOwner) OrElse repoOwner Is "YOUR-REPO-USER-OWNER-OR-ORGANIZATION" OrElse String.IsNullOrEmpty(repoName) OrElse repoName Is "YOUR-REPO-SINGLE-NAME" Then
                Console.Error.WriteLine()
                Console.Error.WriteLine("Error: please configure the credentials in the appsettings.json file")
                Console.ReadLine()
                Return
            End If

            'This "Labeler" class could be used in a different End-User application (Web app, other console app, desktop app, etc.) 
            Dim labeler = New Labeler(ModelPath, repoOwner, repoName, token)

            Await labeler.LabelAllNewIssuesInGitHubRepo()

            Console.WriteLine("Labeling completed")
            Console.ReadLine()
        End Function

        Private Sub SetupAppConfiguration()
            Dim builder = (New ConfigurationBuilder).SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")

            Configuration = builder.Build()
        End Sub

        Public Function GetAbsolutePath(relativePath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

            Return fullPath
        End Function
    End Module
End Namespace
