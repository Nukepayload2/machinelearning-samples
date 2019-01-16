Imports ImageClassification.ModelScorer
Imports System.IO

Namespace ImageClassification
    Public Class Program
        Shared Sub Main(args() As String)
            Dim assetsPath = ModelHelpers.GetAssetsPath("..\..\..\assets")

            Dim tagsTsv = Path.Combine(assetsPath, "inputs", "images", "tags.tsv")
            Dim imagesFolder = Path.Combine(assetsPath, "inputs", "images")
            Dim inceptionPb = Path.Combine(assetsPath, "inputs", "inception", "tensorflow_inception_graph.pb")
            Dim labelsTxt = Path.Combine(assetsPath, "inputs", "inception", "imagenet_comp_graph_label_strings.txt")

            Dim customInceptionPb = Path.Combine(assetsPath, "inputs", "inception_custom", "model_tf.pb")
            Dim customLabelsTxt = Path.Combine(assetsPath, "inputs", "inception_custom", "labels.txt")

            Try
                Dim modelScorer = New TFModelScorer(tagsTsv, imagesFolder, inceptionPb, labelsTxt)
                modelScorer.Score()

            Catch ex As Exception
                ConsoleHelpers.ConsoleWriteException(ex.Message)
            End Try

            ConsoleHelpers.ConsolePressAnyKey()
        End Sub
    End Class
End Namespace
