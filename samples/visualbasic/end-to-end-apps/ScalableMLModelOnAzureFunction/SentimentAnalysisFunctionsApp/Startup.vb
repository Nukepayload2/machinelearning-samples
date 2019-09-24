Imports Microsoft.Azure.Functions.Extensions.DependencyInjection
Imports Microsoft.Extensions.ML
Imports SentimentAnalysisFunctionsApp
Imports SentimentAnalysisFunctionsApp.DataModels

<Assembly: FunctionsStartup(GetType(Startup))>
Namespace SentimentAnalysisFunctionsApp
	Public Class Startup
		Inherits FunctionsStartup

		Public Overrides Sub Configure(builder As IFunctionsHostBuilder)
			builder.Services.AddPredictionEnginePool(Of SentimentData, SentimentPrediction)().FromFile(modelName:= "SentimentAnalysisModel", filePath:="MLModels/sentiment_model.zip", watchForChanges:= True)
		End Sub
	End Class
End Namespace
