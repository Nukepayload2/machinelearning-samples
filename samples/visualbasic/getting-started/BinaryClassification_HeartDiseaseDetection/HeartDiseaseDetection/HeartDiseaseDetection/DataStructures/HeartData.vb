Imports Microsoft.ML.Data

Namespace HeartDiseasePredictionConsoleApp.DataStructures
	Public Class HeartData
		<LoadColumn(0)>
		Public Property Age As Single
		<LoadColumn(1)>
		Public Property Sex As Single
		<LoadColumn(2)>
		Public Property Cp As Single
		<LoadColumn(3)>
		Public Property TrestBps As Single
		<LoadColumn(4)>
		Public Property Chol As Single
		<LoadColumn(5)>
		Public Property Fbs As Single
		<LoadColumn(6)>
		Public Property RestEcg As Single
		<LoadColumn(7)>
		Public Property Thalac As Single
		<LoadColumn(8)>
		Public Property Exang As Single
		<LoadColumn(9)>
		Public Property OldPeak As Single
		<LoadColumn(10)>
		Public Property Slope As Single
		<LoadColumn(11)>
		Public Property Ca As Single
		<LoadColumn(12)>
		Public Property Thal As Single
		<LoadColumn(13)>
		Public Property Label As Boolean
	End Class

End Namespace
