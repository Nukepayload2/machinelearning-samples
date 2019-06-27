Imports System
Imports System.IO

Namespace ObjectDetection
	Friend Class Program
		Public Shared Sub Main()
			Dim assetsRelativePath = "../../../assets"
			Dim assetsPath As String = GetAbsolutePath(assetsRelativePath)
			Dim modelFilePath = Path.Combine(assetsPath, "Model", "TinyYolo2_model.onnx")
			Dim imagesFolder = Path.Combine(assetsPath, "images")

			Try
				Dim modelScorer = New OnnxModelScorer(imagesFolder, modelFilePath)
				modelScorer.Score()
			Catch ex As Exception
				Console.WriteLine(ex.ToString())
			End Try

			Console.WriteLine("========= End of Process..Hit any Key ========")
			Console.ReadLine()
		End Sub

		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function
	End Class
End Namespace



