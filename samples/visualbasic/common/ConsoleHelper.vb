Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Data
Imports Microsoft.ML

Imports System.Reflection

Namespace Common
    Public Module ConsoleHelper
        Public Sub PrintPrediction(prediction As String)
            Console.WriteLine($"*************************************************")
            Console.WriteLine($"Predicted : {prediction}")
            Console.WriteLine($"*************************************************")
        End Sub

        Public Sub PrintRegressionPredictionVersusObserved(predictionCount As String, observedCount As String)
            Console.WriteLine($"-------------------------------------------------")
            Console.WriteLine($"Predicted : {predictionCount}")
            Console.WriteLine($"Actual:     {observedCount}")
            Console.WriteLine($"-------------------------------------------------")
        End Sub


        Public Sub PrintRegressionMetrics(name As String, metrics As RegressionMetrics)
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

        Public Sub PrintBinaryClassificationMetrics(name As String, metrics As CalibratedBinaryClassificationMetrics)
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

        Public Sub PrintMultiClassClassificationMetrics(name As String, metrics As MultiClassClassifierMetrics)
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

        Public Sub PrintRegressionFoldsAverageMetrics(algorithmName As String, crossValidationResults() As _
                (metrics As RegressionMetrics, model As ITransformer, scoredTestData As IDataView))
            Dim L1 = From r In crossValidationResults Select l = r.metrics.L1
            Dim L2 = From r In crossValidationResults Select l = r.metrics.L2
            Dim RMS = From r In crossValidationResults Select l = r.metrics.L1
            Dim lossFunction = From r In crossValidationResults Select r.metrics.LossFn
            Dim R2 = From r In crossValidationResults Select r.metrics.RSquared

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

        Public Sub PrintMulticlassClassificationFoldsAverageMetrics(algorithmName As String, crossValResults() As _
               (metrics As MultiClassClassifierMetrics, model As ITransformer, scoredTestData As IDataView))
            Dim metricsInMultipleFolds = crossValResults.Select(Function(r) r.metrics)

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

        Public Function CalculateStandardDeviation(values As IEnumerable(Of Double)) As Double
            Dim average As Double = values.Average()
            Dim sumOfSquaresOfDifferences As Double = values.Select(Function(val) (val - average) * (val - average)).Sum()
            Dim standardDeviation As Double = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1))
            Return standardDeviation
        End Function

        Public Function CalculateConfidenceInterval95(values As IEnumerable(Of Double)) As Double
            Dim confidenceInterval95 As Double = 1.96 * CalculateStandardDeviation(values) / Math.Sqrt((values.Count() - 1))
            Return confidenceInterval95
        End Function

        Public Sub PrintClusteringMetrics(name As String, metrics As ClusteringMetrics)
            Console.WriteLine($"*************************************************")
            Console.WriteLine($"*       Metrics for {name} clustering model      ")
            Console.WriteLine($"*------------------------------------------------")
            Console.WriteLine($"*       AvgMinScore: {metrics.AvgMinScore}")
            Console.WriteLine($"*       DBI is: {metrics.Dbi}")
            Console.WriteLine($"*************************************************")
        End Sub

        Public Function PeekDataViewInConsole(Of TObservation As {Class, New})(mlContext As MLContext, dataView As IDataView, pipeline As IEstimator(Of ITransformer), Optional numberOfRows As Integer = 4) As List(Of TObservation)
            Dim msg As String = String.Format("Peek data in DataView: Showing {0} rows with the columns specified by TObservation class", numberOfRows.ToString())
            ConsoleWriteHeader(msg)

            'https://github.com/dotnet/machinelearning/blob/master/docs/code/MlNetCookBook.md#how-do-i-look-at-the-intermediate-data
            Dim transformer = pipeline.Fit(dataView)
            Dim transformedData = transformer.Transform(dataView)

            ' 'transformedData' is a 'promise' of data, lazy-loading. Let's actually read it.
            ' Convert to an enumerable of user-defined type.
            Dim someRows = transformedData.AsEnumerable(Of TObservation)(mlContext, reuseRowObject:=False).Take(numberOfRows).ToList()

            someRows.ForEach(Sub(row)
                                 Dim lineToPrint As String = "Row--> "

                                 Dim fieldsInRow = row.GetType().GetFields(BindingFlags.Instance Or BindingFlags.Static Or BindingFlags.NonPublic Or BindingFlags.Public)
                                 For Each field As FieldInfo In fieldsInRow
                                     lineToPrint += $"| {field.Name}: {field.GetValue(row)}"
                                 Next field
                                 Console.WriteLine(lineToPrint)
                             End Sub)

            Return someRows
        End Function

        Public Function PeekVectorColumnDataInConsole(mlContext As MLContext, columnName As String, dataView As IDataView, pipeline As IEstimator(Of ITransformer), Optional numberOfRows As Integer = 4) As List(Of Single())
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

        Public Sub ConsoleWriteHeader(ParamArray lines() As String)
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

        Public Sub ConsoleWriterSection(ParamArray lines() As String)
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

        Public Sub ConsoleWriteException(ParamArray lines() As String)
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

        Public Sub ConsoleWriteWarning(ParamArray lines() As String)
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
