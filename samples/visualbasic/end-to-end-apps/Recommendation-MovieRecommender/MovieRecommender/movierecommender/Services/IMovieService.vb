Imports movierecommender.Models
Imports System.Collections.Generic

Namespace movierecommender.Services
	Public Interface IMovieService
		Function [Get](id As Integer) As Movie
		Function GetAllMovies() As IEnumerable(Of Movie)
		Function GetModelPath() As String
		Function GetRecentMovies() As IEnumerable(Of Movie)
		Function GetSomeSuggestions() As IEnumerable(Of Movie)

		ReadOnly Property GetTrendingMovies As List(Of Movie)
	End Interface
End Namespace