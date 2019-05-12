Imports Microsoft.AspNetCore.Hosting
Imports movierecommender.Models
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace movierecommender.Services
	Public Class MovieService
		Implements IMovieService

		Public ReadOnly Shared _moviesToRecommend As Integer = 6
		Private ReadOnly Shared _trendingMoviesCount As Integer = 20
		Public _movies As New Lazy(Of List(Of Movie))(AddressOf LoadMovieData)
		Private _trendingMovies As List(Of Movie) = LoadTrendingMovies()
		Public ReadOnly Shared _modelpath As String = "model.zip"
		Private ReadOnly _hostingEnvironment As IHostingEnvironment

		Public ReadOnly Property GetTrendingMovies As List(Of Movie) Implements IMovieService.GetTrendingMovies
			Get
				Return LoadTrendingMovies()
			End Get
		End Property

		Public Sub New(hostingEnvironment As IHostingEnvironment)
			_hostingEnvironment = hostingEnvironment
		End Sub

		Public Shared Function LoadTrendingMovies() As List(Of Movie)
			Dim result As New List(Of Movie)

			result.Add(New Movie With {
				.MovieID = 1573,
				.MovieName = "Face/Off (1997)"
			})
			result.Add(New Movie With {
				.MovieID = 1721,
				.MovieName = "Titanic (1997)"
			})
			result.Add(New Movie With {
				.MovieID = 1703,
				.MovieName = "Home Alone 3 (1997)"
			})
			result.Add(New Movie With {
				.MovieID = 49272,
				.MovieName = "Casino Royale (2006)"
			})
			result.Add(New Movie With {
				.MovieID = 5816,
				.MovieName = "Harry Potter and the Chamber of Secrets (2002)"
			})
			result.Add(New Movie With {
				.MovieID = 3578,
				.MovieName = "Gladiator (2000)"
			})
			Return result
		End Function

		Public Function GetModelPath() As String Implements IMovieService.GetModelPath
			Return Path.Combine(_hostingEnvironment.ContentRootPath, "Models", _modelpath)
		End Function

		Public Function GetSomeSuggestions() As IEnumerable(Of Movie) Implements IMovieService.GetSomeSuggestions
			Dim movies() As Movie = GetRecentMovies().ToArray()

			Dim rnd As Random = New Random
			Dim movieselector(_moviesToRecommend - 1) As Integer

			For i As Integer = 0 To _moviesToRecommend - 1
				movieselector(i) = rnd.Next(movies.Length)
			Next i

			Return movieselector.Select(Function(s) movies(s))
		End Function

		Public Function GetRecentMovies() As IEnumerable(Of Movie) Implements IMovieService.GetRecentMovies
			Return GetAllMovies().Where(Function(m) m.MovieName.Contains("20") OrElse m.MovieName.Contains("198") OrElse m.MovieName.Contains("199"))
		End Function

		Public Function [Get](id As Integer) As Movie Implements IMovieService.Get
			Return _movies.Value.Single(Function(m) m.MovieID = id)
		End Function

		Public Function GetAllMovies() As IEnumerable(Of Movie) Implements IMovieService.GetAllMovies
			Return _movies.Value
		End Function

		Private Shared Function LoadMovieData() As List(Of Movie)
			Dim result As New List(Of Movie)

			Dim fileReader As FileStream = File.OpenRead("Content/movies.csv")

			Dim reader As New StreamReader(fileReader)
			Try
				Dim header As Boolean = True
				Dim index As Integer = 0
				Dim line As String = ""
				Do While Not reader.EndOfStream
					If header Then
						line = reader.ReadLine()
						header = False
					End If
					line = reader.ReadLine()
					Dim fields() As String = line.Split(","c)
					Dim MovieID As Integer = Integer.Parse(fields(0).TrimStart(New Char() { "0"c }))
					Dim MovieName As String = fields(1)
					result.Add(New Movie With {
						.MovieID = MovieID,
						.MovieName = MovieName
					})
					index += 1
				Loop
			Finally
				reader?.Dispose()
			End Try

			Return result
		End Function
	End Class
End Namespace