#Disable Warning BC649 ' We don't care about unsused fields here, because they are mapped with the input file.

Imports Microsoft.ML.Data

Namespace GitHubLabeler.DataStructures
	Friend Class GitHubIssuePrediction
		<ColumnName("PredictedLabel")>
		Public Area As String

		Public Score() As Single
	End Class
End Namespace
