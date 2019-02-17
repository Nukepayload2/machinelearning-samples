Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Data
Imports System.IO
Imports mnist.DataStructures

Namespace mnist
    Friend Class Program

        Private Shared ReadOnly TrainDataPath As String = Path.Combine(Environment.CurrentDirectory, "Data", "optdigits-train.csv")
        Private Shared ReadOnly TestDataPath As String = Path.Combine(Environment.CurrentDirectory, "Data", "optdigits-val.csv")
        Private Shared ReadOnly ModelPath As String = Path.Combine(Environment.CurrentDirectory, "MLModels", "Model.zip")

        Shared Sub Main(ByVal args() As String)
            Dim mLContext As New MLContext()
            Train(mLContext)
            TestSomePredictions(mLContext)

            Console.WriteLine("Hit any key to finish the app")
            Console.ReadKey()
        End Sub

        Public Shared Sub Train(ByVal mLContext As MLContext)
            Try
                ' STEP 1: Common data loading configuration
                Dim trainData = mLContext.Data.ReadFromTextFile(path:=TrainDataPath, columns:={
                    New TextLoader.Column(NameOf(InputData.PixelValues), DataKind.R4, 0, 63),
                    New TextLoader.Column("Number", DataKind.R4, 64)
                }, hasHeader:=False, separatorChar:=","c)


                Dim testData = mLContext.Data.ReadFromTextFile(path:=TestDataPath, columns:={
                    New TextLoader.Column(NameOf(InputData.PixelValues), DataKind.R4, 0, 63),
                    New TextLoader.Column("Number", DataKind.R4, 64)
                }, hasHeader:=False, separatorChar:=","c)

                ' STEP 2: Common data process configuration with pipeline data transformations
                Dim dataProcessPipeline = mLContext.Transforms.Concatenate(DefaultColumnNames.Features, NameOf(InputData.PixelValues)).AppendCacheCheckpoint(mLContext)

                ' STEP 3: Set the training algorithm, then create and config the modelBuilder
                Dim trainer = mLContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(labelColumn:="Number", featureColumn:=DefaultColumnNames.Features)
                Dim trainingPipeline = dataProcessPipeline.Append(trainer)

                ' STEP 4: Train the model fitting to the DataSet
                Dim watch = Stopwatch.StartNew()
                Console.WriteLine("=============== Training the model ===============")

                Dim trainedModel As ITransformer = trainingPipeline.Fit(trainData)
                Dim elapsedMs As Long = watch.ElapsedMilliseconds
                Console.WriteLine($"***** Training time: {elapsedMs \ 1000} seconds *****")

                Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
                Dim predictions = trainedModel.Transform(testData)
                Dim metrics = mLContext.MulticlassClassification.Evaluate(data:=predictions, label:="Number", score:=DefaultColumnNames.Score)

                Common.ConsoleHelper.PrintMultiClassClassificationMetrics(trainer.ToString(), metrics)

                Using fs = New FileStream(ModelPath, FileMode.Create, FileAccess.Write, FileShare.Write)
                    mLContext.Model.Save(trainedModel, fs)
                End Using

                Console.WriteLine("The model is saved to {0}", ModelPath)
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                'return null;
            End Try
        End Sub


        Private Shared Sub TestSomePredictions(ByVal mlContext As MLContext)
            Dim trainedModel As ITransformer
            Using stream = New FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                trainedModel = mlContext.Model.Load(stream)
            End Using

            ' Create prediction engine related to the loaded trained model
            Dim predEngine = trainedModel.CreatePredictionEngine(Of InputData, OutPutNum)(mlContext)

            'InputData data1 = SampleMNISTData.MNIST1;
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

            'InputData data2 = SampleMNISTData.MNIST2;
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

            'InputData data3 = SampleMNISTData.MNIST3;
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
