Imports Microsoft.ML.Transforms.Image
Imports System.Drawing

Namespace OnnxObjectDetection
	Public Structure ImageSettings
		Public Const imageHeight As Integer = 416
		Public Const imageWidth As Integer = 416
	End Structure

	Public Class ImageInputData
		<ImageType(ImageSettings.imageHeight, ImageSettings.imageWidth)>
		Public Property Image As Bitmap
	End Class
End Namespace
