Imports eShopDashboard.Queries
Imports Microsoft.AspNetCore.Mvc
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Namespace eShopDashboard.Controllers
	<Produces("application/json")>
	<Route("api/catalog")>
	Public Class CatalogController
		Inherits Controller

		Private ReadOnly _catalogQueries As ICatalogQueries
		Private ReadOnly _orderingQueries As IOrderingQueries

		Public Sub New(queries As ICatalogQueries, orderingQueries As IOrderingQueries)
			_catalogQueries = queries
			_orderingQueries = orderingQueries
		End Sub

		' GET: api/Catalog
		<HttpGet("productSetDetailsByDescription")>
		Public Async Function SimilarProducts(<FromQuery> description As String) As Task(Of IActionResult)
			Const minDepthOrderingThreshold As Integer = 9

			If String.IsNullOrEmpty(description) Then
				Return BadRequest()
			End If

			Dim items = Await _catalogQueries.GetProductsByDescriptionAsync(description)

			If Not items.Any() Then
				Return Ok()
			End If

			Dim products = items.Select(Function(c) c.Id).Cast(Of Integer)()
			Dim depth = Await _orderingQueries.GetProductsHistoryDepthAsync(products)

			items = items.Join(depth, Function(l) l.Id.ToString(), Function(r) r.ProductId.ToString(), Function(l,r) New With {
				Key l,
				Key r
			}).Where(Function(j) j.r.count > minDepthOrderingThreshold).Select(Function(j) j.l)

			Return Ok(items)
		End Function
	End Class
End Namespace