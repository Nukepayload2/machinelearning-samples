Imports Microsoft.AspNetCore.Mvc.ViewFeatures
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Namespace Microsoft.eShopOnContainers.WebDashboardRazor.ReportsContext
	Public Enum SelectedMenu
		Reports_Product
		Reports_Country
	End Enum

	Public Module ViewDataHelpers
		Private selectedMenuKey As String = "selectedMenu"

		<System.Runtime.CompilerServices.Extension> _
		Public Sub SetSelectedMenu(viewData As ViewDataDictionary, selectedMenu As SelectedMenu)
			viewData(selectedMenuKey) = selectedMenu
		End Sub

		<System.Runtime.CompilerServices.Extension> _
		Public Function GetSelectedMenu(viewData As ViewDataDictionary) As SelectedMenu
			Return CType(viewData(selectedMenuKey), SelectedMenu)
		End Function
	End Module
End Namespace
