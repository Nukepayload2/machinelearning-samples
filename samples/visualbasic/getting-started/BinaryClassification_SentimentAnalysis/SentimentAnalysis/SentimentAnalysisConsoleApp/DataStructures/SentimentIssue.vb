Imports Microsoft.ML.Data

Namespace SentimentAnalysisConsoleApp.DataStructures
	Public Class SentimentIssue
		<LoadColumn(0)>
		Public Property Label As Boolean
        <LoadColumn(2)>
        Public Property Text As String
	End Class
End Namespace
