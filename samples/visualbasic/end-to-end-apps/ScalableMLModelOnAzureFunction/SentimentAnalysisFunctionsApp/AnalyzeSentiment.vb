Imports System
Imports System.IO
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Azure.WebJobs
Imports Microsoft.Azure.WebJobs.Extensions.Http
Imports Microsoft.AspNetCore.Http
Imports Microsoft.Extensions.Logging
Imports Newtonsoft.Json
Imports Microsoft.Extensions.ML
Imports SentimentAnalysisFunctionsApp.DataModels

Namespace SentimentAnalysisFunctionsApp
	Public Class AnalyzeSentiment

		Private ReadOnly _predictionEnginePool As PredictionEnginePool(Of SentimentData, SentimentPrediction)

		' AnalyzeSentiment class constructor
		Public Sub New(predictionEnginePool As PredictionEnginePool(Of SentimentData, SentimentPrediction))
			_predictionEnginePool = predictionEnginePool
		End Sub

		<FunctionName("AnalyzeSentiment")>
		Public Async Function Run(<HttpTrigger(AuthorizationLevel.Function, "post", Route := Nothing)> req As HttpRequest, log As ILogger) As Task(Of IActionResult)
			log.LogInformation("C# HTTP trigger function processed a request.")

			'Parse HTTP Request Body
			Dim requestBody As String = Await (New StreamReader(req.Body)).ReadToEndAsync()
			Dim data As SentimentData = JsonConvert.DeserializeObject(Of SentimentData)(requestBody)

			'Make Prediction
			Dim prediction As SentimentPrediction = _predictionEnginePool.Predict(modelName:= "SentimentAnalysisModel", example:= data)

			'Convert prediction to string
			Dim sentiment As String = If(Convert.ToBoolean(prediction.Prediction), "Positive", "Negative")

			'Return Prediction
			Return CType(New OkObjectResult(sentiment), ActionResult)
		End Function
	End Class
End Namespace
