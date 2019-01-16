Imports CreditCardFraudDetection.Common
Imports CreditCardFraudDetection.Common.DataModels
Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Data

Namespace CreditCardFraudDetection.Predictor
    Public Class Predictor
        Private ReadOnly _modelfile As String
        Private ReadOnly _dasetFile As String

        Public Sub New(modelfile As String, dasetFile As String)
            _modelfile = modelfile
            _dasetFile = dasetFile
        End Sub

        Public Sub RunMultiplePredictions(numberOfTransactions As Integer, Optional seed As Integer? = 1)

            Dim mlContext = New MLContext(seed)

            Dim columnsPlus() As TextLoader.Column = {
                New TextLoader.Column("Label", DataKind.BL, 0),
                New TextLoader.Column("V1", DataKind.R4, 1),
                New TextLoader.Column("V2", DataKind.R4, 2),
                New TextLoader.Column("V3", DataKind.R4, 3),
                New TextLoader.Column("V4", DataKind.R4, 4),
                New TextLoader.Column("V5", DataKind.R4, 5),
                New TextLoader.Column("V6", DataKind.R4, 6),
                New TextLoader.Column("V7", DataKind.R4, 7),
                New TextLoader.Column("V8", DataKind.R4, 8),
                New TextLoader.Column("V9", DataKind.R4, 9),
                New TextLoader.Column("V10", DataKind.R4, 10),
                New TextLoader.Column("V11", DataKind.R4, 11),
                New TextLoader.Column("V12", DataKind.R4, 12),
                New TextLoader.Column("V13", DataKind.R4, 13),
                New TextLoader.Column("V14", DataKind.R4, 14),
                New TextLoader.Column("V15", DataKind.R4, 15),
                New TextLoader.Column("V16", DataKind.R4, 16),
                New TextLoader.Column("V17", DataKind.R4, 17),
                New TextLoader.Column("V18", DataKind.R4, 18),
                New TextLoader.Column("V19", DataKind.R4, 19),
                New TextLoader.Column("V20", DataKind.R4, 20),
                New TextLoader.Column("V21", DataKind.R4, 21),
                New TextLoader.Column("V22", DataKind.R4, 22),
                New TextLoader.Column("V23", DataKind.R4, 23),
                New TextLoader.Column("V24", DataKind.R4, 24),
                New TextLoader.Column("V25", DataKind.R4, 25),
                New TextLoader.Column("V26", DataKind.R4, 26),
                New TextLoader.Column("V27", DataKind.R4, 27),
                New TextLoader.Column("V28", DataKind.R4, 28),
                New TextLoader.Column("Amount", DataKind.R4, 29),
                New TextLoader.Column("StratificationColumn", DataKind.R4, 30)
            }

            'LoaderOptimization test data into DataView
            Dim dataTest = mlContext.Data.ReadFromTextFile(_dasetFile, columnsPlus, hasHeader:=True, separatorChar:=","c)

            'Inspect/Peek data from datasource
            ConsoleWriterSection($"Inspect {numberOfTransactions} transactions observed as fraud and {numberOfTransactions} not observed as fraud, from the test datasource:")
            InspectData(mlContext, dataTest, numberOfTransactions)

            ConsoleWriteHeader($"Predictions from saved model:")

            Dim model As ITransformer
            Using file = System.IO.File.OpenRead(_modelfile)
                model = mlContext.Model.Load(file)
            End Using

            Dim predictionEngine = model.CreatePredictionEngine(Of TransactionObservation, TransactionFraudPrediction)(mlContext)
            ConsoleWriterSection($"Test {numberOfTransactions} transactions, from the test datasource, that should be predicted as fraud (true):")
            dataTest.AsEnumerable(Of TransactionObservation)(mlContext, reuseRowObject:=False).
                Where(Function(x) x.Label = True).
                Take(numberOfTransactions).
                Select(Function(testData) testData).ToList().ForEach(
                Sub(testData)
                    Console.WriteLine($"--- Transaction ---")
                    testData.PrintToConsole()
                    predictionEngine.Predict(testData).PrintToConsole()
                    Console.WriteLine($"-------------------")
                End Sub)

            ConsoleWriterSection($"Test {numberOfTransactions} transactions, from the test datasource, that should NOT be predicted as fraud (false):")
            dataTest.AsEnumerable(Of TransactionObservation)(mlContext, reuseRowObject:=False).
                Where(Function(x) x.Label = False).Take(numberOfTransactions).ToList().ForEach(
                Sub(testData)
                    Console.WriteLine($"--- Transaction ---")
                    testData.PrintToConsole()
                    predictionEngine.Predict(testData).PrintToConsole()
                    Console.WriteLine($"-------------------")
                End Sub)
        End Sub

    End Class
End Namespace
