Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Mvc.RazorPages

Namespace eShopDashboard.Pages
	Public Class ErrorModel
		Inherits PageModel

		Public Property RequestId As String

		Public ReadOnly Property ShowRequestId As Boolean
			Get
				Return Not String.IsNullOrEmpty(RequestId)
			End Get
		End Property

		Public Sub OnGet()
			RequestId = If(Activity.Current?.Id, HttpContext.TraceIdentifier)
		End Sub
	End Class
End Namespace
