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

        Public ReadOnly Property ModelInput As String = "data" Implements IOnnxModel.ModelInput
        Public ReadOnly Property ModelOutput As String = "model_outputs0" Implements IOnnxModel.ModelOutput

        Private privateLabels As String()
        Public Property Labels As String() Implements IOnnxModel.Labels
            Get
                Return privateLabels
            End Get
            Private Set(ByVal value As String())
                privateLabels = value
            End Set
        End Property

        Public Sub New(ByVal modelPath As String)
            Dim extractPath = Path.GetFullPath(modelPath.Replace(".zip", Path.DirectorySeparatorChar.ToString()))
            If Not Directory.Exists(extractPath) Then Directory.CreateDirectory(extractPath)
            modelPath = Path.GetFullPath(Path.Combine(extractPath, modelName))
            labelsPath = Path.GetFullPath(Path.Combine(extractPath, labelsName))
            If Not File.Exists(modelPath) OrElse Not File.Exists(labelsPath) Then ExtractArchive(modelPath)
            Labels = File.ReadAllLines(labelsPath)
        End Sub

        Public ReadOnly Property Anchors As (Single, Single)() = {
            (0.573F, 0.677F), (1.87F, 2.06F), (3.34F, 5.47F),
            (7.88F, 3.53F), (9.77F, 9.17F)
        } Implements IOnnxModel.Anchors

        Private Sub ExtractArchive(ByVal modelPath As String)
            Using archive As ZipArchive = ZipFile.OpenRead(modelPath)
                Dim modelEntry = If(archive.Entries.FirstOrDefault(
                    Function(e) e.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase)),
                    CSharp.ThrowExpression(Of ZipArchiveEntry)(New FormatException("The exported .zip archive is missing the model.onnx file")))
                modelEntry.ExtractToFile(modelPath)
                Dim labelsEntry = If(archive.Entries.FirstOrDefault(
                    Function(e) e.Name.Equals(labelsName, StringComparison.OrdinalIgnoreCase)),
                    CSharp.ThrowExpression(Of ZipArchiveEntry)(New FormatException("The exported .zip archive is missing the labels.txt file")))
                labelsEntry.ExtractToFile(labelsPath)
            End Using
        End Sub

        Private Class CSharp
            Shared Function ThrowExpression(Of T)(ByVal e As Exception) As T
                Throw e
            End Function
        End Class
    End Class
End Namespace
