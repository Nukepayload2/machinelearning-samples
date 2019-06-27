Imports Microsoft.ML.Transforms.Image
Imports System.Drawing

Namespace OnnxObjectDetectionE2EAPP
	Public Class ImageInputData
		<ImageType(416, 416)>
		Public Property Image As Bitmap
	End Class
End Namespace
