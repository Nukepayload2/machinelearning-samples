Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Net
Imports Common
Imports Microsoft.ML
Imports SpamDetectionConsoleApp.MLDataStructures

Namespace SpamDetectionConsoleApp
	Friend Class Program
		Private Shared ReadOnly Property AppPath As String
			Get
				Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
			End Get
		End Property
		Private Shared ReadOnly Property DataDirectoryPath As String
			Get
				Return Path.Combine(AppPath, "..", "..", "..", "Data", "spamfolder")
			End Get
		End Property
		Private Shared ReadOnly Property TrainDataPath As String
			Get
				Return Path.Combine(AppPath, "..", "..", "..", "Data", "spamfolder", "SMSSpamCollection")
			End Get
		End Property

		Shared Sub Main(args() As String)
			' Download the dataset if it doesn't exist.
			If Not File.Exists(TrainDataPath) Then
				Using client = New WebClient
					'The code below will download a dataset from a third-party, UCI (link), and may be governed by separate third-party terms. 
					'By proceeding, you agree to those separate terms.
					client.DownloadFile("https://archive.ics.uci.edu/ml/machine-learning-databases/00228/smsspamcollection.zip", "spam.zip")
				End Using

				ZipFile.ExtractToDirectory("spam.zip", DataDirectoryPath)
			End If

			' Set up the MLContext, which is a catalog of components in ML.NET.
			Dim mlContext As MLContext = New MLContext

			' Specify the schema for spam data and read it into DataView.
			Dim data = mlContext.Data.LoadFromTextFile(Of SpamInput)(path:= TrainDataPath, hasHeader:= True, separatorChar:= vbTab)

			' Create the estimator which converts the text label to boolean, featurizes the text, and adds a linear trainer.
			' Data process configuration with pipeline data transformations 
			Dim dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label").Append(mlContext.Transforms.Text.FeaturizeText("FeaturesText", New Microsoft.ML.Transforms.Text.TextFeaturizingEstimator.Options With {
				.WordFeatureExtractor = New Microsoft.ML.Transforms.Text.WordBagEstimator.Options With {
					.NgramLength = 2,
					.UseAllLengths = True
				},
				.CharFeatureExtractor = New Microsoft.ML.Transforms.Text.WordBagEstimator.Options With {
					.NgramLength = 3,
					.UseAllLengths = False
				}
			}, "Message")).Append(mlContext.Transforms.CopyColumns("Features", "FeaturesText")).Append(mlContext.Transforms.NormalizeLpNorm("Features", "Features")).AppendCacheCheckpoint(mlContext)

			' Set the training algorithm 
			Dim trainer = mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.AveragedPerceptron(labelColumnName:= "Label", numberOfIterations:= 10, featureColumnName:= "Features"), labelColumnName:= "Label").Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"))
			Dim trainingPipeLine = dataProcessPipeline.Append(trainer)

			' Evaluate the model using cross-validation.
			' Cross-validation splits our dataset into 'folds', trains a model on some folds and 
			' evaluates it on the remaining fold. We are using 5 folds so we get back 5 sets of scores.
			' Let's compute the average AUC, which should be between 0.5 and 1 (higher is better).
			Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============")
			Dim crossValidationResults = mlContext.MulticlassClassification.CrossValidate(data:= data, estimator:= trainingPipeLine, numberOfFolds:= 5)
			ConsoleHelper.PrintMulticlassClassificationFoldsAverageMetrics(trainer.ToString(), crossValidationResults)

			' Now let's train a model on the full dataset to help us get better results
			Dim model = trainingPipeLine.Fit(data)

			'Create a PredictionFunction from our model 
			Dim predictor = mlContext.Model.CreatePredictionEngine(Of SpamInput, SpamPrediction)(model)

			Console.WriteLine("=============== Predictions for below data===============")
			' Test a few examples
			ClassifyMessage(predictor, "That's a great idea. It should work.")
			ClassifyMessage(predictor, "free medicine winner! congratulations")
			ClassifyMessage(predictor, "Yes we should meet over the weekend!")
			ClassifyMessage(predictor, "you win pills and free entry vouchers")

			Console.WriteLine("=============== End of process, hit any key to finish =============== ")
			Console.ReadLine()
		End Sub

		Public Shared Sub ClassifyMessage(predictor As PredictionEngine(Of SpamInput, SpamPrediction), message As String)
			Dim input = New SpamInput With {.Message = message}
			Dim prediction = predictor.Predict(input)

			Console.WriteLine("The message '{0}' is {1}", input.Message,If(prediction.isSpam = "spam", "spam", "not spam"))
		End Sub
	End Class
End Namespace
