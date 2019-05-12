Imports System
Imports System.IO
Imports System.Linq
Imports Microsoft.ML
Imports ImageClassification.ImageData
Imports ImageClassification.Model.ConsoleHelpers

Namespace ImageClassification.Model
	Public Class ModelBuilder
		Private ReadOnly dataLocation As String
		Private ReadOnly imagesFolder As String
		Private ReadOnly inputModelLocation As String
		Private ReadOnly outputModelLocation As String
		Private ReadOnly mlContext As MLContext
		Private Shared LabelTokey As String = NameOf(LabelTokey)
		Private Shared ImageReal As String = NameOf(ImageReal)
		Private Shared PredictedLabelValue As String = NameOf(PredictedLabelValue)

		Public Sub New(dataLocation As String, imagesFolder As String, inputModelLocation As String, outputModelLocation As String)
			Me.dataLocation = dataLocation
			Me.imagesFolder = imagesFolder
			Me.inputModelLocation = inputModelLocation
			Me.outputModelLocation = outputModelLocation
			mlContext = New MLContext(seed:= 1)
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



			Dim data = mlContext.Data.LoadFromTextFile(Of ImageNetData)(path:=dataLocation, hasHeader:= False)

			Dim pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:= LabelTokey,inputColumnName:="Label").Append(mlContext.Transforms.LoadImages(outputColumnName:= "input", imageFolder:= imagesFolder, inputColumnName:= NameOf(ImageNetData.ImagePath))).Append(mlContext.Transforms.ResizeImages(outputColumnName:= "input", imageWidth:= ImageNetSettings.imageWidth, imageHeight:= ImageNetSettings.imageHeight, inputColumnName:= "input")).Append(mlContext.Transforms.ExtractPixels(outputColumnName:= "input", interleavePixelColors:= ImageNetSettings.channelsLast, offsetImage:= ImageNetSettings.mean)).Append(mlContext.Model.LoadTensorFlowModel(featurizerModelLocation).ScoreTensorFlowModel(outputColumnNames:= { "softmax2_pre_activation" }, inputColumnNames:= { "input" }, addBatchDimensionInput:= True)).Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName:=LabelTokey, featureColumnName:="softmax2_pre_activation")).Append(mlContext.Transforms.Conversion.MapKeyToValue(PredictedLabelValue,"PredictedLabel")).AppendCacheCheckpoint(mlContext)


			' Train the model
			ConsoleWriteHeader("Training classification model")
			Dim model As ITransformer = pipeline.Fit(data)

			' Process the training data through the model
			' This is an optional step, but it's useful for debugging issues
			Dim trainData = model.Transform(data)
			Dim loadedModelOutputColumnNames = trainData.Schema.Where(Function(col) Not col.IsHidden).Select(Function(col) col.Name)
			Dim trainData2 = mlContext.Data.CreateEnumerable(Of ImageNetPipeline)(trainData, False, True).ToList()
			trainData2.ForEach(Function(pr) ConsoleWriteImagePrediction(pr.ImagePath,pr.PredictedLabelValue, pr.Score.Max()))

			' Get some performance metric on the model using training data            
			Dim classificationContext = mlContext.MulticlassClassification
			ConsoleWriteHeader("Classification metrics")
			Dim metrics = classificationContext.Evaluate(trainData, labelColumnName:= LabelTokey, predictedLabelColumnName:= "PredictedLabel")
			Console.WriteLine($"LogLoss is: {metrics.LogLoss}")
			Console.WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(Function(c) c.ToString()))}")

			' Save the model to assets/outputs
			ConsoleWriteHeader("Save model to local file")
			ModelHelpers.DeleteAssets(outputModelLocation)

			mlContext.Model.Save(model, trainData.Schema, outputModelLocation)

			Console.WriteLine($"Model saved: {outputModelLocation}")
		End Sub

	End Class
End Namespace
