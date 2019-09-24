Imports eShopDashboard.Infrastructure.Data.Catalog
Imports Microsoft.EntityFrameworkCore
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports eShopDashboard.EntityModels.Catalog
Imports eShopDashboard.Settings
Imports Microsoft.Extensions.Options

Namespace eShopDashboard.Queries
	Public Class CatalogQueries
		Implements ICatalogQueries

		Private ReadOnly _context As CatalogContext
		Private ReadOnly _settings As CatalogSettings

		Public Sub New(context As CatalogContext, options As IOptions(Of CatalogSettings))
			_context = context
			_settings = options.Value

		End Sub

		Public Async Function GetCatalogItemById(catalogItemId As Integer) As Task(Of CatalogItem) Implements ICatalogQueries.GetCatalogItemById
			Return Await _context.CatalogItems.SingleOrDefaultAsync(Function(ci) ci.Id = catalogItemId)
		End Function

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Public Async Function GetProductsByDescriptionAsync(description As String) As Task(Of IEnumerable(Of Object)) Implements ICatalogQueries.GetProductsByDescriptionAsync
			Dim itemList = Await _context.CatalogItems.Where(Function(c) c.Description.Contains(description)).Select(Function(ci) New With {
				Key ci.Id,
				Key ci.Price,
				Key ci.Description,
				Key .PictureUri = If(_settings.AzureStorageEnabled, _settings.AzurePicBaseUrl & ci.PictureFileName, String.Format(_settings.LocalPicBaseUrl, ci.Id))
			}).ToListAsync()

			Return itemList
		End Function
	End Class
End Namespace