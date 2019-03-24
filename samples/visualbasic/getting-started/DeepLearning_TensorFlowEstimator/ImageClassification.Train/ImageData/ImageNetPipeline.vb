Namespace ImageClassification.ImageData
    Public Class ImageNetPipeline
        Public ImagePath As String
        Public Label As String
        Public PredictedLabelValue As String
        Public Score() As Single
        Public softmax2_pre_activation() As Single
    End Class
End Namespace
