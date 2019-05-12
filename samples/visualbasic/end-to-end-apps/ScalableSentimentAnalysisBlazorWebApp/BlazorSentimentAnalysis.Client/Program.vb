Imports Microsoft.AspNetCore.Blazor.Hosting

Namespace BlazorSentimentAnalysis.Client
	Public Class Program
		Public Shared Sub Main(args() As String)
			CreateHostBuilder(args).Build().Run()
		End Sub

		Public Shared Function CreateHostBuilder(args() As String) As IWebAssemblyHostBuilder
			Return BlazorWebAssemblyHost.CreateDefaultBuilder().UseBlazorStartup(Of Startup)()
		End Function
	End Class
End Namespace
