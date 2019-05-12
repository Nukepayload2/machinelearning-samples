Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace GitHubLabeler.DataStructures
	Public Class FullPrediction
		Public PredictedLabel As String
		Public Score As Single
		Public OriginalSchemaIndex As Integer

'INSTANT VB NOTE: The variable predictedLabel was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable score was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable originalSchemaIndex was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(predictedLabel_Renamed As String, score_Renamed As Single, originalSchemaIndex_Renamed As Integer)
			Me.PredictedLabel = predictedLabel_Renamed
			Me.Score = score_Renamed
			Me.OriginalSchemaIndex = originalSchemaIndex_Renamed
		End Sub
	End Class
End Namespace
