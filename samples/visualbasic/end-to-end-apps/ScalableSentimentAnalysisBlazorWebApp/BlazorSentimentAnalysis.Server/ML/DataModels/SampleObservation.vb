Imports Microsoft.ML.Data

Namespace BlazorSentimentAnalysis.Server.ML.DataModels
	Public Class SampleObservation
		<ColumnName("col0"), LoadColumn(0)>
		Public Property Col0 As String


		<ColumnName("Label"), LoadColumn(1)>
		Public Property Label As Boolean
	End Class
End Namespace
