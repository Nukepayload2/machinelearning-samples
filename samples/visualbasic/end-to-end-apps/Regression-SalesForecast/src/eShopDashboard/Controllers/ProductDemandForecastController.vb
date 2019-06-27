Imports CommonHelpers
Imports eShopDashboard.Forecast
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
		Private ReadOnly productSalesModel As PredictionEnginePool(Of ProductData, ProductUnitPrediction)

		Public Sub New(appSettings As IOptionsSnapshot(Of AppSettings), productSalesModel As PredictionEnginePool(Of ProductData, ProductUnitPrediction))
			Me.appSettings = appSettings.Value

			' Get injected Product Sales Model for scoring
			Me.productSalesModel = productSalesModel
		End Sub

		<HttpGet>
		<Route("product/{productId}/unitdemandestimation")>
		Public Function GetProductUnitDemandEstimation(productId As String, <FromQuery> year As Integer, <FromQuery> month As Integer, <FromQuery> units As Single, <FromQuery> avg As Single, <FromQuery> count As Integer, <FromQuery> max As Single, <FromQuery> min As Single, <FromQuery> prev As Single) As IActionResult
			' Build product sample
			Dim inputExample = New ProductData(productId, year, month, units, avg, count, max, min, prev)

			Dim nextMonthUnitDemandEstimation As ProductUnitPrediction = Nothing

			'Predict
			nextMonthUnitDemandEstimation = Me.productSalesModel.Predict(inputExample)

			Return Ok(nextMonthUnitDemandEstimation.Score)
		End Function
	End Class
End Namespace
