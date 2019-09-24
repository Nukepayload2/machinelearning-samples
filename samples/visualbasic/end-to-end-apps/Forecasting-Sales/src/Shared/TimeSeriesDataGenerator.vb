Imports Microsoft.ML
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq

Namespace eShopForecast
	Public Class TimeSeriesDataGenerator
		''' <summary>
		''' Supplements the data and returns the orignial list of months with addtional months
		''' prepended to total a full 36 months.
		Public Shared Function SupplementData(mlContext As MLContext, productDataSeries As IDataView) As IEnumerable(Of ProductData)
			Return SupplementData(mlContext, mlContext.Data.CreateEnumerable(Of ProductData)(productDataSeries, False))
		End Function


		''' <summary>
		''' Supplements the data and returns the orignial list of months with addtional months
		''' prepended to total a full 36 months.
		Public Shared Function SupplementData(mlContext As MLContext, singleProductSeries As IEnumerable(Of ProductData)) As IEnumerable(Of ProductData)
			Dim supplementedProductSeries = New List(Of ProductData)(singleProductSeries)

			' Get the first month in series
			Dim firstMonth = singleProductSeries.FirstOrDefault(Function(p) p.year = 2017 AndAlso p.month = singleProductSeries.Select(Function(pp) pp.month).Min())

			Dim referenceMonth = firstMonth

			Dim randomCountDelta As Single = 4
			Dim randomMaxDelta As Single = 10

			If singleProductSeries.Count() < 12 Then
				Dim yearDelta = 12 - singleProductSeries.Count()

				For i As Integer = 1 To yearDelta
					Dim month = If(firstMonth.month - i < 1, 12 - MathF.Abs(firstMonth.month - i), firstMonth.month - 1)

					Dim year = If(month > firstMonth.month, firstMonth.year - 1, firstMonth.year)

					Dim calculatedCount = MathF.Round(singleProductSeries.Select(Function(p) p.count).Average()) - randomCountDelta
					Dim calculatedMax = MathF.Round(singleProductSeries.Select(Function(p) p.max).Average()) - randomMaxDelta
					Dim calculatedMin = (New Random).Next(1, 5)

					Dim productData = New ProductData With {
						.next = referenceMonth.units,
						.productId = firstMonth.productId,
						.year = year,
						.month = month,
						.units = referenceMonth.prev,
						.avg = MathF.Round(referenceMonth.prev / calculatedCount),
						.count = calculatedCount,
						.max = calculatedMax,
						.min = calculatedMin,
						.prev = referenceMonth.prev - MathF.Round((referenceMonth.units - referenceMonth.prev) / 2)
					}

					supplementedProductSeries.Insert(0, productData)

					referenceMonth = productData
				Next i
			End If

			Return SupplementDataWithYear(SupplementDataWithYear(supplementedProductSeries))
		End Function

		''' <summary>
		''' If we have 12 months worth of data, this will suppliment the data with an additional 12
		''' PREVIOUS months based on the growth exponent provided
		''' </summary>
		''' <param name="singleProductSeries">The initial 12 months of product data.</param>
		''' <param name="growth">The amount the values should grow year over year.</param>
		''' <returns></returns>
		Private Shared Function SupplementDataWithYear(singleProductSeries As IEnumerable(Of ProductData), Optional growth As Single = 0.1F) As IEnumerable(Of ProductData)
			If singleProductSeries.Count() < 12 Then
				Throw New NotImplementedException("fix this, currently only handles if there's already a full 12 months or more of data.")
			End If

			Dim supplementedProductSeries = New List(Of ProductData)

			Dim growthMultiplier = 1 - growth

			Dim firstYear = singleProductSeries.Take(12)

			For Each product In firstYear
				Dim newUnits = MathF.Floor(product.units * growthMultiplier)
				Dim newCount = (New Random).Next(CInt(Math.Truncate(MathF.Floor(product.count * growthMultiplier))), CInt(Math.Truncate(product.count)))
				Dim newMax = MathF.Floor(product.max * growthMultiplier)
				Dim newMin = (New Random).Next(1, 4)

				Dim newProduct = New ProductData With {
					.next = MathF.Floor(product.next * growthMultiplier),
					.productId = product.productId,
					.year = product.year - 1,
					.month = product.month,
					.units = newUnits,
					.avg = MathF.Round(newUnits / newCount),
					.count = newCount,
					.max = newMax,
					.min = newMin,
					.prev = MathF.Floor(product.prev * growthMultiplier)
				}

				supplementedProductSeries.Add(newProduct)
			Next product

			supplementedProductSeries.AddRange(singleProductSeries)

			Return supplementedProductSeries
		End Function
	End Class
End Namespace
