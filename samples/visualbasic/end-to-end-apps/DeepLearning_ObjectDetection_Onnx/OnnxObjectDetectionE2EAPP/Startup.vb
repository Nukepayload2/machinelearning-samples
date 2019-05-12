Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports OnnxObjectDetectionE2EAPP.Infrastructure
Imports OnnxObjectDetectionE2EAPP.OnnxModelScorers

Namespace OnnxObjectDetectionE2EAPP
	Public Class Startup
'INSTANT VB NOTE: The variable configuration was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(configuration_Renamed As IConfiguration)
			Me.Configuration = configuration_Renamed
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
			services.AddSingleton(Of IOnnxModelScorer, OnnxModelScorer)()
			services.AddTransient(Of IImageFileWriter, ImageFileWriter)()
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
	End Class
End Namespace
