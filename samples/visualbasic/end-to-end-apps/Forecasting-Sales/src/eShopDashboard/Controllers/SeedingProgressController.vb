Imports Microsoft.AspNetCore.Mvc

Namespace eShopDashboard.Controllers
	<Produces("application/json")>
	<Route("api/seeding")>
	Public Class SeedingProgressController
		Inherits Controller

		' GET: api/SeedingProgress
		<HttpGet("progress")>
		Public Function GetCurrent() As IActionResult
			Return Ok(Program.GetSeedingProgress())
		End Function
	End Class
End Namespace