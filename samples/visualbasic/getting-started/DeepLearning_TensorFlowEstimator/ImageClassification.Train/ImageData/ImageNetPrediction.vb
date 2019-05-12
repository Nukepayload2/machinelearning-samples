Imports System

Namespace ImageClassification.ImageData
	Public Class ImageNetPrediction
		Public Score() As Single

		Public PredictedLabelValue As String
	End Class

	Public Class ImageNetWithLabelPrediction
		Inherits ImageNetPrediction

'INSTANT VB NOTE: The variable label was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(pred As ImageNetPrediction, label_Renamed As String)
			Me.Label = label_Renamed
			Score = pred.Score
			PredictedLabelValue = pred.PredictedLabelValue
		End Sub

		Public Label As String
	End Class

End Namespace
