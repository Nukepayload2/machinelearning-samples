Imports System.IO
Imports ImageClassification.Model
Imports ImageClassification.Model.ConsoleHelpers

Namespace ImageClassification.Predict
    Friend Class Program
        Shared Sub Main(args() As String)
            Dim assetsRelativePath As String = "../../../assets"
            Dim assetsPath As String = GetAbsolutePath(assetsRelativePath)


            Dim imagesFolder = Path.Combine(assetsPath, "inputs", "images-for-predictions")
            Dim imageClassifierZip = Path.Combine(assetsPath, "inputs", "MLNETModel", "imageClassifier.zip")

            Try
                'INSTANT VB NOTE: The variable modelScorer was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
                Dim modelScorer_Renamed = New ModelScorer(imagesFolder, imageClassifierZip)
                modelScorer_Renamed.ClassifyImages()
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
