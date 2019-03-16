Imports System.IO
Imports PLplot
Imports Microsoft.ML
Imports Regression_TaxiFarePrediction.DataStructures
Imports Common
Imports Microsoft.ML.Data
Imports Microsoft.Data.DataView
Imports Microsoft.ML.Transforms.NormalizingEstimator

Namespace Regression_TaxiFarePrediction
    Friend Module Program
        Private ReadOnly Property AppPath As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private BaseDatasetsRelativePath As String = "../../../../Data"
        Private TrainDataRelativePath As String = $"{BaseDatasetsRelativePath}/taxi-fare-train.csv"
        Private TestDataRelativePath As String = $"{BaseDatasetsRelativePath}/taxi-fare-test.csv"

        Private TrainDataPath As String = GetAbsolutePath(TrainDataRelativePath)
        Private TestDataPath As String = GetAbsolutePath(TestDataRelativePath)

        Private BaseModelsRelativePath As String = "../../../../MLModels"
        Private ModelRelativePath As String = $"{BaseModelsRelativePath}/TaxiFareModel.zip"

        Private ModelPath As String = GetAbsolutePath(ModelRelativePath)

        Sub Main(args() As String) 'If args[0] == "svg" a vector-based chart will be created instead a .png chart
            'Create ML Context with seed for repeteable/deterministic results
            Dim mlContext As New MLContext(seed:=0)

            ' Create, Train, Evaluate and Save a model
            BuildTrainEvaluateAndSaveModel(mlContext)

            ' Make a single test prediction loding the model from .ZIP file
            TestSinglePrediction(mlContext)

            ' Paint regression distribution chart for a number of elements read from a Test DataSet file
            PlotRegressionChart(mlContext, TestDataPath, 100, args)

            Console.WriteLine("Press any key to exit..")
            Console.ReadLine()
        End Sub

        Private Function BuildTrainEvaluateAndSaveModel(mlContext As MLContext) As ITransformer
            ' STEP 1: Common data loading configuration
            Dim baseTrainingDataView As IDataView = mlContext.Data.LoadFromTextFile(Of TaxiTrip)(TrainDataPath, hasHeader:=True, separatorChar:=","c)
            Dim testDataView As IDataView = mlContext.Data.LoadFromTextFile(Of TaxiTrip)(TestDataPath, hasHeader:=True, separatorChar:=","c)

            'Sample code of removing extreme data like "outliers" for FareAmounts higher than $150 and lower than $1 which can be error-data 
            Dim cnt = baseTrainingDataView.GetColumn(Of Single)(mlContext, NameOf(TaxiTrip.FareAmount)).Count()
            Dim trainingDataView As IDataView = mlContext.Data.FilterRowsByColumn(baseTrainingDataView, NameOf(TaxiTrip.FareAmount), lowerBound:=1, upperBound:=150)
            Dim cnt2 = trainingDataView.GetColumn(Of Single)(mlContext, NameOf(TaxiTrip.FareAmount)).Count()

            ' STEP 2: Common data process configuration with pipeline data transformations
            Dim dataProcessPipeline = mlContext.Transforms.CopyColumns(outputColumnName:=DefaultColumnNames.Label, inputColumnName:=NameOf(TaxiTrip.FareAmount)).Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName:="VendorIdEncoded", inputColumnName:=NameOf(TaxiTrip.VendorId))).Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName:="RateCodeEncoded", inputColumnName:=NameOf(TaxiTrip.RateCode))).Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName:="PaymentTypeEncoded", inputColumnName:=NameOf(TaxiTrip.PaymentType))).Append(mlContext.Transforms.Normalize(outputColumnName:=NameOf(TaxiTrip.PassengerCount), mode:=NormalizerMode.MeanVariance)).Append(mlContext.Transforms.Normalize(outputColumnName:=NameOf(TaxiTrip.TripTime), mode:=NormalizerMode.MeanVariance)).Append(mlContext.Transforms.Normalize(outputColumnName:=NameOf(TaxiTrip.TripDistance), mode:=NormalizerMode.MeanVariance)).Append(mlContext.Transforms.Concatenate(DefaultColumnNames.Features, "VendorIdEncoded", "RateCodeEncoded", "PaymentTypeEncoded", NameOf(TaxiTrip.PassengerCount), NameOf(TaxiTrip.TripTime), NameOf(TaxiTrip.TripDistance)))

            ' (OPTIONAL) Peek data (such as 5 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            ConsoleHelper.PeekDataViewInConsole(mlContext, trainingDataView, dataProcessPipeline, 5)
            ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, DefaultColumnNames.Features, trainingDataView, dataProcessPipeline, 5)

            ' STEP 3: Set the training algorithm, then create and config the modelBuilder - Selected Trainer (SDCA Regression algorithm)                            
            Dim trainer = mlContext.Regression.Trainers.StochasticDualCoordinateAscent(labelColumnName:=DefaultColumnNames.Label, featureColumnName:=DefaultColumnNames.Features)
            Dim trainingPipeline = dataProcessPipeline.Append(trainer)

            ' STEP 4: Train the model fitting to the DataSet
            'The pipeline is trained on the dataset that has been loaded and transformed.
            Console.WriteLine("=============== Training the model ===============")
            Dim trainedModel = trainingPipeline.Fit(trainingDataView)

            ' STEP 5: Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")

            Dim predictions As IDataView = trainedModel.Transform(testDataView)
            Dim metrics = mlContext.Regression.Evaluate(predictions, label:=DefaultColumnNames.Label, score:=DefaultColumnNames.Score)

            Common.ConsoleHelper.PrintRegressionMetrics(trainer.ToString(), metrics)

            ' STEP 6: Save/persist the trained model to a .ZIP file

            Using fs = File.Create(ModelPath)
                trainedModel.SaveTo(mlContext, fs)
            End Using

            Console.WriteLine("The model is saved to {0}", ModelPath)

            Return trainedModel
        End Function

        Private Sub TestSinglePrediction(mlContext As MLContext)
            'Sample: 
            'vendor_id,rate_code,passenger_count,trip_time_in_secs,trip_distance,payment_type,fare_amount
            'VTS,1,1,1140,3.75,CRD,15.5

            Dim taxiTripSample = New TaxiTrip() With {
                .VendorId = "VTS",
                .RateCode = "1",
                .PassengerCount = 1,
                .TripTime = 1140,
                .TripDistance = 3.75F,
                .PaymentType = "CRD",
                .FareAmount = 0
            }

            '''
            Dim trainedModel As ITransformer
            Using stream = New FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                trainedModel = mlContext.Model.Load(stream)
            End Using

            ' Create prediction engine related to the loaded trained model
            Dim predEngine = trainedModel.CreatePredictionEngine(Of TaxiTrip, TaxiTripFarePrediction)(mlContext)

            'Score
            Dim resultprediction = predEngine.Predict(taxiTripSample)
            '''

            Console.WriteLine($"**********************************************************************")
            Console.WriteLine($"Predicted fare: {resultprediction.FareAmount:0.####}, actual fare: 15.5")
            Console.WriteLine($"**********************************************************************")
        End Sub

        Private Sub PlotRegressionChart(mlContext As MLContext, testDataSetPath As String, numberOfRecordsToRead As Integer, args() As String)
            Dim trainedModel As ITransformer
            Using stream = New FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                trainedModel = mlContext.Model.Load(stream)
            End Using

            ' Create prediction engine related to the loaded trained model
            Dim predFunction = trainedModel.CreatePredictionEngine(Of TaxiTrip, TaxiTripFarePrediction)(mlContext)

            Dim chartFileName As String = ""
            Using pl = New PLStream()
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

                ' set axis limits
                Const xMinLimit As Integer = 0
                Const xMaxLimit As Integer = 35 'Rides larger than $35 are not shown in the chart
                Const yMinLimit As Integer = 0
                Const yMaxLimit As Integer = 35 'Rides larger than $35 are not shown in the chart
                pl.env(xMinLimit, xMaxLimit, yMinLimit, yMaxLimit, AxesScale.Independent, AxisBox.BoxTicksLabelsAxes)

                ' Set scaling for mail title text 125% size of default
                pl.schr(0, 1.25)

                ' The main title
                pl.lab("Measured", "Predicted", "Distribution of Taxi Fare Prediction")

                ' plot using different colors
                ' see http://plplot.sourceforge.net/examples.php?demo=02 for palette indices
                pl.col0(1)

                Dim totalNumber As Integer = numberOfRecordsToRead
                Dim testData = (New TaxiTripCsvReader()).GetDataFromCsv(testDataSetPath, totalNumber).ToList()

                'This code is the symbol to paint
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
                    Dim x = New Double(0) {}
                    Dim y = New Double(0) {}

                    'Make Prediction
                    Dim FarePrediction = predFunction.Predict(testData(i))

                    x(0) = testData(i).FareAmount
                    y(0) = FarePrediction.FareAmount

                    'Paint a dot
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

                'Generic function for Y for the regression line
                ' y = (m * x) + b;

                Dim x1 As Double = 1
                'Function for Y1 in the line
                Dim y1 As Double = (m * x1) + b

                Dim x2 As Double = 39
                'Function for Y2 in the line
                Dim y2 As Double = (m * x2) + b

                Dim xArray = New Double(1) {}
                Dim yArray = New Double(1) {}
                xArray(0) = x1
                yArray(0) = y1
                xArray(1) = x2
                yArray(1) = y2

                pl.col0(4)
                pl.line(xArray, yArray)

                ' end page (writes output to disk)
                pl.eop()

                ' output version of PLplot
                Dim verText As Object
                pl.gver(verText)
                Console.WriteLine("PLplot version " & verText)

            End Using ' the pl object is disposed here

            ' Open Chart File In Microsoft Photos App (Or default app, like browser for .svg)

            Console.WriteLine("Showing chart...")
            Dim p = New Process()
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
            Dim records As IEnumerable(Of TaxiTrip) = File.ReadAllLines(dataLocation).Skip(1).Select(Function(x) x.Split(","c)).Select(Function(x) New TaxiTrip() With {
                .VendorId = x(0),
                .RateCode = x(1),
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
