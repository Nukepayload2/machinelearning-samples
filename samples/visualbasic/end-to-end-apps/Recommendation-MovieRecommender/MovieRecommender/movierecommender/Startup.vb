Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.ML
Imports movierecommender.Services
Imports MovieRecommender.DataStructures

Namespace movierecommender
	Public Class Startup
'INSTANT VB NOTE: The variable configuration was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(configuration_Renamed As IConfiguration)
			Me.Configuration = configuration_Renamed
		End Sub

		Public ReadOnly Property Configuration As IConfiguration

		' This method gets called by the runtime. Use this method to add services to the container.
		Public Sub ConfigureServices(services As IServiceCollection)
			services.AddSingleton(Of IProfileService, ProfileService)()
			services.AddSingleton(Of IMovieService, MovieService)()
			services.AddPredictionEnginePool(Of MovieRating, MovieRatingPrediction)().FromFile(Configuration("MLModelPath"))

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
		End Sub

		' This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		Public Sub Configure(app As IApplicationBuilder, env As IHostingEnvironment)
			If env.IsDevelopment() Then
				app.UseDeveloperExceptionPage()
			Else
				app.UseExceptionHandler("/Movies/Error")
			End If

			app.UseStaticFiles()

			app.UseMvc(Sub(routes)
				routes.MapRoute(name:= "default", template:= "{controller=Movies}/{action=Profiles}/{id?}")
			End Sub)
		End Sub
	End Class
End Namespace
