Imports CreditCardFraudDetection.Common.DataModels
Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports System
Imports System.Linq

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

			'Load data as input for predictions
			Dim inputDataForPredictions As IDataView = mlContext.Data.LoadFromTextFile(Of TransactionObservation)(_dasetFile, separatorChar:= ","c, hasHeader:= True)

			Console.WriteLine($"Predictions from saved model:")

			Dim inputSchema As Object
			Dim model As ITransformer = mlContext.Model.Load(_modelfile, inputSchema)

			Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of TransactionObservation, TransactionFraudPredictionWithContribution)(model)
			Console.WriteLine($vbLf & " " & vbLf & " Test {numberOfPredictions} transactions, from the test datasource, that should be predicted as fraud (true):")

			mlContext.Data.CreateEnumerable(Of TransactionObservation)(inputDataForPredictions, reuseRowObject:= False).Where(Function(x) x.Label = True).Take(numberOfPredictions).Select(Function(testData) testData).ToList().ForEach(Sub(testData)
										Console.WriteLine($"--- Transaction ---")
										testData.PrintToConsole()
										predictionEngine.Predict(testData).PrintToConsole()
										Console.WriteLine($"-------------------")
			End Sub)


			Console.WriteLine($vbLf & " " & vbLf & " Test {numberOfPredictions} transactions, from the test datasource, that should NOT be predicted as fraud (false):")

			mlContext.Data.CreateEnumerable(Of TransactionObservation)(inputDataForPredictions, reuseRowObject:= False).Where(Function(x) x.Label = False).Take(numberOfPredictions).ToList().ForEach(Sub(testData)
									   Console.WriteLine($"--- Transaction ---")
									   testData.PrintToConsole()
									   predictionEngine.Predict(testData).PrintToConsole(model.GetOutputSchema(inputDataForPredictions.Schema))
									   Console.WriteLine($"-------------------")
			End Sub)
		End Sub

		Private Class TransactionFraudPredictionWithContribution
			Inherits TransactionFraudPrediction

			Public Property FeatureContributions As Single()

			Public Overloads Sub PrintToConsole(dataview As DataViewSchema)
				MyBase.PrintToConsole()
				Dim slots As VBuffer(Of ReadOnlyMemory(Of Char)) = Nothing
				dataview.GetColumnOrNull("Features").Value.GetSlotNames(slots)
				Dim featureNames = slots.DenseValues().ToArray()
				Console.WriteLine($"Feature Contributions: " & $"[{featureNames(0)}] {FeatureContributions(0)} " & $"[{featureNames(1)}] {FeatureContributions(1)} " & $"[{featureNames(2)}] {FeatureContributions(2)} ... " & $"[{featureNames(27)}] {FeatureContributions(27)} " & $"[{featureNames(28)}] {FeatureContributions(28)}")
			End Sub
		End Class
	End Class
End Namespace
