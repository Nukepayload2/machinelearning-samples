Imports System
Imports System.Collections.Generic
Imports System.Linq

Imports Microsoft.ML.Transforms
Imports Microsoft.ML

Imports ImageClassification.ImageDataStructures
Imports ImageClassification.ModelScorer.ConsoleHelpers
Imports ImageClassification.ModelScorer.ModelHelpers
Imports Microsoft.ML.Data
Imports Microsoft.ML.ImageAnalytics

Namespace ImageClassification.ModelScorer
    Public Class TFModelScorer
        Private ReadOnly dataLocation As String
        Private ReadOnly imagesFolder As String
        Private ReadOnly modelLocation As String
        Private ReadOnly labelsLocation As String
        Private ReadOnly mlContext As MLContext

        Public Sub New(ByVal dataLocation As String, ByVal imagesFolder As String, ByVal modelLocation As String, ByVal labelsLocation As String)
            Me.dataLocation = dataLocation
            Me.imagesFolder = imagesFolder
            Me.modelLocation = modelLocation
            Me.labelsLocation = labelsLocation
            mlContext = New MLContext()
        End Sub

        Public Structure ImageNetSettings
            Public Const imageHeight As Integer = 224
            Public Const imageWidth As Integer = 224
            Public Const mean As Single = 117
            Public Const channelsLast As Boolean = True
        End Structure

        Public Structure InceptionSettings
            ' for checking tensor names, you can use tools like Netron,
            ' which is installed by Visual Studio AI Tools

            ' input tensor name
            Public Const inputTensorName As String = "input"

            ' output tensor name
            Public Const outputTensorName As String = "softmax2"
        End Structure

        Public Sub Score()
            Dim model = LoadModel(dataLocation, imagesFolder, modelLocation)

            Dim predictions = PredictDataUsingModel(dataLocation, imagesFolder, labelsLocation, model).ToArray()

        End Sub

        Private Function LoadModel(ByVal dataLocation As String, ByVal imagesFolder As String, ByVal modelLocation As String) As PredictionEngine(Of ImageNetData, ImageNetPrediction)
            ConsoleWriteHeader("Read model")
            Console.WriteLine($"Model location: {modelLocation}")
            Console.WriteLine($"Images folder: {imagesFolder}")
            Console.WriteLine($"Training file: {dataLocation}")
            Console.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight}), image mean: {ImageNetSettings.mean}")

            Dim loader = New TextLoader(mlContext, {New TextLoader.Column("ImagePath", DataKind.Text, 0)})

            Dim data = loader.Read(New MultiFileSource(dataLocation))

            Dim pipeline = mlContext.Transforms.LoadImages(imageFolder:=imagesFolder, ("ImagePath", "ImageReal")).Append(mlContext.Transforms.Resize("ImageReal", "ImageReal", ImageNetSettings.imageHeight, ImageNetSettings.imageWidth)).Append(mlContext.Transforms.ExtractPixels({New ImagePixelExtractorTransform.ColumnInfo("ImageReal", "input", interleave:=ImageNetSettings.channelsLast, offset:=ImageNetSettings.mean)})).Append(mlContext.Transforms.ScoreTensorFlowModel(modelLocation, {"input"}, {"softmax2"}))

            Dim modeld = pipeline.Fit(data)

            Dim predictionFunction = modeld.CreatePredictionEngine(Of ImageNetData, ImageNetPrediction)(mlContext)

            Return predictionFunction
        End Function

        Protected Iterator Function PredictDataUsingModel(ByVal testLocation As String, ByVal imagesFolder As String, ByVal labelsLocation As String, ByVal model As PredictionEngine(Of ImageNetData, ImageNetPrediction)) As IEnumerable(Of ImageNetData)
            ConsoleWriteHeader("Classificate images")
            Console.WriteLine($"Images folder: {imagesFolder}")
            Console.WriteLine($"Training file: {testLocation}")
            Console.WriteLine($"Labels file: {labelsLocation}")

            Dim labels = ReadLabels(labelsLocation)

            Dim testData = ImageNetData.ReadFromCsv(testLocation, imagesFolder)

            For Each sample In testData
                Dim probs = model.Predict(sample).PredictedLabels
                Dim imageData = New ImageNetDataProbability() With {
                    .ImagePath = sample.ImagePath,
                    .Label = sample.Label
                }
                With GetBestLabel(labels, probs)
                    imageData.PredictedLabel = .Item1
                    imageData.Probability = .Item2
                End With
                imageData.ConsoleWrite()
                Yield imageData
            Next sample
        End Function
    End Class
End Namespace
