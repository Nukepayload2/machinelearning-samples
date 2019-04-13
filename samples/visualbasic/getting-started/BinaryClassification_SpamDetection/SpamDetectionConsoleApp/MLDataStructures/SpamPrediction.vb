Imports Microsoft.ML.Data

Namespace SpamDetectionConsoleApp.MLDataStructures
	Friend Class SpamPrediction
		<ColumnName("PredictedLabel")>
		Public Property isSpam As String
	End Class
End Namespace
