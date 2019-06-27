Imports Microsoft.ML.Data

Namespace TensorFlowImageClassification.ML.DataModels
	Public Class ImageLabelPredictions
		'TODO: Change to fixed output column name for TensorFlow model
		<ColumnName("loss")>
		Public PredictedLabels() As Single
	End Class
End Namespace
