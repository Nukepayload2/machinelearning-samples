Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports AdvancedTaxiFarePrediction.DataStructures
Imports Common
Imports Microsoft.ML
Imports Microsoft.ML.AutoML
Imports Microsoft.ML.Data
Imports PLplot

Namespace AdvancedTaxiFarePrediction
	Friend Module Program
		Private BaseDatasetsRelativePath As String = "Data"

		Private TrainDataRelativePath As String = $"{BaseDatasetsRelativePath}/taxi-fare-train.csv"
		Private TrainDataPath As String = GetAbsolutePath(TrainDataRelativePath)
		Private TrainDataView As IDataView = Nothing

		Private TestDataRelativePath As String = $"{BaseDatasetsRelativePath}/taxi-fare-test.csv"
		Private TestDataPath As String = GetAbsolutePath(TestDataRelativePath)
		Private TestDataView As IDataView = Nothing

		Private BaseModelsRelativePath As String = "../../../MLModels"
		Private ModelRelativePath As String = $"{BaseModelsRelativePath}/TaxiFareModel.zip"
		Private ModelPath As String = GetAbsolutePath(ModelRelativePath)

		Private LabelColumnName As String = "fare_amount"

		Sub Main(args() As String) 'If args[0] == "svg" a vector-based chart will be created instead a .png chart
			Dim mlContext As MLContext = New MLContext

			' Infer columns in the dataset with AutoML
			Dim columnInference = InferColumns(mlContext)

			' Load data from files using inferred columns
			LoadData(mlContext, columnInference)

			' Run an AutoML experiment on the dataset
			Dim experimentResult = RunAutoMLExperiment(mlContext, columnInference)

			' Evaluate the model and print metrics
			EvaluateModel(mlContext, experimentResult.BestRun.Model, experimentResult.BestRun.TrainerName)

			' Save / persist the best model to a.ZIP file
			SaveModel(mlContext, experimentResult.BestRun.Model)

			' Make a single test prediction loading the model from .ZIP file
			TestSinglePrediction(mlContext)

			' Paint regression distribution chart for a number of elements read from a Test DataSet file
			PlotRegressionChart(mlContext, TestDataPath, 100, args)

			' Re-fit best pipeline on train and test data, to produce 
			' a model that is trained on as much data as is available.
			' This is the final model that can be deployed to production.
			Dim refitModel = RefitBestPipeline(mlContext, experimentResult, columnInference)

			' Save the re-fit model to a.ZIP file
			SaveModel(mlContext, refitModel)

			Console.WriteLine("Press any key to exit..")
			Console.ReadLine()
		End Sub

		''' <summary>
		''' Infer columns in the dataset with AutoML.
		''' </summary>
		Private Function InferColumns(mlContext As MLContext) As ColumnInferenceResults
			ConsoleHelper.ConsoleWriteHeader("=============== Inferring columns in dataset ===============")
			Dim columnInference As ColumnInferenceResults = mlContext.Auto().InferColumns(TrainDataPath, LabelColumnName, groupColumns:= False)
			ConsoleHelper.Print(columnInference)
			Return columnInference
		End Function

		''' <summary>
		''' Load data from files using inferred columns.
		''' </summary>
		Private Sub LoadData(mlContext As MLContext, columnInference As ColumnInferenceResults)
			Dim textLoader As TextLoader = mlContext.Data.CreateTextLoader(columnInference.TextLoaderOptions)
			TrainDataView = textLoader.Load(TrainDataPath)
			TestDataView = textLoader.Load(TestDataPath)
		End Sub

		Private Function RunAutoMLExperiment(mlContext As MLContext, columnInference As ColumnInferenceResults) As ExperimentResult(Of RegressionMetrics)
			' STEP 1: Display first few rows of the training data
			ConsoleHelper.ShowDataViewInConsole(mlContext, TrainDataView)

			' STEP 2: Build a pre-featurizer for use in the AutoML experiment.
			' (Internally, AutoML uses one or more train/validation data splits to 
			' evaluate the models it produces. The pre-featurizer is fit only on the 
			' training data split to produce a trained transform. Then, the trained transform 
			' is applied to both the train and validation data splits.)
			Dim preFeaturizer As IEstimator(Of ITransformer) = mlContext.Transforms.Conversion.MapValue("is_cash", { New KeyValuePair(Of String, Boolean)("CSH", True) }, "payment_type")

			' STEP 3: Customize column information returned by InferColumns API
			Dim columnInformation As ColumnInformation = columnInference.ColumnInformation
			columnInformation.CategoricalColumnNames.Remove("payment_type")
			columnInformation.IgnoredColumnNames.Add("payment_type")

			' STEP 4: Initialize a cancellation token source to stop the experiment.
			Dim cts = New CancellationTokenSource

			' STEP 5: Initialize our user-defined progress handler that AutoML will 
			' invoke after each model it produces and evaluates.
			Dim progressHandler = New RegressionExperimentProgressHandler

			' STEP 6: Create experiment settings
			Dim experimentSettings = CreateExperimentSettings(mlContext, cts)

			' STEP 7: Run AutoML regression experiment
			Dim experiment = mlContext.Auto().CreateRegressionExperiment(experimentSettings)
			ConsoleHelper.ConsoleWriteHeader("=============== Running AutoML experiment ===============")
			Console.WriteLine($"Running AutoML regression experiment...")
			Dim stopwatch = System.Diagnostics.Stopwatch.StartNew()
			' Cancel experiment after the user presses any key
			CancelExperimentAfterAnyKeyPress(cts)
			Dim experimentResult As ExperimentResult(Of RegressionMetrics) = experiment.Execute(TrainDataView, columnInformation, preFeaturizer, progressHandler)
			Console.WriteLine($"{experimentResult.RunDetails.Count()} models were returned after {stopwatch.Elapsed.TotalSeconds:0.00} seconds{Environment.NewLine}")

			' Print top models found by AutoML
			PrintTopModels(experimentResult)

			Return experimentResult
		End Function

		''' <summary>
		''' Create AutoML regression experiment settings.
		''' </summary>
		Private Function CreateExperimentSettings(mlContext As MLContext, cts As CancellationTokenSource) As RegressionExperimentSettings
			Dim experimentSettings = New RegressionExperimentSettings
			experimentSettings.MaxExperimentTimeInSeconds = 3600
			experimentSettings.CancellationToken = cts.Token

			' Set the metric that AutoML will try to optimize over the course of the experiment.
			experimentSettings.OptimizingMetric = RegressionMetric.RootMeanSquaredError

			' Set the cache directory to null.
			' This will cause all models produced by AutoML to be kept in memory 
			' instead of written to disk after each run, as AutoML is training.
			' (Please note: for an experiment on a large dataset, opting to keep all 
			' models trained by AutoML in memory could cause your system to run out 
			' of memory.)
			experimentSettings.CacheDirectory = Nothing

			' Don't use LbfgsPoissonRegression and OnlineGradientDescent trainers during this experiment.
			' (These trainers sometimes underperform on this dataset.)
			experimentSettings.Trainers.Remove(RegressionTrainer.LbfgsPoissonRegression)
			experimentSettings.Trainers.Remove(RegressionTrainer.OnlineGradientDescent)

			Return experimentSettings
		End Function

		''' <summary>
		''' Print top models from AutoML experiment.
		''' </summary>
		Private Sub PrintTopModels(experimentResult As ExperimentResult(Of RegressionMetrics))
			' Get top few runs ranked by root mean squared error
			Dim topRuns = experimentResult.RunDetails.Where(Function(r) r.ValidationMetrics IsNot Nothing AndAlso Not Double.IsNaN(r.ValidationMetrics.RootMeanSquaredError)).OrderBy(Function(r) r.ValidationMetrics.RootMeanSquaredError).Take(3)

			Console.WriteLine("Top models ranked by root mean squared error --")
			ConsoleHelper.PrintRegressionMetricsHeader()
			For i = 0 To topRuns.Count() - 1
				Dim run = topRuns.ElementAt(i)
				ConsoleHelper.PrintIterationMetrics(i + 1, run.TrainerName, run.ValidationMetrics, run.RuntimeInSeconds)
			Next i
		End Sub

		''' <summary>
		''' Re-fit best pipeline on all available data.
		''' </summary>
		Private Function RefitBestPipeline(mlContext As MLContext, experimentResult As ExperimentResult(Of RegressionMetrics), columnInference As ColumnInferenceResults) As ITransformer
			ConsoleHelper.ConsoleWriteHeader("=============== Re-fitting best pipeline ===============")
			Dim textLoader = mlContext.Data.CreateTextLoader(columnInference.TextLoaderOptions)
			Dim combinedDataView = textLoader.Load(New MultiFileSource(TrainDataPath, TestDataPath))
			Dim bestRun As RunDetail(Of RegressionMetrics) = experimentResult.BestRun
			Return bestRun.Estimator.Fit(combinedDataView)
		End Function

		''' <summary>
		''' Evaluate the model and print metrics.
		''' </summary>
		Private Sub EvaluateModel(mlContext As MLContext, model As ITransformer, trainerName As String)
			ConsoleHelper.ConsoleWriteHeader("===== Evaluating model's accuracy with test data =====")
			Dim predictions As IDataView = model.Transform(TestDataView)
			Dim metrics = mlContext.Regression.Evaluate(predictions, labelColumnName:= LabelColumnName, scoreColumnName:= "Score")
			ConsoleHelper.PrintRegressionMetrics(trainerName, metrics)
		End Sub

		''' <summary>
		''' Save/persist the best model to a .ZIP file
		''' </summary>
		Private Sub SaveModel(mlContext As MLContext, model As ITransformer)
			ConsoleHelper.ConsoleWriteHeader("=============== Saving the model ===============")
			mlContext.Model.Save(model, TrainDataView.Schema, ModelPath)
			Console.WriteLine("The model is saved to {0}", ModelPath)
		End Sub

		Private Sub CancelExperimentAfterAnyKeyPress(cts As CancellationTokenSource)
			Task.Run(Sub()
				Console.WriteLine($"Press any key to stop the experiment run...")
				Console.ReadKey()
				cts.Cancel()
			End Sub)
		End Sub

		Private Sub TestSinglePrediction(mlContext As MLContext)
			ConsoleHelper.ConsoleWriteHeader("=============== Testing prediction engine ===============")

			' Sample: 
			' vendor_id,rate_code,passenger_count,trip_time_in_secs,trip_distance,payment_type,fare_amount
			' VTS,1,1,1140,3.75,CRD,15.5

			Dim taxiTripSample = New TaxiTrip With {
				.VendorId = "VTS",
				.RateCode = 1,
				.PassengerCount = 1,
				.TripTime = 1140,
				.TripDistance = 3.75F,
				.PaymentType = "CRD",
				.FareAmount = 0
			}

			Dim modelInputSchema As Object
			Dim trainedModel As ITransformer = mlContext.Model.Load(ModelPath, modelInputSchema)

			' Create prediction engine related to the loaded trained model
			Dim predEngine = mlContext.Model.CreatePredictionEngine(Of TaxiTrip, TaxiTripFarePrediction)(trainedModel)

			' Score
			Dim predictedResult = predEngine.Predict(taxiTripSample)

			Console.WriteLine($"**********************************************************************")
			Console.WriteLine($"Predicted fare: {predictedResult.FareAmount:0.####}, actual fare: 15.5")
			Console.WriteLine($"**********************************************************************")
		End Sub

		Private Sub PlotRegressionChart(mlContext As MLContext, testDataSetPath As String, numberOfRecordsToRead As Integer, args() As String)
			Dim trainedModel As ITransformer
			Using stream = New FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
				Dim modelInputSchema As Object
				trainedModel = mlContext.Model.Load(stream, modelInputSchema)
			End Using

			' Create prediction engine related to the loaded trained model
			Dim predFunction = mlContext.Model.CreatePredictionEngine(Of TaxiTrip, TaxiTripFarePrediction)(trainedModel)

			Dim chartFileName As String = ""
			Using pl = New PLStream
				' use SVG backend and write to SineWaves.svg in current directory
				If args.Length = 1 AndAlso args(0) = "svg" Then
					pl.sdev("svg")
					chartFileName = "TaxiRegressionDistribution.svg"
					pl.sfnam(chartFileName)
				Else
					pl.sdev("pngcairo")
					chartFileName = "TaxiRegressionDistribution.png"
					pl.sfnam(chartFileName)
				End If

				' use white background with black foreground
				pl.spal0("cmap0_alternate.pal")

				' Initialize plplot
				pl.init()

				' Set axis limits
				Const xMinLimit As Integer = 0
				Const xMaxLimit As Integer = 35 ' Rides larger than $35 are not shown in the chart
				Const yMinLimit As Integer = 0
				Const yMaxLimit As Integer = 35 ' Rides larger than $35 are not shown in the chart
				pl.env(xMinLimit, xMaxLimit, yMinLimit, yMaxLimit, AxesScale.Independent, AxisBox.BoxTicksLabelsAxes)

				' Set scaling for mail title text 125% size of default
				pl.schr(0, 1.25)

				' The main title
				pl.lab("Measured", "Predicted", "Distribution of Taxi Fare Prediction")

				' plot using different colors
				' see http://plplot.sourceforge.net/examples.php?demo=02 for palette indices
				pl.col0(1)

				Dim totalNumber As Integer = numberOfRecordsToRead
				Dim testData = (New TaxiTripCsvReader).GetDataFromCsv(testDataSetPath, totalNumber).ToList()

				' This code is the symbol to paint
				Dim code As Char = ChrW(9)

				' plot using other color
				'pl.col0(9); //Light Green
				'pl.col0(4); //Red
				pl.col0(2) 'Blue

				Dim yTotal As Double = 0
				Dim xTotal As Double = 0
				Dim xyMultiTotal As Double = 0
				Dim xSquareTotal As Double = 0

				For i As Integer = 0 To testData.Count - 1
					Dim x = New Double(0){}
					Dim y = New Double(0){}

					' Make Prediction
					Dim FarePrediction = predFunction.Predict(testData(i))

					x(0) = testData(i).FareAmount
					y(0) = FarePrediction.FareAmount

					' Paint a dot
					pl.poin(x, y, code)

					xTotal += x(0)
					yTotal += y(0)

					Dim multi As Double = x(0) * y(0)
					xyMultiTotal += multi

					Dim xSquare As Double = x(0) * x(0)
					xSquareTotal += xSquare

					Dim ySquare As Double = y(0) * y(0)

					Console.WriteLine($"-------------------------------------------------")
					Console.WriteLine($"Predicted : {FarePrediction.FareAmount}")
					Console.WriteLine($"Actual:    {testData(i).FareAmount}")
					Console.WriteLine($"-------------------------------------------------")
				Next i

				' Regression Line calculation explanation:
				' https://www.khanacademy.org/math/statistics-probability/describing-relationships-quantitative-data/more-on-regression/v/regression-line-example

				Dim minY As Double = yTotal / totalNumber
				Dim minX As Double = xTotal / totalNumber
				Dim minXY As Double = xyMultiTotal / totalNumber
				Dim minXsquare As Double = xSquareTotal / totalNumber

				Dim m As Double = ((minX * minY) - minXY) / ((minX * minX) - minXsquare)

				Dim b As Double = minY - (m * minX)

				' Generic function for Y for the regression line
				' y = (m * x) + b;

				Dim x1 As Double = 1

				' Function for Y1 in the line
				Dim y1 As Double = (m * x1) + b

				Dim x2 As Double = 39

				' Function for Y2 in the line
				Dim y2 As Double = (m * x2) + b

				Dim xArray = New Double(1){}
				Dim yArray = New Double(1){}
				xArray(0) = x1
				yArray(0) = y1
				xArray(1) = x2
				yArray(1) = y2

				pl.col0(4)
				pl.line(xArray, yArray)

				' End page (writes output to disk)
				pl.eop()

				' Output version of PLplot
				Dim verText As Object
				pl.gver(verText)
				Console.WriteLine("PLplot version " & verText)

			End Using ' The pl object is disposed here

			' Open chart file in Microsoft Photos App (or default app for .svg or .png, like browser)

			Console.WriteLine("Showing chart...")
			Dim p = New Process
			Dim chartFileNamePath As String = ".\" & chartFileName
			p.StartInfo = New ProcessStartInfo(chartFileNamePath) With {.UseShellExecute = True}
			p.Start()
		End Sub

		Public Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function
	End Module

	Public Class TaxiTripCsvReader
		Public Function GetDataFromCsv(dataLocation As String, numMaxRecords As Integer) As IEnumerable(Of TaxiTrip)
			Dim records As IEnumerable(Of TaxiTrip) = File.ReadAllLines(dataLocation).Skip(1).Select(Function(x) x.Split(","c)).Select(Function(x) New TaxiTrip With {
				.VendorId = x(0),
				.RateCode = Single.Parse(x(1)),
				.PassengerCount = Single.Parse(x(2)),
				.TripTime = Single.Parse(x(3)),
				.TripDistance = Single.Parse(x(4)),
				.PaymentType = x(5),
				.FareAmount = Single.Parse(x(6))
			}).Take(numMaxRecords)

			Return records
		End Function
	End Class

End Namespace
