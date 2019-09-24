Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports Microsoft.ML.Data
Imports Microsoft.ML
Imports Microsoft.ML.AutoML
Imports System.Text

Namespace Common
	Public Module ConsoleHelper
		Private Const Width As Integer = 114

		Public Sub PrintRegressionMetrics(name As String, metrics As RegressionMetrics)
			Console.WriteLine($"*************************************************")
			Console.WriteLine($"*       Metrics for {name} regression model      ")
			Console.WriteLine($"*------------------------------------------------")
			Console.WriteLine($"*       LossFn:        {metrics.LossFunction:0.##}")
			Console.WriteLine($"*       R2 Score:      {metrics.RSquared:0.##}")
			Console.WriteLine($"*       Absolute loss: {metrics.MeanAbsoluteError:#.##}")
			Console.WriteLine($"*       Squared loss:  {metrics.MeanSquaredError:#.##}")
			Console.WriteLine($"*       RMS loss:      {metrics.RootMeanSquaredError:#.##}")
			Console.WriteLine($"*************************************************")
		End Sub

		Public Sub PrintBinaryClassificationMetrics(name As String, metrics As BinaryClassificationMetrics)
			Console.WriteLine($"************************************************************")
			Console.WriteLine($"*       Metrics for {name} binary classification model      ")
			Console.WriteLine($"*-----------------------------------------------------------")
			Console.WriteLine($"*       Accuracy: {metrics.Accuracy:P2}")
			Console.WriteLine($"*       Area Under Curve:      {metrics.AreaUnderRocCurve:P2}")
			Console.WriteLine($"*       Area under Precision recall Curve:  {metrics.AreaUnderPrecisionRecallCurve:P2}")
			Console.WriteLine($"*       F1Score:  {metrics.F1Score:P2}")
			Console.WriteLine($"*       PositivePrecision:  {metrics.PositivePrecision:#.##}")
			Console.WriteLine($"*       PositiveRecall:  {metrics.PositiveRecall:#.##}")
			Console.WriteLine($"*       NegativePrecision:  {metrics.NegativePrecision:#.##}")
			Console.WriteLine($"*       NegativeRecall:  {metrics.NegativeRecall:P2}")
			Console.WriteLine($"************************************************************")
		End Sub

'INSTANT VB NOTE: The method PrintMulticlassClassificationMetrics was renamed since Visual Basic does not allow same-signature methods with the same name:
		Public Sub PrintMulticlassClassificationMetrics_Renamed(name As String, metrics As MulticlassClassificationMetrics)
			Console.WriteLine($"************************************************************")
			Console.WriteLine($"*    Metrics for {name} multi-class classification model   ")
			Console.WriteLine($"*-----------------------------------------------------------")
			Console.WriteLine($"    MacroAccuracy = {metrics.MacroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better")
			Console.WriteLine($"    MicroAccuracy = {metrics.MicroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better")
			Console.WriteLine($"    LogLoss = {metrics.LogLoss:0.####}, the closer to 0, the better")
			Console.WriteLine($"    LogLoss for class 1 = {metrics.PerClassLogLoss(0):0.####}, the closer to 0, the better")
			Console.WriteLine($"    LogLoss for class 2 = {metrics.PerClassLogLoss(1):0.####}, the closer to 0, the better")
			Console.WriteLine($"    LogLoss for class 3 = {metrics.PerClassLogLoss(2):0.####}, the closer to 0, the better")
			Console.WriteLine($"************************************************************")
		End Sub

		Public Sub ShowDataViewInConsole(mlContext As MLContext, dataView As IDataView, Optional numberOfRows As Integer = 4)
			Dim msg As String = String.Format("Show data in DataView: Showing {0} rows with the columns", numberOfRows.ToString())
			ConsoleWriteHeader(msg)

			Dim preViewTransformedData = dataView.Preview(maxRows:= numberOfRows)

			For Each row In preViewTransformedData.RowView
				Dim ColumnCollection = row.Values
				Dim lineToPrint As String = "Row--> "
				For Each column As KeyValuePair(Of String, Object) In ColumnCollection
					lineToPrint &= $"| {column.Key}:{column.Value}"
				Next column
				Console.WriteLine(lineToPrint & vbLf)
			Next row
		End Sub

		Friend Sub PrintIterationMetrics(iteration As Integer, trainerName As String, metrics As BinaryClassificationMetrics, runtimeInSeconds As Double?)
			CreateRow($"{iteration,-4} {trainerName,-35} {If(metrics?.Accuracy, Double.NaN),9:F4} {If(metrics?.AreaUnderRocCurve, Double.NaN),8:F4} {If(metrics?.AreaUnderPrecisionRecallCurve, Double.NaN),8:F4} {If(metrics?.F1Score, Double.NaN),9:F4} {runtimeInSeconds.Value,9:F1}", Width)
		End Sub

		Friend Sub PrintIterationMetrics(iteration As Integer, trainerName As String, metrics As MulticlassClassificationMetrics, runtimeInSeconds As Double?)
			CreateRow($"{iteration,-4} {trainerName,-35} {If(metrics?.MicroAccuracy, Double.NaN),14:F4} {If(metrics?.MacroAccuracy, Double.NaN),14:F4} {runtimeInSeconds.Value,9:F1}", Width)
		End Sub

		Friend Sub PrintIterationMetrics(iteration As Integer, trainerName As String, metrics As RegressionMetrics, runtimeInSeconds As Double?)
			CreateRow($"{iteration,-4} {trainerName,-35} {If(metrics?.RSquared, Double.NaN),8:F4} {If(metrics?.MeanAbsoluteError, Double.NaN),13:F2} {If(metrics?.MeanSquaredError, Double.NaN),12:F2} {If(metrics?.RootMeanSquaredError, Double.NaN),8:F2} {runtimeInSeconds.Value,9:F1}", Width)
		End Sub

		Friend Sub PrintIterationException(ex As Exception)
			Console.WriteLine($"Exception during AutoML iteration: {ex}")
		End Sub

		Friend Sub PrintBinaryClassificationMetricsHeader()
			CreateRow($"{"",-4} {"Trainer",-35} {"Accuracy",9} {"AUC",8} {"AUPRC",8} {"F1-score",9} {"Duration",9}", Width)
		End Sub

		Friend Sub PrintMulticlassClassificationMetricsHeader()
			CreateRow($"{"",-4} {"Trainer",-35} {"MicroAccuracy",14} {"MacroAccuracy",14} {"Duration",9}", Width)
		End Sub

		Friend Sub PrintRegressionMetricsHeader()
			CreateRow($"{"",-4} {"Trainer",-35} {"RSquared",8} {"Absolute-loss",13} {"Squared-loss",12} {"RMS-loss",8} {"Duration",9}", Width)
		End Sub

