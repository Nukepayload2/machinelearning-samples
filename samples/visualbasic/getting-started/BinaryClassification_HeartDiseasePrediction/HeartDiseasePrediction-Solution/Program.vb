Imports System
Imports System.IO
Imports HeartDiseasePredictionConsoleApp.DataStructure
Imports Microsoft.ML
Imports Microsoft.ML.Data


Namespace HeartDiseasePredictionConsoleApp
	Public Class Program
		Private Shared BaseDatasetsRelativePath As String = "../../../Data"
		Private Shared TrainDataRelativePath As String = $"{BaseDatasetsRelativePath}/HeartTraining.csv"
		Private Shared TestDataRelativePath As String = $"{BaseDatasetsRelativePath}/HeartTest.csv"

		Private Shared TrainDataPath As String = GetAbsolutePath(TrainDataRelativePath)
		Private Shared TestDataPath As String = GetAbsolutePath(TestDataRelativePath)


		Private Shared BaseModelsRelativePath As String = "../../../../MLModels"
		Private Shared ModelRelativePath As String = $"{BaseModelsRelativePath}/HeartClassification.zip"

		Private Shared ModelPath As String = GetAbsolutePath(ModelRelativePath)

		Public Shared Sub Main(args() As String)
			Dim mlContext = New MLContext()
			BuildTrainEvaluateAndSaveModel(mlContext)

			TestPrediction(mlContext)

			Console.WriteLine("=============== End of process, hit any key to finish ===============")
			Console.ReadKey()
		End Sub

		Private Shared Sub BuildTrainEvaluateAndSaveModel(mlContext As MLContext)

			Dim trainingDataView = mlContext.Data.LoadFromTextFile(Of HeartData)(TrainDataPath, hasHeader:= True, separatorChar:= ";"c)
			Dim testDataView = mlContext.Data.LoadFromTextFile(Of HeartData)(TestDataPath, hasHeader:= True, separatorChar:= ";"c)

			Dim pipeline = mlContext.Transforms.Concatenate("Features", "Age", "Sex", "Cp", "TrestBps", "Chol", "Fbs", "RestEcg", "Thalac", "Exang", "OldPeak", "Slope", "Ca", "Thal").Append(mlContext.BinaryClassification.Trainers.FastTree(labelColumnName:= DefaultColumnNames.Label, featureColumnName:= DefaultColumnNames.Features))

			Console.WriteLine("=============== Training the model ===============")
			Dim trainedModel = pipeline.Fit(trainingDataView)
			Console.WriteLine("")
			Console.WriteLine("")
			Console.WriteLine("=============== Finish the train model. Push Enter ===============")
			Console.WriteLine("")
			Console.WriteLine("")

			Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
			Dim predictions = trainedModel.Transform(testDataView)

			Dim metrics = mlContext.BinaryClassification.Evaluate(data:= predictions, label:= DefaultColumnNames.Label, score:= DefaultColumnNames.Score)
			Console.WriteLine("")
			Console.WriteLine("")
			Console.WriteLine($"************************************************************")
			Console.WriteLine($"*       Metrics for {trainedModel.ToString()} binary classification model      ")
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
			Console.WriteLine("")
			Console.WriteLine("")
			Console.WriteLine("=============== Saving the model to a file ===============")
			Using fs = New FileStream(ModelPath, FileMode.Create, FileAccess.Write, FileShare.Write)
				mlContext.Model.Save(trainedModel, fs)
			End Using
			Console.WriteLine("")
			Console.WriteLine("")
			Console.WriteLine("=============== Model Saved ============= ")
		End Sub


		Private Shared Sub TestPrediction(mlContext As MLContext)
			Dim trainedModel As ITransformer

			Using stream = New FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
				trainedModel = mlContext.Model.Load(stream)
			End Using
			Dim predictionEngine = trainedModel.CreatePredictionEngine(Of HeartData, HeartPrediction)(mlContext)



			For Each heartData In HeartSampleData.heartDatas
				Dim prediction = predictionEngine.Predict(heartData)

				Console.WriteLine($"=============== Single Prediction  ===============")
				Console.WriteLine($"Age: {heartData.Age} ")
				Console.WriteLine($"Sex: {heartData.Sex} ")
				Console.WriteLine($"Cp: {heartData.Cp} ")
				Console.WriteLine($"TrestBps: {heartData.TrestBps} ")
				Console.WriteLine($"Chol: {heartData.Chol} ")
				Console.WriteLine($"Fbs: {heartData.Fbs} ")
				Console.WriteLine($"RestEcg: {heartData.RestEcg} ")
				Console.WriteLine($"Thalac: {heartData.Thalac} ")
				Console.WriteLine($"Exang: {heartData.Exang} ")
				Console.WriteLine($"OldPeak: {heartData.OldPeak} ")
				Console.WriteLine($"Slope: {heartData.Slope} ")
				Console.WriteLine($"Ca: {heartData.Ca} ")
				Console.WriteLine($"Thal: {heartData.Thal} ")
				Console.WriteLine($"Prediction Value: {prediction.Prediction} ")
				Console.WriteLine($"Prediction: {(If(prediction.Prediction, "A disease could be present", "@Not present disease"))} ")
				Console.WriteLine($"Probability: {prediction.Probability} ")
				Console.WriteLine($"==================================================")
				Console.WriteLine("")
				Console.WriteLine("")
			Next heartData

		End Sub


		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath

		End Function
	End Class
End Namespace
