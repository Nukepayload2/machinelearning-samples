Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.ResponseCompression
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Newtonsoft.Json.Serialization
Imports System.Linq

Imports Microsoft.Extensions.ML
Imports BlazorSentimentAnalysis.Server.ML.DataModels

Namespace BlazorSentimentAnalysis.Server
	Public Class Startup
		Public ReadOnly Property Configuration As IConfiguration

'INSTANT VB NOTE: The variable configuration was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(configuration_Renamed As IConfiguration)
			Me.Configuration = configuration_Renamed
		End Sub

		' This method gets called by the runtime. Use this method to add services to the container.
		' For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		Public Sub ConfigureServices(services As IServiceCollection)
			services.AddMvc().AddNewtonsoftJson()
			services.AddResponseCompression(Sub(opts)
				opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat( { "application/octet-stream" })
			End Sub)

			' Register the PredictionEnginePool as a service in the IoC container for DI
			'
			services.AddPredictionEnginePool(Of SampleObservation, SamplePrediction)().FromFile(Configuration("MLModel:MLModelFilePath"))
		End Sub

		' This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		Public Sub Configure(app As IApplicationBuilder, env As IWebHostEnvironment)
			app.UseResponseCompression()

			If env.IsDevelopment() Then
				app.UseDeveloperExceptionPage()
				app.UseBlazorDebugging()
			End If

			app.UseRouting()
			app.UseClientSideBlazorFiles(Of Client.Startup)()
			app.UseEndpoints(Sub(endpoints)
				endpoints.MapDefaultControllerRoute()
				endpoints.MapFallbackToClientSideBlazor(Of Client.Startup)("index.html")
			End Sub)
		End Sub
	End Class
End Namespace
