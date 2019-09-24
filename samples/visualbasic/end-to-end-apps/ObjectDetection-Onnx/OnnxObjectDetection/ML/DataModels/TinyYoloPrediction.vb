Imports Microsoft.ML.Data

Namespace OnnxObjectDetection
	Public Class TinyYoloPrediction
		Implements IOnnxObjectPrediction

		<ColumnName("grid")>
		Public Property PredictedLabels As Single() Implements IOnnxObjectPrediction.PredictedLabels
	End Class
End Namespace
