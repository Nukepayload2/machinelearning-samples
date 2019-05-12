Imports System.Linq
Imports System.Text

Namespace OnnxObjectDetectionE2EAPP.Infrastructure
	Public Module ImageValidationExtensions
		<System.Runtime.CompilerServices.Extension> _
		Public Function IsValidImage(image() As Byte) As Boolean
			Dim imageFormat = GetImageFormat(image)
			Return imageFormat = OnnxObjectDetectionE2EAPP.Infrastructure.ImageValidationExtensions.ImageFormat.jpeg OrElse imageFormat = OnnxObjectDetectionE2EAPP.Infrastructure.ImageValidationExtensions.ImageFormat.png
		End Function

		Public Enum ImageFormat
			bmp
			jpeg
			gif
			tiff
			png
			unknown
		End Enum

		Public Function GetImageFormat(bytes() As Byte) As ImageFormat
			' see http://www.mikekunz.com/image_file_header.html  
			Dim bmp = Encoding.ASCII.GetBytes("BM") ' BMP
			Dim gif = Encoding.ASCII.GetBytes("GIF") ' GIF
			Dim png = New Byte() { 137, 80, 78, 71 } ' PNG
			Dim tiff = New Byte() { 73, 73, 42 } ' TIFF
			Dim tiff2 = New Byte() { 77, 77, 42 } ' TIFF
			Dim jpeg = New Byte() { 255, 216, 255, 224 } ' jpeg
			Dim jpeg2 = New Byte() { 255, 216, 255, 225 } ' jpeg canon
			Dim jpg1 = New Byte() { 255, 216, 255, 219 }
			Dim jpg2 = New Byte() { 255, 216, 255, 226 }

			If bmp.SequenceEqual(bytes.Take(bmp.Length)) Then
				Return ImageFormat.bmp
			End If

			If gif.SequenceEqual(bytes.Take(gif.Length)) Then
				Return ImageFormat.gif
			End If

			If png.SequenceEqual(bytes.Take(png.Length)) Then
				Return ImageFormat.png
			End If

			If tiff.SequenceEqual(bytes.Take(tiff.Length)) Then
				Return ImageFormat.tiff
			End If

			If tiff2.SequenceEqual(bytes.Take(tiff2.Length)) Then
				Return ImageFormat.tiff
			End If

			If jpeg.SequenceEqual(bytes.Take(jpeg.Length)) OrElse jpg1.SequenceEqual(bytes.Take(jpg1.Length)) OrElse jpg2.SequenceEqual(bytes.Take(jpg2.Length)) Then
				Return ImageFormat.jpeg
			End If

			If jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)) Then
				Return ImageFormat.jpeg
			End If

			Return ImageFormat.unknown
		End Function
	End Module
End Namespace
