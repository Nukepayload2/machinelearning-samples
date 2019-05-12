Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging

Namespace OnnxObjectDetectionE2EAPP
	Public Class Program
		Public Shared Sub Main(args() As String)
			CreateWebHostBuilder(args).Build().Run()
		End Sub

		Public Shared Function CreateWebHostBuilder(args() As String) As IWebHostBuilder
			Return WebHost.CreateDefaultBuilder(args).UseStartup(Of Startup)()
		End Function
	End Class
End Namespace
