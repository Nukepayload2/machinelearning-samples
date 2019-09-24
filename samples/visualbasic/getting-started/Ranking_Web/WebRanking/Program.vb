Imports Microsoft.ML
Imports WebRanking.Common
Imports WebRanking.DataStructures
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Net

Namespace WebRanking
	Friend Class Program
		Private Const AssetsPath As String = "../../../Assets"
		Private Const TrainDatasetUrl As String = "https://aka.ms/mlnet-resources/benchmarks/MSLRWeb10KTrain720kRows.tsv"
		Private Const ValidationDatasetUrl As String = "https://aka.ms/mlnet-resources/benchmarks/MSLRWeb10KValidate240kRows.tsv"
		Private Const TestDatasetUrl As String = "https://aka.ms/mlnet-resources/benchmarks/MSLRWeb10KTest240kRows.tsv"

		Private ReadOnly Shared InputPath As String = Path.Combine(AssetsPath, "Input")
		Private ReadOnly Shared OutputPath As String = Path.Combine(AssetsPath, "Output")
		Private ReadOnly Shared TrainDatasetPath As String = Path.Combine(InputPath, "MSLRWeb10KTrain720kRows.tsv")
		Private ReadOnly Shared ValidationDatasetPath As String = Path.Combine(InputPath, "MSLRWeb10KValidate240kRows.tsv")
		Private ReadOnly Shared TestDatasetPath As String = Path.Combine(InputPath, "MSLRWeb10KTest240kRows.tsv")
		Private ReadOnly Shared ModelPath As String = Path.Combine(OutputPath, "RankingModel.zip")

		Shared Sub Main(args() As String)
			' Create a common ML.NET context.
			' Seed set to any number so you have a deterministic environment for repeateable results.
			Dim mlContext As New MLContext(seed:= 0)

			Try
				PrepareData(InputPath, OutputPath, TrainDatasetPath, TrainDatasetUrl, TestDatasetUrl, TestDatasetPath, ValidationDatasetUrl, ValidationDatasetPath)

				' Create the pipeline using the training data's schema; the validation and testing data have the same schema.
				Dim trainData As IDataView = mlContext.Data.LoadFromTextFile(Of SearchResultData)(TrainDatasetPath, separatorChar:= ControlChars.Tab, hasHeader:= True)
				Dim pipeline As IEstimator(Of ITransformer) = CreatePipeline(mlContext, trainData)

				' Train the model on the training dataset. To perform training you need to call the Fit() method.
				Console.WriteLine("===== Train the model on the training dataset =====" & vbLf)
				Dim model As ITransformer = pipeline.Fit(trainData)

				' Evaluate the model using the metrics from the validation dataset; you would then retrain and reevaluate the model until the desired metrics are achieved.
				Console.WriteLine("===== Evaluate the model's result quality with the validation data =====" & vbLf)
				Dim validationData As IDataView = mlContext.Data.LoadFromTextFile(Of SearchResultData)(ValidationDatasetPath, separatorChar:= ControlChars.Tab, hasHeader:= False)
				EvaluateModel(mlContext, model, validationData)

				' Combine the training and validation datasets.
				Dim validationDataEnum = mlContext.Data.CreateEnumerable(Of SearchResultData)(validationData, False)
				Dim trainDataEnum = mlContext.Data.CreateEnumerable(Of SearchResultData)(trainData, False)
				Dim trainValidationDataEnum = validationDataEnum.Concat(trainDataEnum)
				Dim trainValidationData As IDataView = mlContext.Data.LoadFromEnumerable(Of SearchResultData)(trainValidationDataEnum)

				' Train the model on the train + validation dataset.
				Console.WriteLine("===== Train the model on the training + validation dataset =====" & vbLf)
				model = pipeline.Fit(trainValidationData)

				' Evaluate the model using the metrics from the testing dataset; you do this only once and these are your final metrics.
				Console.WriteLine("===== Evaluate the model's result quality with the testing data =====" & vbLf)
				Dim testData As IDataView = mlContext.Data.LoadFromTextFile(Of SearchResultData)(TestDatasetPath, separatorChar:= ControlChars.Tab, hasHeader:= False)
				EvaluateModel(mlContext, model, testData)

				' Combine the training, validation, and testing datasets.
				Dim testDataEnum = mlContext.Data.CreateEnumerable(Of SearchResultData)(testData, False)
				Dim allDataEnum = trainValidationDataEnum.Concat(testDataEnum)
				Dim allData As IDataView = mlContext.Data.LoadFromEnumerable(Of SearchResultData)(allDataEnum)

				' Retrain the model on all of the data, train + validate + test.
				Console.WriteLine("===== Train the model on the training + validation + test dataset =====" & vbLf)
				model = pipeline.Fit(allData)

				' Save and consume the model to perform predictions.
				' Normally, you would use new incoming data; however, for the purposes of this sample, we'll reuse the test data to show how to do predictions.
				ConsumeModel(mlContext, model, ModelPath, testData)
			Catch e As Exception
				Console.WriteLine(e.Message)
			End Try

			Console.Write("Done!")
			Console.ReadLine()
		End Sub

