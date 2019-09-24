Imports eShopDashboard.Infrastructure.Extensions
Imports eShopDashboard.Queries
Imports Microsoft.AspNetCore.Mvc
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace eShopDashboard.Controllers
	<Produces("application/json")>
	<Route("api/ordering")>
	Public Class OrderingController
		Inherits Controller

		Private ReadOnly _queries As IOrderingQueries

		Public Sub New(queries As IOrderingQueries)
			_queries = queries
		End Sub

		<HttpGet("country/{country}/history")>
		Public Async Function CountryHistory(country As String) As Task(Of IActionResult)
			If country.IsBlank() Then
				Return BadRequest()
			End If

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
			Dim items As IEnumerable(Of Object) = Await _queries.GetCountryHistoryAsync(country)

			Return Ok(items)
		End Function

		<HttpGet("country/stats")>
		Public Async Function CountryStats() As Task(Of IActionResult)
'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
			Dim items As IEnumerable(Of Object) = Await _queries.GetCountryStatsAsync()

			Dim typedOrderItems = items.Select(Function(c) New With {
				Key c.next,
				Key c.country,
				Key c.year,
				Key c.month,
				Key c.max,
				Key c.min,
				Key c.std,
				Key c.count,
				Key c.sales,
				Key c.med,
				Key c.prev
			}).ToList()

			Dim csvFile = File(Encoding.UTF8.GetBytes(typedOrderItems.FormatAsCSV()), "text/csv")
			csvFile.FileDownloadName = "countries.stats.csv"
			Return csvFile
		End Function

		<HttpGet("product/{productId}/history")>
		Public Async Function ProductHistory(productId As String) As Task(Of IActionResult)
			If productId.IsBlank() OrElse productId.IsNotAnInt() Then
				Return BadRequest()
			End If

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
			Dim items As IEnumerable(Of Object) = Await _queries.GetProductHistoryAsync(productId)

			Return Ok(items)
		End Function

		<HttpGet("product/{productId}/stats")>
		Public Async Function ProductStats(productId As String) As Task(Of IActionResult)
			If String.IsNullOrEmpty(productId) Then
				Return BadRequest()
			End If

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
			Dim stats As IEnumerable(Of Object) = Await _queries.GetProductStatsAsync(productId)

			Return Ok(stats)
		End Function

		<HttpGet("product/stats")>
		Public Async Function ProductStats() As Task(Of IActionResult)
'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
			Dim items As IEnumerable(Of Object) = Await _queries.GetProductStatsAsync()

			Dim typedOrderItems = items.Select(Function(c) New With {
				Key c.next,
				Key c.productId,
				Key c.year,
				Key c.month,
				Key c.units,
				Key c.avg,
				Key c.count,
				Key c.max,
				Key c.min,
				Key c.prev
			}).ToList()

			Dim csvFile = File(Encoding.UTF8.GetBytes(typedOrderItems.FormatAsCSV()), "text/csv")
			csvFile.FileDownloadName = "products.stats.csv"
			Return csvFile
		End Function
	End Class
End Namespace