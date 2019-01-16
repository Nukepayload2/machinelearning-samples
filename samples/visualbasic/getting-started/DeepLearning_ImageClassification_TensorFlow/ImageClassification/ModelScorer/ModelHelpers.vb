Imports System.IO

Namespace ImageClassification.ModelScorer
    Public Module ModelHelpers
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

        Public Function GetBestLabel(labels() As String, probs() As Single) As (String, Single)
            Dim max = probs.Max()
            Dim index = probs.AsSpan().IndexOf(max)
            Return (labels(index), max)
        End Function

        Public Function ReadLabels(labelsLocation As String) As String()
            Return File.ReadAllLines(labelsLocation)
        End Function

        Public Function Columns(Of T As Class)() As IEnumerable(Of String)
            Return GetType(T).GetProperties().Select(Function(p) p.Name)
        End Function

        Public Function Columns(Of T As Class, U)() As IEnumerable(Of String)
            Dim typeofU = GetType(U)
            Return GetType(T).GetProperties().Where(Function(c) c.PropertyType Is typeofU).Select(Function(p) p.Name)
        End Function

        Public Function Columns(Of T As Class, U, V)() As IEnumerable(Of String)
            Dim typeofUV = {GetType(U), GetType(V)}
            Return GetType(T).GetProperties().Where(Function(c) typeofUV.Contains(c.PropertyType)).Select(Function(p) p.Name)
        End Function

        Public Function Columns(Of T As Class, U, V, W)() As IEnumerable(Of String)
            Dim typeofUVW = {GetType(U), GetType(V), GetType(W)}
            Return GetType(T).GetProperties().Where(Function(c) typeofUVW.Contains(c.PropertyType)).Select(Function(p) p.Name)
        End Function

        Public Function ColumnsNumerical(Of T As Class)() As String()
            Return Columns(Of T, Single, Integer)().ToArray()
        End Function

        Public Function ColumnsString(Of T As Class)() As String()
            Return Columns(Of T, String)().ToArray()
        End Function

        Public Function ColumnsDateTime(Of T As Class)() As String()
            Return Columns(Of T, Date)().ToArray()
        End Function
    End Module
End Namespace
