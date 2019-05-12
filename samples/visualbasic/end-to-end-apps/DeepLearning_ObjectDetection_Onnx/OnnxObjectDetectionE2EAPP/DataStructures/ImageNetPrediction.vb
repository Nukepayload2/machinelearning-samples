Imports Microsoft.ML.Data

Namespace OnnxObjectDetectionE2EAPP
	Public Class ImageNetPrediction
		<ColumnName(OnnxModelScorers.OnnxModelScorer.TinyYoloModelSettings.ModelOutput)>
		Public PredictedLabels() As Single
	End Class
End Namespace
