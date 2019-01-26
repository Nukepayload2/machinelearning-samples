Imports System
Imports System.IO
Imports System.Threading.Tasks
Imports ImageClassification.Model
Imports ImageClassification.Model.ConsoleHelpers

Namespace ImageClassification.Train
	Public Class Program
		Shared Sub Main(args() As String)
			Dim assetsPath = ModelHelpers.GetAssetsPath("..\..\..\assets")

			Dim tagsTsv = Path.Combine(assetsPath, "inputs", "data", "tags.tsv")
			Dim imagesFolder = Path.Combine(assetsPath, "inputs", "data")
			Dim inceptionPb = Path.Combine(assetsPath, "inputs", "inception", "tensorflow_inception_graph.pb")
			Dim imageClassifierZip = Path.Combine(assetsPath, "outputs", "imageClassifier.zip")

			Try
				Dim modelBuilder = New ModelBuilder(tagsTsv, imagesFolder, inceptionPb, imageClassifierZip)
				modelBuilder.BuildAndTrain()
			Catch ex As Exception
				ConsoleWriteException(ex.Message)
			End Try

			ConsolePressAnyKey()
		End Sub
	End Class
End Namespace
