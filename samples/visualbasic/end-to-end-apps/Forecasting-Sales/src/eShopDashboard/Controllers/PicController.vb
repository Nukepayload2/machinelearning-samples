Imports eShopDashboard.Queries
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Mvc
Imports System.IO
Imports System.Net
Imports System.Threading.Tasks

Namespace eShopDashboard.Controllers
	Public Class PicController
		Inherits Controller

		Private ReadOnly _env As IHostingEnvironment
		Private ReadOnly _queries As ICatalogQueries

		Public Sub New(env As IHostingEnvironment, queries As ICatalogQueries)
			_env = env
			_queries = queries
		End Sub

		' GET: /<controller>/
		<HttpGet("api/catalog/items/{catalogItemId:int}/pic")>
		<ProducesResponseType(CInt(HttpStatusCode.NotFound))>
		<ProducesResponseType(CInt(HttpStatusCode.BadRequest))>
		Public Async Function GetImage(catalogItemId As Integer) As Task(Of IActionResult)
			If catalogItemId <= 0 Then
				Return BadRequest()
			End If

			Dim item = Await _queries.GetCatalogItemById(catalogItemId)

			If item IsNot Nothing Then
				If String.IsNullOrEmpty(item.PictureFileName) Then
					Return BlankImage()
				End If

				Dim contentRootPath = _env.ContentRootPath

				Dim path = System.IO.Path.Combine(contentRootPath, "ProductImages", item.PictureFileName)

				If Not System.IO.File.Exists(path) Then
					Return BlankImage()
				End If

				Dim imageFileExtension As String = System.IO.Path.GetExtension(item.PictureFileName)
				Dim mimetype As String = GetImageMimeTypeFromImageFileExtension(imageFileExtension)

				Dim buffer = System.IO.File.ReadAllBytes(path)

				Return File(buffer, mimetype)
			End If

			Return BlankImage()
		End Function

		Private Function BlankImage() As IActionResult
'INSTANT VB NOTE: The local variable blankImage was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Const blankImage_Renamed As String = "coming_soon.png"
			Dim pathBlankImage = Path.Combine(_env.WebRootPath, "images", blankImage_Renamed)

			Dim imageFileExtension As String = Path.GetExtension(pathBlankImage)
			Dim mimetype As String = GetImageMimeTypeFromImageFileExtension(imageFileExtension)

			Dim buffer = System.IO.File.ReadAllBytes(pathBlankImage)

			Return File(buffer, mimetype)
		End Function

		Private Function GetImageMimeTypeFromImageFileExtension(extension As String) As String
			Dim mimetype As String

			Select Case extension
				Case ".png"
					mimetype = "image/png"

				Case ".gif"
					mimetype = "image/gif"

				Case ".jpg", ".jpeg"
					mimetype = "image/jpeg"

				Case ".bmp"
					mimetype = "image/bmp"

				Case ".tiff"
					mimetype = "image/tiff"

				Case ".wmf"
					mimetype = "image/wmf"

				Case ".jp2"
					mimetype = "image/jp2"

				Case ".svg"
					mimetype = "image/svg+xml"

				Case Else
					mimetype = "application/octet-stream"
			End Select

			Return mimetype
		End Function
	End Class
End Namespace