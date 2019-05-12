Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.AspNetCore.Mvc

Namespace TensorFlowImageClassificationWebAPI.Controllers
	<Route("api/[controller]")>
	<ApiController>
	Public Class ValuesController
		Inherits ControllerBase

		' GET api/values
		<HttpGet>
		Public Function [Get]() As ActionResult(Of IEnumerable(Of String))
			Return New String() { "value1", "value2" }
		End Function

		' GET api/values/5
		<HttpGet("{id}")>
		Public Function [Get](id As Integer) As ActionResult(Of String)
			Return "value"
		End Function

		' POST api/values
		<HttpPost>
		Public Sub Post(<FromBody> value As String)
		End Sub

		' PUT api/values/5
		<HttpPut("{id}")>
		Public Sub Put(id As Integer, <FromBody> value As String)
		End Sub

		' DELETE api/values/5
		<HttpDelete("{id}")>
		Public Sub Delete(id As Integer)
		End Sub
	End Class
End Namespace
