Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Hosting

Namespace OnnxObjectDetectionWeb
	Public Class Program
		Public Shared Sub Main(args() As String)
			CreateHostBuilder(args).Build().Run()
		End Sub

		Public Shared Function CreateHostBuilder(args() As String) As IHostBuilder
			Return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(Sub(webBuilder)
			End Sub
				If True Then
					webBuilder.UseStartup(Of Startup)()
				End If)
		End Function
	End Class