'INSTANT VB NOTE: The variable inputPath was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable outputPath was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable trainDatasetPath was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable trainDatasetUrl was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable testDatasetUrl was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable testDatasetPath was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable validationDatasetUrl was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable validationDatasetPath was renamed since Visual Basic does not handle local variables named the same as class members well:
		Private Shared Sub PrepareData(inputPath_Renamed As String, outputPath_Renamed As String, trainDatasetPath_Renamed As String, trainDatasetUrl_Renamed As String, testDatasetUrl_Renamed As String, testDatasetPath_Renamed As String, validationDatasetUrl_Renamed As String, validationDatasetPath_Renamed As String)
			Console.WriteLine("===== Prepare data =====" & vbLf)

			If Not Directory.Exists(outputPath_Renamed) Then
				Directory.CreateDirectory(outputPath_Renamed)
			End If

			If Not Directory.Exists(inputPath_Renamed) Then
				Directory.CreateDirectory(inputPath_Renamed)
			End If

			If Not File.Exists(trainDatasetPath_Renamed) Then
				Console.WriteLine("===== Download the train dataset - this may take several minutes =====" & vbLf)
				Using client = New WebClient
					client.DownloadFile(trainDatasetUrl_Renamed, Program.TrainDatasetPath)
				End Using
			End If

			If Not File.Exists(validationDatasetPath_Renamed) Then
				Console.WriteLine("===== Download the validation dataset - this may take several minutes =====" & vbLf)
				Using client = New WebClient
					client.DownloadFile(validationDatasetUrl_Renamed, validationDatasetPath_Renamed)
				End Using
			End If

			If Not File.Exists(testDatasetPath_Renamed) Then
				Console.WriteLine("===== Download the test dataset - this may take several minutes =====" & vbLf)
				Using client = New WebClient
					client.DownloadFile(testDatasetUrl_Renamed, testDatasetPath_Renamed)
				End Using
			End If

			Console.WriteLine("===== Download is finished =====" & vbLf)
		End Sub

		Private Shared Function CreatePipeline(mlContext As MLContext, dataView As IDataView) As IEstimator(Of ITransformer)
			Const FeaturesVectorName As String = "Features"

			Console.WriteLine("===== Set up the trainer =====" & vbLf)

			' Specify the columns to include in the feature input data.
			Dim featureCols = dataView.Schema.AsQueryable().Select(Function(s) s.Name).Where(Function(c) c <> NameOf(SearchResultData.Label) AndAlso c <> NameOf(SearchResultData.GroupId)).ToArray()

			' Create an Estimator and transform the data:
			' 1. Concatenate the feature columns into a single Features vector.
			' 2. Create a key type for the label input data by using the value to key transform.
			' 3. Create a key type for the group input data by using a hash transform.
			Dim dataPipeline As IEstimator(Of ITransformer) = mlContext.Transforms.Concatenate(FeaturesVectorName, featureCols).Append(mlContext.Transforms.Conversion.MapValueToKey(NameOf(SearchResultData.Label))).Append(mlContext.Transforms.Conversion.Hash(NameOf(SearchResultData.GroupId), NameOf(SearchResultData.GroupId), numberOfBits:= 20))

			' Set the LightGBM LambdaRank trainer.
			Dim trainer As IEstimator(Of ITransformer) = mlContext.Ranking.Trainers.LightGbm(labelColumnName:= NameOf(SearchResultData.Label), featureColumnName:= FeaturesVectorName, rowGroupColumnName:= NameOf(SearchResultData.GroupId))
			Dim trainerPipeline As IEstimator(Of ITransformer) = dataPipeline.Append(trainer)

			Return trainerPipeline
		End Function

		Private Shared Sub EvaluateModel(mlContext As MLContext, model As ITransformer, data As IDataView)
			' Use the model to perform predictions on the test data.
			Dim predictions As IDataView = model.Transform(data)

			Console.WriteLine("===== Use metrics for the data using NDCG@3 =====" & vbLf)

			' Evaluate the metrics for the data using NDCG; by default, metrics for the up to 3 search results in the query are reported (e.g. NDCG@3).
			ConsoleHelper.EvaluateMetrics(mlContext, predictions)

			Console.WriteLine("===== Use metrics for the data using NDCG@10 =====" & vbLf)

			' Evaluate metrics for up to 10 search results (e.g. NDCG@10).
			ConsoleHelper.EvaluateMetrics(mlContext, predictions, 10)
		End Sub

'INSTANT VB NOTE: The variable modelPath was renamed since Visual Basic does not handle local variables named the same as class members well:
		Private Shared Sub ConsumeModel(mlContext As MLContext, model As ITransformer, modelPath_Renamed As String, data As IDataView)
			Console.WriteLine("===== Save the model =====" & vbLf)

			' Save the model
			mlContext.Model.Save(model, Nothing, modelPath_Renamed)

			Console.WriteLine("===== Consume the model =====" & vbLf)

			' Load the model to perform predictions with it.
			Dim predictionPipelineSchema As DataViewSchema = Nothing
			Dim predictionPipeline As ITransformer = mlContext.Model.Load(modelPath_Renamed, predictionPipelineSchema)

			' Predict rankings.
			Dim predictions As IDataView = predictionPipeline.Transform(data)

			' In the predictions, get the scores of the search results included in the first query (e.g. group).
			Dim searchQueries As IEnumerable(Of SearchResultPrediction) = mlContext.Data.CreateEnumerable(Of SearchResultPrediction)(predictions, reuseRowObject:= False)
			Dim firstGroupId = searchQueries.First().GroupId
			Dim firstGroupPredictions As IEnumerable(Of SearchResultPrediction) = searchQueries.Take(100).Where(Function(p) p.GroupId Is firstGroupId).OrderByDescending(Function(p) p.Score).ToList()

			' The individual scores themselves are NOT a useful measure of result quality; instead, they are only useful as a relative measure to other scores in the group. 
			' The scores are used to determine the ranking where a higher score indicates a higher ranking versus another candidate result.
			ConsoleHelper.PrintScores(firstGroupPredictions)
		End Sub
	End Class
End Namespace
