Imports System
Imports System.Linq

Imports Microsoft.ML

Imports CreditCardFraudDetection.Common.DataModels

Namespace CreditCardFraudDetection.Predictor
	Public Class Predictor
		Private ReadOnly _modelfile As String
		Private ReadOnly _dasetFile As String

		Public Sub New(modelfile As String, dasetFile As String)
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: _modelfile = modelfile ?? throw new ArgumentNullException(nameof(modelfile));
			_modelfile = If(modelfile, throw New ArgumentNullException(NameOf(modelfile)))
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: _dasetFile = dasetFile ?? throw new ArgumentNullException(nameof(dasetFile));
			_dasetFile = If(dasetFile, throw New ArgumentNullException(NameOf(dasetFile)))
		End Sub


		Public Sub RunMultiplePredictions(numberOfPredictions As Integer)
			Dim mlContext = New MLContext

			' Load data as input for predictions
			Dim inputDataForPredictions As IDataView = mlContext.Data.LoadFromTextFile(Of TransactionObservation)(_dasetFile, separatorChar:= ","c, hasHeader:= True)

			Console.WriteLine($"Predictions from saved model:")

			Dim inputSchema As Object
			Dim model As ITransformer = mlContext.Model.Load(_modelfile, inputSchema)

			Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of TransactionObservation, TransactionFraudPrediction)(model)

			Console.WriteLine($vbLf & " " & vbLf & " Test {numberOfPredictions} transactions, from the test datasource, that should be predicted as fraud (true):")

			mlContext.Data.CreateEnumerable(Of TransactionObservation)(inputDataForPredictions, reuseRowObject:= False).Where(Function(x) x.Label > 0).Take(numberOfPredictions).Select(Function(testData) testData).ToList().ForEach(Sub(testData)
										Console.WriteLine($"--- Transaction ---")
										testData.PrintToConsole()
										predictionEngine.Predict(testData).PrintToConsole()
										Console.WriteLine($"-------------------")
			End Sub)


			Console.WriteLine($vbLf & " " & vbLf & " Test {numberOfPredictions} transactions, from the test datasource, that should NOT be predicted as fraud (false):")

			mlContext.Data.CreateEnumerable(Of TransactionObservation)(inputDataForPredictions, reuseRowObject:= False).Where(Function(x) x.Label < 1).Take(numberOfPredictions).ToList().ForEach(Sub(testData)
									   Console.WriteLine($"--- Transaction ---")
									   testData.PrintToConsole()
									   predictionEngine.Predict(testData).PrintToConsole()
									   Console.WriteLine($"-------------------")
			End Sub)
		End Sub
	End Class
End Namespace
