Imports Microsoft.ML.Data

Namespace BikeSharingDemand.DataStructures
    Public Class DemandObservation
        ' Note that we're loading only some columns (certain indexes) starting on column number 2
        ' Also, the label column is number 16. 
        ' Columns 14, 15 are not being loaded from the file.
        <LoadColumn(2)>
        Public Property Season As Single
        <LoadColumn(3)>
        Public Property Year As Single
        <LoadColumn(4)>
        Public Property Month As Single
        <LoadColumn(5)>
        Public Property Hour As Single
        <LoadColumn(6)>
        Public Property Holiday As Single
        <LoadColumn(7)>
        Public Property Weekday As Single
        <LoadColumn(8)>
        Public Property WorkingDay As Single
        <LoadColumn(9)>
        Public Property Weather As Single
        <LoadColumn(10)>
        Public Property Temperature As Single
        <LoadColumn(11)>
        Public Property NormalizedTemperature As Single
        <LoadColumn(12)>
        Public Property Humidity As Single
        <LoadColumn(13)>
        Public Property Windspeed As Single
        <LoadColumn(16)>
        <ColumnName("Label")>
        Public Property Count As Single ' This is the observed count, to be used a "label" to predict
    End Class

    Public Module DemandObservationSample
        Public ReadOnly Property SingleDemandSampleData As DemandObservation
            Get
                Return New DemandObservation With {
                    .Season = 3,
                    .Year = 1,
                    .Month = 8,
                    .Hour = 10,
                    .Holiday = 0,
                    .Weekday = 4,
                    .WorkingDay = 1,
                    .Weather = 1,
                    .Temperature = 0.8F,
                    .NormalizedTemperature = 0.7576F,
                    .Humidity = 0.55F,
                    .Windspeed = 0.2239F
                }
            End Get
        End Property
    End Module
End Namespace
