Imports Microsoft.ML.Transforms.Image
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Linq

Namespace TensorFlowImageClassification.ML.DataModels
	Public Class ImageInputData
		<ImageType(227, 227)>
		Public Property Image As Bitmap
	End Class
End Namespace
