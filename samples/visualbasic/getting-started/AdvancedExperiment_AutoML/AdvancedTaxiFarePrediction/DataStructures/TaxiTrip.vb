Imports Microsoft.ML.Data

Namespace AdvancedTaxiFarePrediction.DataStructures
	Public Class TaxiTrip
		<ColumnName("vendor_id")>
		Public VendorId As String

		<ColumnName("rate_code")>
		Public RateCode As Single

		<ColumnName("passenger_count")>
		Public PassengerCount As Single

		<ColumnName("trip_time_in_secs")>
		Public TripTime As Single

		<ColumnName("trip_distance")>
		Public TripDistance As Single

		<ColumnName("payment_type")>
		Public PaymentType As String

		<ColumnName("fare_amount")>
		Public FareAmount As Single
	End Class
End Namespace