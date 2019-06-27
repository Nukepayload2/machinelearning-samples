Imports Microsoft.ML.Data

Namespace ObjectDetection
	Public Class ImageNetData
		<LoadColumn(0)>
		Public ImagePath As String

		<LoadColumn(1)>
		Public Label As String
	End Class

	Public Class ImageNetDataProbability
		Inherits ImageNetData

		Public PredictedLabel As String
		Public Property Probability As Single
	End Class
End Namespace