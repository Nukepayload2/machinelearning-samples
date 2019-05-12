Imports Microsoft.ML.Data

Namespace eShopForecastModelsTrainer
	Public Class CountryData
		' next,country,year,month,max,min,std,count,sales,med,prev
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
	End Class

	Public Class CountrySalesPrediction
		Public Score As Single
	End Class
End Namespace
