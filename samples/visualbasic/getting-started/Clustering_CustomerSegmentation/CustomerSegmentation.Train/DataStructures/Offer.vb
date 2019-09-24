Imports System.Globalization

Namespace CustomerSegmentation.DataStructures
    Public Class Offer
        'Offer #,Campaign,Varietal,Minimum Qty (kg),Discount (%),Origin,Past Peak
        Public Property OfferId As String
        Public Property Campaign As String
        Public Property Varietal As String
        Public Property Minimum As Single
        Public Property Discount As Single
        Public Property Origin As String
        Public Property LastPeak As String

        Public Shared Function ReadFromCsv(file As String) As IEnumerable(Of Offer)
            Return System.IO.File.ReadAllLines(file).Skip(1).Select(Function(x) x.Split(","c)).Select(Function(x) New Offer With {
                .OfferId = x(0),
                .Campaign = x(1),
                .Varietal = x(2),
                .Minimum = Single.Parse(x(3), CultureInfo.InvariantCulture),
                .Discount = Single.Parse(x(4), CultureInfo.InvariantCulture),
                .Origin = x(5),
                .LastPeak = x(6)
            })
        End Function
    End Class
End Namespace
