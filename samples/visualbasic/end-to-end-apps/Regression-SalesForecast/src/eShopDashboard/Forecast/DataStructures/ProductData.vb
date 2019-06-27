Namespace eShopDashboard.Forecast
	''' <summary>
	''' This is the input to the trained model.
	''' </summary>
	Public Class ProductData
		' next,productId,year,month,units,avg,count,max,min,prev
		Public Sub New(productId As String, year As Integer, month As Integer, units As Single, avg As Single, count As Integer, max As Single, min As Single, prev As Single)
			Me.productId = productId
			Me.year = year
			Me.month = month
			Me.units = units
			Me.avg = avg
			Me.count = count
			Me.max = max
			Me.min = min
			Me.prev = prev
		End Sub

		Public [next] As Single

		Public productId As String

		Public year As Single
		Public month As Single
		Public units As Single
		Public avg As Single
		Public count As Single
		Public max As Single
		Public min As Single
		Public prev As Single
	End Class

End Namespace
