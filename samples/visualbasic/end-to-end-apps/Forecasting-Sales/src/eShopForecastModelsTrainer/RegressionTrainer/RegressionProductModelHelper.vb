Imports eShopForecast
Imports Microsoft.ML
Imports System
Imports System.IO
Imports System.Linq
Imports eShopForecastModelsTrainer.ConsoleHelperExt
Imports Common
Imports Microsoft.ML.Data

Namespace eShopForecastModelsTrainer
	Public Class RegressionProductModelHelper
		''' <summary>
		''' Train and save model for predicting the next month's product unit sales
		''' </summary>
		''' <param name="dataPath">Input training file path</param>
		''' <param name="outputModelPath">Trained model path</param>
		Public Shared Sub TrainAndSaveModel(mlContext As MLContext, dataPath As String, Optional outputModelPath As String = "product_month_fastTreeTweedie.zip")
			If File.Exists(outputModelPath) Then
				File.Delete(outputModelPath)
			End If

			CreateProductModelUsingPipeline(mlContext, dataPath, outputModelPath)
		End Sub

		''' <summary>
		''' Build model for predicting next month's product unit sales using Learning Pipelines API
		''' </summary>
		''' <param name="dataPath">Input training file path</param>
		Private Shared Sub CreateProductModelUsingPipeline(mlContext As MLContext, dataPath As String, outputModelPath As String)
			ConsoleWriteHeader("Training product forecasting Regression model")

			Dim trainingDataView = mlContext.Data.LoadFromTextFile(Of ProductData)(dataPath, hasHeader:= True, separatorChar:=","c)

			Dim trainer = mlContext.Regression.Trainers.FastTreeTweedie(labelColumnName:= "Label", featureColumnName:= "Features")

			Dim trainingPipeline = mlContext.Transforms.Concatenate(outputColumnName:= "NumFeatures", NameOf(ProductData.year), NameOf(ProductData.month), NameOf(ProductData.units), NameOf(ProductData.avg), NameOf(ProductData.count), NameOf(ProductData.max), NameOf(ProductData.min), NameOf(ProductData.prev)).Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName:= "CatFeatures", inputColumnName:= NameOf(ProductData.productId))).Append(mlContext.Transforms.Concatenate(outputColumnName:= "Features", "NumFeatures", "CatFeatures")).Append(mlContext.Transforms.CopyColumns(outputColumnName:= "Label", inputColumnName:= NameOf(ProductData.next))).Append(trainer)

			' Cross-Validate with single dataset (since we don't have two datasets, one for training and for evaluate)
			' in order to evaluate and get the model's accuracy metrics
			Console.WriteLine("=============== Cross-validating to get Regression model's accuracy metrics ===============")
			Dim crossValidationResults = mlContext.Regression.CrossValidate(data:=trainingDataView, estimator:=trainingPipeline, numberOfFolds:= 6, labelColumnName:= "Label")
			ConsoleHelper.PrintRegressionFoldsAverageMetrics(trainer.ToString(), crossValidationResults)

			' Train the model
			Dim model = trainingPipeline.Fit(trainingDataView)

			' Save the model for later comsumption from end-user apps
			mlContext.Model.Save(model, trainingDataView.Schema, outputModelPath)
		End Sub

		''' <summary>
		''' Predict samples using saved model
		''' </summary>
		''' <param name="outputModelPath">Model file path</param>
		Public Shared Sub TestPrediction(mlContext As MLContext, Optional outputModelPath As String = "product_month_fastTreeTweedie.zip")
			ConsoleWriteHeader("Testing Product Unit Sales Forecast Regression model")

			' Read the model that has been previously saved by the method SaveModel

			Dim trainedModel As ITransformer
			Using stream = File.OpenRead(outputModelPath)
				Dim modelInputSchema As Object
				trainedModel = mlContext.Model.Load(stream, modelInputSchema)
			End Using

			Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of ProductData, ProductUnitRegressionPrediction)(trainedModel)

			Console.WriteLine("** Testing Product 1 **")

			' Build sample data
			Dim dataSample As ProductData = New ProductData With {
				.productId = 263,
				.month = 10,
				.year = 2017,
				.avg = 91,
				.max = 370,
				.min = 1,
				.count = 10,
				.prev = 1675,
				.units = 910
			}

			' Predict the nextperiod/month forecast to the one provided
			Dim prediction As ProductUnitRegressionPrediction = predictionEngine.Predict(dataSample)
			Console.WriteLine($"Product: {dataSample.productId}, month: {dataSample.month + 1}, year: {dataSample.year} - Real value (units): 551, Forecast Prediction (units): {prediction.Score}")

			dataSample = New ProductData With {
				.productId = 263,
				.month = 11,
				.year = 2017,
				.avg = 29,
				.max = 221,
				.min = 1,
				.count = 35,
				.prev = 910,
				.units = 551
			}

			' Predicts the nextperiod/month forecast to the one provided
			prediction = predictionEngine.Predict(dataSample)
			Console.WriteLine($"Product: {dataSample.productId}, month: {dataSample.month + 1}, year: {dataSample.year} - Forecast Prediction (units): {prediction.Score}")

			Console.WriteLine(" ")

			Console.WriteLine("** Testing Product 2 **")

			dataSample = New ProductData With {
				.productId = 988,
				.month = 10,
				.year = 2017,
				.avg = 43,
				.max = 220,
				.min = 1,
				.count = 25,
				.prev = 1036,
				.units = 1094
			}

			prediction = predictionEngine.Predict(dataSample)
			Console.WriteLine($"Product: {dataSample.productId}, month: {dataSample.month + 1}, year: {dataSample.year} - Real Value (units): 1076, Forecasting (units): {prediction.Score}")

			dataSample = New ProductData With {
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

			prediction = predictionEngine.Predict(dataSample)
			Console.WriteLine($"Product: {dataSample.productId}, month: {dataSample.month + 1}, year: {dataSample.year} - Forecasting (units): {prediction.Score}")
		End Sub
	End Class
End Namespace
