Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Linq

Namespace OnnxObjectDetection
	Public Class CustomVisionModel
		Implements IOnnxModel

		Private Const modelName As String = "model.onnx", labelsName As String = "labels.txt"

		Private ReadOnly labelsPath As String

		Private privateModelPath As String
		Public Property ModelPath As String Implements IOnnxModel.ModelPath
			Get
				Return privateModelPath
			End Get
			Private Set(ByVal value As String)
				privateModelPath = value
			End Set
		End Property

		Public ReadOnly Property ModelInput As String Implements IOnnxModel.ModelInput = "data"
		Public ReadOnly Property ModelOutput As String Implements IOnnxModel.ModelOutput = "model_outputs0"

		Private privateLabels As String()
		Public Property Labels As String() Implements IOnnxModel.Labels
			Get
				Return privateLabels
			End Get
			Private Set(ByVal value As String())
				privateLabels = value
			End Set
		End Property
'INSTANT VB TODO TASK: The following line could not be converted:
		public (Single,Single)() Anchors
		If True Then
			Get
		} = { (0.573F,0.677F), (1.87F,2.06F), (3.34F,5.47F), (7.88F,3.53F), (9.77F,9.17F) }

'INSTANT VB TODO TASK: Local functions are not converted by Instant VB:
'		public CustomVisionModel(string modelPath)
'		{
'			var extractPath = Path.GetFullPath(modelPath.Replace(".zip", Path.DirectorySeparatorChar.ToString()));
'
'			if (!Directory.Exists(extractPath))
'				Directory.CreateDirectory(extractPath);
'
'			ModelPath = Path.GetFullPath(Path.Combine(extractPath, modelName));
'			labelsPath = Path.GetFullPath(Path.Combine(extractPath, labelsName));
'
'			if (!File.Exists(ModelPath) || !File.Exists(labelsPath))
'				ExtractArchive(modelPath);
'
'			Labels = File.ReadAllLines(labelsPath);
'		}

'INSTANT VB TODO TASK: Local functions are not converted by Instant VB:
'		void ExtractArchive(string modelPath)
'		{
'			using (ZipArchive archive = ZipFile.OpenRead(modelPath))
'			{
'				var modelEntry = archive.Entries.FirstOrDefault(e => e.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase)) ?? throw New FormatException("The exported .zip archive is missing the model.onnx file");
'
'				modelEntry.ExtractToFile(ModelPath);
'
'				var labelsEntry = archive.Entries.FirstOrDefault(e => e.Name.Equals(labelsName, StringComparison.OrdinalIgnoreCase)) ?? throw New FormatException("The exported .zip archive is missing the labels.txt file");
'
'				labelsEntry.ExtractToFile(labelsPath);
'			}
'		}
			End Get
		End If
