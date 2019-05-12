Imports System
Imports System.IO
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Logging
Imports OnnxObjectDetectionE2EAPP.Infrastructure
Imports OnnxObjectDetectionE2EAPP.OnnxModelScorers

Namespace OnnxObjectDetectionE2EAPP.Controllers
	<Route("api/[controller]")>
	<ApiController>
	Public Class ObjectDetectionController
		Inherits ControllerBase

		Private ReadOnly _imageWriter As IImageFileWriter
		Private ReadOnly _imagesTmpFolder As String

		Private ReadOnly _logger As ILogger(Of ObjectDetectionController)
		Private ReadOnly _modelScorer As IOnnxModelScorer

		Private base64String As String = String.Empty

		Public Sub New(modelScorer As IOnnxModelScorer, logger As ILogger(Of ObjectDetectionController), imageWriter As IImageFileWriter) 'When using DI/IoC (IImageFileWriter imageWriter)
			'Get injected dependencies
			_modelScorer = modelScorer
			_logger = logger
			_imageWriter = imageWriter
			_imagesTmpFolder = ModelHelpers.GetAbsolutePath("../../../ImagesTemp")
		End Sub

		Public Class Result
			Public Property imageString As String
		End Class

		<HttpGet()>
		Public Function [Get](<FromQuery> url As String) As IActionResult
			Dim imageFileRelativePath As String = "../../.." & url
			Dim imageFilePath As String = ModelHelpers.GetAbsolutePath(imageFileRelativePath)
			Try
				'Detect the objects in the image                
				Dim result = DetectAndPaintImage(imageFilePath)
				Return Ok(result)
			Catch e As Exception
				_logger.LogInformation("Error is: " & e.Message)
				Return BadRequest()
			End Try
		End Function

		<HttpPost>
		<ProducesResponseType(200)>
		<ProducesResponseType(400)>
		<Route("IdentifyObjects")>
		Public Async Function IdentifyObjects(imageFile As IFormFile) As Task(Of IActionResult)
			If imageFile.Length = 0 Then
				Return BadRequest()
			End If

			Dim imageFilePath As String = "", fileName As String = ""
			Try
				'Save the temp image into the temp-folder 
				fileName = Await _imageWriter.UploadImageAsync(imageFile, _imagesTmpFolder)
				imageFilePath = Path.Combine(_imagesTmpFolder, fileName)

				'Detect the objects in the image                
				Dim result = DetectAndPaintImage(imageFilePath)
				Return Ok(result)
			Catch e As Exception
				_logger.LogInformation("Error is: " & e.Message)
				Return BadRequest()
			End Try
		End Function

		Private Function DetectAndPaintImage(imageFilePath As String) As Result
			'Predict the objects in the image
			_modelScorer.DetectObjectsUsingModel(imageFilePath)
			Dim img = _modelScorer.PaintImages(imageFilePath)

			Using m As MemoryStream = New MemoryStream
				img.Save(m, img.RawFormat)
				Dim imageBytes() As Byte = m.ToArray()

				' Convert byte[] to Base64 String
				base64String = Convert.ToBase64String(imageBytes)
				Dim result = New Result With {.imageString = base64String}
				Return result
			End Using
		End Function
	End Class
End Namespace
