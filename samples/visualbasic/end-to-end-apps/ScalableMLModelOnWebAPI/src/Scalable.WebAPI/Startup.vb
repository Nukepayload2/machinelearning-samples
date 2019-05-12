Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Scalable.Model.DataModels
Imports Scalable.Model.Engine
Imports System.IO

Namespace Scalable.WebAPI
	Public Class Startup
'INSTANT VB NOTE: The variable configuration was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(configuration_Renamed As IConfiguration)
			Me.Configuration = configuration_Renamed
		End Sub

		Public ReadOnly Property Configuration As IConfiguration

		' This method gets called by the runtime. Use this method to add services to the container.
		Public Sub ConfigureServices(services As IServiceCollection)
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)

			' Register the MLModelEngine with ObjPooling implementation as 'Singleton'
			' in the IoC container for DI
			'
			services.AddSingleton(Of MLModelEngine(Of SampleObservation, SamplePrediction))(Function(ctx)
									  Dim modelFilePathName As String = GetAbsolutePath(Configuration("MLModel:MLModelFilePath"))
									  Return New MLModelEngine(Of SampleObservation, SamplePrediction)(modelFilePathName)
			End Function)
		End Sub

		' This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		Public Sub Configure(app As IApplicationBuilder, env As IHostingEnvironment)
			If env.IsDevelopment() Then
				app.UseDeveloperExceptionPage()
			End If

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
