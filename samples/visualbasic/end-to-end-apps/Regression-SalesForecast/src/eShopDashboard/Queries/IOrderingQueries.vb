Imports System.Collections.Generic
Imports System.Threading.Tasks

Namespace eShopDashboard.Queries
	Public Interface IOrderingQueries
'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Function GetCountryHistoryAsync(country As String) As Task(Of IEnumerable(Of Object))

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Function GetProductHistoryAsync(productId As String) As Task(Of IEnumerable(Of Object))

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Function GetProductStatsAsync(productId As String) As Task(Of IEnumerable(Of Object))

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Function GetCountryStatsAsync() As Task(Of IEnumerable(Of Object))

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Function GetProductStatsAsync() As Task(Of IEnumerable(Of Object))

'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
		Function GetProductsHistoryDepthAsync(products As IEnumerable(Of Integer)) As Task(Of Object())
	End Interface
End Namespace