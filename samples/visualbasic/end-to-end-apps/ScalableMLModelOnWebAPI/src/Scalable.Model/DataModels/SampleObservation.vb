﻿Imports Microsoft.ML.Data

Namespace Scalable.Model.DataModels
	Public Class SampleObservation
		<ColumnName("Label")>
		Public Property IsToxic As Boolean


		<ColumnName("Text")>
		Public Property SentimentText As String

	End Class
End Namespace
