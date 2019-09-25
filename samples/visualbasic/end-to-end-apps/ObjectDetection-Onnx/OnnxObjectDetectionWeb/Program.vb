Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Hosting

Namespace OnnxObjectDetectionWeb
    Public Class Program
        Public Shared Sub Main(args() As String)
            CreateHostBuilder(args).Build().Run()
        End Sub

        Public Shared Function CreateHostBuilder(ByVal args As String()) As IHostBuilder
            Return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(
                Sub(webBuilder)
                    webBuilder.UseStartup(Of Startup)()
                End Sub)
        End Function
    End Class
End Namespace