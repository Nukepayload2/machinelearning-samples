Imports System.IO

Namespace CustomerSegmentation
    Public Module PathHelper
        Private _dataRoot As New FileInfo(GetType(Program).Assembly.Location)

        Public Function GetAssetsPath(ParamArray paths() As String) As String
            If paths Is Nothing OrElse paths.Length = 0 Then
                Return Nothing
            End If

            Return Path.Combine(paths.Prepend(_dataRoot.Directory.FullName).ToArray())
        End Function

        Public Function DeleteAssets(ParamArray paths() As String) As String
            Dim location = GetAssetsPath(paths)

            If Not String.IsNullOrWhiteSpace(location) AndAlso File.Exists(location) Then
                File.Delete(location)
            End If
            Return location
        End Function

    End Module

End Namespace
