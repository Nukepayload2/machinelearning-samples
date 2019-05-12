Imports System
Imports System.IO
Imports System.Linq
Imports Microsoft.ML
Imports eShopForecastModelsTrainer.ConsoleHelpers
Imports Common
Imports Microsoft.ML.Data

Namespace eShopForecastModelsTrainer
	Public Class CountryModelHelper
		''' <summary>
		''' Train and save model for predicting next month country unit sales
		''' </summary>
		''' <param name="dataPath">Input training file path</param>
		''' <param name="outputModelPath">Trained model path</param>
		Public Shared Sub TrainAndSaveModel(mlContext As MLContext, dataPath As String, Optional outputModelPath As String = "country_month_fastTreeTweedie.zip")
			If File.Exists(outputModelPath) Then
				File.Delete(outputModelPath)
			End If

			CreateCountryModel(mlContext, dataPath, outputModelPath)
		End Sub

		''' <summary>
		''' Build model for predicting next month country unit sales using Learning Pipelines API
		''' </summary>
		''' <param name="dataPath">Input training file path</param>
		''' <returns></returns>
		Private Shared Sub CreateCountryModel(mlContext As MLContext, dataPath As String, outputModelPath As String)
			ConsoleWriteHeader("Training country forecasting model")

			Dim trainingDataView = mlContext.Data.LoadFromTextFile(Of CountryData)(path:=dataPath, hasHeader:= True, separatorChar:= ","c)

			Dim trainer = mlContext.Regression.Trainers.FastTreeTweedie("Label", "Features")

			Dim trainingPipeline = mlContext.Transforms.Concatenate(outputColumnName:= "NumFeatures", NameOf(CountryData.year), NameOf(CountryData.month), NameOf(CountryData.max), NameOf(CountryData.min), NameOf(CountryData.std), NameOf(CountryData.count), NameOf(CountryData.sales), NameOf(CountryData.med), NameOf(CountryData.prev)).Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName:= "CatFeatures", inputColumnName:= NameOf(CountryData.country))).Append(mlContext.Transforms.Concatenate(outputColumnName:= "Features", "NumFeatures", "CatFeatures")).Append(mlContext.Transforms.CopyColumns(outputColumnName:= "Label", inputColumnName:= NameOf(CountryData.next))).Append(trainer)

			' Cross-Validate with single dataset (since we don't have two datasets, one for training and for evaluate)
			' in order to evaluate and get the model's accuracy metrics
			Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============")
			Dim crossValidationResults = mlContext.Regression.CrossValidate(data:=trainingDataView, estimator:=trainingPipeline, numberOfFolds:= 6, labelColumnName:= "Label")
			ConsoleHelper.PrintRegressionFoldsAverageMetrics(trainer.ToString(), crossValidationResults)

			' Create and Train the model
			Dim model = trainingPipeline.Fit(trainingDataView)
			'Save model
			mlContext.Model.Save(model, trainingDataView.Schema, outputModelPath)
		End Sub

		''' <summary>
		''' Predict samples using saved model
		''' </summary>
		''' <param name="outputModelPath">Model file path</param>
		''' <returns></returns>
		Public Shared Sub TestPrediction(mlContext As MLContext, Optional outputModelPath As String = "country_month_fastTreeTweedie.zip")
			ConsoleWriteHeader("Testing Country Sales Forecast model")

			Dim trainedModel As ITransformer
			Using stream = File.OpenRead(outputModelPath)
				Dim modelInputSchema As Object
				trainedModel = mlContext.Model.Load(stream, modelInputSchema)
			End Using

			'ITransformer trainedModel;
			'using (var file = File.OpenRead(outputModelPath))
			'{
			'    trainedModel = TransformerChain
			'        .LoadFrom(mlContext, file);
			'}

			Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of CountryData, CountrySalesPrediction)(trainedModel)

			Console.WriteLine("** Testing Country 1 **")

			' Build sample data
			Dim dataSample = New CountryData With {
				.country = "United Kingdom",
				.month = 10,
				.year = 2017,
				.med = 309.945F,
				.max = 587.902F,
				.min = 135.640F,
				.std = 1063.932092F,
				.prev = 856548.78F,
				.count = 1724,
				.sales = 873612.9F
			}
			' Predict sample data
			Dim prediction = predictionEngine.Predict(dataSample)
			Console.WriteLine($"Country: {dataSample.country}, month to predict: {dataSample.month + 1}, year: {dataSample.year} - Real value (US$): {Math.Pow(6.0084501F, 10)}, Predicted Forecast (US$): {Math.Pow(prediction.Score, 10)}")

			dataSample = New CountryData With {
				.country = "United Kingdom",
				.month = 11,
				.year = 2017,
				.med = 288.72F,
				.max = 501.488F,
				.min = 134.5360F,
				.std = 707.5642F,
				.prev = 873612.9F,
				.count = 2387,
				.sales = 1019647.67F
			}
			prediction = predictionEngine.Predict(dataSample)
			Console.WriteLine($"Country: {dataSample.country}, month to predict: {dataSample.month + 1}, year: {dataSample.year} - Predicted Forecast (US$):  {Math.Pow(prediction.Score, 10)}")

			Console.WriteLine(" ")

			Console.WriteLine("** Testing Country 2 **")
			dataSample = New CountryData With {
				.country = "United States",
				.month = 10,
				.year = 2017,
				.med = 400.17F,
				.max = 573.63F,
				.min = 340.395F,
				.std = 340.3959F,
				.prev = 4264.94F,
				.count = 10,
				.sales = 5322.56F
			}
			prediction = predictionEngine.Predict(dataSample)
			Console.WriteLine($"Country: {dataSample.country}, month to predict: {dataSample.month + 1}, year: {dataSample.year} - Real value (US$): {Math.Pow(3.805769F, 10)}, Predicted Forecast (US$): {Math.Pow(prediction.Score, 10)}")

			dataSample = New CountryData With {
				.country = "United States",
				.month = 11,
				.year = 2017,
				.med = 317.9F,
				.max = 1135.99F,
				.min = 249.44F,
				.std = 409.75528F,
				.prev = 5322.56F,
				.count = 11,
				.sales = 6393.96F
			}
			prediction = predictionEngine.Predict(dataSample)
			Console.WriteLine($"Country: {dataSample.country}, month to predict: {dataSample.month + 1}, year: {dataSample.year} - Predicted Forecast (US$):  {Math.Pow(prediction.Score, 10)}")
		End Sub
	End Class
End Namespace
