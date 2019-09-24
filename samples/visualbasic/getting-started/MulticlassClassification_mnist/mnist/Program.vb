Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports System.IO
Imports mnist.DataStructures
Imports Microsoft.ML.Transforms

Namespace mnist
    Friend Class Program
        Private Shared BaseDatasetsRelativePath As String = "../../../Data"
        Private Shared TrianDataRealtivePath As String = $"{BaseDatasetsRelativePath}/optdigits-train.csv"
        Private Shared TestDataRealtivePath As String = $"{BaseDatasetsRelativePath}/optdigits-val.csv"

        Private Shared TrainDataPath As String = GetAbsolutePath(TrianDataRealtivePath)
        Private Shared TestDataPath As String = GetAbsolutePath(TestDataRealtivePath)

        Private Shared BaseModelsRelativePath As String = "../../../MLModels"
        Private Shared ModelRelativePath As String = $"{BaseModelsRelativePath}/Model.zip"

        Private Shared ModelPath As String = GetAbsolutePath(ModelRelativePath)

        Shared Sub Main(args() As String)
            Dim mlContext As MLContext = New MLContext
            Train(mlContext)
            TestSomePredictions(mlContext)

            Console.WriteLine("Hit any key to finish the app")
            Console.ReadKey()
        End Sub

        Public Shared Sub Train(mlContext As MLContext)
            Try
                ' STEP 1: Common data loading configuration
                Dim trainData = mlContext.Data.LoadFromTextFile(path:=TrainDataPath, columns:={
                    New TextLoader.Column(NameOf(InputData.PixelValues), DataKind.Single, 0, 63),
                    New TextLoader.Column("Number", DataKind.Single, 64)
                }, hasHeader:=False, separatorChar:=","c)


                Dim testData = mlContext.Data.LoadFromTextFile(path:=TestDataPath, columns:={
                    New TextLoader.Column(NameOf(InputData.PixelValues), DataKind.Single, 0, 63),
                    New TextLoader.Column("Number", DataKind.Single, 64)
                }, hasHeader:=False, separatorChar:=","c)

                ' STEP 2: Common data process configuration with pipeline data transformations
                ' Use in-memory cache for small/medium datasets to lower training time. Do NOT use it (remove .AppendCacheCheckpoint()) when handling very large datasets.
                Dim dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Number", keyOrdinality:=ValueToKeyMappingEstimator.KeyOrdinality.ByValue).Append(mlContext.Transforms.Concatenate("Features", NameOf(InputData.PixelValues)).AppendCacheCheckpoint(mlContext))

                ' STEP 3: Set the training algorithm, then create and config the modelBuilder
                Dim trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName:="Label", featureColumnName:="Features")
                Dim trainingPipeline = dataProcessPipeline.Append(trainer).Append(mlContext.Transforms.Conversion.MapKeyToValue("Number", "Label"))

                ' STEP 4: Train the model fitting to the DataSet

                Console.WriteLine("=============== Training the model ===============")
                Dim trainedModel As ITransformer = trainingPipeline.Fit(trainData)

                Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
                Dim predictions = trainedModel.Transform(testData)
                Dim metrics = mlContext.MulticlassClassification.Evaluate(data:=predictions, labelColumnName:="Number", scoreColumnName:="Score")

                Common.ConsoleHelper.PrintMultiClassClassificationMetrics_Renamed(trainer.ToString(), metrics)

                mlContext.Model.Save(trainedModel, trainData.Schema, ModelPath)

                Console.WriteLine("The model is saved to {0}", ModelPath)
            Catch ex As Exception
                Console.WriteLine(ex.ToString())
                'return null;
            End Try
        End Sub

        Public Shared Function GetAbsolutePath(relativePath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

            Return fullPath
        End Function

        Private Shared Sub TestSomePredictions(mlContext As MLContext)
            Dim modelInputSchema As Object
            Dim trainedModel As ITransformer = mlContext.Model.Load(ModelPath, modelInputSchema)

            ' Create prediction engine related to the loaded trained model
            Dim predEngine = mlContext.Model.CreatePredictionEngine(Of InputData, OutPutData)(trainedModel)

            Dim resultprediction1 = predEngine.Predict(SampleMNISTData.MNIST1)

            Console.WriteLine($"Actual: 1     Predicted probability:       zero:  {resultprediction1.Score(0):0.####}")
            Console.WriteLine($"                                           One :  {resultprediction1.Score(1):0.####}")
            Console.WriteLine($"                                           two:   {resultprediction1.Score(2):0.####}")
            Console.WriteLine($"                                           three: {resultprediction1.Score(3):0.####}")
            Console.WriteLine($"                                           four:  {resultprediction1.Score(4):0.####}")
            Console.WriteLine($"                                           five:  {resultprediction1.Score(5):0.####}")
            Console.WriteLine($"                                           six:   {resultprediction1.Score(6):0.####}")
            Console.WriteLine($"                                           seven: {resultprediction1.Score(7):0.####}")
            Console.WriteLine($"                                           eight: {resultprediction1.Score(8):0.####}")
            Console.WriteLine($"                                           nine:  {resultprediction1.Score(9):0.####}")
            Console.WriteLine()

            Dim resultprediction2 = predEngine.Predict(SampleMNISTData.MNIST2)

            Console.WriteLine($"Actual: 7     Predicted probability:       zero:  {resultprediction2.Score(0):0.####}")
            Console.WriteLine($"                                           One :  {resultprediction2.Score(1):0.####}")
            Console.WriteLine($"                                           two:   {resultprediction2.Score(2):0.####}")
            Console.WriteLine($"                                           three: {resultprediction2.Score(3):0.####}")
            Console.WriteLine($"                                           four:  {resultprediction2.Score(4):0.####}")
            Console.WriteLine($"                                           five:  {resultprediction2.Score(5):0.####}")
            Console.WriteLine($"                                           six:   {resultprediction2.Score(6):0.####}")
            Console.WriteLine($"                                           seven: {resultprediction2.Score(7):0.####}")
            Console.WriteLine($"                                           eight: {resultprediction2.Score(8):0.####}")
            Console.WriteLine($"                                           nine:  {resultprediction2.Score(9):0.####}")
            Console.WriteLine()

            Dim resultprediction3 = predEngine.Predict(SampleMNISTData.MNIST3)

            Console.WriteLine($"Actual: 9     Predicted probability:       zero:  {resultprediction3.Score(0):0.####}")
            Console.WriteLine($"                                           One :  {resultprediction3.Score(1):0.####}")
            Console.WriteLine($"                                           two:   {resultprediction3.Score(2):0.####}")
            Console.WriteLine($"                                           three: {resultprediction3.Score(3):0.####}")
            Console.WriteLine($"                                           four:  {resultprediction3.Score(4):0.####}")
            Console.WriteLine($"                                           five:  {resultprediction3.Score(5):0.####}")
            Console.WriteLine($"                                           six:   {resultprediction3.Score(6):0.####}")
            Console.WriteLine($"                                           seven: {resultprediction3.Score(7):0.####}")
            Console.WriteLine($"                                           eight: {resultprediction3.Score(8):0.####}")
            Console.WriteLine($"                                           nine:  {resultprediction3.Score(9):0.####}")
            Console.WriteLine()
        End Sub
    End Class
End Namespace
