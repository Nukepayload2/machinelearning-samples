Imports Microsoft.ML.Data

Namespace BlazorSentimentAnalysis.Server.ML.DataModels
	Public Class SamplePrediction
		' ColumnName attribute is used to change the column name from
		' its default value, which is the name of the field.
		<ColumnName("PredictedLabel")>
		Public Property Prediction As Boolean

		Public Property Score As Single
	End Class
End Namespace



