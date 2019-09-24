Imports eShopForecast

Namespace eShopForecastModelsTrainer.Data
	Public Module SampleProductData
		Public ReadOnly Property MonthlyData As ProductData()

		Sub New()
			MonthlyData = New ProductData() {
				New ProductData With {
					.productId = 263,
					.month = 11,
					.year = 2017,
					.avg = 29,
					.max = 221,
					.min = 1,
					.count = 35,
					.prev = 910,
					.units = 551
				},
				New ProductData With {
					.productId = 988,
					.month = 11,
					.year = 2017,
					.avg = 41,
					.max = 225,
					.min = 4,
					.count = 26,
					.prev = 1094,
					.units = 1076
				}
			}
		End Sub
	End Module
End Namespace
