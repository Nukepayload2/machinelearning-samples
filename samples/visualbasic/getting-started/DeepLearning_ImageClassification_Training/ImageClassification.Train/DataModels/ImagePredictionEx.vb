Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace ImageClassification.DataModels
	Public Class ImagePredictionEx
		Public ImagePath As String
		Public Label As String
		Public PredictedLabel As UInt32
		Public Score() As Single
	End Class
End Namespace
