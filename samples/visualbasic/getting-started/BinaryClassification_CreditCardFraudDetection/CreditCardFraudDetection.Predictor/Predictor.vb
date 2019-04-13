Imports CreditCardFraudDetection.Common.DataModels
Imports Microsoft.ML

Namespace CreditCardFraudDetection.Predictor
    Public Class Predictor
        Private ReadOnly _modelfile As String
        Private ReadOnly _dasetFile As String

        Public Sub New(modelfile As String, dasetFile As String)
            If modelfile Is Nothing Then
                Throw New ArgumentNullException(NameOf(modelfile))
            End If
            If dasetFile Is Nothing Then
                Throw New ArgumentNullException(NameOf(dasetFile))
            End If

            _modelfile = modelfile
            _dasetFile = dasetFile
        End Sub

        Public Sub RunMultiplePredictions(numberOfPredictions As Integer)

            Dim mlContext = New MLContext

            'Load data as input for predictions
            Dim inputDataForPredictions As IDataView = mlContext.Data.LoadFromTextFile(Of TransactionObservation)(_dasetFile, separatorChar:=","c, hasHeader:=True)

            Console.WriteLine($"Predictions from saved model:")

            Dim model As ITransformer = mlContext.Model.Load(_modelfile, Nothing)

            Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of TransactionObservation, TransactionFraudPrediction)(model)
            Console.WriteLine(vbLf & " " & vbLf & $" Test {numberOfPredictions} transactions, from the test datasource, that should be predicted as fraud (true):")

            mlContext.Data.CreateEnumerable(Of TransactionObservation)(inputDataForPredictions, reuseRowObject:=False).
                Where(Function(x) x.Label = True).Take(numberOfPredictions).
                Select(Function(testData) testData).ToList().
                ForEach(Sub(testData)
                            Console.WriteLine($"--- Transaction ---")
                            testData.PrintToConsole()
                            predictionEngine.Predict(testData).PrintToConsole()
                            Console.WriteLine($"-------------------")
                        End Sub)


            Console.WriteLine(vbLf & " " & vbLf & $" Test {numberOfPredictions} transactions, from the test datasource, that should NOT be predicted as fraud (false):")
            mlContext.Data.CreateEnumerable(Of TransactionObservation)(inputDataForPredictions, reuseRowObject:=False).
                Where(Function(x) x.Label = False).Take(numberOfPredictions).ToList().
                ForEach(Sub(testData)
                            Console.WriteLine($"--- Transaction ---")
                            testData.PrintToConsole()
                            predictionEngine.Predict(testData).PrintToConsole()
                            Console.WriteLine($"-------------------")
                        End Sub)
        End Sub

    End Class
End Namespace
