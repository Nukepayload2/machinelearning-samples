Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace CustomerSegmentation.DataStructures
	Public Class PivotData
		Public Property C1 As Single
		Public Property C2 As Single
		Public Property C3 As Single
		Public Property C4 As Single
		Public Property C5 As Single
		Public Property C6 As Single
		Public Property C7 As Single
		Public Property C8 As Single
		Public Property C9 As Single
		Public Property C10 As Single
		Public Property C11 As Single
		Public Property C12 As Single
		Public Property C13 As Single
		Public Property C14 As Single
		Public Property C15 As Single
		Public Property C16 As Single
		Public Property C17 As Single
		Public Property C18 As Single
		Public Property C19 As Single
		Public Property C20 As Single
		Public Property C21 As Single
		Public Property C22 As Single
		Public Property C23 As Single
		Public Property C24 As Single
		Public Property C25 As Single
		Public Property C26 As Single
		Public Property C27 As Single
		Public Property C28 As Single
		Public Property C29 As Single
		Public Property C30 As Single
		Public Property C31 As Single
		Public Property C32 As Single
		Public Property LastName As String

		Public Overrides Function ToString() As String
			Return $"{C1},{C2},{C3},{C4},{C5},{C6},{C7},{C8},{C9}," + $"{C10},{C11},{C12},{C13},{C14},{C15},{C16},{C17},{C18},{C19}," + $"{C20},{C21},{C22},{C23},{C24},{C25},{C26},{C27},{C28},{C29}," + $"{C30},{C31},{C32},{LastName}"
		End Function

		Public Shared Sub SaveToCsv(salesData As IEnumerable(Of PivotData), file As String)
			Dim columns = "C1,C2,C3,C4,C5,C6,C7,C8,C9," &
						  "C10,C11,C12,C13,C14,C15,C16,C17,C18,C19," &
						  "C20,C21,C22,C23,C24,C25,C26,C27,C28,C29," &
						  $"C30,C31,C32,{NameOf(LastName)}"

			System.IO.File.WriteAllLines(file, salesData.Select(Function(s) s.ToString()).Prepend(columns))
		End Sub
	End Class
End Namespace
