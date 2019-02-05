Imports Microsoft.ML.Data
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace mnist.DataStructures
	Friend Class InputData
		<ColumnName("PixelValues")>
		<VectorType(64)>
		Public PixelValues() As Single
	End Class
End Namespace
