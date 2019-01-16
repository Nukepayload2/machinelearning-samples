Imports Microsoft.ML.Data
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace BikeSharingDemand.DataStructures
	Public Class DemandPrediction
		<ColumnName("Score")>
		Public PredictedCount As Single
	End Class
End Namespace
