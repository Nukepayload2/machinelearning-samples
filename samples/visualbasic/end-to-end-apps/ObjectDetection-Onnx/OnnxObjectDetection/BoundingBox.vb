Imports System.Drawing

Namespace OnnxObjectDetection
	Public Class BoundingBoxDimensions
		Public Property X As Single
		Public Property Y As Single
		Public Property Height As Single
		Public Property Width As Single
	End Class

	Public Class BoundingBox
		Public Property Dimensions As BoundingBoxDimensions

		Public Property Label As String

		Public Property Confidence As Single

		Public ReadOnly Property Rect As RectangleF
			Get
				Return New RectangleF(Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height)
			End Get
		End Property

		Public Property BoxColor As Color

		Public ReadOnly Property Description As String
			Get
				Return $"{Label} ({(Confidence * 100).ToString("0")}%)"
			End Get
		End Property

		Private Shared ReadOnly classColors() As Color = { Color.Khaki, Color.Fuchsia, Color.Silver, Color.RoyalBlue, Color.Green, Color.DarkOrange, Color.Purple, Color.Gold, Color.Red, Color.Aquamarine, Color.Lime, Color.AliceBlue, Color.Sienna, Color.Orchid, Color.Tan, Color.LightPink, Color.Yellow, Color.HotPink, Color.OliveDrab, Color.SandyBrown, Color.DarkTurquoise }

		Public Shared Function GetColor(index As Integer) As Color
			Return If(index < classColors.Length, classColors(index), classColors(index Mod classColors.Length))
		End Function
	End Class
End Namespace