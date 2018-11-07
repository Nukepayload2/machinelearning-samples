﻿Imports System.IO

Namespace ImageClassification.ImageDataStructures
    Public Class ImageNetData
        Public ImagePath As String

        Public Label As String

        Public Shared Function ReadFromCsv(file As String, folder As String) As IEnumerable(Of ImageNetData)
            Return IO.File.ReadAllLines(file).Select(Function(x) x.Split(vbTab)).Select(Function(x) New ImageNetData With {
                .ImagePath = Path.Combine(folder, x(0)),
                .Label = x(1)
            })
        End Function
    End Class

    Public Class ImageNetDataProbability
        Inherits ImageNetData

        Public PredictedLabel As String
        Public Property Probability() As Single
    End Class
End Namespace
