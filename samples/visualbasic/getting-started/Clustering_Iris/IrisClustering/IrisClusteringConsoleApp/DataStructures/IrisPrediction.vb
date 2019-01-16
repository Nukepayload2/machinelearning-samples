Imports Microsoft.ML.Data

Namespace Clustering_Iris.DataStructures
    ' IrisPrediction is the result returned from prediction operations
    Public Class IrisPrediction
        <ColumnName("PredictedLabel")>
        Public SelectedClusterId As UInteger

        <ColumnName("Score")>
        Public Distance() As Single
    End Class
End Namespace
