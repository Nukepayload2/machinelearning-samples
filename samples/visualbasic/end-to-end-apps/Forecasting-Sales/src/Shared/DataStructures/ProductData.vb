Imports Microsoft.ML.Data

Namespace eShopForecast
	Public Class ProductData
		' The index of column in LoadColumn(int index) should be matched with the position of columns in the underlying data file.
		' The next column is used by the Regression algorithm as the Label (e.g. the value that is being predicted by the Regression model).
		<LoadColumn(0)>
		Public [next] As Single

		<LoadColumn(1)>
		Public productId As Single

		<LoadColumn(2)>
		Public year As Single

		<LoadColumn(3)>
		Public month As Single

		<LoadColumn(4)>
		Public units As Single

		<LoadColumn(5)>
		Public avg As Single

		<LoadColumn(6)>
		Public count As Single

		<LoadColumn(7)>
		Public max As Single

		<LoadColumn(8)>
		Public min As Single

		<LoadColumn(9)>
		Public prev As Single

		Public Overrides Function ToString() As String
			Return $"ProductData [ productId: {productId}, year: {year}, month: {month:00}, next: {[next]:0000}, units: {units:0000}, avg: {avg:000}, count: {count:00}, max: {max:000}, min: {min}, prev: {prev:0000} ]"
		End Function
	End Class
End Namespace
