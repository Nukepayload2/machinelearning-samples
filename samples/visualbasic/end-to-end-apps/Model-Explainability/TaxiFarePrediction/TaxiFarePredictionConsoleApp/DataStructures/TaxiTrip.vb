Imports Microsoft.ML.Data
Imports System

Namespace TaxiFareRegression.DataStructures
	Public Interface IModelEntity
		Sub PrintToConsole()
	End Interface

	Public Class TaxiTrip
		Implements IModelEntity

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
		Public Sub PrintToConsole() Implements IModelEntity.PrintToConsole
			Console.WriteLine($"Label: {FareAmount}")
			Console.WriteLine($"Features: [VendorID] {VendorId} [RateCode] {RateCode} [PassengerCount] {PassengerCount} [TripTime] {TripTime} TripDistance: {TripDistance} PaymentType: {PaymentType}")
		End Sub
	End Class


End Namespace