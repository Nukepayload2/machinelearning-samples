Imports Dapper
Imports eShopDashboard.Infrastructure.Data.Ordering
Imports Microsoft.EntityFrameworkCore
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Linq
Imports System.Threading.Tasks

Namespace eShopDashboard.Queries
	Public Class OrderingQueries
		Implements IOrderingQueries

		Private ReadOnly _connectionString As String
		Private ReadOnly _orderingContext As OrderingContext

		Public Sub New(orderingContext As OrderingContext)
			_orderingContext = orderingContext
			_connectionString = _orderingContext.Database.GetDbConnection().ConnectionString
		End Sub

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Public Async Function GetCountryHistoryAsync(country As String) As Task(Of IEnumerable(Of Object)) Implements IOrderingQueries.GetCountryHistoryAsync
			Using connection = New SqlConnection(_connectionString)
				connection.Open()

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
				Return Await connection.QueryAsync(Of Object)(OrderingQueriesText.CountryHistory(country), New With {Key country})
			End Using
		End Function

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Public Async Function GetProductHistoryAsync(productId As String) As Task(Of IEnumerable(Of Object)) Implements IOrderingQueries.GetProductHistoryAsync
			Using connection = New SqlConnection(_connectionString)
				connection.Open()

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
				Return Await connection.QueryAsync(Of Object)(OrderingQueriesText.ProductHistory(productId), New With {Key productId})
			End Using
		End Function

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Public Async Function GetProductStatsAsync(productId As String) As Task(Of IEnumerable(Of Object)) Implements IOrderingQueries.GetProductStatsAsync
			Dim productHistory = Await GetProductHistoryAsync(productId)

			Return productHistory.Where(Function(p) p.next IsNot Nothing AndAlso p.prev IsNot Nothing)
		End Function

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Public Async Function GetCountryStatsAsync() As Task(Of IEnumerable(Of Object)) Implements IOrderingQueries.GetCountryStatsAsync
			Dim countryStats = Await GetCountryHistoryAsync(Nothing)

			Return countryStats.Where(Function(p) p.next IsNot Nothing AndAlso p.prev IsNot Nothing)
		End Function

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Public Async Function GetProductStatsAsync() As Task(Of IEnumerable(Of Object)) Implements IOrderingQueries.GetProductStatsAsync
			Dim productStats = Await GetProductHistoryAsync(Nothing)

			Return productStats.Where(Function(p) p.next IsNot Nothing AndAlso p.prev IsNot Nothing)
		End Function

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Public Function GetProductsHistoryDepthAsync(products As IEnumerable(Of Integer)) As Task(Of Object()) Implements IOrderingQueries.GetProductsHistoryDepthAsync
'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
			Return _orderingContext.OrderItems.Where(Function(c) products.Contains(c.ProductId)).Select(Function(c) New With {
				Key c.ProductId,
				Key c.Order.OrderDate.Month,
				Key c.Order.OrderDate.Year
			}).Distinct().GroupBy(Function(k) k.ProductId, Function(g) New With {
				Key g.Year,
				Key g.Month
			}, Function(k, g) New With {
				Key .ProductId = k,
				Key .count = g.Count()
			}).Cast(Of Object)().ToArrayAsync()
		End Function
	End Class
End Namespace