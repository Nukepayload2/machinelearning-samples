Imports Microsoft.ML
Imports OnnxObjectDetection
Imports OpenCvSharp
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Rectangle = System.Windows.Shapes.Rectangle

Namespace OnnxObjectDetectionApp
	Partial Public Class MainWindow
		Inherits System.Windows.Window

		Private capture As VideoCapture
		Private cameraCaptureCancellationTokenSource As CancellationTokenSource

		Private outputParser As OnnxOutputParser
		Private tinyYoloPredictionEngine As PredictionEngine(Of ImageInputData, TinyYoloPrediction)
		Private customVisionPredictionEngine As PredictionEngine(Of ImageInputData, CustomVisionPrediction)

		Private Shared ReadOnly modelsDirectory As String = Path.Combine(Environment.CurrentDirectory, "ML\OnnxModels")

		Public Sub New()
			InitializeComponent()
			LoadModel()
		End Sub

		Protected Overrides Sub OnActivated(e As EventArgs)
			MyBase.OnActivated(e)
			StartCameraCapture()
		End Sub

		Protected Overrides Sub OnDeactivated(e As EventArgs)
			MyBase.OnDeactivated(e)
			StopCameraCapture()
		End Sub

		Private Sub LoadModel()
			' Check for an Onnx model exported from Custom Vision
			Dim customVisionExport = Directory.GetFiles(modelsDirectory, "*.zip").FirstOrDefault()

			' If there is one, use it.
			If customVisionExport IsNot Nothing Then
				Dim customVisionModel = New CustomVisionModel(customVisionExport)
				Dim modelConfigurator = New OnnxModelConfigurator(customVisionModel)

				outputParser = New OnnxOutputParser(customVisionModel)
				customVisionPredictionEngine = modelConfigurator.GetMlNetPredictionEngine(Of CustomVisionPrediction)()
			Else ' Otherwise default to Tiny Yolo Onnx model
				Dim tinyYoloModel = New TinyYoloModel(Path.Combine(modelsDirectory, "TinyYolo2_model.onnx"))
				Dim modelConfigurator = New OnnxModelConfigurator(tinyYoloModel)

				outputParser = New OnnxOutputParser(tinyYoloModel)
				tinyYoloPredictionEngine = modelConfigurator.GetMlNetPredictionEngine(Of TinyYoloPrediction)()
			End If
		End Sub

		Private Sub StartCameraCapture()
			cameraCaptureCancellationTokenSource = New CancellationTokenSource
			Task.Run(Function() CaptureCamera(cameraCaptureCancellationTokenSource.Token), cameraCaptureCancellationTokenSource.Token)
		End Sub

		Private Sub StopCameraCapture()
			cameraCaptureCancellationTokenSource?.Cancel()
		End Sub

		Private Async Function CaptureCamera(token As CancellationToken) As Task
			If capture Is Nothing Then
				capture = New VideoCapture(CaptureDevice.DShow)
			End If

			capture.Open(0)

			If capture.IsOpened() Then
				Do While Not token.IsCancellationRequested
					Using MemoryStream memoryStream = capture.RetrieveMat 
						.Flip(FlipMode.Y).ToMemoryStream()
					End Using

					Await Application.Current.Dispatcher.InvokeAsync(Sub()
						Dim imageSource = New BitmapImage

						imageSource.BeginInit()
						imageSource.CacheOption = BitmapCacheOption.OnLoad
						imageSource.StreamSource = memoryStream
						imageSource.EndInit()

						WebCamImage.Source = imageSource
					End Sub)

					Dim bitmapImage = New Bitmap(memoryStream)

					Await ParseWebCamFrame(bitmapImage, token)
				Loop

				capture.Release()
			End If
		End Function

		Private Async Function ParseWebCamFrame(bitmap As Bitmap, token As CancellationToken) As Task
			If customVisionPredictionEngine Is Nothing AndAlso tinyYoloPredictionEngine Is Nothing Then
				Return
			End If

			Dim frame = New ImageInputData With {.Image = bitmap}
			Dim filteredBoxes = DetectObjectsUsingModel(frame)

			If Not token.IsCancellationRequested Then
				Await Application.Current.Dispatcher.InvokeAsync(Sub()
					DrawOverlays(filteredBoxes, WebCamImage.ActualHeight, WebCamImage.ActualWidth)
				End Sub)
			End If
		End Function

		Public Function DetectObjectsUsingModel(imageInputData As ImageInputData) As List(Of BoundingBox)
			Dim labels = If(customVisionPredictionEngine?.Predict(imageInputData).PredictedLabels, tinyYoloPredictionEngine?.Predict(imageInputData).PredictedLabels)
			Dim boundingBoxes = outputParser.ParseOutputs(labels)
			Dim filteredBoxes = outputParser.FilterBoundingBoxes(boundingBoxes, 5, 0.5F)
			Return filteredBoxes
		End Function

		Private Sub DrawOverlays(filteredBoxes As List(Of BoundingBox), originalHeight As Double, originalWidth As Double)
			WebCamCanvas.Children.Clear()

			For Each box In filteredBoxes
				' process output boxes
				Dim x As Double = Math.Max(box.Dimensions.X, 0)
				Dim y As Double = Math.Max(box.Dimensions.Y, 0)
				Dim width As Double = Math.Min(originalWidth - x, box.Dimensions.Width)
				Dim height As Double = Math.Min(originalHeight - y, box.Dimensions.Height)

				' fit to current image size
				x = originalWidth * x / ImageSettings.imageWidth
				y = originalHeight * y / ImageSettings.imageHeight
				width = originalWidth * width / ImageSettings.imageWidth
				height = originalHeight * height / ImageSettings.imageHeight

				Dim boxColor = box.BoxColor.ToMediaColor()

				Dim objBox = New Rectangle With {
					.Width = width,
					.Height = height,
					.Fill = New SolidColorBrush(Colors.Transparent),
					.Stroke = New SolidColorBrush(boxColor),
					.StrokeThickness = 2.0,
					.Margin = New Thickness(x, y, 0, 0)
				}

				Dim objDescription = New TextBlock With {
					.Margin = New Thickness(x + 4, y + 4, 0, 0),
					.Text = box.Description,
					.FontWeight = FontWeights.Bold,
					.Width = 126,
					.Height = 21,
					.TextAlignment = TextAlignment.Center
				}

				Dim objDescriptionBackground = New Rectangle With {
					.Width = 134,
					.Height = 29,
					.Fill = New SolidColorBrush(boxColor),
					.Margin = New Thickness(x, y, 0, 0)
				}

				WebCamCanvas.Children.Add(objDescriptionBackground)
				WebCamCanvas.Children.Add(objDescription)
				WebCamCanvas.Children.Add(objBox)
			Next box
		End Sub
	End Class

	Friend Module ColorExtensions
		<System.Runtime.CompilerServices.Extension> _
		Friend Function ToMediaColor(drawingColor As System.Drawing.Color) As System.Windows.Media.Color
			Return System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B)
		End Function
	End Module
End Namespace
