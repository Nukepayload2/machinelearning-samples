Imports System.Drawing

Namespace OnnxObjectDetectionE2EAPP
	Friend Class YoloBoundingBox
		Public Property Label As String
		Public Property X As Single
		Public Property Y As Single
		Public Property Height As Single
		Public Property Width As Single
		Public Property Confidence As Single

		Public ReadOnly Property Rect As RectangleF
			Get
				Return New RectangleF(X, Y, Width, Height)
			End Get
		End Property
	End Class
End Namespace