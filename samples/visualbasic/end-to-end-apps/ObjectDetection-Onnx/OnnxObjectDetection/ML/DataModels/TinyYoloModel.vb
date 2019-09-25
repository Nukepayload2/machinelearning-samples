Namespace OnnxObjectDetection
    Public Class TinyYoloModel
        Implements IOnnxModel

        Private privateModelPath As String

        Public Sub New(privateModelPath As String)
            Me.privateModelPath = privateModelPath
        End Sub

        Public Property ModelPath As String Implements IOnnxModel.ModelPath
            Get
                Return privateModelPath
            End Get
            Private Set(ByVal value As String)
                privateModelPath = value
            End Set
        End Property

        Public ReadOnly Property ModelInput As String = "image" Implements IOnnxModel.ModelInput
        Public ReadOnly Property ModelOutput As String = "grid" Implements IOnnxModel.ModelOutput

        Public ReadOnly Property Labels As String() = {
            "aeroplane", "bicycle", "bird", "boat", "bottle", "bus", "car",
            "cat", "chair", "cow", "diningtable", "dog", "horse", "motorbike",
            "person", "pottedplant", "sheep", "sofa", "train", "tvmonitor"
        } Implements IOnnxModel.Labels

        Public ReadOnly Property Anchors As (Single, Single)() = {
            (1.08F, 1.19F), (3.42F, 4.41F), (6.63F, 11.38F), (9.42F, 5.11F), (16.62F, 10.52F)
        } Implements IOnnxModel.Anchors

    End Class
End Namespace
