Imports Microsoft.ML.Data

Namespace HeartDiseasePredictionConsoleApp.DataStructure
	Public Class HeartPrediction
		' ColumnName attribute is used to change the column name from
		' its default value, which is the name of the field.
		<ColumnName("PredictedLabel")>
		Public Prediction As Boolean

		' No need to specify ColumnName attribute, because the field
		' name "Probability" is the column name we want.
		Public Probability As Single

		Public Score As Single
	End Class
End Namespace