'INSTANT VB NOTE: The variable width was renamed since Visual Basic does not handle local variables named the same as class members well:
		Private Sub CreateRow(message As String, width_Renamed As Integer)
			Console.WriteLine("|" & message.PadRight(width_Renamed - 2) & "|")
		End Sub

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

		Public Sub Print(results As ColumnInferenceResults)
			Console.WriteLine("Inferred dataset columns --")
			Call (New ColumnInferencePrinter(results)).Print()
			Console.WriteLine()
		End Sub

		Public Function BuildStringTable(arrValues As IList(Of String())) As String
			Dim maxColumnsWidth() As Integer = GetMaxColumnsWidth(arrValues)
			Dim headerSpliter = New String("-"c, maxColumnsWidth.Sum(Function(i) i + 3) - 1)

			Dim sb = New StringBuilder
			For rowIndex As Integer = 0 To arrValues.Count - 1
				If rowIndex = 0 Then
					sb.AppendFormat("  {0} ", headerSpliter)
					sb.AppendLine()
				End If

				Dim colIndex As Integer = 0
				Do While colIndex < arrValues(0).Length
					' Print cell
					Dim cell As String = arrValues(rowIndex)(colIndex)
					cell = cell.PadRight(maxColumnsWidth(colIndex))
					sb.Append(" | ")
					sb.Append(cell)
					colIndex += 1
				Loop

				' Print end of line
				sb.Append(" | ")
				sb.AppendLine()

				' Print splitter
				If rowIndex = 0 Then
					sb.AppendFormat(" |{0}| ", headerSpliter)
					sb.AppendLine()
				End If

				If rowIndex = arrValues.Count - 1 Then
					sb.AppendFormat("  {0} ", headerSpliter)
				End If
			Next rowIndex

			Return sb.ToString()
		End Function

		Private Function GetMaxColumnsWidth(arrValues As IList(Of String())) As Integer()
			Dim maxColumnsWidth = New Integer((arrValues(0).Length) - 1){}
			Dim colIndex As Integer = 0
			Do While colIndex < arrValues(0).Length
				For rowIndex As Integer = 0 To arrValues.Count - 1
					Dim newLength As Integer = arrValues(rowIndex)(colIndex).Length
					Dim oldLength As Integer = maxColumnsWidth(colIndex)

					If newLength > oldLength Then
						maxColumnsWidth(colIndex) = newLength
					End If
				Next rowIndex
				colIndex += 1
			Loop

			Return maxColumnsWidth
		End Function

		Private Class ColumnInferencePrinter
			Private Shared ReadOnly TableHeaders() As String = { "Name", "Data Type", "Purpose" }

			Private ReadOnly _results As ColumnInferenceResults

			Public Sub New(results As ColumnInferenceResults)
				_results = results
			End Sub

			Public Sub Print()
				Dim tableRows = New List(Of String())

				' Add headers
				tableRows.Add(TableHeaders)

				' Add column data
				Dim info = _results.ColumnInformation
				AppendTableRow(tableRows, info.LabelColumnName, "Label")
				AppendTableRow(tableRows, info.ExampleWeightColumnName, "Weight")
				AppendTableRow(tableRows, info.SamplingKeyColumnName, "Sampling Key")
				AppendTableRows(tableRows, info.CategoricalColumnNames, "Categorical")
				AppendTableRows(tableRows, info.NumericColumnNames, "Numeric")
				AppendTableRows(tableRows, info.TextColumnNames, "Text")
				AppendTableRows(tableRows, info.IgnoredColumnNames, "Ignored")

				Console.WriteLine(ConsoleHelper.BuildStringTable(tableRows))
			End Sub

			Private Sub AppendTableRow(tableRows As ICollection(Of String()), columnName As String, columnPurpose As String)
				If columnName Is Nothing Then
					Return
				End If

				tableRows.Add( { columnName, GetColumnDataType(columnName), columnPurpose })
			End Sub

			Private Sub AppendTableRows(tableRows As ICollection(Of String()), columnNames As IEnumerable(Of String), columnPurpose As String)
				For Each columnName In columnNames
					AppendTableRow(tableRows, columnName, columnPurpose)
				Next columnName
			End Sub

			Private Function GetColumnDataType(columnName As String) As String
				Return _results.TextLoaderOptions.Columns.First(Function(c) c.Name = columnName).DataKind.ToString()
			End Function
		End Class
	End Module
End Namespace
