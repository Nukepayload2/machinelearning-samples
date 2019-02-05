Imports Microsoft.ML.Data

Namespace MulticlassClassification_Iris.DataStructures
    Public Class IrisData
        <LoadColumn(0)>
        Public Label As Single

        <LoadColumn(1)>
        Public SepalLength As Single

        <LoadColumn(2)>
        Public SepalWidth As Single

        <LoadColumn(3)>
        Public PetalLength As Single

        <LoadColumn(4)>
        Public PetalWidth As Single
    End Class
End Namespace