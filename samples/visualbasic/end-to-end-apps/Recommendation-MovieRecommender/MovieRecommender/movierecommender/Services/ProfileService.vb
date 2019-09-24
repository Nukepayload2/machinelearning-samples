Imports movierecommender.Models
Imports System.Collections.Generic
Imports System.IO

Namespace movierecommender.Services
	Public Class ProfileService
		Implements IProfileService

		Private _profile As New List(Of Profile)(LoadProfileData())

		Public ReadOnly Property GetProfiles As List(Of Profile) Implements IProfileService.GetProfiles
			Get
				Return _profile
			End Get
		End Property


		Public _activeprofileid As Integer = -1

		Public Function GetProfileWatchedMovies(id As Integer) As List(Of (movieId As Integer, movieRating As Integer))
			For Each Profile As Profile In _profile
				If id = Profile.ProfileID Then
					Return Profile.ProfileMovieRatings
				End If
			Next Profile

			Return Nothing
		End Function

		Public Function GetProfileByID(id As Integer) As Profile Implements IProfileService.GetProfileByID
			For Each Profile As Profile In _profile
				If id = Profile.ProfileID Then
					Return Profile
				End If
			Next Profile

			Return Nothing
		End Function

		Private Shared Function LoadProfileData() As List(Of Profile)
			Dim result As New List(Of Profile)

			Dim fileReader As FileStream = File.OpenRead("Content/Profiles.csv")
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
					Dim ProfileID As Integer = Integer.Parse(fields(0).TrimStart(New Char() { "0"c }))
					Dim ProfileImageName As String = fields(1)
					Dim ProfileName As String = fields(2)

					Dim ratings As New List(Of (movieId As Integer, movieRating As Integer))

					For i As Integer = 3 To fields.Length - 1 Step 2
						ratings.Add((Integer.Parse(fields(i)), Integer.Parse(fields(i+1))))
					Next i
					result.Add(New Profile With {
						.ProfileID = ProfileID,
						.ProfileImageName = ProfileImageName,
						.ProfileName = ProfileName,
						.ProfileMovieRatings = ratings
					})
					index += 1
				Loop
			Finally
				If reader IsNot Nothing Then
					reader.Dispose()
				End If
			End Try
			Return result
		End Function


	End Class
End Namespace
