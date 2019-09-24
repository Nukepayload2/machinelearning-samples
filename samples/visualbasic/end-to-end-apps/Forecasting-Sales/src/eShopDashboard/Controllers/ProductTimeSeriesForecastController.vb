Imports eShopForecast
Imports eShopDashboard.Settings
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Options
Imports Microsoft.ML.Transforms.TimeSeries
Imports Microsoft.ML
Imports System.Linq
Imports eShopDashboard.Queries
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports System

Namespace eShopDashboard.Controllers
	<Produces("application/json")>
	<Route("api/producttimeseriesforecast")>
	Public Class ProductTimeSeriesForecastController
		Inherits Controller

		Private ReadOnly appSettings As AppSettings
		Private ReadOnly mlContext As MLContext = New MLContext
		Private ReadOnly _queries As IOrderingQueries

		Public Sub New(appSettings As IOptionsSnapshot(Of AppSettings), queries As IOrderingQueries)
			Me.appSettings = appSettings.Value
			Me._queries = queries
		End Sub

		<HttpGet>
		<Route("product/{productId}/unittimeseriesestimation")>
		Public Async Function GetProductUnitDemandEstimation(productId As String) As Task(Of IActionResult)
			' Get product history
			Dim productHistory = Await _queries.GetProductDataAsync(productId)

			' Supplement the history with synthetic data
			Dim supplementedProductHistory = TimeSeriesDataGenerator.SupplementData(mlContext, productHistory)
			Dim supplementedProductHistoryLength = supplementedProductHistory.Count() ' 36
			Dim supplementedProductDataView = mlContext.Data.LoadFromEnumerable(supplementedProductHistory)

			' Create and add the forecast estimator to the pipeline.
			Dim forecastEstimator As IEstimator(Of ITransformer) = mlContext.Forecasting.ForecastBySsa(outputColumnName:= NameOf(ProductUnitTimeSeriesPrediction.ForecastedProductUnits), inputColumnName:= NameOf(ProductData.units), windowSize:= 12, seriesLength:= supplementedProductHistoryLength, trainSize:= supplementedProductHistoryLength, horizon:= 1, confidenceLevel:= 0.95F, confidenceLowerBoundColumn:= NameOf(ProductUnitTimeSeriesPrediction.ConfidenceLowerBound), confidenceUpperBoundColumn:= NameOf(ProductUnitTimeSeriesPrediction.ConfidenceUpperBound)) ' TODO: See above comment.

			' Train the forecasting model for the specified product's data series.
			Dim forecastTransformer As ITransformer = forecastEstimator.Fit(supplementedProductDataView)

			' Create the forecast engine used for creating predictions.
			Dim forecastEngine As TimeSeriesPredictionEngine(Of ProductData, ProductUnitTimeSeriesPrediction) = forecastTransformer.CreateTimeSeriesEngine(Of ProductData, ProductUnitTimeSeriesPrediction)(mlContext)

			' Predict
			Dim nextMonthUnitDemandEstimation = forecastEngine.Predict()

			Return Ok(nextMonthUnitDemandEstimation.ForecastedProductUnits.First())
		End Function
	End Class
End Namespace