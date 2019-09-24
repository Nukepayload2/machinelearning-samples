Imports ImageClassification.DataModels
Imports System.IO
Imports Microsoft.ML

Namespace ImageClassification.Model
    Public Class ModelScorer
        Private ReadOnly imagesFolder As String
        Private ReadOnly modelLocation As String
        Private ReadOnly mlContext As MLContext

        Public Sub New(imagesFolder As String, modelLocation As String)
            Me.imagesFolder = imagesFolder
            Me.modelLocation = modelLocation
            mlContext = New MLContext(seed:=1)
        End Sub

        Public Sub ClassifyImages()
            ConsoleWriteHeader("Loading model")
            Console.WriteLine("")
            Console.WriteLine($"Model loaded: {modelLocation}")

            ' Load the model
            Dim modelInputSchema As Object
            Dim loadedModel As ITransformer = mlContext.Model.Load(modelLocation, modelInputSchema)

            ' Make prediction engine (input = ImageDataForScoring, output = ImagePrediction)
            Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of ImageData, ImagePrediction)(loadedModel)

            Dim imagesToPredict As IEnumerable(Of ImageData) = LoadImagesFromDirectory(imagesFolder, True)

            ConsoleWriteHeader("Predicting classifications...")

            'Predict the first image in the folder
            '
            Dim imageToPredict As ImageData = New ImageData With {.ImagePath = imagesToPredict.First().ImagePath}

            Dim prediction = predictionEngine.Predict(imageToPredict)

            Console.WriteLine("")
            Console.WriteLine($"ImageFile : [{Path.GetFileName(imageToPredict.ImagePath)}], " & $"Scores : [{String.Join(",", prediction.Score)}], " & $"Predicted Label : {prediction.PredictedLabelValue}")

            '////

            'Predict all images in the folder
            '
            Console.WriteLine("")
            Console.WriteLine("Predicting several images...")

            For Each currentImageToPredict As ImageData In imagesToPredict
                Dim currentPrediction = predictionEngine.Predict(currentImageToPredict)
                Console.WriteLine("")
                Console.WriteLine($"ImageFile : [{Path.GetFileName(currentImageToPredict.ImagePath)}], " & $"Scores : [{String.Join(",", currentPrediction.Score)}], " & $"Predicted Label : {currentPrediction.PredictedLabelValue}")
            Next currentImageToPredict
            '////

        End Sub

        Public Shared Iterator Function LoadImagesFromDirectory(folder As String, Optional useFolderNameasLabel As Boolean = True) As IEnumerable(Of ImageData)
            Dim files = Directory.GetFiles(folder, "*", searchOption:=SearchOption.AllDirectories)

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
    End Class
End Namespace
