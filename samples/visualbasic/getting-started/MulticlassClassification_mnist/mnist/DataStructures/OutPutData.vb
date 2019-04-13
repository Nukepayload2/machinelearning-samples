Imports Microsoft.ML.Data
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace mnist.DataStructures
	Friend Class OutPutData
		<ColumnName("Score")>
		Public Score() As Single
	End Class
End Namespace
