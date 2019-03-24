Imports Microsoft.ML.Data

Namespace SpamDetectionConsoleApp.MLDataStructures
	Friend Class SpamInput
		<LoadColumn(0)>
		Public Property Label As String
		<LoadColumn(1)>
		Public Property Message As String
	End Class
End Namespace
