Imports System.IO

Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports MulticlassClassification_Iris.DataStructures

Namespace MulticlassClassification_Iris
    Public Module Program
        Private ReadOnly Property AppPath As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private BaseDatasetsRelativePath As String = "../../../../Data"
        Private TrainDataRelativePath As String = $"{BaseDatasetsRelativePath}/iris-train.txt"
        Private TestDataRelativePath As String = $"{BaseDatasetsRelativePath}/iris-test.txt"

        Private TrainDataPath As String = GetAbsolutePath(TrainDataRelativePath)
        Private TestDataPath As String = GetAbsolutePath(TestDataRelativePath)

        Private BaseModelsRelativePath As String = "../../../../MLModels"
        Private ModelRelativePath As String = $"{BaseModelsRelativePath}/IrisClassificationModel.zip"

        Private ModelPath As String = GetAbsolutePath(ModelRelativePath)

        Public Sub Main(args() As String)
            ' Create MLContext to be shared across the model creation workflow objects 
            ' Set a random seed for repeatable/deterministic results across multiple trainings.
            Dim mlContext = New MLContext(seed:=0)

            '1.
            BuildTrainEvaluateAndSaveModel(mlContext)

            '2.
            TestSomePredictions(mlContext)

            Console.WriteLine("=============== End of process, hit any key to finish ===============")
            Console.ReadKey()
        End Sub

        Private Sub BuildTrainEvaluateAndSaveModel(mlContext As MLContext)
            ' STEP 1: Common data loading configuration
            Dim trainingDataView = mlContext.Data.LoadFromTextFile(Of IrisData)(TrainDataPath, hasHeader:=True)
            Dim testDataView = mlContext.Data.LoadFromTextFile(Of IrisData)(TestDataPath, hasHeader:=True)


            ' STEP 2: Common data process configuration with pipeline data transformations
            Dim dataProcessPipeline = mlContext.Transforms.Concatenate(DefaultColumnNames.Features, NameOf(IrisData.SepalLength), NameOf(IrisData.SepalWidth), NameOf(IrisData.PetalLength), NameOf(IrisData.PetalWidth)).AppendCacheCheckpoint(mlContext)
            ' Use in-memory cache for small/medium datasets to lower training time. 
            ' Do NOT use it (remove .AppendCacheCheckpoint()) when handling very large datasets. 

            ' STEP 3: Set the training algorithm, then append the trainer to the pipeline  
            Dim trainer = mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(labelColumnName:=DefaultColumnNames.Label, featureColumnName:=DefaultColumnNames.Features)
            Dim trainingPipeline = dataProcessPipeline.Append(trainer)

            ' STEP 4: Train the model fitting to the DataSet

            'Measure training time
            Dim watch = System.Diagnostics.Stopwatch.StartNew()

            Console.WriteLine("=============== Training the model ===============")
            Dim trainedModel As ITransformer = trainingPipeline.Fit(trainingDataView)

            'Stop measuring time
            watch.Stop()
            Dim elapsedMs As Long = watch.ElapsedMilliseconds
            Console.WriteLine($"***** Training time: {elapsedMs \ 1000} seconds *****")


            ' STEP 5: Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
            Dim predictions = trainedModel.Transform(testDataView)
            Dim metrics = mlContext.MulticlassClassification.Evaluate(predictions, DefaultColumnNames.Label, DefaultColumnNames.Score)

            Common.ConsoleHelper.PrintMultiClassClassificationMetrics(trainer.ToString(), metrics)

            ' STEP 6: Save/persist the trained model to a .ZIP file
            Using fs = New FileStream(ModelPath, FileMode.Create, FileAccess.Write, FileShare.Write)
                mlContext.Model.Save(trainedModel, fs)
            End Using

            Console.WriteLine("The model is saved to {0}", ModelPath)
        End Sub

        Private Sub TestSomePredictions(mlContext As MLContext)
            'Test Classification Predictions with some hard-coded samples 

            Dim trainedModel As ITransformer
            Using stream = New FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                trainedModel = mlContext.Model.Load(stream)
            End Using

            ' Create prediction engine related to the loaded trained model
            Dim predEngine = trainedModel.CreatePredictionEngine(Of IrisData, IrisPrediction)(mlContext)

            'Score sample 1
            Dim resultprediction1 = predEngine.Predict(SampleIrisData.Iris1)

            Console.WriteLine($"Actual: setosa.     Predicted probability: setosa:      {resultprediction1.Score(0):0.####}")
            Console.WriteLine($"                                           versicolor:  {resultprediction1.Score(1):0.####}")
            Console.WriteLine($"                                           virginica:   {resultprediction1.Score(2):0.####}")
            Console.WriteLine()

            'Score sample 2
            Dim resultprediction2 = predEngine.Predict(SampleIrisData.Iris2)

            Console.WriteLine($"Actual: Virginica.     Predicted probability: setosa:      {resultprediction2.Score(0):0.####}")
            Console.WriteLine($"                                           versicolor:  {resultprediction2.Score(1):0.####}")
            Console.WriteLine($"                                           virginica:   {resultprediction2.Score(2):0.####}")
            Console.WriteLine()

            'Score sample 3
            Dim resultprediction3 = predEngine.Predict(SampleIrisData.Iris3)

            Console.WriteLine($"Actual: setosa.     Predicted probability: setosa:      {resultprediction3.Score(0):0.####}")
            Console.WriteLine($"                                           versicolor:  {resultprediction3.Score(1):0.####}")
            Console.WriteLine($"                                           virginica:   {resultprediction3.Score(2):0.####}")
            Console.WriteLine()

        End Sub

        Public Function GetAbsolutePath(relativePath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

            Return fullPath
        End Function
    End Module
End Namespace
