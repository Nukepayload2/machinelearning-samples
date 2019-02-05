Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.ML.Data

Namespace ImageClassification.ImageData
    Public Class ImageNetData
        <LoadColumn(0)>
        Public ImagePath As String

        <LoadColumn(1)>
        Public Label As String

        Public Shared Function ReadFromCsv(file As String, folder As String) As IEnumerable(Of ImageNetData)
            Return System.IO.File.ReadAllLines(file).Select(Function(x) x.Split(vbTab)).
                Select(Function(x) New ImageNetData() With {
                .ImagePath = Path.Combine(folder, x(0)),
                .Label = x(1)
            })
        End Function
    End Class

    Public Class ImageNetDataProbability
        Inherits ImageNetData

        Public Property Probability() As Single


        Public Sub ConsoleWriteLine()
            Dim defaultForeground = Console.ForegroundColor
            Dim labelColor = ConsoleColor.Green

            Console.Write($"ImagePath: {ImagePath} predicted as ")
            Console.ForegroundColor = labelColor
            Console.Write(Label)
            Console.ForegroundColor = defaultForeground
            Console.Write(" with probability ")
            Console.ForegroundColor = labelColor
            Console.Write(Probability)
            Console.ForegroundColor = defaultForeground
            Console.WriteLine("")
        End Sub
    End Class

    Public Class ImageNetPipeline
        Public ImagePath As String
        Public Label As String
        Public PredictedLabelValue As String
        Public Score() As Single
        Public softmax2_pre_activation() As Single
    End Class
End Namespace
