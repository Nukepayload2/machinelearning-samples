Imports System
Imports System.Linq

Namespace eShopDashboard.Infrastructure.Setup
	Public Class SeedingStatus
		Private ReadOnly _needsSeeding As Boolean

'INSTANT VB NOTE: The variable needsSeeding was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(needsSeeding_Renamed As Boolean)
			_needsSeeding = needsSeeding_Renamed
		End Sub

'INSTANT VB NOTE: The variable recordsToLoad was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(recordsToLoad_Renamed As Integer)
			If recordsToLoad_Renamed <= 0 Then
				Throw New ArgumentOutOfRangeException(NameOf(recordsToLoad_Renamed), "Must be greater than zero!")
			End If

			_needsSeeding = True
			Me.RecordsToLoad = recordsToLoad_Renamed
		End Sub

		Public Sub New(ParamArray seedingStatuses() As SeedingStatus)
			RecordsToLoad = seedingStatuses.Where(Function(s) s.NeedsSeeding).Sum(Function(s) s.RecordsToLoad)

			_needsSeeding = RecordsToLoad > 0
		End Sub

		Public ReadOnly Property NeedsSeeding As Boolean
			Get
				Return _needsSeeding AndAlso RecordsLoaded < RecordsToLoad
			End Get
		End Property

		Public ReadOnly Property PercentComplete As Integer
			Get
				Return CInt(Math.Truncate(Decimal.Round(If(RecordsToLoad = 0, 100, RecordsLoaded / CDec(RecordsToLoad) * 100))))
			End Get
		End Property

		Public Property RecordsLoaded As Integer

		Public ReadOnly Property RecordsToLoad As Integer

		Public Sub SetAsComplete()
			RecordsLoaded = RecordsToLoad
		End Sub
	End Class
End Namespace