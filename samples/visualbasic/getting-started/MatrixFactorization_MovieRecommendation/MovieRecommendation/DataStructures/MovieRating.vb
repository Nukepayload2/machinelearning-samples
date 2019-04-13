Imports Microsoft.ML.Data
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace MovieRecommendationConsoleApp.DataStructures
	Public Class MovieRating
		<LoadColumn(0)>
		Public userId As Single

		<LoadColumn(1)>
		Public movieId As Single

		<LoadColumn(2)>
		Public Label As Single
	End Class
End Namespace
