Imports Microsoft.AspNetCore
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Configuration

Namespace BlazorSentimentAnalysis.Server
	Public Class Program
		Public Shared Sub Main(args() As String)
			BuildWebHost(args).Run()
		End Sub

		Public Shared Function BuildWebHost(args() As String) As IWebHost
			Return WebHost.CreateDefaultBuilder(args).UseConfiguration((New ConfigurationBuilder).AddCommandLine(args).Build()).UseStartup(Of Startup)().Build()
		End Function
	End Class
End Namespace
