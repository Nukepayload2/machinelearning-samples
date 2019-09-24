Imports System.IO
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.ML
Imports Microsoft.ML
Imports TensorFlowImageClassification.ML
Imports TensorFlowImageClassification.ML.DataModels

Namespace TensorFlowImageClassification
	Public Class Startup
		Private ReadOnly _tensorFlowModelFilePath As String
		Private ReadOnly _mlnetModel As ITransformer

'INSTANT VB NOTE: The variable configuration was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(configuration_Renamed As IConfiguration)
			Me.Configuration = configuration_Renamed

			_tensorFlowModelFilePath = GetAbsolutePath(Me.Configuration("MLModel:TensorFlowModelFilePath"))

			'///////////////////////////////////////////////////////////////
			'Configure the ML.NET model for the pre-trained TensorFlow model
			Dim tensorFlowModelConfigurator As New TensorFlowModelConfigurator(_tensorFlowModelFilePath)
			_mlnetModel = tensorFlowModelConfigurator.Model
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

			'///////////////////////////////////////////////////////////////////////////
			' Register the PredictionEnginePool as a service in the IoC container for DI
			'
			services.AddPredictionEnginePool(Of ImageInputData, ImageLabelPredictions)()
			services.AddOptions(Of PredictionEnginePoolOptions(Of ImageInputData, ImageLabelPredictions))().Configure(Sub(options)
					options.ModelLoader = New InMemoryModelLoader(_mlnetModel)
			End Sub)
		End Sub

		' This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		Public Sub Configure(app As IApplicationBuilder, env As IHostingEnvironment)
			If env.IsDevelopment() Then
				app.UseDeveloperExceptionPage()
			Else
				app.UseExceptionHandler("/Error")
				' The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts()
			End If

			app.UseHttpsRedirection()
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
