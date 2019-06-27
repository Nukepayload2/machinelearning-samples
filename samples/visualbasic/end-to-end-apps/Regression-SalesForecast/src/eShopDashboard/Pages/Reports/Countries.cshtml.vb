Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.AspNetCore.Mvc.RazorPages
Imports Microsoft.eShopOnContainers.WebDashboardRazor.ReportsContext

Namespace eShopDashboard.Pages.Reports
	Public Class CountriesModel
		Inherits PageModel

		Public Sub OnGet()
			ViewData.SetSelectedMenu(SelectedMenu.Reports_Country)
		End Sub
	End Class
End Namespace