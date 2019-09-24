Imports System
Imports Microsoft.ML.AutoML
Imports Microsoft.ML.Data

Namespace Common
	''' <summary>
	''' Progress handler that AutoML will invoke after each model it produces and evaluates.
	''' </summary>
	Public Class BinaryExperimentProgressHandler
		Implements IProgress(Of RunDetail(Of BinaryClassificationMetrics))

		Private _iterationIndex As Integer

		Public Sub Report(iterationResult As RunDetail(Of BinaryClassificationMetrics))
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (_iterationIndex++ == 0)
			If _iterationIndex = 0 Then
				_iterationIndex += 1
				ConsoleHelper.PrintBinaryClassificationMetricsHeader()
			Else
				_iterationIndex += 1
			End If

			If iterationResult.Exception IsNot Nothing Then
				ConsoleHelper.PrintIterationException(iterationResult.Exception)
			Else
				ConsoleHelper.PrintIterationMetrics(_iterationIndex, iterationResult.TrainerName, iterationResult.ValidationMetrics, iterationResult.RuntimeInSeconds)
			End If
		End Sub
	End Class

	''' <summary>
	''' Progress handler that AutoML will invoke after each model it produces and evaluates.
	''' </summary>
	Public Class MulticlassExperimentProgressHandler
		Implements IProgress(Of RunDetail(Of MulticlassClassificationMetrics))

		Private _iterationIndex As Integer

		Public Sub Report(iterationResult As RunDetail(Of MulticlassClassificationMetrics))
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (_iterationIndex++ == 0)
			If _iterationIndex = 0 Then
				_iterationIndex += 1
				ConsoleHelper.PrintMulticlassClassificationMetricsHeader()
			Else
				_iterationIndex += 1
			End If

			If iterationResult.Exception IsNot Nothing Then
				ConsoleHelper.PrintIterationException(iterationResult.Exception)
			Else
				ConsoleHelper.PrintIterationMetrics(_iterationIndex, iterationResult.TrainerName, iterationResult.ValidationMetrics, iterationResult.RuntimeInSeconds)
			End If
		End Sub
	End Class

	''' <summary>
	''' Progress handler that AutoML will invoke after each model it produces and evaluates.
	''' </summary>
	Public Class RegressionExperimentProgressHandler
		Implements IProgress(Of RunDetail(Of RegressionMetrics))

		Private _iterationIndex As Integer

		Public Sub Report(iterationResult As RunDetail(Of RegressionMetrics))
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (_iterationIndex++ == 0)
			If _iterationIndex = 0 Then
				_iterationIndex += 1
				ConsoleHelper.PrintRegressionMetricsHeader()
			Else
				_iterationIndex += 1
			End If

			If iterationResult.Exception IsNot Nothing Then
				ConsoleHelper.PrintIterationException(iterationResult.Exception)
			Else
				ConsoleHelper.PrintIterationMetrics(_iterationIndex, iterationResult.TrainerName, iterationResult.ValidationMetrics, iterationResult.RuntimeInSeconds)
			End If
		End Sub
	End Class
End Namespace
