Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports PowerAnomalyDetection.DataStructures

Namespace myApp
	Friend Class Program
		Private Shared DatasetsRelativePath As String = "../../../Data"
		Private Shared TrainingDatarelativePath As String = $"{DatasetsRelativePath}/power-export_min.csv"

		Private Shared TrainingDataPath As String = GetAbsolutePath(TrainingDatarelativePath)

		Private Shared BaseModelsRelativePath As String = "../../../MLModels"
		Private Shared ModelRelativePath As String = $"{BaseModelsRelativePath}/PowerAnomalyDetectionModel.zip"

		Private Shared ModelPath As String = GetAbsolutePath(ModelRelativePath)

		Shared Sub Main()
			Dim mlContext = New MLContext(seed:=0)

			' load data
			Dim dataView = mlContext.Data.LoadFromTextFile(Of MeterData)(TrainingDataPath, separatorChar:= ","c, hasHeader:= True)

			' transform options
			BuildTrainModel(mlContext, dataView) ' using SsaSpikeEstimator

			DetectAnomalies(mlContext, dataView)

			Console.WriteLine(vbLf & "Press any key to exit")
			Console.Read()
		End Sub


		Public Shared Sub BuildTrainModel(mlContext As MLContext, dataView As IDataView)
			' Configure the Estimator
			Const PValueSize As Integer = 30
			Const SeasonalitySize As Integer = 30
			Const TrainingSize As Integer = 90
			Const ConfidenceInterval As Integer = 98

			Dim outputColumnName As String = NameOf(SpikePrediction.Prediction)
			Dim inputColumnName As String = NameOf(MeterData.ConsumptionDiffNormalized)

			Dim trainigPipeLine = mlContext.Transforms.DetectSpikeBySsa(outputColumnName, inputColumnName, confidence:= ConfidenceInterval, pvalueHistoryLength:= PValueSize, trainingWindowSize:= TrainingSize, seasonalityWindowSize:= SeasonalitySize)

			Dim trainedModel As ITransformer = trainigPipeLine.Fit(dataView)

			' STEP 6: Save/persist the trained model to a .ZIP file
			mlContext.Model.Save(trainedModel, dataView.Schema, ModelPath)

			Console.WriteLine("The model is saved to {0}", ModelPath)
			Console.WriteLine("")
		End Sub

		Public Shared Sub DetectAnomalies(mlContext As MLContext, dataView As IDataView)
			Dim modelInputSchema As Object
			Dim trainedModel As ITransformer = mlContext.Model.Load(ModelPath, modelInputSchema)

			Dim transformedData = trainedModel.Transform(dataView)

			' Getting the data of the newly created column as an IEnumerable
			Dim predictions As IEnumerable(Of SpikePrediction) = mlContext.Data.CreateEnumerable(Of SpikePrediction)(transformedData, False)

			Dim colCDN = dataView.GetColumn(Of Single)("ConsumptionDiffNormalized").ToArray()
			Dim colTime = dataView.GetColumn(Of DateTime)("time").ToArray()

			' Output the input data and predictions
			Console.WriteLine("======Displaying anomalies in the Power meter data=========")
			Console.WriteLine("Date              " & vbTab & "ReadingDiff" & vbTab & "Alert" & vbTab & "Score" & vbTab & "P-Value")

			Dim i As Integer = 0
			For Each p In predictions
				If p.Prediction(0) = 1 Then
					Console.BackgroundColor = ConsoleColor.DarkYellow
					Console.ForegroundColor = ConsoleColor.Black
				End If
				Console.WriteLine("{0}" & vbTab & "{1:0.0000}" & vbTab & "{2:0.00}" & vbTab & "{3:0.00}" & vbTab & "{4:0.00}", colTime(i), colCDN(i), p.Prediction(0), p.Prediction(1), p.Prediction(2))
				Console.ResetColor()
				i += 1
			Next p
		End Sub

		Public Shared Function GetAbsolutePath(relativeDatasetPath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativeDatasetPath)

			Return fullPath
		End Function
	End Class
End Namespace