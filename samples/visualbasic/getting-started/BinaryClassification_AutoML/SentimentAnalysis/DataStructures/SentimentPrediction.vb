Imports Microsoft.ML.Data

Namespace SentimentAnalysis.DataStructures
	Public Class SentimentPrediction
		' ColumnName attribute is used to change the column name from
		' its default value, which is the name of the field.
		<ColumnName("PredictedLabel")>
		Public Property Prediction As Boolean

		Public Property Score As Single
	End Class
End Namespace