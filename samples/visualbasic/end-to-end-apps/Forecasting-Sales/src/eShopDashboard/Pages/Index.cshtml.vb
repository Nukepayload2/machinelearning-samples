Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.AspNetCore.Mvc.RazorPages

Namespace eShopDashboard.Pages
	Public Class IndexModel
		Inherits PageModel

		Public Function OnGet() As IActionResult
			Return RedirectToPage("/Reports/Products")
		End Function
	End Class
End Namespace