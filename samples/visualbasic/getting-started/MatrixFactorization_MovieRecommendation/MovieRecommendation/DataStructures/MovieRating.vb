Imports Microsoft.ML.Data

Namespace MovieRecommendationConsoleApp.DataStructures
    Public Class MovieRating
        <LoadColumn(0)>
        Public userId As Single

        <LoadColumn(1)>
        Public movieId As Single

        <LoadColumn(2)>
        Public Label As Single
    End Class
End Namespace
