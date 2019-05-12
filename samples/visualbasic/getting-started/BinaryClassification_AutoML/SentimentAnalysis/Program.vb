Imports System
Imports System.IO
Imports System.Linq
Imports Common
Imports Microsoft.ML
Imports Microsoft.ML.AutoML
Imports Microsoft.ML.Data
Imports SentimentAnalysis.DataStructures

Namespace SentimentAnalysis
	Friend Module Program
		Private ReadOnly BaseDatasetsRelativePath As String = "Data"
		Private ReadOnly TrainDataRelativePath As String = $"{BaseDatasetsRelativePath}/wikipedia-detox-250-line-data.tsv"
		Private ReadOnly TestDataRelativePath As String = $"{BaseDatasetsRelativePath}/wikipedia-detox-250-line-test.tsv"
		Private TrainDataPath As String = GetAbsolutePath(TrainDataRelativePath)
		Private TestDataPath As String = GetAbsolutePath(TestDataRelativePath)

		Private ReadOnly BaseModelsRelativePath As String = "../../../MLModels"
		Private ReadOnly ModelRelativePath As String = $"{BaseModelsRelativePath}/SentimentModel.zip"
		Private ModelPath As String = GetAbsolutePath(ModelRelativePath)

		Private ExperimentTime As UInteger = 60

		Sub Main(args() As String)
			Dim mlContext = New MLContext

			' Create, train, evaluate and save a model
			BuildTrainEvaluateAndSaveModel(mlContext)

			' Make a single test prediction loading the model from .ZIP file
			TestSinglePrediction(mlContext)

			ConsoleHelper.ConsoleWriteHeader("=============== End of process, hit any key to finish ===============")
			Console.ReadKey()
		End Sub

		Private Function BuildTrainEvaluateAndSaveModel(mlContext As MLContext) As ITransformer
			' STEP 1: Load data
			Dim trainingDataView As IDataView = mlContext.Data.LoadFromTextFile(Of SentimentIssue)(TrainDataPath, hasHeader:= True)
			Dim testDataView As IDataView = mlContext.Data.LoadFromTextFile(Of SentimentIssue)(TestDataPath, hasHeader:= True)

			' STEP 2: Display first few rows of training data
			ConsoleHelper.ShowDataViewInConsole(mlContext, trainingDataView)

			' STEP 3: Initialize our user-defined progress handler that AutoML will 
			' invoke after each model it produces and evaluates.
			Dim progressHandler = New BinaryExperimentProgressHandler

			' STEP 4: Run AutoML binary classification experiment
			ConsoleHelper.ConsoleWriteHeader("=============== Running AutoML experiment ===============")
			Console.WriteLine($"Running AutoML binary classification experiment for {ExperimentTime} seconds...")
			Dim experimentResult As ExperimentResult(Of BinaryClassificationMetrics) = mlContext.Auto().CreateBinaryClassificationExperiment(ExperimentTime).Execute(trainingDataView, progressHandler:= progressHandler)

			' Print top models found by AutoML
			Console.WriteLine()
			PrintTopModels(experimentResult)

			' STEP 5: Evaluate the model and print metrics
			ConsoleHelper.ConsoleWriteHeader("=============== Evaluating model's accuracy with test data ===============")
			Dim bestRun As RunDetail(Of BinaryClassificationMetrics) = experimentResult.BestRun
			Dim trainedModel As ITransformer = bestRun.Model
			Dim predictions = trainedModel.Transform(testDataView)
			Dim metrics = mlContext.BinaryClassification.EvaluateNonCalibrated(data:=predictions, scoreColumnName:= "Score")
			ConsoleHelper.PrintBinaryClassificationMetrics(bestRun.TrainerName, metrics)

			' STEP 6: Save/persist the trained model to a .ZIP file
			mlContext.Model.Save(trainedModel, trainingDataView.Schema, ModelPath)

			Console.WriteLine("The model is saved to {0}", ModelPath)

			Return trainedModel
		End Function

		' (OPTIONAL) Try/test a single prediction by loading the model from the file, first.
		Private Sub TestSinglePrediction(mlContext As MLContext)
			ConsoleHelper.ConsoleWriteHeader("=============== Testing prediction engine ===============")
			Dim sampleStatement As SentimentIssue = New SentimentIssue With {.Text = "This is a very rude movie"}

			Dim modelInputSchema As Object
			Dim trainedModel As ITransformer = mlContext.Model.Load(ModelPath, modelInputSchema)
			Console.WriteLine($"=============== Loaded Model OK  ===============")

			' Create prediction engine related to the loaded trained model
			Dim predEngine= mlContext.Model.CreatePredictionEngine(Of SentimentIssue, SentimentPrediction)(trainedModel)
			Console.WriteLine($"=============== Created Prediction Engine OK  ===============")
			' Score
			Dim predictedResult = predEngine.Predict(sampleStatement)

			Console.WriteLine($"=============== Single Prediction  ===============")
			Console.WriteLine($"Text: {sampleStatement.Text} | Prediction: {(If(Convert.ToBoolean(predictedResult.Prediction), "Toxic", "Non Toxic"))} sentiment")
			Console.WriteLine($"==================================================")
		End Sub

		Public Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function

		''' <summary>
		''' Prints top models from AutoML experiment.
		''' </summary>
		Private Sub PrintTopModels(experimentResult As ExperimentResult(Of BinaryClassificationMetrics))
			' Get top few runs ranked by accuracy
			Dim topRuns = experimentResult.RunDetails.Where(Function(r) r.ValidationMetrics IsNot Nothing AndAlso Not Double.IsNaN(r.ValidationMetrics.Accuracy)).OrderByDescending(Function(r) r.ValidationMetrics.Accuracy).Take(3)

			Console.WriteLine("Top models ranked by accuracy --")
			ConsoleHelper.PrintBinaryClassificationMetricsHeader()
			For i = 0 To topRuns.Count() - 1
				Dim run = topRuns.ElementAt(i)
				ConsoleHelper.PrintIterationMetrics(i + 1, run.TrainerName, run.ValidationMetrics, run.RuntimeInSeconds)
			Next i
		End Sub
	End Module
End Namespace