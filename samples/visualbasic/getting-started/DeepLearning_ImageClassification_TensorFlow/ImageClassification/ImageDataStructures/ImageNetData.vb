Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace ImageClassification.ImageDataStructures
	Public Class ImageNetData
		Public ImagePath As String

		Public Label As String

		Public Shared Function ReadFromCsv(ByVal file As String, ByVal folder As String) As IEnumerable(Of ImageNetData)
            Return System.IO.File.ReadAllLines(file).Select(Function(x) x.Split(vbTab)).Select(Function(x) New ImageNetData With {
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
