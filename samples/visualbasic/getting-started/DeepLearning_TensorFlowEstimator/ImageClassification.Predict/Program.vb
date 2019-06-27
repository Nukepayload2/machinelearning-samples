Imports System
Imports System.IO
Imports System.Threading.Tasks
Imports ImageClassification.Model
Imports ImageClassification.Model.ConsoleHelpers

Namespace ImageClassification.Predict
	Friend Class Program
		Shared Sub Main(args() As String)
			Dim assetsRelativePath As String = "../../../assets"
			Dim assetsPath As String = GetAbsolutePath(assetsRelativePath)

			Dim tagsTsv = Path.Combine(assetsPath, "inputs", "data", "images_list.tsv")
			Dim imagesFolder = Path.Combine(assetsPath, "inputs", "data")
			Dim imageClassifierZip = Path.Combine(assetsPath, "inputs", "imageClassifier.zip")

			Try
				Dim modelScorer = New ModelScorer(tagsTsv, imagesFolder, imageClassifierZip)
				modelScorer.ClassifyImages()
			Catch ex As Exception
				ConsoleWriteException(ex.ToString())
			End Try

			ConsolePressAnyKey()
		End Sub

		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function
	End Class
End Namespace
