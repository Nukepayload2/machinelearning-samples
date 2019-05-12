Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Http


Namespace OnnxObjectDetectionE2EAPP.Infrastructure
	''' <summary>
	''' Interface to use in DI/IoC
	''' </summary>
	Public Interface IImageFileWriter
		Function UploadImageAsync(file As IFormFile, imagesTempFolder As String) As Task(Of String)
		Sub DeleteImageTempFile(filePathName As String)
	End Interface

	''' <summary>
	''' Implementation class to inject with DI/IoC
	''' </summary>
	Public Class ImageFileWriter
		Implements IImageFileWriter

		Public Async Function UploadImageAsync(file As IFormFile, imagesTempFolder As String) As Task(Of String) Implements IImageFileWriter.UploadImageAsync
			If CheckIfImageFile(file) Then
				Return Await WriteFile(file, imagesTempFolder)
			End If

			Return "Invalid image file"
		End Function

		''' <summary>
		''' Method to check if file is image file
		''' </summary>
		''' <param name="file"></param>
		''' <returns></returns>
		Private Function CheckIfImageFile(file As IFormFile) As Boolean
			Dim fileBytes() As Byte
			Using ms = New MemoryStream
				file.CopyTo(ms)
				fileBytes = ms.ToArray()
			End Using

			Return ImageValidationExtensions.GetImageFormat(fileBytes) <> ImageValidationExtensions.ImageFormat.unknown
		End Function

		''' <summary>
		''' Method to write file onto the disk
		''' </summary>
		''' <param name="file"></param>
		''' <returns></returns>
		Public Async Function WriteFile(file As IFormFile, imagesTempFolder As String) As Task(Of String)
			Dim fileName As String
			Try
				Dim extension = "." & file.FileName.Split("."c)(file.FileName.Split("."c).Length - 1)
				fileName = Guid.NewGuid().ToString() & extension 'Create a new name for the file

				Dim filePathName = Path.Combine(Directory.GetCurrentDirectory(), imagesTempFolder, fileName)

				Using fileStream = New FileStream(filePathName, FileMode.Create)
					Await file.CopyToAsync(fileStream)
				End Using
			Catch e As Exception
				Return e.Message
			End Try

			Return fileName
		End Function

		Public Sub DeleteImageTempFile(filePathName As String) Implements IImageFileWriter.DeleteImageTempFile
			Try
				File.Delete(filePathName)
			Catch e As Exception
				Throw e
			End Try
		End Sub

	End Class
End Namespace
