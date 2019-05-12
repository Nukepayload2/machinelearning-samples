Imports Microsoft.ML.Data

Namespace eShopForecastModelsTrainer
	Public Class ProductData
		' next,productId,year,month,units,avg,count,max,min,prev
		<LoadColumn(0)>
		Public [next] As Single

		<LoadColumn(1)>
		Public productId As String

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
	End Class

	Public Class ProductUnitPrediction
		Public Score As Single
	End Class
End Namespace
