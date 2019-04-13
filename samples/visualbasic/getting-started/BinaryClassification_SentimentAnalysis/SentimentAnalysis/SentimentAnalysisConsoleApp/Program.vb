Imports System
Imports System.IO
Imports Microsoft.ML
Imports SentimentAnalysisConsoleApp.DataStructures
Imports Common
Imports Microsoft.ML.DataOperationsCatalog

Namespace SentimentAnalysisConsoleApp
	Friend Module Program
		Private ReadOnly Property AppPath As String
			Get
				Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
			End Get
		End Property

		Private ReadOnly BaseDatasetsRelativePath As String = "../../../../Data"
		Private ReadOnly DataRelativePath As String = $"{BaseDatasetsRelativePath}/wikiDetoxAnnotated40kRows.tsv"

		Private DataPath As String = GetAbsolutePath(DataRelativePath)

		Private ReadOnly BaseModelsRelativePath As String = "../../../../MLModels"
		Private ReadOnly ModelRelativePath As String = $"{BaseModelsRelativePath}/SentimentModel.zip"

		Private ModelPath As String = GetAbsolutePath(ModelRelativePath)

		Sub Main(args() As String)
			'Create MLContext to be shared across the model creation workflow objects 
			'Set a random seed for repeatable/deterministic results across multiple trainings.
			Dim mlContext = New MLContext(seed:= 1)

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
			Dim dataView As IDataView = mlContext.Data.LoadFromTextFile(Of SentimentIssue)(DataPath, hasHeader:= True)

			Dim trainTestSplit As TrainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction:= 0.2)
			Dim trainingData As IDataView = trainTestSplit.TrainSet
			Dim testData As IDataView = trainTestSplit.TestSet

			' STEP 2: Common data process configuration with pipeline data transformations          
			Dim dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName:= "Features", inputColumnName:=NameOf(SentimentIssue.Text))

			' (OPTIONAL) Peek data (such as 2 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
			ConsoleHelper.PeekDataViewInConsole(mlContext, dataView, dataProcessPipeline, 2)
			'Peak the transformed features column
			'ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", dataView, dataProcessPipeline, 1);

			' STEP 3: Set the training algorithm, then create and config the modelBuilder                            
			Dim trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName:= "Label", featureColumnName:= "Features")
			Dim trainingPipeline = dataProcessPipeline.Append(trainer)

			'Measure training time
			Dim watch = System.Diagnostics.Stopwatch.StartNew()

			' STEP 4: Train the model fitting to the DataSet
			Console.WriteLine("=============== Training the model ===============")
			Dim trainedModel As ITransformer = trainingPipeline.Fit(trainingData)

			'Stop measuring time
			watch.Stop()
			Dim elapsedMs As Long = watch.ElapsedMilliseconds
			Console.WriteLine($"***** Training time: {elapsedMs \ 1000} seconds *****")

			' STEP 5: Evaluate the model and show accuracy stats
			Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
			Dim predictions = trainedModel.Transform(testData)
			Dim metrics = mlContext.BinaryClassification.Evaluate(data:=predictions, labelColumnName:= "Label", scoreColumnName:= "Score")

			ConsoleHelper.PrintBinaryClassificationMetrics(trainer.ToString(), metrics)

			' STEP 6: Save/persist the trained model to a .ZIP file
			mlContext.Model.Save(trainedModel, trainingData.Schema, ModelPath)

			Console.WriteLine("The model is saved to {0}", ModelPath)

			Return trainedModel
		End Function

		' (OPTIONAL) Try/test a single prediction by loding the model from the file, first.
		Private Sub TestSinglePrediction(mlContext As MLContext)
			Dim sampleStatement As SentimentIssue = New SentimentIssue With {.Text = "This is a very rude movie"}

			Dim modelInputSchema As Object
			Dim trainedModel As ITransformer = mlContext.Model.Load(ModelPath, modelInputSchema)

			' Create prediction engine related to the loaded trained model
			Dim predEngine= mlContext.Model.CreatePredictionEngine(Of SentimentIssue, SentimentPrediction)(trainedModel)

			'Score
			Dim resultprediction = predEngine.Predict(sampleStatement)

			Console.WriteLine($"=============== Single Prediction  ===============")
			Console.WriteLine($"Text: {sampleStatement.Text} | Prediction: {(If(Convert.ToBoolean(resultprediction.Prediction), "Toxic", "Non Toxic"))} sentiment | Probability of being toxic: {resultprediction.Probability} ")
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