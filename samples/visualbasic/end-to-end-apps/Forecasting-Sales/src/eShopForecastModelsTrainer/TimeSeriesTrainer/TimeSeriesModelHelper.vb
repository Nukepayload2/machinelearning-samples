Imports System.IO
Imports eShopForecast
Imports eShopForecastModelsTrainer.Data
Imports Microsoft.ML
Imports Microsoft.ML.Transforms.TimeSeries

Namespace eShopForecastModelsTrainer
    Public Class TimeSeriesModelHelper
        ''' <summary>
        ''' Predicts future product sales using time series forecasting with SSA (single spectrum analysis).
        ''' </summary>
        ''' <param name="mlContext">ML.NET context.</param>
        ''' <param name="dataPath">Input training data file path.</param>
        Public Shared Sub PerformTimeSeriesProductForecasting(mlContext As MLContext, dataPath As String)
            Console.WriteLine("=============== Forecasting Product Units ===============")

            ' Forecast units sold for product with Id == 988.
            Dim productId = 988
            ForecastProductUnits(mlContext, productId, dataPath)
        End Sub

        ''' <summary>
        ''' Train and save model for predicting future product sales.
        ''' </summary>
        ''' <param name="mlContext">ML.NET context.</param>
        ''' <param name="productId">Id of the product series to forecast.</param>
        ''' <param name="dataPath">Input training data file path.</param>
        Private Shared Sub ForecastProductUnits(mlContext As MLContext, productId As Integer, dataPath As String)
            Dim productModelPath = $"product{productId}_month_timeSeriesSSA.zip"

            If File.Exists(productModelPath) Then
                File.Delete(productModelPath)
            End If

            Dim productDataView As IDataView = LoadData(mlContext, productId, dataPath)
            Dim singleProductDataSeries = mlContext.Data.CreateEnumerable(Of ProductData)(productDataView, False).OrderBy(Function(p) p.month)
            Dim lastMonthProductData As ProductData = singleProductDataSeries.Last()

            TrainAndSaveModel(mlContext, productDataView, productModelPath)
            TestPrediction(mlContext, lastMonthProductData, productModelPath)
        End Sub

        ''' <summary>
        ''' Loads the monthly product data series for a product with the specified id.
        ''' </summary>
        ''' <param name="mlContext">ML.NET context.</param>
        ''' <param name="productId">Product id.</param>
        ''' <param name="dataPath">Input training data file path.</param>
        Private Shared Function LoadData(mlContext As MLContext, productId As Single, dataPath As String) As IDataView
            ' Load the data series for the specific product that will be used for forecasting sales.
            Dim allProductsDataView As IDataView = mlContext.Data.LoadFromTextFile(Of ProductData)(dataPath, hasHeader:=True, separatorChar:=","c)
            Dim productDataView As IDataView = mlContext.Data.FilterRowsByColumn(allProductsDataView, NameOf(ProductData.productId), productId, productId + 1)

            Return productDataView
        End Function

        ''' <summary>
        ''' Build model for predicting next month's product unit sales using time series forecasting.
        ''' </summary>
        ''' <param name="mlContext">ML.NET context.</param>
        ''' <param name="productDataSeries">ML.NET IDataView representing the loaded product data series.</param>
        ''' <param name="outputModelPath">Trained model path.</param>
        Private Shared Sub TrainAndSaveModel(mlContext As MLContext, productDataView As IDataView, outputModelPath As String)
            ConsoleWriteHeader("Training product forecasting Time Series model")

            Dim supplementedProductDataSeries = TimeSeriesDataGenerator.SupplementData(mlContext, productDataView)
            Dim supplementedProductDataSeriesLength = supplementedProductDataSeries.Count() ' 36
            Dim supplementedProductDataView = mlContext.Data.LoadFromEnumerable(supplementedProductDataSeries, productDataView.Schema)

            ' Create and add the forecast estimator to the pipeline.
            Dim forecastEstimator As IEstimator(Of ITransformer) = mlContext.Forecasting.ForecastBySsa(outputColumnName:=NameOf(ProductUnitTimeSeriesPrediction.ForecastedProductUnits), inputColumnName:=NameOf(ProductData.units), windowSize:=12, seriesLength:=supplementedProductDataSeriesLength, trainSize:=supplementedProductDataSeriesLength, horizon:=2, confidenceLevel:=0.95F, confidenceLowerBoundColumn:=NameOf(ProductUnitTimeSeriesPrediction.ConfidenceLowerBound), confidenceUpperBoundColumn:=NameOf(ProductUnitTimeSeriesPrediction.ConfidenceUpperBound)) ' TODO: See above comment.

            ' Train the forecasting model for the specified product's data series.
            Dim forecastTransformer As ITransformer = forecastEstimator.Fit(supplementedProductDataView)

            ' Create the forecast engine used for creating predictions.
            Dim forecastEngine As TimeSeriesPredictionEngine(Of ProductData, ProductUnitTimeSeriesPrediction) = forecastTransformer.CreateTimeSeriesEngine(Of ProductData, ProductUnitTimeSeriesPrediction)(mlContext)

            ' Save the forecasting model so that it can be loaded within an end-user app.
            forecastEngine.CheckPoint(mlContext, outputModelPath)
        End Sub

        ''' <summary>
        ''' Predict samples using saved model.
        ''' </summary>
        ''' <param name="mlContext">ML.NET context.</param>
        ''' <param name="lastMonthProductData">The last month of product data in the monthly data series.</param>
        ''' <param name="outputModelPath">Model file path</param>
        Private Shared Sub TestPrediction(mlContext As MLContext, lastMonthProductData As ProductData, outputModelPath As String)
            ConsoleWriteHeader("Testing product unit sales forecast Time Series model")

            ' Load the forecast engine that has been previously saved.
            Dim forecaster As ITransformer
            Using file = System.IO.File.OpenRead(outputModelPath)
                Dim schema As DataViewSchema
                forecaster = mlContext.Model.Load(file, schema)
            End Using

            ' We must create a new prediction engine from the persisted model.
            Dim forecastEngine As TimeSeriesPredictionEngine(Of ProductData, ProductUnitTimeSeriesPrediction) = forecaster.CreateTimeSeriesEngine(Of ProductData, ProductUnitTimeSeriesPrediction)(mlContext)

            ' Get the prediction; this will include the forecasted product units sold for the next 2 months since this the time period specified in the `horizon` parameter when the forecast estimator was originally created.
            Console.WriteLine(vbLf & "** Original prediction **")
            Dim originalSalesPrediction As ProductUnitTimeSeriesPrediction = forecastEngine.Predict()

            ' Compare the units of the first forecasted month to the actual units sold for the next month.
            Dim predictionMonth = If(lastMonthProductData.month = 12, 1, lastMonthProductData.month + 1)
            Dim predictionYear = If(predictionMonth < lastMonthProductData.month, lastMonthProductData.year + 1, lastMonthProductData.year)
            Console.WriteLine($"Product: {lastMonthProductData.productId}, Month: {predictionMonth}, Year: {predictionYear} " & $"- Real Value (units): {lastMonthProductData.next}, Forecasted (units): {originalSalesPrediction.ForecastedProductUnits(0)}")

            ' Get the first forecasted month's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{originalSalesPrediction.ConfidenceLowerBound(0)} - {originalSalesPrediction.ConfidenceUpperBound(0)}]" & vbLf)

            ' Get the units of the second forecasted month.
            Console.WriteLine($"Product: {lastMonthProductData.productId}, Month: {lastMonthProductData.month + 2}, Year: {lastMonthProductData.year}, " & $"Forecasted (units): {originalSalesPrediction.ForecastedProductUnits(1)}")

            ' Get the second forecasted month's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{originalSalesPrediction.ConfidenceLowerBound(1)} - {originalSalesPrediction.ConfidenceUpperBound(1)}]" & vbLf)

            ' Update the forecasting model with the next month's actual product data to get an updated prediction; this time, only forecast product sales for 1 month ahead.
            Console.WriteLine("** Updated prediction **")
            Dim newProductData As ProductData = SampleProductData.MonthlyData.Where(Function(p) p.productId = lastMonthProductData.productId).Single()
            Dim updatedSalesPrediction As ProductUnitTimeSeriesPrediction = forecastEngine.Predict(newProductData, horizon:=1)

            ' Save the updated forecasting model.
            forecastEngine.CheckPoint(mlContext, outputModelPath)

            ' Get the units of the updated forecast.
            predictionMonth = If(lastMonthProductData.month >= 11, (lastMonthProductData.month + 2) Mod 12, lastMonthProductData.month + 2)
            predictionYear = If(predictionMonth < lastMonthProductData.month, lastMonthProductData.year + 1, lastMonthProductData.year)
            Console.WriteLine($"Product: {lastMonthProductData.productId}, Month: {predictionMonth}, Year: {predictionYear}, " & $"Forecasted (units): {updatedSalesPrediction.ForecastedProductUnits(0)}")

            ' Get the updated forecast's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{updatedSalesPrediction.ConfidenceLowerBound(0)} - {updatedSalesPrediction.ConfidenceUpperBound(0)}]" & vbLf)
        End Sub
    End Class
End Namespace
