#Disable Warning BC649 ' We don't care about unsused fields here, because they are mapped with the input file.

Imports Microsoft.ML.Data

Namespace GitHubLabeler.DataStructures
	'The only purpose of this class is for peek data after transforming it with the pipeline
	Friend Class GitHubIssue
		<LoadColumn(0)>
		Public ID As String

		<LoadColumn(1)>
		Public Area As String ' This is an issue label, for example "area-System.Threading"

		<LoadColumn(2)>
		Public Title As String

		<LoadColumn(3)>
		Public Description As String
	End Class
End Namespace
