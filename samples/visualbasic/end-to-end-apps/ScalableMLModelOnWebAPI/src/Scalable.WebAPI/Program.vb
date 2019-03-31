Imports Microsoft.AspNetCore
Imports Microsoft.AspNetCore.Hosting

Namespace Scalable.WebAPI
	Public Class Program
		Public Shared Sub Main(args() As String)
			CreateWebHostBuilder(args).Build().Run()
		End Sub

		Public Shared Function CreateWebHostBuilder(args() As String) As IWebHostBuilder
			Return WebHost.CreateDefaultBuilder(args).UseStartup(Of Startup)()
		End Function
	End Class
End Namespace
