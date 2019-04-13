Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq

Imports BikeSharingDemand.DataStructures

Imports Microsoft.ML

Namespace BikeSharingDemand
	Public Module ModelScoringTester
		Public Sub VisualizeSomePredictions(mlContext As MLContext, modelName As String, testDataLocation As String, predEngine As PredictionEngine(Of DemandObservation, DemandPrediction), numberOfPredictions As Integer)
			'Make a few prediction tests 
			' Make the provided number of predictions and compare with observed data from the test dataset
			Dim testData = ReadSampleDataFromCsvFile(testDataLocation, numberOfPredictions)

			For i As Integer = 0 To numberOfPredictions - 1
				'Score
				Dim resultprediction = predEngine.Predict(testData(i))

				Common.ConsoleHelper.PrintRegressionPredictionVersusObserved(resultprediction.PredictedCount.ToString(), testData(i).Count.ToString())
			Next i

		End Sub

		'This method is using regular .NET System.IO.File and LinQ to read just some sample data to test/predict with 
		Public Function ReadSampleDataFromCsvFile(dataLocation As String, numberOfRecordsToRead As Integer) As List(Of DemandObservation)
			Return File.ReadLines(dataLocation).Skip(1).Where(Function(x) Not String.IsNullOrWhiteSpace(x)).Select(Function(x) x.Split(","c)).Select(Function(x) New DemandObservation With {
				.Season = Single.Parse(x(2), CultureInfo.InvariantCulture),
				.Year = Single.Parse(x(3), CultureInfo.InvariantCulture),
				.Month = Single.Parse(x(4), CultureInfo.InvariantCulture),
				.Hour = Single.Parse(x(5), CultureInfo.InvariantCulture),
				.Holiday = Single.Parse(x(6), CultureInfo.InvariantCulture),
				.Weekday = Single.Parse(x(7), CultureInfo.InvariantCulture),
				.WorkingDay = Single.Parse(x(8), CultureInfo.InvariantCulture),
				.Weather = Single.Parse(x(9), CultureInfo.InvariantCulture),
				.Temperature = Single.Parse(x(10), CultureInfo.InvariantCulture),
				.NormalizedTemperature = Single.Parse(x(11), CultureInfo.InvariantCulture),
				.Humidity = Single.Parse(x(12), CultureInfo.InvariantCulture),
				.Windspeed = Single.Parse(x(13), CultureInfo.InvariantCulture),
				.Count = Single.Parse(x(16), CultureInfo.InvariantCulture)
			}).Take(numberOfRecordsToRead).ToList()
		End Function
	End Module
End Namespace
