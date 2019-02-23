Namespace GitHubLabeler.DataStructures
    Public Class FullPrediction
        Public PredictedLabel As String
        Public Score As Single
        Public OriginalSchemaIndex As Integer

        Public Sub New(ByVal predictedLabel As String, ByVal score As Single, ByVal originalSchemaIndex As Integer)
            Me.PredictedLabel = predictedLabel
            Me.Score = score
            Me.OriginalSchemaIndex = originalSchemaIndex
        End Sub
    End Class
End Namespace
