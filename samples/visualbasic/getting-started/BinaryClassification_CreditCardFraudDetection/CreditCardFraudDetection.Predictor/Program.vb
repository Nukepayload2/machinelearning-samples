Imports CreditCardFraudDetection.Common
Imports System
Imports System.IO

Namespace CreditCardFraudDetection.Predictor
	Friend Class Program
		Shared Sub Main(args() As String)
			Dim assetsPath As String = GetAbsolutePath("../../../assets")
			Dim trainOutput As String = GetAbsolutePath("../../../../CreditCardFraudDetection.Trainer\assets\output")

			CopyModelAndDatasetFromTrainingProject(trainOutput, assetsPath)

			Dim inputDatasetForPredictions = Path.Combine(assetsPath,"input", "testData.csv")
			Dim modelFilePath = Path.Combine(assetsPath, "input", "fastTree.zip")

			' Create model predictor to perform a few predictions
			Dim modelPredictor = New Predictor(modelFilePath,inputDatasetForPredictions)

			modelPredictor.RunMultiplePredictions(numberOfPredictions:=5)

			Console.WriteLine("=============== Press any key ===============")
			Console.ReadKey()
		End Sub

		Public Shared Sub CopyModelAndDatasetFromTrainingProject(trainOutput As String, assetsPath As String)
			If Not File.Exists(Path.Combine(trainOutput, "testData.csv")) OrElse Not File.Exists(Path.Combine(trainOutput, "fastTree.zip")) Then
				Console.WriteLine("***** YOU NEED TO RUN THE TRAINING PROJECT IN THE FIRST PLACE *****")
				Console.WriteLine("=============== Press any key ===============")
				Console.ReadKey()
				Environment.Exit(0)
			End If

			' copy files from train output
			Directory.CreateDirectory(assetsPath)
			For Each file In Directory.GetFiles(trainOutput)

				Dim fileDestination = Path.Combine(Path.Combine(assetsPath, "input"), Path.GetFileName(file))
				If System.IO.File.Exists(fileDestination) Then
					LocalConsoleHelper.DeleteAssets(fileDestination)
				End If

				System.IO.File.Copy(file, Path.Combine(Path.Combine(assetsPath, "input"), Path.GetFileName(file)))
			Next file

		End Sub

		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function
	End Class
End Namespace
