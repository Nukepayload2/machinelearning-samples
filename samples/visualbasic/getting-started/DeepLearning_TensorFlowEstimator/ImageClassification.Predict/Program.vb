Imports System
Imports System.IO
Imports System.Threading.Tasks
Imports ImageClassification.Model
Imports ImageClassification.Model.ConsoleHelpers

Namespace ImageClassification.Predict
    Module Program
        Sub Main(args() As String)
            Dim assetsPath = ModelHelpers.GetAssetsPath("..\..\..\assets")

            Dim tagsTsv = Path.Combine(assetsPath, "inputs", "data", "tags.tsv")
            Dim imagesFolder = Path.Combine(assetsPath, "inputs", "data")
            Dim imageClassifierZip = Path.Combine(assetsPath, "inputs", "imageClassifier.zip")

            Try
                Dim modelScorer = New ModelScorer(tagsTsv, imagesFolder, imageClassifierZip)
                modelScorer.ClassifyImages()
            Catch ex As Exception
                ConsoleWriteException(ex.Message)
            End Try

            ConsolePressAnyKey()
        End Sub
    End Module
End Namespace
