Namespace eShopForecast
	''' <summary>
	''' This is the output of the scored regression model, the prediction.
	''' </summary>
	Public Class ProductUnitRegressionPrediction
		' Below columns are produced by the model's predictor.
		Public Score As Single
	End Class

	''' <summary>
	''' This is the output of the scored time series model, the prediction.
	''' </summary>
	Public Class ProductUnitTimeSeriesPrediction
		Public Property ForecastedProductUnits As Single()

		Public Property ConfidenceLowerBound As Single()

		Public Property ConfidenceUpperBound As Single()
	End Class
End Namespace
