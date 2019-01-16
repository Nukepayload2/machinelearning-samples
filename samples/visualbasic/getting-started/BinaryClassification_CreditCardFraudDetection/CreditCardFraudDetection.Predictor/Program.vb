Imports CreditCardFraudDetection.Common
Imports System.IO

Namespace CreditCardFraudDetection.Predictor
	Friend Class Program
		Shared Sub Main(args() As String)
			Dim assetsPath = GetAssetsPath("..\..\..\assets")
			Dim trainOutput = GetAssetsPath("..\..\..\..\CreditCardFraudDetection.Trainer\assets\output")


			If Not File.Exists(Path.Combine(trainOutput, "testData.csv")) OrElse Not File.Exists(Path.Combine(trainOutput, "fastTree.zip")) Then
                ConsoleWriteWarning("YOU SHOULD RUN TRAIN PROJECT FIRST")
                ConsolePressAnyKey()
				Return
			End If

			' copy files from train output
			Directory.CreateDirectory(assetsPath)
			For Each file In Directory.GetFiles(trainOutput)

				Dim fileDestination = Path.Combine(Path.Combine(assetsPath, "input"), Path.GetFileName(file))
				If IO.File.Exists(fileDestination) Then
                    DeleteAssets(fileDestination)
				End If

                IO.File.Copy(file, Path.Combine(Path.Combine(assetsPath, "input"), Path.GetFileName(file)))
			Next file

			Dim dataSetFile = Path.Combine(assetsPath,"input", "testData.csv")
			Dim modelFile = Path.Combine(assetsPath, "input", "fastTree.zip")


			Dim modelEvaluator = New Predictor(modelFile,dataSetFile)

			Dim numberOfTransactions As Integer = 5
			modelEvaluator.RunMultiplePredictions(numberOfTransactions)

            ConsolePressAnyKey()
		End Sub
	End Class
End Namespace
