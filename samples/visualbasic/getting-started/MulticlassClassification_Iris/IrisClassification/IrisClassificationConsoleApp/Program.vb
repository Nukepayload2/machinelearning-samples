Imports System
Imports System.IO

Imports Microsoft.ML.Runtime.Learners
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML
Imports MulticlassClassification_Iris.DataStructures
Imports Microsoft.ML.Data

Namespace MulticlassClassification_Iris
    Public Module Program
        Private ReadOnly Property AppPath() As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private BaseDatasetsLocation As String = "../../../../Data"
        Private TrainDataPath As String = $"{BaseDatasetsLocation}/iris-train.txt"
        Private TestDataPath As String = $"{BaseDatasetsLocation}/iris-test.txt"

        Private BaseModelsPath As String = "../../../../MLModels"
        Private ModelPath As String = $"{BaseModelsPath}/IrisClassificationModel.zip"

        Public Sub Main(ByVal args() As String)
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

        Private Sub BuildTrainEvaluateAndSaveModel(ByVal mlContext As MLContext)
            ' STEP 1: Common data loading configuration
            Dim textLoader = mlContext.Data.CreateTextReader(New TextLoader.Arguments() With {
                .Separator = vbTab,
                .HasHeader = True,
                .Column = {
                    New TextLoader.Column("Label", DataKind.R4, 0),
                    New TextLoader.Column("SepalLength", DataKind.R4, 1),
                    New TextLoader.Column("SepalWidth", DataKind.R4, 2),
                    New TextLoader.Column("PetalLength", DataKind.R4, 3),
                    New TextLoader.Column("PetalWidth", DataKind.R4, 4)
                }
            })
            Dim trainingDataView = textLoader.Read(TrainDataPath)
            Dim testDataView = textLoader.Read(TestDataPath)

            ' STEP 2: Common data process configuration with pipeline data transformations
            Dim dataProcessPipeline = mlContext.Transforms.Concatenate("Features", "SepalLength", "SepalWidth", "PetalLength", "PetalWidth")

            ' STEP 3: Set the training algorithm, then create and config the modelBuilder                            
            Dim modelBuilder = New Common.ModelBuilder(Of IrisData, IrisPrediction)(mlContext, dataProcessPipeline)
            ' We apply our selected Trainer 
            Dim trainer = mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(labelColumn:="Label", featureColumn:="Features")
            modelBuilder.AddTrainer(trainer)

            ' STEP 4: Train the model fitting to the DataSet
            'The pipeline is trained on the dataset that has been loaded and transformed.
            Console.WriteLine("=============== Training the model ===============")
            modelBuilder.Train(trainingDataView)

            ' STEP 5: Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
            Dim metrics = modelBuilder.EvaluateMultiClassClassificationModel(testDataView, "Label")
            Common.ConsoleHelper.PrintMultiClassClassificationMetrics(trainer.ToString(), metrics)

            ' STEP 6: Save/persist the trained model to a .ZIP file
            Console.WriteLine("=============== Saving the model to a file ===============")
            modelBuilder.SaveModelAsFile(ModelPath)
        End Sub

        Private Sub TestSomePredictions(ByVal mlContext As MLContext)
            'Test Classification Predictions with some hard-coded samples 

            Dim modelScorer = New Common.ModelScorer(Of IrisData, IrisPrediction)(mlContext)
            modelScorer.LoadModelFromZipFile(ModelPath)

            Dim prediction = modelScorer.PredictSingle(SampleIrisData.Iris1)
            Console.WriteLine($"Actual: setosa.     Predicted probability: setosa:      {prediction.Score(0):0.####}")
            Console.WriteLine($"                                           versicolor:  {prediction.Score(1):0.####}")
            Console.WriteLine($"                                           virginica:   {prediction.Score(2):0.####}")
            Console.WriteLine()

            prediction = modelScorer.PredictSingle(SampleIrisData.Iris2)
            Console.WriteLine($"Actual: virginica.  Predicted probability: setosa:      {prediction.Score(0):0.####}")
            Console.WriteLine($"                                           versicolor:  {prediction.Score(1):0.####}")
            Console.WriteLine($"                                           virginica:   {prediction.Score(2):0.####}")
            Console.WriteLine()

            prediction = modelScorer.PredictSingle(SampleIrisData.Iris3)
            Console.WriteLine($"Actual: versicolor. Predicted probability: setosa:      {prediction.Score(0):0.####}")
            Console.WriteLine($"                                           versicolor:  {prediction.Score(1):0.####}")
            Console.WriteLine($"                                           virginica:   {prediction.Score(2):0.####}")
            Console.WriteLine()

        End Sub
    End Module
End Namespace