Imports Microsoft.ML.Data

Namespace TaxiFarePrediction.DataStructures
	Public Class TaxiTrip
		<LoadColumn(0)>
		Public VendorId As String

		<LoadColumn(1)>
		Public RateCode As String

		<LoadColumn(2)>
		Public PassengerCount As Single

		<LoadColumn(3)>
		Public TripTime As Single

		<LoadColumn(4)>
		Public TripDistance As Single

		<LoadColumn(5)>
		Public PaymentType As String

		<LoadColumn(6)>
		Public FareAmount As Single
	End Class
End Namespace