Imports Microsoft.ML.Data
Imports System.IO

Namespace ImageClassification.ImageData
    Public Class ImageNetData
        <LoadColumn(0)>
        Public ImagePath As String

        <LoadColumn(1)>
        Public Label As String

        Public Shared Function ReadFromCsv(fileName As String, folder As String) As IEnumerable(Of ImageNetData)
            Return File.ReadAllLines(fileName).Select(Function(x) x.Split(vbTab)).
                Select(Function(x) New ImageNetData With {.ImagePath = Path.Combine(folder, x(0))})
        End Function
    End Class

End Namespace
