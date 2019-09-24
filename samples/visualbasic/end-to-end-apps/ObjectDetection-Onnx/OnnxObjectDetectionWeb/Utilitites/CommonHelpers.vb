Imports System.IO

Namespace OnnxObjectDetectionWeb.Utilities
	Public Module CommonHelpers
		Public Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)
			Return fullPath
		End Function
	End Module
End Namespace
