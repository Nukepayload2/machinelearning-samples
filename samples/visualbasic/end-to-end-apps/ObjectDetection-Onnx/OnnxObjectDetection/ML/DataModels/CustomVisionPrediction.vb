Imports Microsoft.ML.Data

Namespace OnnxObjectDetection
	Public Class CustomVisionPrediction
		Implements IOnnxObjectPrediction

		<ColumnName("model_outputs0")>
		Public Property PredictedLabels As Single() Implements IOnnxObjectPrediction.PredictedLabels
	End Class
End Namespace
