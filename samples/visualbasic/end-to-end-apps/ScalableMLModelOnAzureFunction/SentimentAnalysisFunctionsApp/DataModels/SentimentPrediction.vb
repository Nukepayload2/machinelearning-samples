Imports Microsoft.ML.Data

Namespace SentimentAnalysisFunctionsApp.DataModels
	Public Class SentimentPrediction
		Inherits SentimentData

		<ColumnName("PredictedLabel")>
		Public Property Prediction As Boolean

		Public Property Probability As Single

		Public Property Score As Single
	End Class
End Namespace
