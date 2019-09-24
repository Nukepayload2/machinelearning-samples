Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports Microsoft.ML.Trainers
Imports System
Imports System.IO

Namespace ProductRecommender
	Friend Class Program
		'1. Do remember to replace amazon0302.txt with dataset from https://snap.stanford.edu/data/amazon0302.html
		'2. Replace column names with ProductID and CoPurchaseProductID. It should look like this:
		'   ProductID	CoPurchaseProductID
		'   0	1
		'   0  2
		Private Shared BaseDataSetRelativePath As String = "../../../Data"
		Private Shared TrainingDataRelativePath As String = $"{BaseDataSetRelativePath}/Amazon0302.txt"
		Private Shared TrainingDataLocation As String = GetAbsolutePath(TrainingDataRelativePath)

		Private Shared BaseModelRelativePath As String = "../../../Model"
		Private Shared ModelRelativePath As String = $"{BaseModelRelativePath}/model.zip"
		Private Shared ModelPath As String = GetAbsolutePath(ModelRelativePath)

		Shared Sub Main(args() As String)
			'STEP 1: Create MLContext to be shared across the model creation workflow objects 
			Dim mlContext As MLContext = New MLContext

			'STEP 2: Read the trained data using TextLoader by defining the schema for reading the product co-purchase dataset
			'        Do remember to replace amazon0302.txt with dataset from https://snap.stanford.edu/data/amazon0302.html
			Dim traindata = mlContext.Data.LoadFromTextFile(path:=TrainingDataLocation, columns:= {
				New TextLoader.Column("Label", DataKind.Single, 0),
				New TextLoader.Column(name:=NameOf(ProductEntry.ProductID), dataKind:=DataKind.UInt32, source:= New TextLoader.Range() { New TextLoader.Range(0) }, keyCount:= New KeyCount(262111)),
				New TextLoader.Column(name:=NameOf(ProductEntry.CoPurchaseProductID), dataKind:=DataKind.UInt32, source:= New TextLoader.Range() { New TextLoader.Range(1) }, keyCount:= New KeyCount(262111))
			}, hasHeader:= True, separatorChar:= ControlChars.Tab)

			'STEP 3: Your data is already encoded so all you need to do is specify options for MatrxiFactorizationTrainer with a few extra hyperparameters
			'        LossFunction, Alpa, Lambda and a few others like K and C as shown below and call the trainer. 
			Dim options As MatrixFactorizationTrainer.Options = New MatrixFactorizationTrainer.Options
			options.MatrixColumnIndexColumnName = NameOf(ProductEntry.ProductID)
			options.MatrixRowIndexColumnName = NameOf(ProductEntry.CoPurchaseProductID)
			options.LabelColumnName= "Label"
			options.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass
			options.Alpha = 0.01
			options.Lambda = 0.025
			' For better results use the following parameters
			'options.K = 100;
			'options.C = 0.00001;

			'Step 4: Call the MatrixFactorization trainer by passing options.
			Dim est = mlContext.Recommendation().Trainers.MatrixFactorization(options)

			'STEP 5: Train the model fitting to the DataSet
			'Please add Amazon0302.txt dataset from https://snap.stanford.edu/data/amazon0302.html to Data folder if FileNotFoundException is thrown.
			Dim model As ITransformer = est.Fit(traindata)

			'STEP 6: Create prediction engine and predict the score for Product 63 being co-purchased with Product 3.
			'        The higher the score the higher the probability for this particular productID being co-purchased 
			Dim predictionengine = mlContext.Model.CreatePredictionEngine(Of ProductEntry, Copurchase_prediction)(model)
			Dim prediction = predictionengine.Predict(New ProductEntry With {
				.ProductID = 3,
				.CoPurchaseProductID = 63
			})

			Console.WriteLine(vbLf & " For ProductID = 3 and  CoPurchaseProductID = 63 the predicted score is " & Math.Round(prediction.Score, 1))
			Console.WriteLine("=============== End of process, hit any key to finish ===============")
			Console.ReadKey()
		End Sub

		Public Shared Function GetAbsolutePath(relativeDatasetPath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativeDatasetPath)

			Return fullPath
		End Function

		Public Class Copurchase_prediction
			Public Property Score As Single
		End Class

		Public Class ProductEntry
			<KeyType(count := 262111)>
			Public Property ProductID As UInteger

			<KeyType(count := 262111)>
			Public Property CoPurchaseProductID As UInteger
		End Class
	End Class
End Namespace
