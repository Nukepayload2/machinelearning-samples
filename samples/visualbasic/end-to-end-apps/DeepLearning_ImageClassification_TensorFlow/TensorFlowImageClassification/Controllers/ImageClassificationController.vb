Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.ML
Imports TensorFlowImageClassification.ImageHelpers
Imports TensorFlowImageClassification.ML.DataModels

Namespace TensorFlowImageClassification.Controllers
	<Route("api/[controller]")>
	<ApiController>
	Public Class ImageClassificationController
		Inherits ControllerBase

		Public ReadOnly Property Configuration As IConfiguration
		Private ReadOnly _predictionEnginePool As PredictionEnginePool(Of ImageInputData, ImageLabelPredictions)
		Private ReadOnly _logger As ILogger(Of ImageClassificationController)
		Private ReadOnly _labelsFilePath As String

'INSTANT VB NOTE: The variable configuration was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(predictionEnginePool As PredictionEnginePool(Of ImageInputData, ImageLabelPredictions), configuration_Renamed As IConfiguration, logger As ILogger(Of ImageClassificationController)) 'When using DI/IoC
			' Get the ML Model Engine injected, for scoring
			_predictionEnginePool = predictionEnginePool

			Me.Configuration = configuration_Renamed
			_labelsFilePath = GetAbsolutePath(Me.Configuration("MLModel:LabelsFilePath"))

			'Get other injected dependencies
			_logger = logger
		End Sub

		<HttpPost>
		<ProducesResponseType(200)>
		<ProducesResponseType(400)>
		<Route("classifyimage")>
		Public Async Function ClassifyImage(imageFile As IFormFile) As Task(Of IActionResult)
			If imageFile.Length = 0 Then
				Return BadRequest()
			End If

			Dim imageMemoryStream As MemoryStream = New MemoryStream
			Await imageFile.CopyToAsync(imageMemoryStream)

			'Check that the image is valid
			Dim imageData() As Byte = imageMemoryStream.ToArray()
			If Not imageData.IsValidImage() Then
				Return StatusCode(StatusCodes.Status415UnsupportedMediaType)
			End If

			'Convert to Image
			Dim image As Image = Image.FromStream(imageMemoryStream)

			'Convert to Bitmap
			Dim bitmapImage As Bitmap = CType(image, Bitmap)

			_logger.LogInformation($"Start processing image...")

			'Measure execution time
			Dim watch = System.Diagnostics.Stopwatch.StartNew()

			'Set the specific image data into the ImageInputData type used in the DataView
			Dim imageInputData As ImageInputData = New ImageInputData With {.Image = bitmapImage}

			'Predict code for provided image
			Dim imageLabelPredictions As ImageLabelPredictions = _predictionEnginePool.Predict(imageInputData)

			'Stop measuring time
			watch.Stop()
			Dim elapsedMs = watch.ElapsedMilliseconds
			_logger.LogInformation($"Image processed in {elapsedMs} miliseconds")

			'Predict the image's label (The one with highest probability)
			Dim imageBestLabelPrediction As ImagePredictedLabelWithProbability = FindBestLabelWithProbability(imageLabelPredictions, imageInputData)

			Return Ok(imageBestLabelPrediction)
		End Function

		Private Function FindBestLabelWithProbability(imageLabelPredictions As ImageLabelPredictions, imageInputData As ImageInputData) As ImagePredictedLabelWithProbability
			'Read TF model's labels (labels.txt) to classify the image across those labels
			Dim labels = ReadLabels(_labelsFilePath)

			Dim probabilities() As Single = imageLabelPredictions.PredictedLabels

			'Set a single label as predicted or even none if probabilities were lower than 70%
			Dim imageBestLabelPrediction = New ImagePredictedLabelWithProbability With {.ImageId = imageInputData.GetHashCode().ToString()}

'INSTANT VB TODO TASK: VB has no equivalent to the C# deconstruction assignments:
			(imageBestLabelPrediction.PredictedLabel, imageBestLabelPrediction.Probability) = GetBestLabel(labels, probabilities)

			Return imageBestLabelPrediction
		End Function

		Private Function GetBestLabel(labels() As String, probs() As Single) As (String, Single)
			Dim max = probs.Max()
			Dim index = probs.AsSpan().IndexOf(max)

			If max > 0.7 Then
				Return (labels(index), max)
			Else
				Return ("None", max)
			End If
		End Function

		Private Function ReadLabels(labelsLocation As String) As String()
			Return System.IO.File.ReadAllLines(labelsLocation)
		End Function

		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)
			Return fullPath
		End Function

		' GET api/ImageClassification
		<HttpGet>
		Public Function [Get]() As ActionResult(Of IEnumerable(Of String))
			Return New String() { "ACK Heart beat 1", "ACK Heart beat 2" }
		End Function
	End Class
End Namespace