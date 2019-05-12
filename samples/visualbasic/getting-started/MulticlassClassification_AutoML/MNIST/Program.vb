Imports System
Imports System.IO
Imports System.Linq
Imports Common
Imports Microsoft.ML
Imports Microsoft.ML.AutoML
Imports Microsoft.ML.Data
Imports MNIST.DataStructures

Namespace MNIST
	Friend Class Program
		Private Shared BaseDatasetsRelativePath As String = "Data"
		Private Shared TrainDataRelativePath As String = $"{BaseDatasetsRelativePath}/optdigits-train.csv"
		Private Shared TestDataRelativePath As String = $"{BaseDatasetsRelativePath}/optdigits-test.csv"
		Private Shared TrainDataPath As String = GetAbsolutePath(TrainDataRelativePath)
		Private Shared TestDataPath As String = GetAbsolutePath(TestDataRelativePath)

		Private Shared BaseModelsRelativePath As String = "../../../MLModels"
		Private Shared ModelRelativePath As String = $"{BaseModelsRelativePath}/Model.zip"
		Private Shared ModelPath As String = GetAbsolutePath(ModelRelativePath)

		Private Shared ExperimentTime As UInteger = 60

		Shared Sub Main(args() As String)
			Dim mlContext As MLContext = New MLContext
			Train(mlContext)
			TestSomePredictions(mlContext)

			Console.WriteLine("Hit any key to finish the app")
			Console.ReadKey()
		End Sub

		Public Shared Sub Train(mlContext As MLContext)
			Try
				' STEP 1: Load the data
				Dim trainData = mlContext.Data.LoadFromTextFile(path:= TrainDataPath, columns := {
					New TextLoader.Column(NameOf(InputData.PixelValues), DataKind.Single, 0, 63),
					New TextLoader.Column("Number", DataKind.Single, 64)
				}, hasHeader := False, separatorChar := ","c)

				Dim testData = mlContext.Data.LoadFromTextFile(path:= TestDataPath, columns:= {
					New TextLoader.Column(NameOf(InputData.PixelValues), DataKind.Single, 0, 63),
					New TextLoader.Column("Number", DataKind.Single, 64)
				}, hasHeader:= False, separatorChar:= ","c)

				' STEP 2: Initialize our user-defined progress handler that AutoML will 
				' invoke after each model it produces and evaluates.
				Dim progressHandler = New MulticlassExperimentProgressHandler

				' STEP 3: Run an AutoML multiclass classification experiment
				ConsoleHelper.ConsoleWriteHeader("=============== Running AutoML experiment ===============")
				Console.WriteLine($"Running AutoML multiclass classification experiment for {ExperimentTime} seconds...")
				Dim experimentResult As ExperimentResult(Of MulticlassClassificationMetrics) = mlContext.Auto().CreateMulticlassClassificationExperiment(ExperimentTime).Execute(trainData, "Number", progressHandler:= progressHandler)

				' Print top models found by AutoML
				Console.WriteLine()
				PrintTopModels(experimentResult)

				' STEP 4: Evaluate the model and print metrics
				ConsoleHelper.ConsoleWriteHeader("===== Evaluating model's accuracy with test data =====")
				Dim bestRun As RunDetail(Of MulticlassClassificationMetrics) = experimentResult.BestRun
				Dim trainedModel As ITransformer = bestRun.Model
				Dim predictions = trainedModel.Transform(testData)
				Dim metrics = mlContext.MulticlassClassification.Evaluate(data:=predictions, labelColumnName:= "Number", scoreColumnName:= "Score")
				ConsoleHelper.PrintMulticlassClassificationMetrics_Renamed(bestRun.TrainerName, metrics)

				' STEP 5: Save/persist the trained model to a .ZIP file
				mlContext.Model.Save(trainedModel, trainData.Schema, ModelPath)

				Console.WriteLine("The model is saved to {0}", ModelPath)
			Catch ex As Exception
				Console.WriteLine(ex)
			End Try
		End Sub

		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function

		''' <summary>
		''' Print top models from AutoML experiment.
		''' </summary>
		Private Shared Sub PrintTopModels(experimentResult As ExperimentResult(Of MulticlassClassificationMetrics))
			' Get top few runs ranked by accuracy
			Dim topRuns = experimentResult.RunDetails.Where(Function(r) r.ValidationMetrics IsNot Nothing AndAlso Not Double.IsNaN(r.ValidationMetrics.MicroAccuracy)).OrderByDescending(Function(r) r.ValidationMetrics.MicroAccuracy).Take(3)

			Console.WriteLine("Top models ranked by accuracy --")
			ConsoleHelper.PrintMulticlassClassificationMetricsHeader()
			For i = 0 To topRuns.Count() - 1
				Dim run = topRuns.ElementAt(i)
				ConsoleHelper.PrintIterationMetrics(i + 1, run.TrainerName, run.ValidationMetrics, run.RuntimeInSeconds)
			Next i
		End Sub

		Private Shared Sub TestSomePredictions(mlContext As MLContext)
			ConsoleHelper.ConsoleWriteHeader("=============== Testing prediction engine ===============")

			Dim modelInputSchema As Object
			Dim trainedModel As ITransformer = mlContext.Model.Load(ModelPath, modelInputSchema)

			' Create prediction engine related to the loaded trained model
			Dim predEngine = mlContext.Model.CreatePredictionEngine(Of InputData, OutputData)(trainedModel)

			'InputData data1 = SampleMNISTData.MNIST1;
			Dim predictedResult1 = predEngine.Predict(SampleMNISTData.MNIST1)

			Console.WriteLine($"Actual: 7     Predicted probability:       zero:  {predictedResult1.Score(0):0.####}")
			Console.WriteLine($"                                           One :  {predictedResult1.Score(1):0.####}")
			Console.WriteLine($"                                           two:   {predictedResult1.Score(2):0.####}")
			Console.WriteLine($"                                           three: {predictedResult1.Score(3):0.####}")
			Console.WriteLine($"                                           four:  {predictedResult1.Score(4):0.####}")
			Console.WriteLine($"                                           five:  {predictedResult1.Score(5):0.####}")
			Console.WriteLine($"                                           six:   {predictedResult1.Score(6):0.####}")
			Console.WriteLine($"                                           seven: {predictedResult1.Score(7):0.####}")
			Console.WriteLine($"                                           eight: {predictedResult1.Score(8):0.####}")
			Console.WriteLine($"                                           nine:  {predictedResult1.Score(9):0.####}")
			Console.WriteLine()

			Dim predictedResult2 = predEngine.Predict(SampleMNISTData.MNIST2)

			Console.WriteLine($"Actual: 1     Predicted probability:       zero:  {predictedResult2.Score(0):0.####}")
			Console.WriteLine($"                                           One :  {predictedResult2.Score(1):0.####}")
			Console.WriteLine($"                                           two:   {predictedResult2.Score(2):0.####}")
			Console.WriteLine($"                                           three: {predictedResult2.Score(3):0.####}")
			Console.WriteLine($"                                           four:  {predictedResult2.Score(4):0.####}")
			Console.WriteLine($"                                           five:  {predictedResult2.Score(5):0.####}")
			Console.WriteLine($"                                           six:   {predictedResult2.Score(6):0.####}")
			Console.WriteLine($"                                           seven: {predictedResult2.Score(7):0.####}")
			Console.WriteLine($"                                           eight: {predictedResult2.Score(8):0.####}")
			Console.WriteLine($"                                           nine:  {predictedResult2.Score(9):0.####}")
			Console.WriteLine()
		End Sub
	End Class
End Namespace
