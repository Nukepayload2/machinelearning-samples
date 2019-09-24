Namespace MulticlassClassification_Iris.DataStructures
    Public Class SampleIrisData
        Friend Shared ReadOnly Iris1 As IrisData = New IrisData With {
            .SepalLength = 5.1F,
            .SepalWidth = 3.3F,
            .PetalLength = 1.6F,
            .PetalWidth = 0.2F
        }

        Friend Shared ReadOnly Iris2 As IrisData = New IrisData With {
            .SepalLength = 6.0F,
            .SepalWidth = 3.4F,
            .PetalLength = 6.1F,
            .PetalWidth = 2.0F
        }

        Friend Shared ReadOnly Iris3 As IrisData = New IrisData With {
            .SepalLength = 4.4F,
            .SepalWidth = 3.1F,
            .PetalLength = 2.5F,
            .PetalWidth = 1.2F
        }
    End Class
End Namespace