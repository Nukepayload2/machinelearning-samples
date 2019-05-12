Imports movierecommender.Models
Imports System.Collections.Generic

Namespace movierecommender.Services
	Public Interface IProfileService
		Function GetProfileByID(id As Integer) As Profile

		Function GetProfileWatchedMovies(id As Integer) As List(Of (movieId As Integer, movieRating As Integer))

		ReadOnly Property GetProfiles As List(Of Profile)
	End Interface
End Namespace