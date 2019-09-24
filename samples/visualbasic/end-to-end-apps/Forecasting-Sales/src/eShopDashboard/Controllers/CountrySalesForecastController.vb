Imports eShopForecast
Imports eShopDashboard.Settings
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.ML
Imports Microsoft.Extensions.Options

Namespace eShopDashboard.Controllers
	<Produces("application/json")>
	<Route("api/countrysalesforecast")>
	Public Class CountrySalesForecastController
		Inherits Controller

		Private ReadOnly appSettings As AppSettings
		Private ReadOnly countrySalesModel As PredictionEnginePool(Of CountryData, CountrySalesPrediction)
		Private ReadOnly logger As ILogger(Of CountrySalesForecastController)

		Public Sub New(appSettings As IOptionsSnapshot(Of AppSettings), countrySalesModel As PredictionEnginePool(Of CountryData, CountrySalesPrediction), logger As ILogger(Of CountrySalesForecastController))
			Me.appSettings = appSettings.Value

			' Get injected Country Sales Model for scoring
			Me.countrySalesModel = countrySalesModel

			Me.logger = logger
		End Sub

		<HttpGet>
		<Route("country/{country}/salesforecast")>
		Public Function GetCountrySalesForecast(country As String, <FromQuery> year As Integer, <FromQuery> month As Integer, <FromQuery> med As Single, <FromQuery> max As Single, <FromQuery> min As Single, <FromQuery> prev As Single, <FromQuery> count As Integer, <FromQuery> sales As Single, <FromQuery> std As Single) As IActionResult
			' Build country sample
			Dim countrySample = New CountryData With {
				.country = country,
				.year = year,
				.month = month,
				.max = max,
				.min = min,
				.std = std,
				.count = count,
				.sales = sales,
				.med = med,
				.prev = prev
			}

			Me.logger.LogInformation($"Start predicting")
			'Measure execution time
			Dim watch = System.Diagnostics.Stopwatch.StartNew()

			Dim nextMonthSalesForecast As CountrySalesPrediction = Nothing

			'Predict
			nextMonthSalesForecast = Me.countrySalesModel.Predict(countrySample)

			'Stop measuring time
			watch.Stop()
			Dim elapsedMs As Long = watch.ElapsedMilliseconds
			Me.logger.LogInformation($"Prediction processed in {elapsedMs} miliseconds")

			Return Ok(nextMonthSalesForecast.Score)
		End Function
	End Class
End Namespace
