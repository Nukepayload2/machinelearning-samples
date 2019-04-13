Imports Microsoft.ML.Data
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace Clustering_Iris.DataStructures
	' IrisPrediction is the result returned from prediction operations
	Public Class IrisPrediction
		<ColumnName("PredictedLabel")>
		Public SelectedClusterId As UInteger

		<ColumnName("Score")>
		Public Distance() As Single
	End Class
End Namespace
