Namespace OnnxObjectDetection
	Public Class TinyYoloModel
		Implements IOnnxModel

		Private privateModelPath As String
		Public Property ModelPath As String Implements IOnnxModel.ModelPath
			Get
				Return privateModelPath
			End Get
			Private Set(ByVal value As String)
				privateModelPath = value
			End Set
		End Property

		Public ReadOnly Property ModelInput As String Implements IOnnxModel.ModelInput = "image"
		Public ReadOnly Property ModelOutput As String Implements IOnnxModel.ModelOutput = "grid"

		Public ReadOnly Property Labels As String() Implements IOnnxModel.Labels = { "aeroplane", "bicycle", "bird", "boat", "bottle", "bus", "car", "cat", "chair", "cow", "diningtable", "dog", "horse", "motorbike", "person", "pottedplant", "sheep", "sofa", "train", "tvmonitor" }

'INSTANT VB TODO TASK: The following line could not be converted:
		public (Single,Single)() Anchors
		If True Then
			Get
		} = { (1.08F,1.19F), (3.42F,4.41F), (6.63F,11.38F), (9.42F,5.11F), (16.62F,10.52F) }

'INSTANT VB TODO TASK: Local functions are not converted by Instant VB:
'		public TinyYoloModel(string modelPath)
'		{
'			ModelPath = modelPath;
'		}
			End Get
		End If
