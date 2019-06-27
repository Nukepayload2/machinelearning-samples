Imports Microsoft.ML.Data

Namespace LargeDatasets.DataStructures
	Public Class UrlPrediction
		' ColumnName attribute is used to change the column name from
		' its default value, which is the name of the field.
		<ColumnName("PredictedLabel")>
		Public Prediction As Boolean

		Public Score As Single
	End Class
End Namespace
