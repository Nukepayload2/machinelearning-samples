Imports Microsoft.ML.Data

Namespace SpikeDetection.WinForms
	Public Class ProductSalesData
		<LoadColumn(0)>
		Public Month As String

		<LoadColumn(1)>
		Public numSales As Single
	End Class
End Namespace
