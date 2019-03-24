Imports Microsoft.ML.Data

Namespace ShampooSales.DataStructures
	Public Class ShampooSalesPrediction
		'vector to hold alert,score,p-value values
		<VectorType(3)>
		Public Property Prediction As Double()
	End Class
End Namespace
