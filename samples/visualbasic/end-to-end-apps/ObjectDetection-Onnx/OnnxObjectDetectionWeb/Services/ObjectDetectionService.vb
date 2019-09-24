Imports Microsoft.Extensions.ML
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports OnnxObjectDetection

Namespace OnnxObjectDetectionWeb.Services
	Public Interface IObjectDetectionService
		Sub DetectObjectsUsingModel(imageInputData As ImageInputData)
		Function DrawBoundingBox(imageFilePath As String) As Image
	End Interface

	Public Class ObjectDetectionService
		Implements IObjectDetectionService

		Private filteredBoxes As List(Of BoundingBox)
		Private ReadOnly outputParser As New OnnxOutputParser(New TinyYoloModel(Nothing))
		Private ReadOnly predictionEngine As PredictionEnginePool(Of ImageInputData, TinyYoloPrediction)

		Public Sub New(predictionEngine As PredictionEnginePool(Of ImageInputData, TinyYoloPrediction))
			Me.predictionEngine = predictionEngine
		End Sub

		Public Sub DetectObjectsUsingModel(imageInputData As ImageInputData) Implements IObjectDetectionService.DetectObjectsUsingModel
			Dim probs = predictionEngine.Predict(imageInputData).PredictedLabels
			Dim boundingBoxes As List(Of BoundingBox) = outputParser.ParseOutputs(probs)
			filteredBoxes = outputParser.FilterBoundingBoxes(boundingBoxes, 5, .5F)
		End Sub

		Public Function DrawBoundingBox(imageFilePath As String) As Image Implements IObjectDetectionService.DrawBoundingBox
			Dim image As Image = Image.FromFile(imageFilePath)
			Dim originalHeight = image.Height
			Dim originalWidth = image.Width
			For Each box In filteredBoxes
				'// process output boxes
				Dim x = CUInt(Math.Truncate(Math.Max(box.Dimensions.X, 0)))
				Dim y = CUInt(Math.Truncate(Math.Max(box.Dimensions.Y, 0)))
				Dim width = CUInt(Math.Min(originalWidth - x, box.Dimensions.Width))
				Dim height = CUInt(Math.Min(originalHeight - y, box.Dimensions.Height))

				' fit to current image size
				x = CUInt(originalWidth) * x \ ImageSettings.imageWidth
				y = CUInt(originalHeight) * y \ ImageSettings.imageHeight
				width = CUInt(originalWidth) * width \ ImageSettings.imageWidth
				height = CUInt(originalHeight) * height \ ImageSettings.imageHeight

				Using thumbnailGraphic As Graphics = Graphics.FromImage(image)
					thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality
					thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality
					thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic

					' Define Text Options
					Dim drawFont As New Font("Arial", 12, FontStyle.Bold)
					Dim size As SizeF = thumbnailGraphic.MeasureString(box.Description, drawFont)
					Dim fontBrush As New SolidBrush(Color.Black)
					Dim atPoint As New Point(CInt(x), CInt(y) - CInt(size.Height) - 1)

					' Define BoundingBox options
					Dim pen As New Pen(box.BoxColor, 3.2F)
					Dim colorBrush As New SolidBrush(box.BoxColor)

					' Draw text on image 
					thumbnailGraphic.FillRectangle(colorBrush, CInt(x), CInt(y - size.Height - 1), CInt(size.Width), CInt(size.Height))
					thumbnailGraphic.DrawString(box.Description, drawFont, fontBrush, atPoint)

					' Draw bounding box on image
					thumbnailGraphic.DrawRectangle(pen, x, y, width, height)
				End Using
			Next box
			Return image
		End Function
	End Class
End Namespace
