Imports Microsoft.AspNetCore.Components.Builder
Imports Microsoft.Extensions.DependencyInjection

Namespace BlazorSentimentAnalysis.Client
	Public Class Startup
		Public Sub ConfigureServices(services As IServiceCollection)
		End Sub

		Public Sub Configure(app As IComponentsApplicationBuilder)
			app.AddComponent(Of App)("app")
		End Sub
	End Class
End Namespace
