Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.ML
Imports Microsoft.Extensions.Options
Imports Microsoft.ML
Imports movierecommender.Models
Imports movierecommender.Services
Imports MovieRecommender.DataStructures
Imports Newtonsoft.Json
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Net.Http
Imports System.Text

Namespace movierecommender.Controllers
	Public Class MoviesController
		Inherits Controller

		Private ReadOnly _movieService As IMovieService
		Private ReadOnly _profileService As IProfileService
		Private ReadOnly _appSettings As AppSettings
		Private ReadOnly _logger As ILogger(Of MoviesController)
		Private ReadOnly _model As PredictionEnginePool(Of MovieRating, MovieRatingPrediction)

        Public Sub New(model As PredictionEnginePool(Of MovieRating, MovieRatingPrediction), logger As ILogger(Of MoviesController), appSettings As IOptions(Of AppSettings), movieService_Renamed As IMovieService, profileService As IProfileService)
            _movieService = movieService_Renamed
            _profileService = profileService
            _logger = logger
            _appSettings = appSettings.Value
            _model = model
        End Sub

        Public Function Choose() As ActionResult
			Return View(_movieService.GetSomeSuggestions())
		End Function

		Public Function Recommend(id As Integer) As ActionResult
			Dim activeprofile = _profileService.GetProfileByID(id)

			' 1. Create the ML.NET environment and load the already trained model
			Dim mlContext As MLContext = New MLContext

			Dim ratings As New List(Of (movieId As Integer, normalizedScore As Single))
			Dim MovieRatings = _profileService.GetProfileWatchedMovies(id)
			Dim WatchedMovies As New List(Of Movie)

            For Each x In MovieRatings
                WatchedMovies.Add(_movieService.Get(x.movieId))
            Next

            Dim prediction As MovieRatingPrediction = Nothing
			For Each movie In _movieService.GetTrendingMovies
				' Call the Rating Prediction for each movie prediction
				 prediction = _model.Predict(New MovieRating With {
					 .userId = id.ToString(),
					 .movieId = movie.MovieID.ToString()
				 })

				' Normalize the prediction scores for the "ratings" b/w 0 - 100
				Dim normalizedscore As Single = Sigmoid(prediction.Score)

				' Add the score for recommendation of each movie in the trending movie list
				 ratings.Add((movie.MovieID, normalizedscore))
			Next movie

			'3. Provide rating predictions to the view to be displayed
			ViewData("watchedmovies") = WatchedMovies
			ViewData("ratings") = ratings
			ViewData("trendingmovies") = _movieService.GetTrendingMovies
			Return View(activeprofile)
		End Function

		Public Function Sigmoid(x As Single) As Single
			Return CSng(100/(1 + Math.Exp(-x)))
		End Function

		Public Function Watch() As ActionResult
			Return View()
		End Function

		Public Function Profiles() As ActionResult
'INSTANT VB NOTE: The local variable profiles was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim profiles_Renamed = _profileService.GetProfiles
			Return View(profiles_Renamed)
		End Function

		Public Function Watched(id As Integer) As ActionResult
			Dim activeprofile = _profileService.GetProfileByID(id)
			Dim MovieRatings = _profileService.GetProfileWatchedMovies(id)
			Dim WatchedMovies As New List(Of Movie)

            For Each x In MovieRatings
                WatchedMovies.Add(_movieService.Get(x.movieId))
            Next

            ViewData("watchedmovies") = WatchedMovies
			ViewData("trendingmovies") = _movieService.GetTrendingMovies
			Return View(activeprofile)
		End Function

		Public Class JsonContent
			Inherits StringContent

			Public Sub New(obj As Object)
				MyBase.New(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
			End Sub
		End Class
	End Class
End Namespace
