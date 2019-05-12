Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.ML.Data

Namespace PowerAnomalyDetection.DataStructures
	Friend Class MeterData
		<LoadColumn(0)>
		Public Property name As String
		<LoadColumn(1)>
		Public Property time As DateTime
		<LoadColumn(2)>
		Public Property ConsumptionDiffNormalized As Single
	End Class
End Namespace
