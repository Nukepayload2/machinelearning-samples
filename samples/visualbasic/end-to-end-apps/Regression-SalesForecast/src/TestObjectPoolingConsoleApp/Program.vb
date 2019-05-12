Imports CommonHelpers
Imports Microsoft.ML
Imports System
Imports System.Threading
Imports System.Threading.Tasks

Imports TestObjectPoolingConsoleApp.DataStructures

Namespace TestObjectPoolingConsoleApp
	Friend Class Program
		Shared Sub Main(args() As String)
			Dim cts As CancellationTokenSource = New CancellationTokenSource

			' Create an opportunity for the user to cancel.
			Task.Run(Sub()
				If Console.ReadKey().KeyChar = "c"c OrElse Console.ReadKey().KeyChar = "C"c Then
					cts.Cancel()
				End If
			End Sub)

			Dim mlContext As New MLContext(seed:=1)
			Dim modelFolder As String = $"Forecast/ModelFiles"
			Dim modelFilePathName As String = $"ModelFiles/country_month_fastTreeTweedie.zip"
			Dim countrySalesModel = New MLModelEngine(Of CountryData, CountrySalesPrediction)(mlContext, modelFilePathName, minPredictionEngineObjectsInPool:= 50, maxPredictionEngineObjectsInPool:= 2000, expirationTime:=30000)

			Console.WriteLine("Current number of objects in pool: {0:####.####}", countrySalesModel.CurrentPredictionEnginePoolSize)

			'Single Prediction
			Dim singleCountrySample = New CountryData("Australia", 2017, 1, 477, 164, 2486, 9, 10345, 281, 1029)
			Dim singleNextMonthPrediction = countrySalesModel.Predict(singleCountrySample)

			Console.WriteLine("Prediction: {0:####.####}", singleNextMonthPrediction.Score)

			' Create a high demand for the modelEngine objects.
			Parallel.For(0, 1000000, Sub(i, loopState)
				'Sample country data
				'next,country,year,month,max,min,std,count,sales,med,prev
				'4.23056080166201,Australia,2017,1,477.34,164.916,2486.1346772137,9,10345.71,281.7,1029.11

				Dim countrySample = New CountryData("Australia", 2017, 1, 477, 164, 2486, 9, 10345, 281, i)

				' This is the bottleneck in our application. All threads in this loop
				' must serialize their access to the static Console class.
				Console.CursorLeft = 0
				Dim nextMonthPrediction = countrySalesModel.Predict(countrySample)

				'(Wait for a 1/10 second)
				'System.Threading.Thread.Sleep(1000);

				Console.WriteLine("Prediction: {0:####.####}", nextMonthPrediction.Score)
				Console.WriteLine("-----------------------------------------")
				Console.WriteLine("Current number of objects in pool: {0:####.####}", countrySalesModel.CurrentPredictionEnginePoolSize)

				If cts.Token.IsCancellationRequested Then
					loopState.Stop()
				End If

			End Sub)

			Console.WriteLine("-----------------------------------------")
			Console.WriteLine("Current number of objects in pool: {0:####.####}", countrySalesModel.CurrentPredictionEnginePoolSize)


			Console.WriteLine("Press the Enter key to exit.")
			Console.ReadLine()
			cts.Dispose()
		End Sub

	End Class


End Namespace
