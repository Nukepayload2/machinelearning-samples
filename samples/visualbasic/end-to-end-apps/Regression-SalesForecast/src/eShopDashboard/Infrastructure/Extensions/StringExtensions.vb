Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace eShopDashboard.Infrastructure.Extensions
	Public Module StringExtensions
		<System.Runtime.CompilerServices.Extension> _
		Public Function JoinTags(items As IEnumerable(Of String)) As String
			Return String.Join("-"c, If(items, {String.Empty}))
		End Function

		<System.Runtime.CompilerServices.Extension> _
		Public Function IsBlank(stringObject As String) As Boolean
			Return String.IsNullOrWhiteSpace(stringObject)
		End Function

		<System.Runtime.CompilerServices.Extension> _
		Public Function IsNotAnInt(stringObject As String) As Boolean
			Dim result As Integer
			Return Not Integer.TryParse(stringObject, result)
		End Function

		<System.Runtime.CompilerServices.Extension> _
		Public Function FormatAsCSV(Of T As Class)(value As IEnumerable(Of T)) As String
			Dim stringBuilder As StringBuilder = New StringBuilder
			Dim properties = GetType(T).GetProperties()
			stringBuilder.AppendLine(String.Join(",", properties.Select(Function(p) p.Name)))
			For Each csvLine In value
				Dim columnValues = properties.Select(Function(p) p.GetValue(csvLine)?.ToString()).Select(Function(p) p.FormatAsCSV())

				stringBuilder.AppendLine(String.Join(",", columnValues))
			Next csvLine
			Return stringBuilder.ToString()
		End Function

		<System.Runtime.CompilerServices.Extension> _
		Public Function IsValidCSV(value As String) As Boolean
			Return value.IndexOfAny(New Char() { """"c, ","c }) = -1
		End Function

		<System.Runtime.CompilerServices.Extension> _
		Public Function FormatAsCSV(value As String) As String
			Return If(String.IsNullOrEmpty(value), String.Empty, (If(value.IsValidCSV(), value, String.Format("""{0}""", value.Replace("""", """""")))))
		End Function
	End Module
End Namespace