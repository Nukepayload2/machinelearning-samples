Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Data
Imports System.Runtime.CompilerServices

Namespace CreditCardFraudDetection.Common
    Public Module ConsoleExtensions

        <Extension>
        Public Sub ToConsole(result As MultiClassClassifierMetrics)
            Console.WriteLine($"Acuracy macro: {result.AccuracyMacro}")
            Console.WriteLine($"Acuracy micro: {result.AccuracyMicro}")
            Console.WriteLine($"Log loss: {result.LogLoss}")
            Console.WriteLine($"Log loss reduction: {result.LogLossReduction}")
            Console.WriteLine($"Per class log loss: ")
            Dim count As Integer = 0
            For i = 0 To result.PerClassLogLoss.Length - 1
                Dim logLossClass = result.PerClassLogLoss(i)
                Console.WriteLine(vbTab & $" [{i}]: {logLossClass}")
            Next
            Console.WriteLine($"Top K: {result.TopK}")
            Console.WriteLine($"Top K accuracy: {result.TopKAccuracy}")
        End Sub

        <Extension>
        Public Sub ToConsole(result As CalibratedBinaryClassificationMetrics)
            Console.WriteLine($"Area under ROC curve: {result.Auc}")
            Console.WriteLine($"Area under the precision/recall curve: {result.Auprc}")
            Console.WriteLine($"Entropy: {result.Entropy}")
            Console.WriteLine($"F1 Score: {result.F1Score}")
            Console.WriteLine($"Log loss: {result.LogLoss}")
            Console.WriteLine($"Log loss reduction: {result.LogLossReduction}")
            Console.WriteLine($"Negative precision: {result.NegativePrecision}")
            Console.WriteLine($"Positive precision: {result.PositivePrecision}")
            Console.WriteLine($"Positive recall: {result.PositiveRecall}")

        End Sub

        <Extension>
        Public Sub ToConsole(result As RegressionMetrics)
            Console.WriteLine($"L1: {result.L1}")
            Console.WriteLine($"L2: {result.L2}")
            Console.WriteLine($"Loss function: {result.LossFn}")
            Console.WriteLine($"Root mean square of the L2 loss: {result.Rms}")
            Console.WriteLine($"R scuared: {result.RSquared}")
        End Sub

        <Extension>
        Public Function ReadModel(mlContext As MLContext, modelLocation As String) As ITransformer
            Dim model As ITransformer
            Using file = System.IO.File.OpenRead(modelLocation)
                model = mlContext.Model.Load(file)
            End Using
            Return model
        End Function
    End Module

End Namespace
