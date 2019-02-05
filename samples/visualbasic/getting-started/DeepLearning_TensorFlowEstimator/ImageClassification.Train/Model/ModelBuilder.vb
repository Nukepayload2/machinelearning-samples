Imports System
Imports System.IO
Imports System.Linq
Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports Microsoft.ML.ImageAnalytics
Imports ImageClassification.ImageData
Imports ImageClassification.Model.ConsoleHelpers

Namespace ImageClassification.Model
	Public Class ModelBuilder
		Private ReadOnly dataLocation As String
		Private ReadOnly imagesFolder As String
		Private ReadOnly inputModelLocation As String
		Private ReadOnly outputModelLocation As String
		Private ReadOnly mlContext As MLContext

        Public Sub New(dataLocation As String, imagesFolder As String, inputModelLocation As String, outputModelLocation As String)
            Me.dataLocation = dataLocation
            Me.imagesFolder = imagesFolder
            Me.inputModelLocation = inputModelLocation
            Me.outputModelLocation = outputModelLocation
            mlContext = New MLContext(seed:=1)
        End Sub

        Private Structure ImageNetSettings
			Public Const imageHeight As Integer = 224
			Public Const imageWidth As Integer = 224
			Public Const mean As Single = 117
			Public Const scale As Single = 1
			Public Const channelsLast As Boolean = True
		End Structure

		Public Sub BuildAndTrain()
			Dim featurizerModelLocation = inputModelLocation

			ConsoleWriteHeader("Read model")
			Console.WriteLine($"Model location: {featurizerModelLocation}")
			Console.WriteLine($"Images folder: {imagesFolder}")
			Console.WriteLine($"Training file: {dataLocation}")
			Console.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight}), image mean: {ImageNetSettings.mean}")

            Dim data = mlContext.Data.ReadFromTextFile(Of ImageNetData)(dataLocation, hasHeader:=True)

            Dim pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "LabelTokey").
                Append(mlContext.Transforms.LoadImages(imagesFolder, ("ImagePath", "ImageReal"))).
                Append(mlContext.Transforms.Resize("ImageReal", "ImageReal", ImageNetSettings.imageHeight, ImageNetSettings.imageWidth)).
                Append(mlContext.Transforms.ExtractPixels(New ImagePixelExtractorTransform.ColumnInfo("ImageReal", "input", interleave:=ImageNetSettings.channelsLast, offset:=ImageNetSettings.mean))).
                Append(mlContext.Transforms.ScoreTensorFlowModel(featurizerModelLocation, {"input"}, {"softmax2_pre_activation"})).
                Append(mlContext.MulticlassClassification.Trainers.LogisticRegression("LabelTokey", "softmax2_pre_activation")).
                Append(mlContext.Transforms.Conversion.MapKeyToValue(("PredictedLabel", "PredictedLabelValue")))

            ' Train the pipeline
            ConsoleWriteHeader("Training classification model")
            Dim model = pipeline.Fit(Data)

            ' Process the training data through the model
            ' This is an optional step, but it's useful for debugging issues
            Dim trainData = model.Transform(data)
			Dim loadedModelOutputColumnNames = trainData.Schema.Where(Function(col) Not col.IsHidden).Select(Function(col) col.Name)
			Dim trainData2 = trainData.AsEnumerable(Of ImageNetPipeline)(mlContext, False, True).ToList()
            trainData2.ForEach(Sub(pr) ConsoleWriteImagePrediction(pr.ImagePath, pr.PredictedLabelValue, pr.Score.Max()))

            ' Get some performance metric on the model using training data            
            Dim sdcaContext = New MulticlassClassificationContext(mlContext)
			ConsoleWriteHeader("Classification metrics")
			Dim metrics = sdcaContext.Evaluate(trainData, label:= "LabelTokey", predictedLabel:= "PredictedLabel")
			Console.WriteLine($"LogLoss is: {metrics.LogLoss}")
			Console.WriteLine($"PerClassLogLoss is: {String.Join(", ", metrics.PerClassLogLoss.Select(Function(c) c.ToString()))}")

			' Save the model to assets/outputs
			ConsoleWriteHeader("Save model to local file")
			ModelHelpers.DeleteAssets(outputModelLocation)
			Using f = New FileStream(outputModelLocation, FileMode.Create)
				mlContext.Model.Save(model, f)
			End Using

			Console.WriteLine($"Model saved: {outputModelLocation}")
		End Sub

	End Class
End Namespace
