Imports System
Imports Microsoft.ML
Imports System.IO
Imports SpikeDetection.DataStructures

Namespace SpikeDetection
	Friend Module Program
		Private BaseDatasetsRelativePath As String = "../../../../Data"
		Private DatasetRelativePath As String = $"{BaseDatasetsRelativePath}/product-sales.csv"

		Private DatasetPath As String = GetAbsolutePath(DatasetRelativePath)

		Private BaseModelsRelativePath As String = "../../../../MLModels"
		Private ModelRelativePath As String = $"{BaseModelsRelativePath}/ProductSalesModel.zip"

		Private ModelPath As String = GetAbsolutePath(ModelRelativePath)

		Sub Main()
			' Create MLContext to be shared across the model creation workflow objects 
			Dim mlcontext As MLContext = New MLContext

			'assign the Number of records in dataset file to cosntant variable
			Const size As Integer = 36

			'STEP 1: Common data loading configuration
			Dim dataView As IDataView = mlcontext.Data.LoadFromTextFile(Of ProductSalesData)(path:= DatasetPath, hasHeader:= True, separatorChar:= ","c)

			'To detech temporay changes in the pattern
			DetectSpike(mlcontext,size,dataView)

			'To detect persistent change in the pattern
			DetectChangepoint(mlcontext, size, dataView)

			Console.WriteLine("=============== End of process, hit any key to finish ===============")

			Console.ReadLine()
		End Sub

		Private Sub DetectSpike(mlcontext As MLContext, size As Integer, dataView As IDataView)
		   Console.WriteLine("Detect temporary changes in pattern")

			'STEP 2: Set the training algorithm    
			Dim trainingPipeLine = mlcontext.Transforms.DetectIidSpike(outputColumnName:= NameOf(ProductSalesPrediction.Prediction), inputColumnName:= NameOf(ProductSalesData.numSales),confidence:= 95, pvalueHistoryLength:= size \ 4)

			'STEP 3:Train the model by fitting the dataview
			Console.WriteLine("=============== Training the model using Spike Detection algorithm ===============")
			Dim trainedModel As ITransformer = trainingPipeLine.Fit(dataView)
			Console.WriteLine("=============== End of training process ===============")

			'Apply data transformation to create predictions.
			Dim transformedData As IDataView = trainedModel.Transform(dataView)
			Dim predictions = mlcontext.Data.CreateEnumerable(Of ProductSalesPrediction)(transformedData, reuseRowObject:= False)

			Console.WriteLine("Alert" & vbTab & "Score" & vbTab & "P-Value")
			For Each p In predictions
				If p.Prediction(0) = 1 Then
					Console.BackgroundColor = ConsoleColor.DarkYellow
					Console.ForegroundColor = ConsoleColor.Black
				End If
				Console.WriteLine("{0}" & vbTab & "{1:0.00}" & vbTab & "{2:0.00}", p.Prediction(0), p.Prediction(1), p.Prediction(2))
				Console.ResetColor()
			Next p
			Console.WriteLine("")
		End Sub

		Private Sub DetectChangepoint(mlcontext As MLContext, size As Integer, dataView As IDataView)
		  Console.WriteLine("Detect Persistent changes in pattern")

		  'STEP 2: Set the training algorithm    
		  Dim trainingPipeLine = mlcontext.Transforms.DetectIidChangePoint(outputColumnName:= NameOf(ProductSalesPrediction.Prediction), inputColumnName:= NameOf(ProductSalesData.numSales), confidence:= 95, changeHistoryLength:= size \ 4)

		  'STEP 3:Train the model by fitting the dataview
		  Console.WriteLine("=============== Training the model Using Change Point Detection Algorithm===============")
		  Dim trainedModel As ITransformer = trainingPipeLine.Fit(dataView)
		  Console.WriteLine("=============== End of training process ===============")

		  'Apply data transformation to create predictions.
		  Dim transformedData As IDataView = trainedModel.Transform(dataView)
		  Dim predictions = mlcontext.Data.CreateEnumerable(Of ProductSalesPrediction)(transformedData, reuseRowObject:= False)

		  Console.WriteLine($"{NameOf(ProductSalesPrediction.Prediction)} column obtained post-transformation.")
		  Console.WriteLine("Alert" & vbTab & "Score" & vbTab & "P-Value" & vbTab & "Martingale value")

		  For Each p In predictions
			 If p.Prediction(0) = 1 Then
				 Console.WriteLine("{0}" & vbTab & "{1:0.00}" & vbTab & "{2:0.00}" & vbTab & "{3:0.00}  <-- alert is on, predicted changepoint", p.Prediction(0), p.Prediction(1), p.Prediction(2), p.Prediction(3))
			 Else
				 Console.WriteLine("{0}" & vbTab & "{1:0.00}" & vbTab & "{2:0.00}" & vbTab & "{3:0.00}", p.Prediction(0), p.Prediction(1), p.Prediction(2), p.Prediction(3))
			 End If
		  Next p
			Console.WriteLine("")
		End Sub

		Public Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function
	End Module
End Namespace