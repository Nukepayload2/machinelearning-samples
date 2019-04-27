Imports Microsoft.ML.Data

Namespace Scalable.WebAPI.ML.DataModels
	Public Class SamplePrediction
		' ColumnName attribute is used to change the column name from
		' its default value, which is the name of the field.
		<ColumnName("PredictedLabel")>
		Public Property IsToxic As Boolean

		<ColumnName("Score")>
		Public Property Score As Single
	End Class
End Namespace



