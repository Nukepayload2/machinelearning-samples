Imports Microsoft.eShopOnContainers.Services.AI.SalesForecasting.MLNet.API.Forecasting
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Options

Namespace Microsoft.eShopOnContainers.Services.AI.SalesForecasting.MLNet.API.Controllers
	<Route("api/v1/ForecastingAI")>
	Public Class SalesForecastingController
		Inherits Controller

		Private ReadOnly appSettings As AppSettings
		Private ReadOnly productSales As IProductSales
		Private ReadOnly countrySales As ICountrySales

		Public Sub New(appSettings As IOptionsSnapshot(Of AppSettings), productSales As IProductSales, countrySales As ICountrySales)
			Me.appSettings = appSettings.Value
			Me.productSales = productSales
			Me.countrySales = countrySales
		End Sub

		<HttpGet>
		<Route("product/{productId}/unitdemandestimation")>
		Public Function GetProductUnitDemandEstimation(productId As String, <FromQuery> year As Integer, <FromQuery> month As Integer, <FromQuery> units As Single, <FromQuery> avg As Single, <FromQuery> count As Integer, <FromQuery> max As Single, <FromQuery> min As Single, <FromQuery> prev As Single, <FromQuery> price As Single, <FromQuery> color As String, <FromQuery> size As String, <FromQuery> shape As String, <FromQuery> agram As String, <FromQuery> bgram As String, <FromQuery> ygram As String, <FromQuery> zgram As String) As IActionResult
			Dim nextMonthUnitDemandEstimation = productSales.Predict($"{appSettings.AIModelsPath}/product_month_fastTreeTweedle.zip", productId, year, month, units, avg, count, max, min, prev, price, color, size, shape, agram, bgram, ygram, zgram)

			Return Ok(nextMonthUnitDemandEstimation.Score)
		End Function

		<HttpGet>
		<Route("country/{country}/salesforecast")>
		Public Function GetCountrySalesForecast(country As String, <FromQuery> year As Integer, <FromQuery> month As Integer, <FromQuery> avg As Single, <FromQuery> max As Single, <FromQuery> min As Single, <FromQuery> p_max As Single, <FromQuery> p_min As Single, <FromQuery> p_med As Single, <FromQuery> prev As Single, <FromQuery> count As Integer, <FromQuery> sales As Single, <FromQuery> std As Single) As IActionResult
			Dim nextMonthSalesForecast = countrySales.Predict($"{appSettings.AIModelsPath}/country_month_fastTreeTweedle.zip", country, year, month, sales, avg, count, max, min, p_max, p_med, p_min, std, prev)

			Return Ok(nextMonthSalesForecast.Score)
		End Function
	End Class
End Namespace
