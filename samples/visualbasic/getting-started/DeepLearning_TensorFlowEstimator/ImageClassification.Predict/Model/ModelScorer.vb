Imports System
Imports System.Linq
Imports ImageClassification.ImageData
Imports System.IO
Imports Microsoft.ML
Imports ImageClassification.Model.ConsoleHelpers

Namespace ImageClassification.Model
	Public Class ModelScorer
		Private ReadOnly dataLocation As String
		Private ReadOnly imagesFolder As String
		Private ReadOnly modelLocation As String
		Private ReadOnly mlContext As MLContext

		Public Sub New(dataLocation As String, imagesFolder As String, modelLocation As String)
			Me.dataLocation = dataLocation
			Me.imagesFolder = imagesFolder
			Me.modelLocation = modelLocation
			mlContext = New MLContext(seed:= 1)
		End Sub

		Public Sub ClassifyImages()
			ConsoleWriteHeader("Loading model")
			Console.WriteLine($"Model loaded: {modelLocation}")

			' Load the model
			Dim modelInputSchema As Object
			Dim loadedModel As ITransformer = mlContext.Model.Load(modelLocation, modelInputSchema)

			' Make prediction function (input = ImageNetData, output = ImageNetPrediction)
			Dim predictor = mlContext.Model.CreatePredictionEngine(Of ImageNetData, ImageNetPrediction)(loadedModel)
			' Read csv file into List<ImageNetData>
			Dim imageListToPredict = ImageNetData.ReadFromCsv(dataLocation, imagesFolder).ToList()

			ConsoleWriteHeader("Making classifications")
            ' There is a bug (https://github.com/dotnet/machinelearning/issues/1138), 
            ' that always buffers the response from the predictor
            ' so we have to make a copy-by-value op everytime we get a response
            ' from the predictor
            imageListToPredict.Select(Function(td) New With {
                Key td,
                Key .pred = predictor.Predict(td)
            }).Select(Function(pr) (pr.td.ImagePath, pr.pred.PredictedLabelValue, pr.pred.Score)).ToList().ForEach(Sub(pr) ConsoleWriteImagePrediction(pr.ImagePath, pr.PredictedLabelValue, pr.Score.Max()))
        End Sub
	End Class
End Namespace
