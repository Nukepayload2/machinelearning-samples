Imports Microsoft.ML.Data
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace ImageClassification.DataModels
	Public Class ImagePredictionEx
		Public ImagePath As String
		Public Label As String
		Public PredictedLabelValue As String
		Public Score() As Single

		'[ColumnName("InceptionV3/Predictions/Reshape")]
		'public float[] ImageFeatures;  //In Inception v1: "softmax2_pre_activation"
	End Class
End Namespace
