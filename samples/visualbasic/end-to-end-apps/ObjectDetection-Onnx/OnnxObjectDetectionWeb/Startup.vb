Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.ML
Imports OnnxObjectDetectionWeb.Infrastructure
Imports OnnxObjectDetectionWeb.Services
Imports OnnxObjectDetectionWeb.Utilities
Imports OnnxObjectDetection

Namespace OnnxObjectDetectionWeb
	Public Class Startup
		Private ReadOnly _onnxModelFilePath As String
		Private ReadOnly _mlnetModelFilePath As String

'INSTANT VB NOTE: The variable configuration was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(configuration_Renamed As IConfiguration)
			Me.Configuration = configuration_Renamed

			_onnxModelFilePath = CommonHelpers.GetAbsolutePath(Me.Configuration("MLModel:OnnxModelFilePath"))
			_mlnetModelFilePath = CommonHelpers.GetAbsolutePath(Me.Configuration("MLModel:MLNETModelFilePath"))

			Dim onnxModelConfigurator = New OnnxModelConfigurator(New TinyYoloModel(_onnxModelFilePath))

			onnxModelConfigurator.SaveMLNetModel(_mlnetModelFilePath)
		End Sub

		Public ReadOnly Property Configuration As IConfiguration

		' This method gets called by the runtime. Use this method to add services to the container.
		Public Sub ConfigureServices(services As IServiceCollection)
			services.Configure(Of CookiePolicyOptions)(Sub(options)
				' This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = Function(context) True
				options.MinimumSameSitePolicy = SameSiteMode.None
			End Sub)

			services.AddControllers()
			services.AddRazorPages()

			services.AddPredictionEnginePool(Of ImageInputData, TinyYoloPrediction)().FromFile(_mlnetModelFilePath)

			services.AddTransient(Of IImageFileWriter, ImageFileWriter)()
			services.AddTransient(Of IObjectDetectionService, ObjectDetectionService)()
		End Sub

		' This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		Public Sub Configure(app As IApplicationBuilder, env As IWebHostEnvironment)
			If env.EnvironmentName = Environments.Development Then
				app.UseDeveloperExceptionPage()
			Else
				app.UseExceptionHandler("/Error")
			End If

			app.UseStaticFiles()
			app.UseCookiePolicy()

			app.UseRouting()
			app.UseEndpoints(Sub(endpoints)
				endpoints.MapControllers()
				endpoints.MapRazorPages()
			End Sub)
		End Sub
	End Class
End Namespace
