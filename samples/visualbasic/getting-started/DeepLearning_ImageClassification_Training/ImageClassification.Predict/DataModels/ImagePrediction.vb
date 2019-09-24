Imports Microsoft.ML.Data
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace ImageClassification.DataModels
	Public Class ImagePrediction
		<ColumnName("Score")>
		Public Score() As Single

		<ColumnName("PredictedLabel")>
		Public PredictedLabel As UInt32
	End Class
End Namespace
