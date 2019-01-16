Imports Microsoft.ML.Data

Namespace SentimentAnalysisConsoleApp.DataStructures
    Public Class SentimentPrediction
        ' ColumnName attribute is used to change the column name from
        ' its default value, which is the name of the field.
        <ColumnName("PredictedLabel")>
        Public Property Prediction As Boolean

        ' No need to specify ColumnName attribute, because the field
        ' name "Probability" is the column name we want.
        Public Property Probability As Single

        Public Property Score As Single
    End Class
End Namespace