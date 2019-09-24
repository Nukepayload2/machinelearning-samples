Imports System.Collections.Generic

Namespace movierecommender.Models
	Public Class Profile
		Public Property ProfileID As Integer
		Public Property ProfileImageName As String
		Public Property ProfileName As String
		Public Property ProfileMovieRatings As List(Of (movieId As Integer, movieRating As Integer))
			Get
			Set(value As List(Of (movieId As Integer, movieRating As Integer)))
			End Set
			End Get
		End Property
