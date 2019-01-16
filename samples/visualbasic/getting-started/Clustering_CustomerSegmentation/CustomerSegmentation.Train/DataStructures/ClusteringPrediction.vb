Imports Microsoft.ML.Data

Namespace CustomerSegmentation.DataStructures
    Public Class ClusteringPrediction
        <ColumnName("PredictedLabel")>
        Public SelectedClusterId As UInteger
        <ColumnName("Score")>
        Public Distance() As Single
        <ColumnName("PCAFeatures")>
        Public Location() As Single
        <ColumnName("LastName")>
        Public LastName As String
    End Class
End Namespace
