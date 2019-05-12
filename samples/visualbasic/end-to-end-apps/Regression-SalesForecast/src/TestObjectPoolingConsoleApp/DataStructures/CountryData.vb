Namespace TestObjectPoolingConsoleApp.DataStructures
	''' <summary>
	''' This is the input to the trained model.
	''' </summary>
	Public Class CountryData
		' next,country,year,month,max,min,std,count,sales,med,prev
		Public Sub New(country As String, year As Integer, month As Integer, max As Single, min As Single, std As Single, count As Integer, sales As Single, med As Single, prev As Single)
			Me.country = country

			Me.year = year
			Me.month = month
			Me.max = max
			Me.min = min
			Me.std = std
			Me.count = count
			Me.sales = sales
			Me.med = med
			Me.prev = prev
		End Sub

		Public [next] As Single

		Public country As String

		Public year As Single
		Public month As Single
		Public max As Single
		Public min As Single
		Public std As Single
		Public count As Single
		Public sales As Single
		Public med As Single
		Public prev As Single
	End Class
End Namespace
