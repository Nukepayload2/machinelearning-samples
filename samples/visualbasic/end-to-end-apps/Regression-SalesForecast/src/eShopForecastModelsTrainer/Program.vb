Imports Microsoft.ML
Imports System
Imports System.IO
Imports System.Threading.Tasks
Imports eShopForecastModelsTrainer.ConsoleHelpers

Namespace eShopForecastModelsTrainer
	Friend Class Program
		Private Shared ReadOnly BaseDatasetsRelativePath As String = "../../../Data"
		Private Shared ReadOnly CountryDataRealtivePath As String = $"{BaseDatasetsRelativePath}/countries.stats.csv"
		Private Shared ReadOnly ProductDataRealtivePath As String = $"{BaseDatasetsRelativePath}/products.stats.csv"

		Private Shared ReadOnly CountryDataPath As String = GetAbsolutePath(CountryDataRealtivePath)
		Private Shared ReadOnly ProductDataPath As String = GetAbsolutePath(ProductDataRealtivePath)

		Shared Sub Main(args() As String)

			Try
				Dim mlContext As New MLContext(seed:= 1) 'Seed set to any number so you have a deterministic environment

				ProductModelHelper.TrainAndSaveModel(mlContext, ProductDataPath)
				ProductModelHelper.TestPrediction(mlContext)

				CountryModelHelper.TrainAndSaveModel(mlContext, CountryDataPath)
				CountryModelHelper.TestPrediction(mlContext)
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
