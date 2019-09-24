Imports System.Drawing

Namespace ObjectDetection.YoloParser
	Public Class BoundingBoxDimensions
		Inherits DimensionsBase

	End Class

	Public Class YoloBoundingBox
		Public Property Dimensions As BoundingBoxDimensions

		Public Property Label As String

		Public Property Confidence As Single

		Public ReadOnly Property Rect As RectangleF
			Get
				Return New RectangleF(Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height)
			End Get
		End Property

		Public Property BoxColor As Color
	End Class

End Namespace