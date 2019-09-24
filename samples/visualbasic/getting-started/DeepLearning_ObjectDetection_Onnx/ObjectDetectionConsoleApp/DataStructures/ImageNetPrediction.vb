Imports Microsoft.ML.Data

Namespace ObjectDetection.DataStructures
	Public Class ImageNetPrediction
		<ColumnName("grid")>
		Public PredictedLabels() As Single
	End Class
End Namespace
