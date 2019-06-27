Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Logging
Imports OnnxObjectDetectionE2EAPP.Infrastructure
Imports OnnxObjectDetectionE2EAPP.Services
Imports OnnxObjectDetectionE2EAPP.Utilities

Namespace OnnxObjectDetectionE2EAPP.Controllers
	<Route("api/[controller]")>
	<ApiController>
	Public Class ObjectDetectionController
		Inherits ControllerBase

		Private ReadOnly _imagesTmpFolder As String

		Private ReadOnly _logger As ILogger(Of ObjectDetectionController)
		Private ReadOnly _objectDetectionService As IObjectDetectionService

		Private base64String As String = String.Empty
		Public Sub New(ObjectDetectionService As IObjectDetectionService, logger As ILogger(Of ObjectDetectionController), imageWriter As IImageFileWriter) 'When using DI/IoC (IImageFileWriter imageWriter)
			'Get injected dependencies
			_objectDetectionService = ObjectDetectionService
			_logger = logger
			_imagesTmpFolder = CommonHelpers.GetAbsolutePath("../../../ImagesTemp")
		End Sub

		Public Class Result
			Public Property imageString As String
		End Class

		<HttpGet()>
		Public Function [Get](<FromQuery> url As String) As IActionResult
			Dim imageFileRelativePath As String = "../../../assets" & url
			Dim imageFilePath As String = CommonHelpers.GetAbsolutePath(imageFileRelativePath)
			Try
				Dim image As Image = Image.FromFile(imageFilePath)
				'Convert to Bitmap
				Dim bitmapImage As Bitmap = CType(image, Bitmap)

				'Set the specific image data into the ImageInputData type used in the DataView
				Dim imageInputData As ImageInputData = New ImageInputData With {.Image = bitmapImage}

				'Detect the objects in the image                
				Dim result = DetectAndPaintImage(imageInputData,imageFilePath)
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
			Try
				Dim imageMemoryStream As MemoryStream = New MemoryStream
				Await imageFile.CopyToAsync(imageMemoryStream)

				'Check that the image is valid
				Dim imageData() As Byte = imageMemoryStream.ToArray()
				If Not imageData.IsValidImage() Then
					Return StatusCode(StatusCodes.Status415UnsupportedMediaType)
				End If

				'Convert to Image
				Dim image As Image = Image.FromStream(imageMemoryStream)

				Dim fileName As String = String.Format("{0}.Jpeg", image.GetHashCode())
				Dim imageFilePath As String = Path.Combine(_imagesTmpFolder, fileName)
				'save image to a path
				image.Save(imageFilePath, ImageFormat.Jpeg)

				'Convert to Bitmap
				Dim bitmapImage As Bitmap = CType(image, Bitmap)

				_logger.LogInformation($"Start processing image...")

				'Measure execution time
				Dim watch = System.Diagnostics.Stopwatch.StartNew()

				'Set the specific image data into the ImageInputData type used in the DataView
				Dim imageInputData As ImageInputData = New ImageInputData With {.Image = bitmapImage}

				'Detect the objects in the image                
				Dim result = DetectAndPaintImage(imageInputData, imageFilePath)

				'Stop measuring time
				watch.Stop()
				Dim elapsedMs = watch.ElapsedMilliseconds
				_logger.LogInformation($"Image processed in {elapsedMs} miliseconds")
				Return Ok(result)
			Catch e As Exception
				_logger.LogInformation("Error is: " & e.Message)
				Return BadRequest()
			End Try
		End Function

		Private Function DetectAndPaintImage(imageInputData As ImageInputData, imageFilePath As String) As Result
			'Predict the objects in the image
			_objectDetectionService.DetectObjectsUsingModel(imageInputData)
			Dim img = _objectDetectionService.PaintImages(imageFilePath)

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
