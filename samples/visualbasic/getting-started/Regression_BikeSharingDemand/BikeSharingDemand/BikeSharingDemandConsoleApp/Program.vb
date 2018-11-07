﻿Imports System
Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime
Imports Microsoft.ML.Runtime.Api

Imports BikeSharingDemand.DataStructures

Namespace BikeSharingDemand
	Friend Module Program
		Private ModelsLocation As String = "../../../../MLModels"

		Private DatasetsLocation As String = "../../../../Data"
		Private TrainingDataLocation As String = $"{DatasetsLocation}/hour_train.csv"
		Private TestDataLocation As String = $"{DatasetsLocation}/hour_test.csv"

		Sub Main(ByVal args() As String)
			' Create MLContext to be shared across the model creation workflow objects 
			' Set a random seed for repeatable/deterministic results across multiple trainings.
			Dim mlContext = New MLContext(seed:= 0)

			' 1. Common data loading
			Dim dataLoader As New DataLoader(mlContext)
			Dim trainingDataView = dataLoader.GetDataView(TrainingDataLocation)
			Dim testDataView = dataLoader.GetDataView(TestDataLocation)

			' 2. Common data pre-process with pipeline data transformations
			Dim dataProcessor = New DataProcessor(mlContext)
			Dim dataProcessPipeline = dataProcessor.DataProcessPipeline

			' (Optional) Peek data in training DataView after applying the ProcessPipeline's transformations  
			Common.ConsoleHelper.PeekDataViewInConsole(Of DemandObservation)(mlContext, trainingDataView, dataProcessPipeline, 10)
			Common.ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", trainingDataView, dataProcessPipeline, 10)

            ' Definition of regression trainers/algorithms to use
            'var regressionLearners = new (string name, IEstimator<ITransformer> value)[]
            Dim regressionLearners As (name As String, value As IEstimator(Of ITransformer))() = {("FastTree", mlContext.Regression.Trainers.FastTree()), ("Poisson", mlContext.Regression.Trainers.PoissonRegression()), ("SDCA", mlContext.Regression.Trainers.StochasticDualCoordinateAscent()), ("FastTreeTweedie", mlContext.Regression.Trainers.FastTreeTweedie())}

            ' 3. Phase for Training, Evaluation and model file persistence
            ' Per each regression trainer: Train, Evaluate, and Save a different model
            For Each learner In regressionLearners
				Console.WriteLine("================== Training model ==================")
				Dim modelBuilder = New Common.ModelBuilder(Of DemandObservation,DemandPrediction)(mlContext, dataProcessPipeline)
				modelBuilder.AddTrainer(learner.value)
				Dim trainedModel = modelBuilder.Train(trainingDataView)

				Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
				Dim metrics = modelBuilder.EvaluateRegressionModel(testDataView, "Count", "Score")
				Common.ConsoleHelper.PrintRegressionMetrics(learner.name, metrics)

				'Save the model file that can be used by any application
				modelBuilder.SaveModelAsFile($"{ModelsLocation}/{learner.name}Model.zip")
			Next learner

			' 4. Try/test Predictions with the created models
			' The following test predictions could be implemented/deployed in a different application (production apps)
			' that's why it is seggregated from the previous loop
			' For each trained model, test 10 predictions           
			For Each learner In regressionLearners
				'Load current model
				Dim modelScorer = New Common.ModelScorer(Of DemandObservation, DemandPrediction)(mlContext)
				modelScorer.LoadModelFromZipFile($"{ModelsLocation}/{learner.name}Model.zip")

				Console.WriteLine($"================== Visualize/test 10 predictions for model {learner.name}Model.zip ==================")
				'Visualize 10 tests comparing prediction with actual/observed values from the test dataset
				ModelScoringTester.VisualizeSomePredictions(mlContext,learner.name, TestDataLocation, modelScorer, 10)
			Next learner

			Common.ConsoleHelper.ConsolePressAnyKey()

		End Sub
	End Module
End Namespace