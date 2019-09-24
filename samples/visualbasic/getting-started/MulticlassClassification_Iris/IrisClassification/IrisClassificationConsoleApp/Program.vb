Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports MulticlassClassification_Iris.DataStructures

Namespace MulticlassClassification_Iris
	Public Module Program
		Private ReadOnly Property AppPath As String
			Get
				Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
			End Get
		End Property

		Private BaseDatasetsRelativePath As String = "../../../../Data"
		Private TrainDataRelativePath As String = $"{BaseDatasetsRelativePath}/iris-train.txt"
		Private TestDataRelativePath As String = $"{BaseDatasetsRelativePath}/iris-test.txt"

		Private TrainDataPath As String = GetAbsolutePath(TrainDataRelativePath)
		Private TestDataPath As String = GetAbsolutePath(TestDataRelativePath)

		Private BaseModelsRelativePath As String = "../../../../MLModels"
		Private ModelRelativePath As String = $"{BaseModelsRelativePath}/IrisClassificationModel.zip"

		Private ModelPath As String = GetAbsolutePath(ModelRelativePath)

		Public Sub Main(args() As String)
			' Create MLContext to be shared across the model creation workflow objects 
			' Set a random seed for repeatable/deterministic results across multiple trainings.
			Dim mlContext = New MLContext(seed:= 0)

			'1.
			BuildTrainEvaluateAndSaveModel(mlContext)

			'2.
			TestSomePredictions(mlContext)

			Console.WriteLine("=============== End of process, hit any key to finish ===============")
			Console.ReadKey()
		End Sub

		Private Sub BuildTrainEvaluateAndSaveModel(mlContext As MLContext)
			' STEP 1: Common data loading configuration
			Dim trainingDataView = mlContext.Data.LoadFromTextFile(Of IrisData)(TrainDataPath, hasHeader:= True)
			Dim testDataView = mlContext.Data.LoadFromTextFile(Of IrisData)(TestDataPath, hasHeader:= True)


			' STEP 2: Common data process configuration with pipeline data transformations
			Dim dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:= "KeyColumn", inputColumnName:= NameOf(IrisData.Label)).Append(mlContext.Transforms.Concatenate("Features", NameOf(IrisData.SepalLength), NameOf(IrisData.SepalWidth), NameOf(IrisData.PetalLength), NameOf(IrisData.PetalWidth)).AppendCacheCheckpoint(mlContext))
																	   ' Use in-memory cache for small/medium datasets to lower training time. 
																	   ' Do NOT use it (remove .AppendCacheCheckpoint()) when handling very large datasets. 

			' STEP 3: Set the training algorithm, then append the trainer to the pipeline  
			Dim trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName:= "KeyColumn", featureColumnName:= "Features").Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:= NameOf(IrisData.Label), inputColumnName:= "KeyColumn"))

			Dim trainingPipeline = dataProcessPipeline.Append(trainer)

			' STEP 4: Train the model fitting to the DataSet
			Console.WriteLine("=============== Training the model ===============")
			Dim trainedModel As ITransformer = trainingPipeline.Fit(trainingDataView)

			' STEP 5: Evaluate the model and show accuracy stats
			Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
			Dim predictions = trainedModel.Transform(testDataView)
			Dim metrics = mlContext.MulticlassClassification.Evaluate(predictions, "Label", "Score")

			Common.ConsoleHelper.PrintMultiClassClassificationMetrics_Renamed(trainer.ToString(), metrics)

			' STEP 6: Save/persist the trained model to a .ZIP file
			mlContext.Model.Save(trainedModel, trainingDataView.Schema, ModelPath)
			Console.WriteLine("The model is saved to {0}", ModelPath)
		End Sub

		Private Sub TestSomePredictions(mlContext As MLContext)
			'Test Classification Predictions with some hard-coded samples 
			Dim modelInputSchema As Object
			Dim trainedModel As ITransformer = mlContext.Model.Load(ModelPath, modelInputSchema)

			' Create prediction engine related to the loaded trained model
			Dim predEngine = mlContext.Model.CreatePredictionEngine(Of IrisData, IrisPrediction)(trainedModel)

			' During prediction we will get Score column with 3 float values.
			' We need to find way to map each score to original label.
			' In order to do that we need to get TrainingLabelValues from Score column.
			' TrainingLabelValues on top of Score column represent original labels for i-th value in Score array.
			' Let's look how we can convert key value for PredictedLabel to original labels.
			' We need to read KeyValues for "PredictedLabel" column.
			Dim keys As VBuffer(Of Single) = Nothing
			predEngine.OutputSchema("PredictedLabel").GetKeyValues(keys)
			Dim labelsArray = keys.DenseValues().ToArray()

			' Since we apply MapValueToKey estimator with default parameters, key values
			' depends on order of occurence in data file. Which is "Iris-setosa", "Iris-versicolor", "Iris-virginica"
			' So if we have Score column equal to [0.2, 0.3, 0.5] that's mean what score for
			' Iris-setosa is 0.2
			' Iris-versicolor is 0.3
			' Iris-virginica is 0.5.
			'Add a dictionary to map the above float values to strings. 
			Dim IrisFlowers As New Dictionary(Of Single, String)
			IrisFlowers.Add(0, "Setosa")
			IrisFlowers.Add(1, "versicolor")
			IrisFlowers.Add(2, "virginica")

			Console.WriteLine("=====Predicting using model====")
			'Score sample 1
			Dim resultprediction1 = predEngine.Predict(SampleIrisData.Iris1)

			Console.WriteLine($"Actual: setosa.     Predicted label and score:  {IrisFlowers(labelsArray(0))}: {resultprediction1.Score(0):0.####}")
			Console.WriteLine($"                                                {IrisFlowers(labelsArray(1))}: {resultprediction1.Score(1):0.####}")
			Console.WriteLine($"                                                {IrisFlowers(labelsArray(2))}: {resultprediction1.Score(2):0.####}")
			Console.WriteLine()

			'Score sample 2
			Dim resultprediction2 = predEngine.Predict(SampleIrisData.Iris2)

			Console.WriteLine($"Actual: Virginica.   Predicted label and score:  {IrisFlowers(labelsArray(0))}: {resultprediction2.Score(0):0.####}")
			Console.WriteLine($"                                                 {IrisFlowers(labelsArray(1))}: {resultprediction2.Score(1):0.####}")
			Console.WriteLine($"                                                 {IrisFlowers(labelsArray(2))}: {resultprediction2.Score(2):0.####}")
			Console.WriteLine()

			'Score sample 3
			Dim resultprediction3 = predEngine.Predict(SampleIrisData.Iris3)

			Console.WriteLine($"Actual: Versicolor.   Predicted label and score: {IrisFlowers(labelsArray(0))}: {resultprediction3.Score(0):0.####}")
			Console.WriteLine($"                                                 {IrisFlowers(labelsArray(1))}: {resultprediction3.Score(1):0.####}")
			Console.WriteLine($"                                                 {IrisFlowers(labelsArray(2))}: {resultprediction3.Score(2):0.####}")
			Console.WriteLine()
		End Sub

		Public Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function
	End Module
End Namespace
