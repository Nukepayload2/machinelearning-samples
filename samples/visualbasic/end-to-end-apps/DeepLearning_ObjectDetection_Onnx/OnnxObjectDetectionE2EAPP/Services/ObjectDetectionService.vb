Imports Microsoft.Extensions.ML
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace OnnxObjectDetectionE2EAPP.Services
	Public Interface IObjectDetectionService
		Sub DetectObjectsUsingModel(imageInputData As ImageInputData)
		Function PaintImages(imageFilePath As String) As Image
	End Interface
	Public Class ObjectDetectionService
		Implements IObjectDetectionService

		Private ReadOnly _parser As YoloWinMlParser = New YoloWinMlParser
		Private filteredBoxes As IList(Of YoloBoundingBox)
		Private ReadOnly model As PredictionEnginePool(Of ImageInputData, ImageObjectPrediction)

		Public Sub New(model As PredictionEnginePool(Of ImageInputData, ImageObjectPrediction))
			Me.model = model
		End Sub

		Public Sub DetectObjectsUsingModel(imageInputData As ImageInputData) Implements IObjectDetectionService.DetectObjectsUsingModel
			Dim probs = model.Predict(imageInputData).PredictedLabels
			Dim boundingBoxes As IList(Of YoloBoundingBox) = _parser.ParseOutputs(probs)
			filteredBoxes = _parser.NonMaxSuppress(boundingBoxes, 5, .5F)
		End Sub

		Public Function PaintImages(imageFilePath As String) As Image Implements IObjectDetectionService.PaintImages
			Dim image As Image = Image.FromFile(imageFilePath)
			Dim originalHeight = image.Height
			Dim originalWidth = image.Width
			For Each box In filteredBoxes
				'// process output boxes
				Dim x = CUInt(Math.Truncate(Math.Max(box.X, 0)))
				Dim y = CUInt(Math.Truncate(Math.Max(box.Y, 0)))
				Dim w = CUInt(Math.Min(originalWidth - x, box.Width))
				Dim h = CUInt(Math.Min(originalHeight - y, box.Height))

				' fit to current image size
				x = CUInt(originalWidth) * x \ 416
				y = CUInt(originalHeight) * y \ 416
				w = CUInt(originalWidth) * w \ 416
				h = CUInt(originalHeight) * h \ 416

				Dim text As String = String.Format("{0} ({1})", box.Label, box.Confidence)

				Using graph As Graphics = Graphics.FromImage(image)
					graph.CompositingQuality = CompositingQuality.HighQuality
					graph.SmoothingMode = SmoothingMode.HighQuality
					graph.InterpolationMode = InterpolationMode.HighQualityBicubic

					Dim drawFont As New Font("Arial", 16)
					Dim redBrush As New SolidBrush(Color.Red)
					Dim atPoint As New Point(CInt(x), CInt(y))
					Dim pen As New Pen(Color.Yellow, 4.0F)
					Dim yellowBrush As New SolidBrush(Color.Yellow)

					' Fill rectangle on which the text is displayed.
					Dim rect As New RectangleF(x, y, w, 20)
					graph.FillRectangle(yellowBrush, rect)
					'draw text in red color
					graph.DrawString(text, drawFont, redBrush, atPoint)
					'draw rectangle around object
					graph.DrawRectangle(pen, x, y, w, h)
				End Using
			Next box
			Return image
		End Function
	End Class
End Namespace
