Imports Microsoft.ML.Data

Namespace SentimentAnalysisFunctionsApp.DataModels
	Public Class SentimentData
		<LoadColumn(0)>
		Public SentimentText As String

		<LoadColumn(1)>
		<ColumnName("Label")>
		Public Sentiment As Boolean
	End Class
End Namespace
