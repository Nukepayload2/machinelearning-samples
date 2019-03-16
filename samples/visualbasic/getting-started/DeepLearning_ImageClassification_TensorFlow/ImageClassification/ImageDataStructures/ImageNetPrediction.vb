Imports ImageClassification.ModelScorer
Imports Microsoft.ML.Data

Namespace ImageClassification.ImageDataStructures
	Public Class ImageNetPrediction
		<ColumnName(TFModelScorer.InceptionSettings.outputTensorName)>
		Public PredictedLabels() As Single
	End Class
End Namespace
