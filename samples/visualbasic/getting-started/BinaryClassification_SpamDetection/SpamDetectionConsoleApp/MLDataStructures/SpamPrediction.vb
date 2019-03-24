Imports Microsoft.ML.Data

Namespace SpamDetectionConsoleApp.MLDataStructures
	Friend Class SpamPrediction
		<ColumnName("PredictedLabel")>
		Public Property isSpam As Boolean

		Public Property Score As Single
		Public Property Probability As Single
	End Class
End Namespace
