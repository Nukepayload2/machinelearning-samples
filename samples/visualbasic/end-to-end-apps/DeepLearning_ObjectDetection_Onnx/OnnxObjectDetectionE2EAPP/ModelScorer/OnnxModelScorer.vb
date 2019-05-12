Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Linq
Imports Microsoft.ML
Imports Microsoft.ML.Transforms.Image

Namespace OnnxObjectDetectionE2EAPP.OnnxModelScorers
	Public Interface IOnnxModelScorer
		Sub DetectObjectsUsingModel(imagesFilePath As String)
		Function CreatePredictionEngine(imagesFolder As String, modelLocation As String) As PredictionEngine(Of ImageNetData, ImageNetPrediction)
		Function PaintImages(imageFilePath As String) As Image
	End Interface

	Public Class OnnxModelScorer
		Implements IOnnxModelScorer

		Private ReadOnly _imagesLocation As String
		Private ReadOnly _imagesTmpFolder As String
		Private ReadOnly _modelLocation As String
		Private ReadOnly _mlContext As MLContext
		Private filteredBoxes As IList(Of YoloBoundingBox)

		Private _boxes As IList(Of YoloBoundingBox) = New List(Of YoloBoundingBox)
		Private ReadOnly _parser As YoloWinMlParser = New YoloWinMlParser

#Disable Warning BCIDE0032
		Private ReadOnly _predictionEngine As PredictionEngine(Of ImageNetData, ImageNetPrediction)
		Public ReadOnly Property PredictionEngine As PredictionEngine(Of ImageNetData, ImageNetPrediction)
			Get
				Return _predictionEngine
			End Get
		End Property

		Public Sub New()
			Dim assetsPath = ModelHelpers.GetAbsolutePath("../../../Model")
			_imagesTmpFolder = ModelHelpers.GetAbsolutePath("../../../tempImages")
			_modelLocation = Path.Combine(assetsPath, "TinyYolo2_model.onnx")
			'_modelLocation = Path.Combine(assetsPath, "yolov3.onnx");

			_mlContext = New MLContext

			' Create the prediction function in the constructor, once, as it is an expensive operation
			' Note that, on average, this call takes around 200x longer than one prediction, so you want to cache it
			' and reuse the prediction function, instead of creating one per prediction.
			' IMPORTANT: Remember that the 'Predict()' method is not reentrant. 
			' If you want to use multiple threads for simultaneous prediction, 
			' make sure each thread is using its own PredictionFunction (e.g. In DI/IoC use .AddScoped())
			' or use a critical section when using the Predict() method.
			_predictionEngine = Me.CreatePredictionEngine(_imagesTmpFolder, _modelLocation)
		End Sub

		Public Structure ImageNetSettings
			Public Const imageHeight As Integer = 416
			Public Const imageWidth As Integer = 416
		End Structure

		Public Structure TinyYoloModelSettings
			' for checking TIny yolo2 Model input and  output  parameter names,
			'you can use tools like Netron, 
			' which is installed by Visual Studio AI Tools

			' input tensor name
			Public Const ModelInput As String = "image"

			' output tensor name
			Public Const ModelOutput As String = "grid"
		End Structure

		Public Function CreatePredictionEngine(imagesFolder As String, modelLocation As String) As PredictionEngine(Of ImageNetData, ImageNetPrediction) Implements IOnnxModelScorer.CreatePredictionEngine
			Console.WriteLine("Read model")
			Console.WriteLine($"Model location: {modelLocation}")
			Console.WriteLine($"Images folder: {imagesFolder}")
			Console.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight})")

			Dim dataView = CreateDataView()

			Dim pipeline = _mlContext.Transforms.LoadImages(outputColumnName:= "image", imageFolder:= imagesFolder, inputColumnName:= NameOf(ImageNetData.ImagePath)).Append(_mlContext.Transforms.ResizeImages(resizing:= ImageResizingEstimator.ResizingKind.Fill, outputColumnName:= "image", imageWidth:= ImageNetSettings.imageWidth, imageHeight:= ImageNetSettings.imageHeight, inputColumnName:= "image")).Append(_mlContext.Transforms.ExtractPixels(outputColumnName:= "image")).Append(_mlContext.Transforms.ApplyOnnxModel(modelFile:= modelLocation, outputColumnNames:= { TinyYoloModelSettings.ModelOutput }, inputColumnNames:= { TinyYoloModelSettings.ModelInput }))

			Dim model = pipeline.Fit(dataView)

'INSTANT VB NOTE: The variable predictionEngine was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim predictionEngine_Renamed = _mlContext.Model.CreatePredictionEngine(Of ImageNetData, ImageNetPrediction)(model)

			Return predictionEngine_Renamed
		End Function

		Public Sub DetectObjectsUsingModel(imagesFilePath As String) Implements IOnnxModelScorer.DetectObjectsUsingModel
			Dim imageInputData = New ImageNetData With {.ImagePath = imagesFilePath}
			Dim probs = _predictionEngine.Predict(imageInputData).PredictedLabels
			Dim boundingBoxes As IList(Of YoloBoundingBox) = _parser.ParseOutputs(probs)
			filteredBoxes = _parser.NonMaxSuppress(boundingBoxes, 5, .5F)
		End Sub

		Public Function PaintImages(imageFilePath As String) As Image Implements IOnnxModelScorer.PaintImages
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

			Dim outputImagePath As String = ModelHelpers.GetAbsolutePath("../../../Output/outputImage.jpg")
			image.Save(outputImagePath)
			Return image
		End Function

		Private Function CreateDataView() As IDataView
			'Create empty DataView. We just need the schema to call fit()
			Dim list As New List(Of ImageNetData)
			'list.Add(new ImageInputData() { ImagePath = "image-name.jpg" });   //Since we just need the schema, no need to provide anything here
			Dim enumerableData As IEnumerable(Of ImageNetData) = list
			Dim dv = _mlContext.Data.LoadFromEnumerable(enumerableData)
			Return dv
		End Function
	End Class
End Namespace

