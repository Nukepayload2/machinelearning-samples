Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.Data.DataView
Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports Microsoft.ML.Transforms.TimeSeries

Namespace myApp
	Friend Class Program
		Private Class MeterData
			<LoadColumn(0)>
			Public Property name As String
			<LoadColumn(1)>
			Public Property time As DateTime
			<LoadColumn(2)>
			Public Property ConsumptionDiffNormalized As Single
		End Class

		Private Class SpikePrediction
			<VectorType(3)>
			Public Property Prediction As Double()
		End Class

		Private Shared DatasetsLocation As String = "../../../Data"
		Private Shared TrainingData As String = $"{DatasetsLocation}/power-export_min.csv"

		Public Shared Function LoadPowerDataMin(ml As MLContext) As IDataView
			Dim dataView = ml.Data.LoadFromTextFile(Of MeterData)(TrainingData, separatorChar:= ","c, hasHeader:= True)

			' take a peek to make sure data is loaded
			'var col = dataView.GetColumn<float>(ml, "ConsumptionDiffNormalized").ToArray(); 

			Return dataView
		End Function

		Shared Sub Main()
			Dim ml = New MLContext

			' load data
			Dim dataView = LoadPowerDataMin(ml)

			' transform options
			BuildTrainEvaluateModel(ml, dataView) ' using SsaSpikeEstimator

			Console.WriteLine(vbLf & "Press any key to exit")
			Console.Read()
		End Sub


		Public Shared Sub BuildTrainEvaluateModel(ml As MLContext, dataView As IDataView)
			' Configure the Estimator
			Const PValueSize As Integer = 30
			Const SeasonalitySize As Integer = 30
			Const TrainingSize As Integer = 90
			Const ConfidenceInterval As Integer = 98

			Dim outputColumnName As String = NameOf(SpikePrediction.Prediction)
			Dim inputColumnName As String = NameOf(MeterData.ConsumptionDiffNormalized)

			Dim estimator = ml.Transforms.SsaSpikeEstimator(outputColumnName, inputColumnName, confidence:= ConfidenceInterval, pvalueHistoryLength:= PValueSize, trainingWindowSize:= TrainingSize, seasonalityWindowSize:= SeasonalitySize)

			Dim model = estimator.Fit(dataView)

			Dim transformedData = model.Transform(dataView)

			' Getting the data of the newly created column as an IEnumerable
			Dim predictionColumn As IEnumerable(Of SpikePrediction) = ml.Data.CreateEnumerable(Of SpikePrediction)(transformedData, False)

			Dim colCDN = dataView.GetColumn(Of Single)(ml, "ConsumptionDiffNormalized").ToArray()
			Dim colTime = dataView.GetColumn(Of DateTime)(ml, "time").ToArray()

			' Output the input data and predictions
			Console.WriteLine($"{outputColumnName} column obtained post-transformation.")
			Console.WriteLine("Date              " & vbTab & "ReadingDiff" & vbTab & "Alert" & vbTab & "Score" & vbTab & "P-Value")

			Dim i As Integer = 0
			For Each p In predictionColumn
				If p.Prediction(0) = 1 Then
					Console.BackgroundColor = ConsoleColor.DarkYellow
					Console.ForegroundColor = ConsoleColor.Black
				End If
				Console.WriteLine("{0}" & vbTab & "{1:0.0000}" & vbTab & "{2:0.00}" & vbTab & "{3:0.00}" & vbTab & "{4:0.00}", colTime(i), colCDN(i), p.Prediction(0), p.Prediction(1), p.Prediction(2))
				Console.ResetColor()
				i += 1
			Next p
		End Sub
	End Class
End Namespace