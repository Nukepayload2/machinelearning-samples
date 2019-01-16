Imports CreditCardFraudDetection.Common
Imports Microsoft.ML
Imports System.IO

Namespace CreditCardFraudDetection.Trainer
    Friend Class Program
        Shared Sub Main(args() As String)
            Dim assetsPath = GetAssetsPath("..\..\..\assets")
            Dim zipDataSet = Path.Combine(assetsPath, "input", "creditcardfraud-dataset.zip")
            Dim dataSetFile = Path.Combine(assetsPath, "input", "creditcard.csv")

            Try
                'Unzip datasets as they are significantly large, too large for GitHub if not zipped
                UnZipDataSet(zipDataSet, dataSetFile)

                ' Create a common ML.NET context.
                ' Seed set to any number so you have a deterministic environment for repeateable results
                Dim mlContext As New MLContext(seed:=1)

                Dim modelBuilder = New ModelBuilder(mlContext, assetsPath, dataSetFile)
                modelBuilder.PreProcessData(mlContext)

                ConsoleWriteHeader("Creating and training the model")
                modelBuilder.TrainFastTreeAndSaveModels()
            Catch e As Exception
                ConsoleWriteException({e.Message, e.StackTrace})
            End Try

            ConsolePressAnyKey()
        End Sub
    End Class
End Namespace
