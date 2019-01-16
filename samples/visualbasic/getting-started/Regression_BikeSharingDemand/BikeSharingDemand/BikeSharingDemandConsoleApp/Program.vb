﻿Imports System
Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports System.IO
Imports Microsoft.ML.Data

Imports BikeSharingDemand.DataStructures
Imports Common

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

			' 1. Common data loading configuration
			Dim textLoader = mlContext.Data.CreateTextReader(columns:= {
				New TextLoader.Column("Season", DataKind.R4, 2),
				New TextLoader.Column("Year", DataKind.R4, 3),
				New TextLoader.Column("Month", DataKind.R4, 4),
				New TextLoader.Column("Hour", DataKind.R4, 5),
				New TextLoader.Column("Holiday", DataKind.R4, 6),
				New TextLoader.Column("Weekday", DataKind.R4, 7),
				New TextLoader.Column("WorkingDay", DataKind.R4, 8),
				New TextLoader.Column("Weather", DataKind.R4, 9),
				New TextLoader.Column("Temperature", DataKind.R4, 10),
				New TextLoader.Column("NormalizedTemperature", DataKind.R4, 11),
				New TextLoader.Column("Humidity", DataKind.R4, 12),
				New TextLoader.Column("Windspeed", DataKind.R4, 13),
				New TextLoader.Column("Count", DataKind.R4, 16)
			}, hasHeader:= True, separatorChar:= ","c)

			Dim trainingDataView = textLoader.Read(TrainingDataLocation)
			Dim testDataView = textLoader.Read(TestDataLocation)

			' 2. Common data pre-process with pipeline data transformations
			Dim dataProcessPipeline = mlContext.Transforms.CopyColumns("Count", "Label").Append(mlContext.Transforms.Concatenate("Features", "Season", "Year", "Month", "Hour", "Holiday", "Weekday", "Weather", "Temperature", "NormalizedTemperature", "Humidity", "Windspeed"))

			' (Optional) Peek data in training DataView after applying the ProcessPipeline's transformations  
			Common.ConsoleHelper.PeekDataViewInConsole(Of DemandObservation)(mlContext, trainingDataView, dataProcessPipeline, 10)
			Common.ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", trainingDataView, dataProcessPipeline, 10)

            ' Definition of regression trainers/algorithms to use
            Dim regressionLearners As (name As String, value As IEstimator(Of ITransformer))() = {
                ("FastTree", mlContext.Regression.Trainers.FastTree()),
                ("Poisson", mlContext.Regression.Trainers.PoissonRegression()),
                ("SDCA", mlContext.Regression.Trainers.StochasticDualCoordinateAscent()),
                ("FastTreeTweedie", mlContext.Regression.Trainers.FastTreeTweedie())
            }

            ' 3. Phase for Training, Evaluation and model file persistence
            ' Per each regression trainer: Train, Evaluate, and Save a different model
            For Each learner In regressionLearners
				Console.WriteLine("=============== Training the current model ===============")
				Dim trainingPipeline = dataProcessPipeline.Append(learner.value)
				Dim trainedModel = trainingPipeline.Fit(trainingDataView)

				Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
				Dim predictions As IDataView = trainedModel.Transform(testDataView)
				Dim metrics = mlContext.Regression.Evaluate(predictions, label:= "Count", score:= "Score")
				ConsoleHelper.PrintRegressionMetrics(learner.value.ToString(), metrics)

				'Save the model file that can be used by any application
				Dim modelPath As String = $"{ModelsLocation}/{learner.name}Model.zip"
				Using fs = New FileStream(modelPath, FileMode.Create, FileAccess.Write, FileShare.Write)
					mlContext.Model.Save(trainedModel, fs)
				End Using

				Console.WriteLine("The model is saved to {0}", modelPath)
			Next learner

			' 4. Try/test Predictions with the created models
			' The following test predictions could be implemented/deployed in a different application (production apps)
			' that's why it is seggregated from the previous loop
			' For each trained model, test 10 predictions           
			For Each learner In regressionLearners
				'Load current model from .ZIP file
				Dim trainedModel As ITransformer
				Dim modelPath As String = $"{ModelsLocation}/{learner.name}Model.zip"
				Using stream = New FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
					trainedModel = mlContext.Model.Load(stream)
				End Using

				' Create prediction engine related to the loaded trained model
				Dim predEngine = trainedModel.CreatePredictionEngine(Of DemandObservation, DemandPrediction)(mlContext)

				Console.WriteLine($"================== Visualize/test 10 predictions for model {learner.name}Model.zip ==================")
				'Visualize 10 tests comparing prediction with actual/observed values from the test dataset
				ModelScoringTester.VisualizeSomePredictions(mlContext,learner.name, TestDataLocation, predEngine, 10)
			Next learner

			Common.ConsoleHelper.ConsolePressAnyKey()

		End Sub
	End Module
End Namespace
