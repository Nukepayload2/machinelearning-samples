Imports System
Imports Microsoft.ML
Imports ShampooSales.DataStructures
Imports System.IO

Namespace ShampooSales
	Friend Module Program
		Private BaseDatasetsRelativePath As String = "../../../../Data"
		Private DatasetRelativePath As String = $"{BaseDatasetsRelativePath}/shampoo-sales.csv"

		Private DatasetPath As String = GetAbsolutePath(DatasetRelativePath)

		Private BaseModelsRelativePath As String = "../../../../MLModels"
		Private ModelRelativePath As String = $"{BaseModelsRelativePath}/ShampooSalesModel.zip"

		Private ModelPath As String = GetAbsolutePath(ModelRelativePath)

		Sub Main()
            ' Create MLContext to be shared across the model creation workflow objects 
            Dim mlcontext As New MLContext

            'assign the Number of records in dataset file to cosntant variable
            Const size As Integer = 36

			'STEP 1: Common data loading configuration
			Dim dataView As IDataView = mlcontext.Data.LoadFromTextFile(Of ShampooSalesData)(path:= DatasetPath, hasHeader:=True, separatorChar:=","c)

			'STEP 2: Set the training algorithm    
			Dim trainingPipeLine = mlcontext.Transforms.DetectIidSpike(outputColumnName:= NameOf(ShampooSalesPrediction.Prediction), inputColumnName:= NameOf(ShampooSalesData.numSales),confidence:= 95, pvalueHistoryLength:= size \ 4)

			'STEP 3:Train the model by fitting the dataview
			Console.WriteLine("=============== Training the model ===============")
			Dim trainedModel As ITransformer = trainingPipeLine.Fit(dataView)
			Console.WriteLine("=============== End of training process ===============")

			'Apply data transformation to create predictions.
			Dim transformedData As IDataView = trainedModel.Transform(dataView)
			Dim predictions = mlcontext.Data.CreateEnumerable(Of ShampooSalesPrediction)(transformedData, reuseRowObject:= False)

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
			Console.WriteLine("=============== End of process, hit any key to finish ===============")

			Console.ReadLine()
		End Sub

		Public Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function
	End Module
End Namespace