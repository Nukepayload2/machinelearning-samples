Imports Microsoft.ML.Data
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace ObjectDetection
	Public Class ImageNetData
		<LoadColumn(0)>
		Public ImagePath As String

		<LoadColumn(1)>
		Public Label As String

		Public Shared Function ReadFromCsv(file As String, folder As String) As IEnumerable(Of ImageNetData)
            Return System.IO.File.ReadAllLines(file).Select(Function(x) x.Split(vbTab)).Select(Function(x) New ImageNetData With {
                .ImagePath = Path.Combine(folder, x(0)),
                .Label = x(1)
            })
        End Function
	End Class

	Public Class ImageNetDataProbability
		Inherits ImageNetData

		Public PredictedLabel As String
		Public Property Probability As Single
	End Class
End Namespace
