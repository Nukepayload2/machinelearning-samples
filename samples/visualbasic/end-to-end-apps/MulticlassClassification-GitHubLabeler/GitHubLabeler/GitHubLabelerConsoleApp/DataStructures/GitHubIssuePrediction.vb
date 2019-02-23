Imports Microsoft.ML.Data

Namespace GitHubLabeler.DataStructures
    Friend Class GitHubIssuePrediction
        <ColumnName("PredictedLabel")>
        Public Area As String

        Public Score As Single()
    End Class
End Namespace
