Imports Newtonsoft.Json

Namespace eShopDashboard.EntityModels.Catalog
	Public Class CatalogTag
		Public Property ProductId As Integer
		Public Property Description As String

		<JsonProperty("tagrams")>
		Public Property Tagrams As String()
	End Class

	Public Class CatalogFullTag
		Inherits CatalogTag

		<JsonProperty("color")>
		Public Property Color As String()
		<JsonProperty("size")>
		Public Property Size As String()
		<JsonProperty("quantity")>
		Public Property Quantity As String()
		<JsonProperty("shape")>
		Public Property Shape As String()

		Public Property agram As String
		Public Property bgram As String
		Public Property abgram As String
		Public Property ygram As String
		Public Property zgram As String
		Public Property yzgram As String
	End Class
End Namespace
