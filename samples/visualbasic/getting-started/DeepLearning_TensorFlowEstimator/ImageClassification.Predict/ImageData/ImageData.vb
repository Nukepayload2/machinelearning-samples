Imports Microsoft.ML.Data
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace ImageClassification.DataModels
	Public Class ImageData
		<LoadColumn(0)>
		Public ImagePath As String

		<LoadColumn(1)>
		Public Label As String
	End Class

End Namespace
