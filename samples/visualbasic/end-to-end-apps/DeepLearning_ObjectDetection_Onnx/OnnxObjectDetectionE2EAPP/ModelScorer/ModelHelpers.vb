Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports OnnxObjectDetectionE2EAPP

Namespace OnnxObjectDetectionE2EAPP.OnnxModelScorers
	Public Module ModelHelpers

		Public Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)
			Return fullPath
		End Function
	End Module
End Namespace
