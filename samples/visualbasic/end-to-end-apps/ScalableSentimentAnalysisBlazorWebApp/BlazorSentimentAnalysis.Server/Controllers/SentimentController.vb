Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports BlazorSentimentAnalysis.Server.ML.DataModels
Imports Microsoft.Extensions.ML
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc

Namespace BlazorSentimentAnalysis.Server.Controllers
	<Route("api/[controller]")>
	<ApiController>
	Public Class SentimentController
		Inherits ControllerBase

		Private ReadOnly _predictionEnginePool As PredictionEnginePool(Of SampleObservation, SamplePrediction)

		Public Sub New(predictionEnginePool As PredictionEnginePool(Of SampleObservation, SamplePrediction))
			' Get the ML Model Engine injected, for scoring
			_predictionEnginePool = predictionEnginePool
		End Sub

		<HttpGet("[action]")>
		<Route("sentimentprediction")>
		Public Function PredictSentiment(<FromQuery> sentimentText As String) As ActionResult(Of Single)
			Dim sampleText As String = sentimentText

			' Predict sentiment using ML.NET model
			Dim sampleData As SampleObservation = New SampleObservation With {.Col0 = sentimentText}
			'Predict sentiment
			Dim prediction As SamplePrediction = _predictionEnginePool.Predict(sampleData)
			Dim percentage As Single = CalculatePercentage(prediction.Score)

			Return percentage
		End Function

		Public Function CalculatePercentage(value As Double) As Single
			Return 100 * (1.0F / (1.0F + CSng(Math.Exp(-value))))
		End Function
	End Class
End Namespace