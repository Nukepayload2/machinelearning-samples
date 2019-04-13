Imports Microsoft.ML.Data

Namespace ShampooSalesAnomalyDetection
	Friend Class ShampooSalesPrediction
		' Vector to hold Alert, Score, and P-Value values
		<VectorType(3)>
		Public Property Prediction As Double()
	End Class
End Namespace
