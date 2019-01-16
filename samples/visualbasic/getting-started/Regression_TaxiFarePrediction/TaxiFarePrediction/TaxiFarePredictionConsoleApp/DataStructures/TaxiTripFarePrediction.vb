Imports Microsoft.ML.Data
Imports Microsoft.ML.Runtime.Api

Namespace Regression_TaxiFarePrediction.DataStructures
    Public Class TaxiTripFarePrediction
        <ColumnName("Score")>
        Public FareAmount As Single
    End Class
End Namespace