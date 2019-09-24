Imports Microsoft.ML
Imports System.IO

Namespace eShopForecastModelsTrainer
    Friend Class Program
        Private Shared ReadOnly BaseDatasetsRelativePath As String = "../../../Data"
        Private Shared ReadOnly CountryDataRealtivePath As String = $"{BaseDatasetsRelativePath}/countries.stats.csv"
        Private Shared ReadOnly ProductDataRealtivePath As String = $"{BaseDatasetsRelativePath}/products.stats.csv"

        Private Shared ReadOnly CountryDataPath As String = GetAbsolutePath(CountryDataRealtivePath)
        Private Shared ReadOnly ProductDataPath As String = GetAbsolutePath(ProductDataRealtivePath)

        Shared Sub Main(args() As String)
            Try
                ' This sample shows two different ML tasks and algorithms that can be used for forecasting:
                ' 1.) Regression using FastTreeTweedie Regression
                ' 2.) Time Series using Single Spectrum Analysis
                ' Each of these techniques are used to forecast monthly units for the same products so that you can compare the forecasts.

                Dim mlContext As New MLContext(seed:=1) 'Seed set to any number so you have a deterministic environment

                ConsoleWriteHeader("Forecast using Regression model")

                RegressionProductModelHelper.TrainAndSaveModel(mlContext, ProductDataPath)
                RegressionProductModelHelper.TestPrediction(mlContext)

                RegressionCountryModelHelper.TrainAndSaveModel(mlContext, CountryDataPath)
                RegressionCountryModelHelper.TestPrediction(mlContext)

                ConsoleWriteHeader("Forecast using Time Series SSA estimation")

                TimeSeriesModelHelper.PerformTimeSeriesProductForecasting(mlContext, ProductDataPath)
            Catch ex As Exception
                ConsoleWriteException(ex.ToString())
            End Try
            ConsolePressAnyKey()
        End Sub

        Public Shared Function GetAbsolutePath(relativeDatasetPath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativeDatasetPath)

            Return fullPath
        End Function
    End Class
End Namespace
