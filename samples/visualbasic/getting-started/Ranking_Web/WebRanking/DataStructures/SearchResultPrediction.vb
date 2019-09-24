Namespace WebRanking.DataStructures
	' Representation of the prediction made by the model (e.g. ranker).
	Public Class SearchResultPrediction
		Public Property GroupId As UInteger

		Public Property Label As UInteger

		' Prediction made by the model that is used to indicate the relative ranking of the candidate search results.
		Public Property Score As Single

		' Values that are influential in determining the relevance of a data instance. This is a vector that contains concatenated columns from the underlying dataset.
		Public Property Features As Single()
	End Class
End Namespace
