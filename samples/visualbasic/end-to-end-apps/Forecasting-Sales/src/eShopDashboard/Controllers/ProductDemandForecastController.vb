Imports eShopForecast
Imports eShopDashboard.Settings
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.ML
Imports Microsoft.Extensions.Options

Namespace eShopDashboard.Controllers
	<Produces("application/json")>
	<Route("api/productdemandforecast")>
	Public Class ProductDemandForecastController
		Inherits Controller

		Private ReadOnly appSettings As AppSettings
		Private ReadOnly productSalesModel As PredictionEnginePool(Of ProductData, ProductUnitRegressionPrediction)

		Public Sub New(appSettings As IOptionsSnapshot(Of AppSettings), productSalesModel As PredictionEnginePool(Of ProductData, ProductUnitRegressionPrediction))
			Me.appSettings = appSettings.Value

			' Get injected Product Sales Model for scoring
			Me.productSalesModel = productSalesModel
		End Sub

		<HttpGet>
		<Route("product/{productId}/unitdemandestimation")>
		Public Function GetProductUnitDemandEstimation(productId As Single, <FromQuery> year As Integer, <FromQuery> month As Integer, <FromQuery> units As Single, <FromQuery> avg As Single, <FromQuery> count As Integer, <FromQuery> max As Single, <FromQuery> min As Single, <FromQuery> prev As Single) As IActionResult
			' Build product sample
			Dim inputExample = New ProductData With {
				.productId = productId,
				.year = year,
				.month = month,
				.units = units,
				.avg = avg,
				.count = count,
				.max = max,
				.min = min,
				.prev = prev
			}

			Dim nextMonthUnitDemandEstimation As ProductUnitRegressionPrediction = Nothing

			'Predict
			nextMonthUnitDemandEstimation = Me.productSalesModel.Predict(inputExample)

			Return Ok(nextMonthUnitDemandEstimation.Score)
		End Function
	End Class
End Namespace
