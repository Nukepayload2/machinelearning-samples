Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Threading.Tasks
Imports ImageClassification.DataModels
Imports Microsoft.ML
Imports Microsoft.ML.Data

Namespace ImageClassification.Predict
	Friend Class Program
		Shared Sub Main(args() As String)
			Dim assetsRelativePath As String = "../../../assets"
			Dim assetsPath As String = GetAbsolutePath(assetsRelativePath)

			Dim imagesForPredictions As String = Path.Combine(assetsPath, "inputs", "images-for-predictions")

			Dim imageClassifierModelZipFilePath = Path.Combine(assetsPath, "inputs", "MLNETModel", "imageClassifier.zip")

			Try
				Dim mlContext As New MLContext(seed:= 1)

				Console.WriteLine($"Loading model from: {imageClassifierModelZipFilePath}")

				' Load the model
				Dim modelInputSchema As Object
				Dim loadedModel As ITransformer = mlContext.Model.Load(imageClassifierModelZipFilePath, modelInputSchema)

				' Create prediction engine to try a single prediction (input = ImageData, output = ImagePrediction)
				Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of ImageData, ImagePrediction)(loadedModel)

				Dim imagesToPredict As IEnumerable(Of ImageData) = LoadImagesFromDirectory(imagesForPredictions, True)

				'Predict the first image in the folder
				Dim imageToPredict As ImageData = New ImageData With {.ImagePath = imagesToPredict.First().ImagePath}

				Dim prediction = predictionEngine.Predict(imageToPredict)

				Dim index = prediction.PredictedLabel

				' Obtain the original label names to map through the predicted label-index
				Dim keys As VBuffer(Of ReadOnlyMemory(Of Char)) = Nothing
				predictionEngine.OutputSchema("LabelAsKey").GetKeyValues(keys)
				Dim originalLabels = keys.DenseValues().ToArray()

				Console.WriteLine($"ImageFile : [{Path.GetFileName(imageToPredict.ImagePath)}], " & $"Scores : [{String.Join(",", prediction.Score)}], " & $"Predicted Label : {originalLabels(index)}")

				'Predict all images in the folder
				'
				Console.WriteLine("")
				Console.WriteLine("Predicting several images...")

				For Each currentImageToPredict As ImageData In imagesToPredict
					Dim currentPrediction = predictionEngine.Predict(currentImageToPredict)
					Dim currentIndex = currentPrediction.PredictedLabel
					Console.WriteLine($"ImageFile : [{Path.GetFileName(currentImageToPredict.ImagePath)}], " & $"Scores : [{String.Join(",", currentPrediction.Score)}], " & $"Predicted Label : {originalLabels(currentIndex)}")
				Next currentImageToPredict

			Catch ex As Exception
				Console.WriteLine(ex.ToString())
			End Try

			Console.WriteLine("Press any key to end the app..")
			Console.ReadKey()
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

		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function
	End Class
End Namespace
