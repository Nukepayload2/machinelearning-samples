Imports Microsoft.ML.Data

Namespace LargeDatasets.DataStructures
	Public Class UrlData
		<LoadColumn(0)>
		Public LabelColumn As String

		<LoadColumn(1, 3231961)>
		<VectorType(3231961)>
		Public FeatureVector() As Single
	End Class
End Namespace
