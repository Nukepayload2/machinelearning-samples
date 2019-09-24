Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading.Tasks
Imports ImageClassification.DataModels
Imports Microsoft.ML
Imports Microsoft.ML.Transforms
Imports Microsoft.ML.DataOperationsCatalog
Imports System.Linq
Imports Microsoft.ML.Data
Imports Common

Namespace ImageClassification.Train
	Public Class Program
		Shared Sub Main(args() As String)
			Dim assetsRelativePath As String = "../../../assets"
			Dim assetsPath As String = GetAbsolutePath(assetsRelativePath)

			Dim outputMlNetModelFilePath = Path.Combine(assetsPath, "outputs", "imageClassifier.zip")
			Dim imagesForPredictions As String = Path.Combine(assetsPath, "inputs", "images-for-predictions", "FlowersForPredictions")

			Dim imagesDownloadFolderPath As String = Path.Combine(assetsPath, "inputs", "images")

			' 1. Download the image set and unzip
			Dim finalImagesFolderName As String = DownloadImageSet(imagesDownloadFolderPath)
			Dim fullImagesetFolderPath As String = Path.Combine(imagesDownloadFolderPath, finalImagesFolderName)

			Dim mlContext As New MLContext(seed:= 1)

			' 2. Load the initial full image-set into an IDataView and shuffle so it'll be better balanced
			Dim images As IEnumerable(Of ImageData) = LoadImagesFromDirectory(folder:= fullImagesetFolderPath, useFolderNameasLabel:= True)
			Dim fullImagesDataset As IDataView = mlContext.Data.LoadFromEnumerable(images)
			Dim shuffledFullImagesDataset As IDataView = mlContext.Data.ShuffleRows(fullImagesDataset)

			' 3. Split the data 80:20 into train and test sets, train and evaluate.
			Dim trainTestData As TrainTestData = mlContext.Data.TrainTestSplit(shuffledFullImagesDataset, testFraction:= 0.2)
			Dim trainDataView As IDataView = trainTestData.TrainSet
			Dim testDataView As IDataView = trainTestData.TestSet

			'// OPTIONAL (*1*)  
			' Prepare the Validation set to be used by the internal TensorFlow training process
			' This step is optional but needed if you want to get validation performed while training in TensorFlow
			Dim transformedValidationDataView As IDataView = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:= "LabelAsKey", inputColumnName:= "Label", keyOrdinality:= ValueToKeyMappingEstimator.KeyOrdinality.ByValue).Fit(testDataView).Transform(testDataView)

			' 4. Define the model's training pipeline 
			Dim pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:= "LabelAsKey", inputColumnName:= "Label", keyOrdinality:= ValueToKeyMappingEstimator.KeyOrdinality.ByValue).Append(mlContext.Model.ImageClassification("ImagePath", "LabelAsKey", arch:= ImageClassificationEstimator.Architecture.ResnetV2101, epoch:= 100, batchSize:= 30, metricsCallback:= Sub(metrics) Console.WriteLine(metrics), validationSet:= transformedValidationDataView))

			' 4. Train/create the ML model
			Console.WriteLine("*** Training the image classification model with DNN Transfer Learning on top of the selected pre-trained model/architecture ***")
			Dim trainedModel As ITransformer = pipeline.Fit(trainDataView)

			' 5. Get the quality metrics (accuracy, etc.)
			EvaluateModel(mlContext, testDataView, trainedModel)

			' 6. Try a single prediction simulating an end-user app
			TrySinglePrediction(imagesForPredictions, mlContext, trainedModel)

			' 7. Save the model to assets/outputs (You get ML.NET .zip model file and TensorFlow .pb model file)
			mlContext.Model.Save(trainedModel, trainDataView.Schema, outputMlNetModelFilePath)
			Console.WriteLine($"Model saved to: {outputMlNetModelFilePath}")

			Console.WriteLine("Press any key to finish")
			Console.ReadKey()
		End Sub

		Private Shared Sub EvaluateModel(mlContext As MLContext, testDataset As IDataView, trainedModel As ITransformer)
			Console.WriteLine("Making predictions in bulk for evaluating model's quality...")

			Dim predictionsDataView As IDataView = trainedModel.Transform(testDataset)

			Dim metrics = mlContext.MulticlassClassification.Evaluate(predictionsDataView, labelColumnName:="LabelAsKey", predictedLabelColumnName:= "PredictedLabel")
			ConsoleHelper.PrintMultiClassClassificationMetrics_Renamed("TensorFlow DNN Transfer Learning", metrics)

			Console.WriteLine("*** Showing all the predictions ***")
			' Find the original label names.
			Dim keys As VBuffer(Of ReadOnlyMemory(Of Char)) = Nothing
			predictionsDataView.Schema("LabelAsKey").GetKeyValues(keys)
			Dim originalLabels = keys.DenseValues().ToArray()

			Dim predictions As List(Of ImagePredictionEx) = mlContext.Data.CreateEnumerable(Of ImagePredictionEx)(predictionsDataView, False, True).ToList()
			predictions.ForEach(Sub(pred) ConsoleWriteImagePrediction(pred.ImagePath, pred.Label, (originalLabels(pred.PredictedLabel)).ToString(), pred.Score.Max()))

		End Sub

		Private Shared Sub TrySinglePrediction(imagesForPredictions As String, mlContext As MLContext, trainedModel As ITransformer)
			' Create prediction function to try one prediction
			Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of ImageData, ImagePrediction)(trainedModel)

			Dim testImages As IEnumerable(Of ImageData) = LoadImagesFromDirectory(imagesForPredictions, True)
			Dim imageToPredict As ImageData = New ImageData With {.ImagePath = testImages.First().ImagePath}

			Dim prediction = predictionEngine.Predict(imageToPredict)

			' Find the original label names.
			Dim keys As VBuffer(Of ReadOnlyMemory(Of Char)) = Nothing
			predictionEngine.OutputSchema("LabelAsKey").GetKeyValues(keys)

			Dim originalLabels = keys.DenseValues().ToArray()
			Dim index = prediction.PredictedLabel

			Console.WriteLine($"ImageFile : [{Path.GetFileName(imageToPredict.ImagePath)}], " & $"Scores : [{String.Join(",", prediction.Score)}], " & $"Predicted Label : {originalLabels(index)}")
		End Sub

		Public Shared Iterator Function LoadImagesFromDirectory(folder As String, Optional useFolderNameasLabel As Boolean = True) As IEnumerable(Of ImageData)
			Dim files = Directory.GetFiles(folder, "*", searchOption:= SearchOption.AllDirectories)

			For Each file In files
				If (Path.GetExtension(file) <> ".jpg") AndAlso (Path.GetExtension(file) <> ".png") Then
					Continue For
				End If

				Dim label = Path.GetFileName(file)
				If useFolderNameasLabel Then
					label = Directory.GetParent(file).Name
				Else
					Dim index As Integer = 0
					Do While index < label.Length
						If Not Char.IsLetter(label.Chars(index)) Then
							label = label.Substring(0, index)
							Exit Do
						End If
						index += 1
					Loop
				End If

				Yield New ImageData With {
					.ImagePath = file,
					.Label = label
				}

			Next file
		End Function

		Public Shared Function DownloadImageSet(imagesDownloadFolder As String) As String
			' get a set of images to teach the network about the new classes

			'SINGLE SMALL FLOWERS IMAGESET (200 files)
			Dim fileName As String = "flower_photos_small_set.zip"
			Dim url As String = $"https://mlnetfilestorage.file.core.windows.net/imagesets/flower_images/flower_photos_small_set.zip?st=2019-08-07T21%3A27%3A44Z&se=2030-08-08T21%3A27%3A00Z&sp=rl&sv=2018-03-28&sr=f&sig=SZ0UBX47pXD0F1rmrOM%2BfcwbPVob8hlgFtIlN89micM%3D"
			Web.Download(url, imagesDownloadFolder, fileName)
			Compress.UnZip(Path.Join(imagesDownloadFolder, fileName), imagesDownloadFolder)

			'SINGLE FULL FLOWERS IMAGESET (3,600 files)
			'string fileName = "flower_photos.tgz";
			'string url = $"http://download.tensorflow.org/example_images/{fileName}";
			'Web.Download(url, imagesDownloadFolder, fileName);
			'Compress.ExtractTGZ(Path.Join(imagesDownloadFolder, fileName), imagesDownloadFolder);

			Return Path.GetFileNameWithoutExtension(fileName)
		End Function

		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function

		Public Shared Sub ConsoleWriteImagePrediction(ImagePath As String, Label As String, PredictedLabel As String, Probability As Single)
			Dim defaultForeground = Console.ForegroundColor
			Dim labelColor = ConsoleColor.Magenta
			Dim probColor = ConsoleColor.Blue

			Console.Write("Image File: ")
			Console.ForegroundColor = labelColor
			Console.Write($"{Path.GetFileName(ImagePath)}")
			Console.ForegroundColor = defaultForeground
			Console.Write(" original labeled as ")
			Console.ForegroundColor = labelColor
			Console.Write(Label)
			Console.ForegroundColor = defaultForeground
			Console.Write(" predicted as ")
			Console.ForegroundColor = labelColor
			Console.Write(PredictedLabel)
			Console.ForegroundColor = defaultForeground
			Console.Write(" with score ")
			Console.ForegroundColor = probColor
			Console.Write(Probability)
			Console.ForegroundColor = defaultForeground
			Console.WriteLine("")
		End Sub

	End Class
End Namespace

