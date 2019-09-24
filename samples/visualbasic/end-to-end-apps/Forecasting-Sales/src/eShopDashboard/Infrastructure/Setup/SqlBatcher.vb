Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Text

Namespace SqlBatchInsert
	Public Class SqlBatcher(Of T As Class)
		Private ReadOnly _data() As T
		Private ReadOnly _insertLine As String
		Private ReadOnly _properties() As PropertyInfo
		Private ReadOnly _batchBuilder As StringBuilder = New StringBuilder
		Private ReadOnly _rowBuilder As StringBuilder = New StringBuilder

		Private _rowPointer As Integer = 0

		Public Sub New(data() As T, tableName As String, ParamArray columns() As String)
			Dim baseType As Type = GetType(T)
			_data = data

			'string[] columns = header.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray(); ;

			_properties = New PropertyInfo(columns.Length - 1){}

			Dim i As Integer = 0
			For Each col In columns
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: _properties[i++] = baseType.GetProperty(col);
				_properties(i) = baseType.GetProperty(col)
				i += 1
			Next col

			_insertLine = $"insert {tableName} ({String.Join(", ", _properties.Select(Function(pi) pi.Name))}) values"
		End Sub

		Public ReadOnly Property RowPointer As Integer
			Get
				Return _rowPointer
			End Get
		End Property

		Public Function GetInsertCommand(Optional numRows As Integer = 1000) As String
			If numRows < 1 OrElse numRows > 1000 Then
				Throw New ArgumentOutOfRangeException(NameOf(numRows), "parameter must be between 1 and 1000")
			End If

			If _rowPointer >= _data.Length Then
				Return String.Empty
			End If

			_batchBuilder.Clear()
			_batchBuilder.AppendLine(_insertLine)

			Dim i As Integer = 0
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while (_rowPointer < _data.Length && i++ < numRows)
			Do While _rowPointer < _data.Length AndAlso i < numRows
				i += 1
				Dim isLastRow = _rowPointer = _data.Length - 1 OrElse i = numRows

				_batchBuilder.AppendLine($"({GetRowValues(_rowPointer)}){(If(isLastRow, ";", ","))}")
				_rowPointer += 1
			Loop
			i += 1

			Return _batchBuilder.ToString()
		End Function

'INSTANT VB NOTE: The variable rowPointer was renamed since Visual Basic does not handle local variables named the same as class members well:
		Private Function GetRowValues(rowPointer_Renamed As Integer) As String
			_rowBuilder.Clear()

			Dim item As T = _data(rowPointer_Renamed)

			Dim i As Integer = 0
			For Each pi In _properties
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: _rowBuilder.Append(string.Format("{0}{1}", (i++ == 0 ? "" : ", "), GetColumnValue(pi, item), TangibleStringInterpolationMarker));
				_rowBuilder.Append($"{(If(i = 0, "", ", "))}{GetColumnValue(pi, item)}")
				i += 1
			Next pi

			Return _rowBuilder.ToString()
		End Function

		Private Function GetColumnValue(pi As PropertyInfo, item As T) As Object
			Dim value As Object = Nothing

			value = pi.GetValue(item)

			Dim type = If(value Is Nothing, "null", pi.PropertyType.Name)

			Select Case type
				Case "null"
					Return "null"

				Case "Decimal"
					Return DirectCast(value, Decimal).ToString(CultureInfo.InvariantCulture)

				Case "DateTime"
					Return $"'{(DirectCast(value, DateTime)):s}'"

				Case "String"
					Return $"'{value.ToString().Replace("'", "''")}'"

				Case "Boolean"
					Return $"{(If(DirectCast(value, Boolean), "1", "0"))}"

				Case Else
					Return value.ToString()
			End Select
		End Function
	End Class
End Namespace
