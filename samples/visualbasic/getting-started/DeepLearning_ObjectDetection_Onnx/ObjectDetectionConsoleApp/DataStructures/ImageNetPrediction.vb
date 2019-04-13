Imports Microsoft.ML.Data

Namespace ObjectDetection
	Public Class ImageNetPrediction
		<ColumnName(OnnxModelScorer.TinyYoloModelSettings.ModelOutput)>
		Public PredictedLabels() As Single
	End Class
End Namespace
