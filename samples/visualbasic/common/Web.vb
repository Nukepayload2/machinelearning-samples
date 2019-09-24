Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

Namespace Common
	Public Class Web
		Public Shared Function Download(url As String, destDir As String, destFileName As String) As Boolean
			If destFileName Is Nothing Then
				destFileName = url.Split(Path.DirectorySeparatorChar).Last()
			End If

			Directory.CreateDirectory(destDir)

			Dim relativeFilePath As String = Path.Combine(destDir, destFileName)

			If File.Exists(relativeFilePath) Then
				Console.WriteLine($"{relativeFilePath} already exists.")
				Return False
			End If

			Dim wc = New WebClient
			Console.WriteLine($"Downloading {relativeFilePath}")
'INSTANT VB NOTE: The local variable download was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim download_Renamed = Task.Run(Sub() wc.DownloadFile(url, relativeFilePath))
			Do While Not download_Renamed.IsCompleted
				Thread.Sleep(1000)
				Console.Write(".")
			Loop
			Console.WriteLine("")
			Console.WriteLine($"Downloaded {relativeFilePath}")

			Return True
		End Function
	End Class
End Namespace
