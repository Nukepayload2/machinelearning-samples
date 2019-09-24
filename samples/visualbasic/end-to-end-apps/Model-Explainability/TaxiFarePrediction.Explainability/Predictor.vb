Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports System
Imports System.Linq
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports TaxiFareRegression.DataStructures


Namespace TaxiFareRegression.Explainability
	Public Class Predictor
		Private ReadOnly _modelfile As String
		Private ReadOnly _datasetFile As String
		Private Shared context As MLContext
		Private Shared model As ITransformer
		Private Shared predictionEngine As PredictionEngine(Of TaxiTrip, TaxiTripFarePredictionWithContribution)
		Public Sub New(modelfile As String, datasetFile As String)
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: _modelfile = modelfile ?? throw new ArgumentNullException(nameof(modelfile));
			_modelfile = If(modelfile, throw New ArgumentNullException(NameOf(modelfile)))
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: _datasetFile = datasetFile ?? throw new ArgumentNullException(nameof(datasetFile));
			_datasetFile = If(datasetFile, throw New ArgumentNullException(NameOf(datasetFile)))

			context = New MLContext

				Dim inputSchema As Object
				model = context.Model.Load(_modelfile, inputSchema)

			predictionEngine = context.Model.CreatePredictionEngine(Of TaxiTrip, TaxiTripFarePredictionWithContribution)(model)
		End Sub

		Public Function RunMultiplePredictions(numberOfPredictions As Integer) As List(Of DataStructures.TaxiFarePrediction)

			'Load data as input for predictions
			Dim inputDataForPredictions As IDataView = context.Data.LoadFromTextFile(Of TaxiTrip)(_datasetFile, hasHeader:= True, separatorChar:= ","c)

			Console.WriteLine($"Predictions from saved model:")

			Console.WriteLine($vbLf & " " & vbLf & " Test {numberOfPredictions} transactions, from the test datasource, that should be predicted as fraud (true):")

			Dim transactionList As New List(Of DataStructures.TaxiFarePrediction)
			Dim prediction As TaxiTripFarePredictionWithContribution
			Dim explainedPrediction As DataStructures.TaxiFarePrediction

			context.Data.CreateEnumerable(Of TaxiTrip)(inputDataForPredictions, reuseRowObject:= False).Take(numberOfPredictions).Select(Function(testData) testData).ToList().ForEach(Sub(testData)
										testData.PrintToConsole()
										prediction = predictionEngine.Predict(testData)
										explainedPrediction = New DataStructures.TaxiFarePrediction(prediction.FareAmount, prediction.GetFeatureContributions(model.GetOutputSchema(inputDataForPredictions.Schema)))
										transactionList.Add(explainedPrediction)
			End Sub)

			Return transactionList
		End Function

	End Class
End Namespace
