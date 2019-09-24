Imports ICSharpCode.SharpZipLib.Core
Imports ICSharpCode.SharpZipLib.GZip
Imports ICSharpCode.SharpZipLib.Tar
Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks

Namespace Common
	Public Class Compress
		Public Shared Sub ExtractGZip(gzipFileName As String, targetDir As String)
			' Use a 4K buffer. Any larger is a waste.    
			Dim dataBuffer(4095) As Byte

			Using fs As System.IO.Stream = New FileStream(gzipFileName, FileMode.Open, FileAccess.Read)
				Using gzipStream As New GZipInputStream(fs)
					' Change this to your needs
					Dim fnOut As String = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(gzipFileName))

					Using fsOut As FileStream = File.Create(fnOut)
						StreamUtils.Copy(gzipStream, fsOut, dataBuffer)
					End Using
				End Using
			End Using
		End Sub

		Public Shared Sub UnZip(gzArchiveName As String, destFolder As String)
			Dim flag = gzArchiveName.Split(Path.DirectorySeparatorChar).Last().Split("."c).First() & ".bin"
			If File.Exists(Path.Combine(destFolder, flag)) Then
				Return
			End If

			Console.WriteLine($"Extracting.")
			Dim task = System.Threading.Tasks.Task.Run(Sub()
				ZipFile.ExtractToDirectory(gzArchiveName, destFolder)
			End Sub)

			Do While Not task.IsCompleted
				Thread.Sleep(200)
				Console.Write(".")
			Loop

			File.Create(Path.Combine(destFolder, flag))
			Console.WriteLine("")
			Console.WriteLine("Extracting is completed.")
		End Sub

		Public Shared Sub ExtractTGZ(gzArchiveName As String, destFolder As String)
			Dim flag = gzArchiveName.Split(Path.DirectorySeparatorChar).Last().Split("."c).First() & ".bin"
			If File.Exists(Path.Combine(destFolder, flag)) Then
				Return
			End If

			Console.WriteLine($"Extracting.")
			Dim task = System.Threading.Tasks.Task.Run(Sub()
				Using inStream = File.OpenRead(gzArchiveName)
					Using gzipStream = New GZipInputStream(inStream)
						Using tarArchive As TarArchive = TarArchive.CreateInputTarArchive(gzipStream)
							tarArchive.ExtractContents(destFolder)
						End Using
					End Using
				End Using
			End Sub)

			Do While Not task.IsCompleted
				Thread.Sleep(200)
				Console.Write(".")
			Loop

			File.Create(Path.Combine(destFolder, flag))
			Console.WriteLine("")
			Console.WriteLine("Extracting is completed.")
		End Sub
	End Class
End Namespace