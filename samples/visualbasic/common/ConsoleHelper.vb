Imports Microsoft.ML.Data
Imports Microsoft.ML
Imports Microsoft.Data.DataView
Imports Microsoft.ML.TrainCatalogBase

Namespace Common
    Public Module ConsoleHelper
        Public Sub PrintPrediction(ByVal prediction As String)
            Console.WriteLine($"*************************************************")
            Console.WriteLine($"Predicted : {prediction}")
            Console.WriteLine($"*************************************************")
        End Sub

        Public Sub PrintRegressionPredictionVersusObserved(ByVal predictionCount As String, ByVal observedCount As String)
            Console.WriteLine($"-------------------------------------------------")
            Console.WriteLine($"Predicted : {predictionCount}")
            Console.WriteLine($"Actual:     {observedCount}")
            Console.WriteLine($"-------------------------------------------------")
        End Sub

        Public Sub PrintRegressionMetrics(ByVal name As String, ByVal metrics As RegressionMetrics)
            Console.WriteLine($"*************************************************")
            Console.WriteLine($"*       Metrics for {name} regression model      ")
            Console.WriteLine($"*------------------------------------------------")
            Console.WriteLine($"*       LossFn:        {metrics.LossFn:0.##}")
            Console.WriteLine($"*       R2 Score:      {metrics.RSquared:0.##}")
            Console.WriteLine($"*       Absolute loss: {metrics.L1:#.##}")
            Console.WriteLine($"*       Squared loss:  {metrics.L2:#.##}")
            Console.WriteLine($"*       RMS loss:      {metrics.Rms:#.##}")
            Console.WriteLine($"*************************************************")
        End Sub

        Public Sub PrintBinaryClassificationMetrics(ByVal name As String, ByVal metrics As CalibratedBinaryClassificationMetrics)
            Console.WriteLine($"************************************************************")
            Console.WriteLine($"*       Metrics for {name} binary classification model      ")
            Console.WriteLine($"*-----------------------------------------------------------")
            Console.WriteLine($"*       Accuracy: {metrics.Accuracy:P2}")
            Console.WriteLine($"*       Auc:      {metrics.Auc:P2}")
            Console.WriteLine($"*       Auprc:  {metrics.Auprc:P2}")
            Console.WriteLine($"*       F1Score:  {metrics.F1Score:P2}")
            Console.WriteLine($"*       LogLoss:  {metrics.LogLoss:#.##}")
            Console.WriteLine($"*       LogLossReduction:  {metrics.LogLossReduction:#.##}")
            Console.WriteLine($"*       PositivePrecision:  {metrics.PositivePrecision:#.##}")
            Console.WriteLine($"*       PositiveRecall:  {metrics.PositiveRecall:#.##}")
            Console.WriteLine($"*       NegativePrecision:  {metrics.NegativePrecision:#.##}")
            Console.WriteLine($"*       NegativeRecall:  {metrics.NegativeRecall:P2}")
            Console.WriteLine($"************************************************************")
        End Sub

        Public Sub PrintMultiClassClassificationMetrics(ByVal name As String, ByVal metrics As MultiClassClassifierMetrics)
            Console.WriteLine($"************************************************************")
            Console.WriteLine($"*    Metrics for {name} multi-class classification model   ")
            Console.WriteLine($"*-----------------------------------------------------------")
            Console.WriteLine($"    AccuracyMacro = {metrics.AccuracyMacro:0.####}, a value between 0 and 1, the closer to 1, the better")
            Console.WriteLine($"    AccuracyMicro = {metrics.AccuracyMicro:0.####}, a value between 0 and 1, the closer to 1, the better")
            Console.WriteLine($"    LogLoss = {metrics.LogLoss:0.####}, the closer to 0, the better")
            Console.WriteLine($"    LogLoss for class 1 = {metrics.PerClassLogLoss(0):0.####}, the closer to 0, the better")
            Console.WriteLine($"    LogLoss for class 2 = {metrics.PerClassLogLoss(1):0.####}, the closer to 0, the better")
            Console.WriteLine($"    LogLoss for class 3 = {metrics.PerClassLogLoss(2):0.####}, the closer to 0, the better")
            Console.WriteLine($"************************************************************")
        End Sub

        Public Sub PrintRegressionFoldsAverageMetrics(ByVal algorithmName As String, ByVal crossValidationResults() As CrossValidationResult(Of RegressionMetrics))
            Dim L1 = crossValidationResults.Select(Function(r) r.Metrics.L1)
            Dim L2 = crossValidationResults.Select(Function(r) r.Metrics.L2)
            Dim RMS = crossValidationResults.Select(Function(r) r.Metrics.L1)
            Dim lossFunction = crossValidationResults.Select(Function(r) r.Metrics.LossFn)
            Dim R2 = crossValidationResults.Select(Function(r) r.Metrics.RSquared)

            Console.WriteLine($"*************************************************************************************************************")
            Console.WriteLine($"*       Metrics for {algorithmName} Regression model      ")
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------")
            Console.WriteLine($"*       Average L1 Loss:    {L1.Average():0.###} ")
            Console.WriteLine($"*       Average L2 Loss:    {L2.Average():0.###}  ")
            Console.WriteLine($"*       Average RMS:          {RMS.Average():0.###}  ")
            Console.WriteLine($"*       Average Loss Function: {lossFunction.Average():0.###}  ")
            Console.WriteLine($"*       Average R-squared: {R2.Average():0.###}  ")
            Console.WriteLine($"*************************************************************************************************************")
        End Sub

        Public Sub PrintMulticlassClassificationFoldsAverageMetrics(ByVal algorithmName As String, ByVal crossValResults() As CrossValidationResult(Of MultiClassClassifierMetrics))
            Dim metricsInMultipleFolds = crossValResults.Select(Function(r) r.Metrics)

            Dim microAccuracyValues = metricsInMultipleFolds.Select(Function(m) m.AccuracyMicro)
            Dim microAccuracyAverage = microAccuracyValues.Average()
            Dim microAccuraciesStdDeviation = CalculateStandardDeviation(microAccuracyValues)
            Dim microAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(microAccuracyValues)

            Dim macroAccuracyValues = metricsInMultipleFolds.Select(Function(m) m.AccuracyMacro)
            Dim macroAccuracyAverage = macroAccuracyValues.Average()
            Dim macroAccuraciesStdDeviation = CalculateStandardDeviation(macroAccuracyValues)
            Dim macroAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(macroAccuracyValues)

            Dim logLossValues = metricsInMultipleFolds.Select(Function(m) m.LogLoss)
            Dim logLossAverage = logLossValues.Average()
            Dim logLossStdDeviation = CalculateStandardDeviation(logLossValues)
            Dim logLossConfidenceInterval95 = CalculateConfidenceInterval95(logLossValues)

            Dim logLossReductionValues = metricsInMultipleFolds.Select(Function(m) m.LogLossReduction)
            Dim logLossReductionAverage = logLossReductionValues.Average()
            Dim logLossReductionStdDeviation = CalculateStandardDeviation(logLossReductionValues)
            Dim logLossReductionConfidenceInterval95 = CalculateConfidenceInterval95(logLossReductionValues)

            Console.WriteLine($"*************************************************************************************************************")
            Console.WriteLine($"*       Metrics for {algorithmName} Multi-class Classification model      ")
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------")
            Console.WriteLine($"*       Average MicroAccuracy:    {microAccuracyAverage:0.###}  - Standard deviation: ({microAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({microAccuraciesConfidenceInterval95:#.###})")
            Console.WriteLine($"*       Average MacroAccuracy:    {macroAccuracyAverage:0.###}  - Standard deviation: ({macroAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({macroAccuraciesConfidenceInterval95:#.###})")
            Console.WriteLine($"*       Average LogLoss:          {logLossAverage:#.###}  - Standard deviation: ({logLossStdDeviation:#.###})  - Confidence Interval 95%: ({logLossConfidenceInterval95:#.###})")
            Console.WriteLine($"*       Average LogLossReduction: {logLossReductionAverage:#.###}  - Standard deviation: ({logLossReductionStdDeviation:#.###})  - Confidence Interval 95%: ({logLossReductionConfidenceInterval95:#.###})")
            Console.WriteLine($"*************************************************************************************************************")

        End Sub

        Public Function CalculateStandardDeviation(ByVal values As IEnumerable(Of Double)) As Double
            Dim average As Double = values.Average()
            Dim sumOfSquaresOfDifferences As Double = values.Select(Function(val) (val - average) * (val - average)).Sum()
            Dim standardDeviation As Double = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1))
            Return standardDeviation
        End Function

        Public Function CalculateConfidenceInterval95(ByVal values As IEnumerable(Of Double)) As Double
            Dim confidenceInterval95 As Double = 1.96 * CalculateStandardDeviation(values) / Math.Sqrt((values.Count() - 1))
            Return confidenceInterval95
        End Function

        Public Sub PrintClusteringMetrics(ByVal name As String, ByVal metrics As ClusteringMetrics)
            Console.WriteLine($"*************************************************")
            Console.WriteLine($"*       Metrics for {name} clustering model      ")
            Console.WriteLine($"*------------------------------------------------")
            Console.WriteLine($"*       AvgMinScore: {metrics.AvgMinScore}")
            Console.WriteLine($"*       DBI is: {metrics.Dbi}")
            Console.WriteLine($"*************************************************")
        End Sub

        Public Sub ShowDataViewInConsole(ByVal mlContext As MLContext, ByVal dataView As IDataView, Optional ByVal numberOfRows As Integer = 4)
            Dim msg As String = String.Format("Show data in DataView: Showing {0} rows with the columns", numberOfRows.ToString())
            ConsoleWriteHeader(msg)

            Dim preViewTransformedData = dataView.Preview(maxRows:=numberOfRows)

            For Each row In preViewTransformedData.RowView
                Dim ColumnCollection = row.Values
                Dim lineToPrint As String = "Row--> "
                For Each column As KeyValuePair(Of String, Object) In ColumnCollection
                    lineToPrint &= $"| {column.Key}:{column.Value}"
                Next column
                Console.WriteLine(lineToPrint & vbLf)
            Next row
        End Sub

        Public Sub PeekDataViewInConsole(ByVal mlContext As MLContext, ByVal dataView As IDataView, ByVal pipeline As IEstimator(Of ITransformer), Optional ByVal numberOfRows As Integer = 4)
            Dim msg As String = String.Format("Peek data in DataView: Showing {0} rows with the columns", numberOfRows.ToString())
            ConsoleWriteHeader(msg)

            'https://github.com/dotnet/machinelearning/blob/master/docs/code/MlNetCookBook.md#how-do-i-look-at-the-intermediate-data
            Dim transformer = pipeline.Fit(dataView)
            Dim transformedData = transformer.Transform(dataView)

            ' 'transformedData' is a 'promise' of data, lazy-loading. call Preview  
            'and iterate through the returned collection from preview.

            Dim preViewTransformedData = transformedData.Preview(maxRows:=numberOfRows)

            For Each row In preViewTransformedData.RowView
                Dim ColumnCollection = row.Values
                Dim lineToPrint As String = "Row--> "
                For Each column As KeyValuePair(Of String, Object) In ColumnCollection
                    lineToPrint &= $"| {column.Key}:{column.Value}"
                Next column
                Console.WriteLine(lineToPrint & vbLf)
            Next row
        End Sub

        Public Function PeekVectorColumnDataInConsole(ByVal mlContext As MLContext, ByVal columnName As String, ByVal dataView As IDataView, ByVal pipeline As IEstimator(Of ITransformer), Optional ByVal numberOfRows As Integer = 4) As List(Of Single())
            Dim msg As String = String.Format("Peek data in DataView: : Show {0} rows with just the '{1}' column", numberOfRows, columnName)
            ConsoleWriteHeader(msg)

            Dim transformer = pipeline.Fit(dataView)
            Dim transformedData = transformer.Transform(dataView)

            ' Extract the 'Features' column.
            Dim someColumnData = transformedData.GetColumn(Of Single())(mlContext, columnName).Take(numberOfRows).ToList()

            ' print to console the peeked rows
            someColumnData.ForEach(Sub(row)
                                       Dim concatColumn As String = String.Empty
                                       For Each f As Single In row
                                           concatColumn += f.ToString()
                                       Next f
                                       Console.WriteLine(concatColumn)
                                   End Sub)

            Return someColumnData
        End Function

        Public Sub ConsoleWriteHeader(ParamArray ByVal lines() As String)
            Dim defaultColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine(" ")
            For Each line In lines
                Console.WriteLine(line)
            Next line
            Dim maxLength = lines.Select(Function(x) x.Length).Max()
            Console.WriteLine(New String("#"c, maxLength))
            Console.ForegroundColor = defaultColor
        End Sub

        Public Sub ConsoleWriterSection(ParamArray ByVal lines() As String)
            Dim defaultColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Blue
            Console.WriteLine(" ")
            For Each line In lines
                Console.WriteLine(line)
            Next line
            Dim maxLength = lines.Select(Function(x) x.Length).Max()
            Console.WriteLine(New String("-"c, maxLength))
            Console.ForegroundColor = defaultColor
        End Sub

        Public Sub ConsolePressAnyKey()
            Dim defaultColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine(" ")
            Console.WriteLine("Press any key to finish.")
            Console.ReadKey()
        End Sub

        Public Sub ConsoleWriteException(ParamArray ByVal lines() As String)
            Dim defaultColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Red
            Const exceptionTitle As String = "EXCEPTION"
            Console.WriteLine(" ")
            Console.WriteLine(exceptionTitle)
            Console.WriteLine(New String("#"c, exceptionTitle.Length))
            Console.ForegroundColor = defaultColor
            For Each line In lines
                Console.WriteLine(line)
            Next line
        End Sub

        Public Sub ConsoleWriteWarning(ParamArray ByVal lines() As String)
            Dim defaultColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.DarkMagenta
            Const warningTitle As String = "WARNING"
            Console.WriteLine(" ")
            Console.WriteLine(warningTitle)
            Console.WriteLine(New String("#"c, warningTitle.Length))
            Console.ForegroundColor = defaultColor
            For Each line In lines
                Console.WriteLine(line)
            Next line
        End Sub
    End Module
End Namespace
