Imports eShopDashboard.EntityModels.Catalog
Imports System.Collections.Generic
Imports System.Threading.Tasks

Namespace eShopDashboard.Queries
	Public Interface ICatalogQueries
		Function GetCatalogItemById(catalogItemId As Integer) As Task(Of CatalogItem)

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Function GetProductsByDescriptionAsync(description As String) As Task(Of IEnumerable(Of Object))
	End Interface
End Namespace