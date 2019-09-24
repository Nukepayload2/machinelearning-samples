Imports Microsoft.ML
Imports ImageClassification.ImageDataStructures
Imports ImageClassification.ModelScorer.ConsoleHelpers
Imports ImageClassification.ModelScorer.ModelHelpers

Namespace ImageClassification.ModelScorer
    Public Class TFModelScorer
        Private ReadOnly dataLocation As String
        Private ReadOnly imagesFolder As String
        Private ReadOnly modelLocation As String
        Private ReadOnly labelsLocation As String
        Private ReadOnly mlContext As MLContext
        Private Shared ImageReal As String = NameOf(ImageReal)

        Public Sub New(dataLocation As String, imagesFolder As String, modelLocation As String, labelsLocation As String)
            Me.dataLocation = dataLocation
            Me.imagesFolder = imagesFolder
            Me.modelLocation = modelLocation
            Me.labelsLocation = labelsLocation
            mlContext = New MLContext
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

        Private Function LoadModel(dataLocation As String, imagesFolder As String, modelLocation As String) As PredictionEngine(Of ImageNetData, ImageNetPrediction)
            ConsoleWriteHeader("Read model")
            Console.WriteLine($"Model location: {modelLocation}")
            Console.WriteLine($"Images folder: {imagesFolder}")
            Console.WriteLine($"Training file: {dataLocation}")
            Console.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight}), image mean: {ImageNetSettings.mean}")

            Dim data = mlContext.Data.LoadFromTextFile(Of ImageNetData)(dataLocation, hasHeader:=True)

            Dim pipeline = mlContext.Transforms.LoadImages(outputColumnName:="input", imageFolder:=imagesFolder, inputColumnName:=NameOf(ImageNetData.ImagePath)).Append(mlContext.Transforms.ResizeImages(outputColumnName:="input", imageWidth:=ImageNetSettings.imageWidth, imageHeight:=ImageNetSettings.imageHeight, inputColumnName:="input")).Append(mlContext.Transforms.ExtractPixels(outputColumnName:="input", interleavePixelColors:=ImageNetSettings.channelsLast, offsetImage:=ImageNetSettings.mean)).Append(mlContext.Model.LoadTensorFlowModel(modelLocation).ScoreTensorFlowModel(outputColumnNames:={"softmax2"}, inputColumnNames:={"input"}, addBatchDimensionInput:=True))

            Dim model As ITransformer = pipeline.Fit(data)

            Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of ImageNetData, ImageNetPrediction)(model)

            Return predictionEngine
        End Function

        Protected Iterator Function PredictDataUsingModel(testLocation As String, imagesFolder As String, labelsLocation As String, model As PredictionEngine(Of ImageNetData, ImageNetPrediction)) As IEnumerable(Of ImageNetData)
            ConsoleWriteHeader("Classificate images")
            Console.WriteLine($"Images folder: {imagesFolder}")
            Console.WriteLine($"Training file: {testLocation}")
            Console.WriteLine($"Labels file: {labelsLocation}")

            Dim labels = ReadLabels(labelsLocation)

            Dim testData = ImageNetData.ReadFromCsv(testLocation, imagesFolder)

            For Each sample In testData
                Dim probs = model.Predict(sample).PredictedLabels
                Dim imageData = New ImageNetDataProbability With {
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
