Imports Microsoft.ML.Data

Namespace eShopForecast
	Public Class CountryData
		<LoadColumn(0)>
		Public [next] As Single

		<LoadColumn(1)>
		Public country As String

		<LoadColumn(2)>
		Public year As Single

		<LoadColumn(3)>
		Public month As Single

		<LoadColumn(4)>
		Public max As Single

		<LoadColumn(5)>
		Public min As Single

		<LoadColumn(6)>
		Public std As Single

		<LoadColumn(7)>
		Public count As Single

		<LoadColumn(8)>
		Public sales As Single

		<LoadColumn(9)>
		Public med As Single

		<LoadColumn(10)>
		Public prev As Single

		Public Overrides Function ToString() As String
			Return $"CountryData [next: {[next]}, country: {country}, year: {year}, month: {month}, max: {max}, min: {min}, std: {std}, count: {count}, sales: {sales}, med: {med}, prev: {prev}]"
		End Function
	End Class
End Namespace
