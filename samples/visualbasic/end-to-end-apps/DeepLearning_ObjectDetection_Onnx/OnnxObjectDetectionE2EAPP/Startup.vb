Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports OnnxObjectDetectionE2EAPP.Infrastructure
Imports Microsoft.Extensions.ML
Imports OnnxObjectDetectionE2EAPP.Services
Imports System.IO
Imports OnnxObjectDetectionE2EAPP.Utilities
Imports OnnxObjectDetectionE2EAPP.MLModel

Namespace OnnxObjectDetectionE2EAPP
	Public Class Startup
		Private ReadOnly _onnxModelFilePath As String
		Private ReadOnly _mlnetModelFilePath As String
'INSTANT VB NOTE: The variable configuration was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(configuration_Renamed As IConfiguration)
			Me.Configuration = configuration_Renamed

			_onnxModelFilePath = GetAbsolutePath(Me.Configuration("MLModel:OnnxModelFilePath"))
			_mlnetModelFilePath = GetAbsolutePath(Me.Configuration("MLModel:MLNETModelFilePath"))

			Dim onnxModelConfigurator As New OnnxModelConfigurator(_onnxModelFilePath)

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

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)

			services.AddPredictionEnginePool(Of ImageInputData, ImageObjectPrediction)().FromFile(_mlnetModelFilePath)

			services.AddTransient(Of IImageFileWriter, ImageFileWriter)()
			services.AddTransient(Of IObjectDetectionService, ObjectDetectionService)()
		End Sub

		' This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		Public Sub Configure(app As IApplicationBuilder, env As IHostingEnvironment)
			If env.IsDevelopment() Then
				app.UseDeveloperExceptionPage()
			Else
				app.UseExceptionHandler("/Error")
			End If

			app.UseStaticFiles()
			app.UseCookiePolicy()

			app.UseMvc()
		End Sub

		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)
			Return fullPath
		End Function
	End Class
End Namespace
