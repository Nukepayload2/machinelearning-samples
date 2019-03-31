﻿Imports System
Imports Microsoft.AspNetCore.Mvc
Imports Scalable.Model.DataModels
Imports Scalable.Model.Engine

Namespace Scalable.WebAPI.Controllers
    <Route("api/[controller]")>
    <ApiController>
    Public Class PredictorController
        Inherits ControllerBase

        Private ReadOnly _modelEngine As MLModelEngine(Of SampleObservation, SamplePrediction)

        Public Sub New(modelEngine As MLModelEngine(Of SampleObservation, SamplePrediction))
            ' Get the ML Model Engine injected, for scoring
            _modelEngine = modelEngine
        End Sub

        ' GET api/predictor/sentimentprediction?sentimentText=ML.NET is awesome!
        <HttpGet>
        <Route("sentimentprediction")>
        Public Function PredictSentiment(<FromQuery> sentimentText As String) As ActionResult(Of String)
            Dim sampleData As SampleObservation = New SampleObservation With {.SentimentText = sentimentText}

            'Predict sentiment
            Dim prediction As SamplePrediction = _modelEngine.Predict(sampleData)

            Dim isToxic As Boolean = prediction.IsToxic
            Dim probability As Single = CalculatePercentage(prediction.Score)
            Dim retVal As String = $"Prediction: Is Toxic?: '{isToxic.ToString()}' with {probability.ToString()}% probability of toxicity  for the text '{sentimentText}'"

            Return retVal

        End Function

        Public Shared Function CalculatePercentage(value As Double) As Single
            Return 100 * (1.0F / (1.0F + CSng(Math.Exp(-value))))
        End Function

    End Class

End Namespace
