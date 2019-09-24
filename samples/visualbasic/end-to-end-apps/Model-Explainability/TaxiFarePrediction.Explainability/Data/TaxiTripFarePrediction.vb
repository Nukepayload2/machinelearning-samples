Imports Microsoft.ML
Imports Microsoft.ML.Data

Namespace TaxiFareRegression.DataStructures
    Public Class TaxiTripFarePrediction
        <ColumnName("Score")>
        Public FareAmount As Single
    End Class

    Public Class TaxiTripFarePredictionWithContribution
        Inherits TaxiTripFarePrediction

        Public Property FeatureContributions As Single()

        Public Function GetFeatureContributions(dataview As DataViewSchema) As List(Of FeatureContribution)
            'base.PrintToConsole();
            Dim slots As VBuffer(Of ReadOnlyMemory(Of Char)) = Nothing
            dataview.GetColumnOrNull("Features").Value.GetSlotNames(slots)
            Dim featureNames = slots.DenseValues().ToArray()
            Dim featureList As New List(Of FeatureContribution)
            For i As Integer = 0 To featureNames.Count() - 1
                Dim featureName As String = featureNames(i).ToString()
                If featureName = "PassengerCount" OrElse featureName = "TripTime" OrElse featureName = "TripDistance" Then
                    featureList.Add(New FeatureContribution(featureName, FeatureContributions(i)))
                End If
            Next i

            Return featureList

        End Function
    End Class

    Public Class TaxiFarePrediction
        Inherits TaxiTripFarePredictionWithContribution

        Public Property Features As List(Of FeatureContribution)

        Public Sub New(PredictedFareAmount As Single, Features As List(Of FeatureContribution))
            Me.FareAmount = PredictedFareAmount
            Me.Features = Features
        End Sub
    End Class

    Public Class FeatureContribution
        Public Name As String
        Public Value As Single

        Public Sub New(Name As String, Value As Single)
            Me.Name = Name
            Me.Value = Value
        End Sub
    End Class
End Namespace