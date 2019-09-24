Imports Microsoft.ML
Imports TaxiFareRegression.DataStructures

Namespace TaxiFareRegression
    Public Class Predictor
        Private ReadOnly _modelfile As String
        Private ReadOnly _datasetFile As String
        Private Shared context As MLContext
        Private Shared model As ITransformer
        Private Shared predictionEngine As PredictionEngine(Of TaxiTrip, TaxiTripFarePredictionWithContribution)
        Public Sub New(modelfile As String, datasetFile As String)
            If modelfile Is Nothing Then
                Throw New ArgumentNullException(NameOf(modelfile))
            End If

            If datasetFile Is Nothing Then
                Throw New ArgumentNullException(NameOf(datasetFile))
            End If

            _modelfile = modelfile
            _datasetFile = datasetFile

            context = New MLContext

            Dim inputSchema As Object
            model = context.Model.Load(_modelfile, inputSchema)

            predictionEngine = context.Model.CreatePredictionEngine(Of TaxiTrip, TaxiTripFarePredictionWithContribution)(model)
        End Sub

        Public Function RunMultiplePredictions(numberOfPredictions As Integer) As List(Of TaxiFarePrediction)

            'Load data as input for predictions
            Dim inputDataForPredictions As IDataView = context.Data.LoadFromTextFile(Of TaxiTrip)(_datasetFile, hasHeader:=True, separatorChar:=","c)

            Console.WriteLine($"Predictions from saved model:")

            Console.WriteLine(vbLf & " " & vbLf & $" Test {numberOfPredictions} transactions, from the test datasource, that should be predicted as fraud (true):")

            Dim transactionList As New List(Of TaxiFarePrediction)
            Dim prediction As TaxiTripFarePredictionWithContribution
            Dim explainedPrediction As TaxiFarePrediction

            context.Data.CreateEnumerable(Of TaxiTrip)(inputDataForPredictions, reuseRowObject:=False).Take(numberOfPredictions).Select(Function(testData) testData).ToList().ForEach(Sub(testData)
                                                                                                                                                                                          testData.PrintToConsole()
                                                                                                                                                                                          prediction = predictionEngine.Predict(testData)
                                                                                                                                                                                          explainedPrediction = New TaxiFarePrediction(prediction.FareAmount, prediction.GetFeatureContributions(model.GetOutputSchema(inputDataForPredictions.Schema)))
                                                                                                                                                                                          transactionList.Add(explainedPrediction)
                                                                                                                                                                                      End Sub)

            Return transactionList
        End Function

    End Class
End Namespace
