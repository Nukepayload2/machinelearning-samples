Imports Common
Imports ICSharpCode.SharpZipLib.GZip
Imports ICSharpCode.SharpZipLib.Tar
Imports Microsoft.ML
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Net
Imports LargeDatasets.DataStructures
Imports Microsoft.ML.DataOperationsCatalog

Namespace LargeDatasets
	Friend Class Program
		Private Shared originalDataDirectoryRelativePath As String = "../../../Data/OriginalUrlData"
		Private Shared originalDataReltivePath As String = "../../../Data/OriginalUrlData/url_svmlight"
		Private Shared preparedDataReltivePath As String = "../../../Data/PreparedUrlData/url_svmlight"

		Private Shared originalDataDirectoryPath As String = GetAbsolutePath(originalDataDirectoryRelativePath)
		Private Shared originalDataPath As String = GetAbsolutePath(originalDataReltivePath)
		Private Shared preparedDataPath As String = GetAbsolutePath(preparedDataReltivePath)
		Shared Sub Main(args() As String)
			'STEP 1: Download dataset
			DownloadDataset(originalDataDirectoryPath)

			'Step 2: Prepare data by adding second column with value total number of features.
			PrepareDataset(originalDataPath, preparedDataPath)

			Dim mlContext As MLContext = New MLContext

			'STEP 3: Common data loading configuration  
			Dim fullDataView = mlContext.Data.LoadFromTextFile(Of UrlData)(path:= Path.Combine(preparedDataPath, "*"), hasHeader:= False, allowSparse:= True)

			'Step 4: Divide the whole dataset into 80% training and 20% testing data.
			Dim trainTestData As TrainTestData = mlContext.Data.TrainTestSplit(fullDataView, testFraction:= 0.2, seed:= 1)
			Dim trainDataView As IDataView = trainTestData.TrainSet
			Dim testDataView As IDataView = trainTestData.TestSet

			'Step 5: Map label value from string to bool
			Dim UrlLabelMap = New Dictionary(Of String, Boolean)
			UrlLabelMap("+1") = True 'Malicious url
			UrlLabelMap("-1") = False 'Benign
			Dim dataProcessingPipeLine = mlContext.Transforms.Conversion.MapValue("LabelKey", UrlLabelMap, "LabelColumn")
			ConsoleHelper.PeekDataViewInConsole(mlContext, trainDataView, dataProcessingPipeLine, 2)

			'Step 6: Append trainer to pipeline
			Dim trainingPipeLine = dataProcessingPipeLine.Append(mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine(labelColumnName:= "LabelKey", featureColumnName:= "FeatureVector"))

			'Step 7: Train the model
			Console.WriteLine("====Training the model=====")
			Dim mlModel = trainingPipeLine.Fit(trainDataView)
			Console.WriteLine("====Completed Training the model=====")
			Console.WriteLine("")

			'Step 8: Evaluate the model
			Console.WriteLine("====Evaluating the model=====")
			Dim predictions = mlModel.Transform(testDataView)
			Dim metrics = mlContext.BinaryClassification.Evaluate(data:= predictions, labelColumnName:= "LabelKey", scoreColumnName:= "Score")
			ConsoleHelper.PrintBinaryClassificationMetrics(mlModel.ToString(),metrics)

			' Try a single prediction
			Console.WriteLine("====Predicting sample data=====")
			Dim predEngine = mlContext.Model.CreatePredictionEngine(Of UrlData, UrlPrediction)(mlModel)
			' Create sample data to do a single prediction with it 
			Dim sampleDatas = CreateSingleDataSample(mlContext, trainDataView)
			For Each sampleData In sampleDatas
				Dim predictionResult As UrlPrediction = predEngine.Predict(sampleData)
				Console.WriteLine($"Single Prediction --> Actual value: {sampleData.LabelColumn} | Predicted value: {predictionResult.Prediction}")
			Next sampleData
			Console.WriteLine("====End of Process..Press any key to exit====")
			Console.ReadLine()
		End Sub

		Public Shared Sub DownloadDataset(originalDataDirectoryPath As String)
			If Not Directory.Exists(originalDataDirectoryPath) Then
				Console.WriteLine("====Downloading and extracting data====")
				Using client = New WebClient
					'The code below will download a dataset from a third-party, UCI (link), and may be governed by separate third-party terms. 
					'By proceeding, you agree to those separate terms.
					client.DownloadFile("https://archive.ics.uci.edu/ml/machine-learning-databases/url/url_svmlight.tar.gz", "url_svmlight.zip")
				End Using

				Dim inputStream As Stream = File.OpenRead("url_svmlight.zip")
				Dim gzipStream As Stream = New GZipInputStream(inputStream)
				Dim tarArchive As TarArchive = TarArchive.CreateInputTarArchive(gzipStream)
				tarArchive.ExtractContents(originalDataDirectoryPath)

				tarArchive.Close()
				gzipStream.Close()
				inputStream.Close()
				Console.WriteLine("====Downloading and extracting is completed====")
			End If
		End Sub

		Private Shared Sub PrepareDataset(originalDataPath As String, preparedDataPath As String)
			'Create folder for prepared Data path if it does not exist.
			If Not Directory.Exists(preparedDataPath) Then
				Directory.CreateDirectory(preparedDataPath)
			End If
				Console.WriteLine("====Preparing Data====")
				Console.WriteLine("")
				'ML.Net API checks for number of features column before the sparse matrix format
				'So add total number of features i.e 3231961 as second column by taking all the files from originalDataPath
				'and save those files in preparedDataPath.
				If Directory.GetFiles(preparedDataPath).Length = 0 Then
					Dim ext = New List(Of String) From {".svm"}
					Dim filesInDirectory = Directory.GetFiles(originalDataPath, "*.*", SearchOption.AllDirectories).Where(Function(s) ext.Contains(Path.GetExtension(s)))
					For Each file In filesInDirectory
						AddFeaturesColumn(Path.GetFullPath(file), preparedDataPath)
					Next file
				End If
				Console.WriteLine("====Data Preparation is done====")
				Console.WriteLine("")
				Console.WriteLine("original data path= {0}", originalDataPath)
				Console.WriteLine("")
				Console.WriteLine("prepared data path= {0}", preparedDataPath)
				Console.WriteLine("")
		End Sub

		Private Shared Sub AddFeaturesColumn(sourceFilePath As String, preparedDataPath As String)
			Dim sourceFileName As String = Path.GetFileName(sourceFilePath)
			Dim preparedFilePath As String = Path.Combine(preparedDataPath, sourceFileName)

			'if the file does not exist in preparedFilePath then copy from sourceFilePath and then add new column
			If Not File.Exists(preparedFilePath) Then
				File.Copy(sourceFilePath, preparedFilePath, True)
			End If
			Dim newColumnData As String = "3231961"
			Dim CSVDump() As String = File.ReadAllLines(preparedFilePath)
			Dim CSV As List(Of List(Of String)) = CSVDump.Select(Function(x) x.Split(" "c).ToList()).ToList()
			For i As Integer = 0 To CSV.Count - 1
				CSV(i).Insert(1, newColumnData)
			Next i

			File.WriteAllLines(preparedFilePath, CSV.Select(Function(x) String.Join(ControlChars.Tab, x)))
		End Sub

		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function
		Private Shared Function CreateSingleDataSample(mlContext As MLContext, dataView As IDataView) As List(Of UrlData)
			' Here (ModelInput object) you could provide new test data, hardcoded or from the end-user application, instead of the row from the file.
			Dim sampleForPredictions As List(Of UrlData) = mlContext.Data.CreateEnumerable(Of UrlData)(dataView, False).Take(4).ToList()

			Return sampleForPredictions
		End Function
	End Class
End Namespace
