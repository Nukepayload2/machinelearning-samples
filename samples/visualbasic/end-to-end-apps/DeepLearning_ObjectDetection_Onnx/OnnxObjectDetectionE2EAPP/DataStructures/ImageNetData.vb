Imports Microsoft.ML.Data
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace OnnxObjectDetectionE2EAPP
	Public Class ImageNetData
		<LoadColumn(0)>
		Public ImagePath As String
	End Class
End Namespace
