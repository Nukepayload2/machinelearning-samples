Imports System
Imports Newtonsoft.Json

Namespace eShopDashboard.EntityModels.Catalog
	Public Class CatalogItem
		Private _tags As CatalogFullTag
		Private _invalidTagJson As Boolean

		Public Property Id As Integer

		Public Property Name As String

		Public Property Description As String

		Public Property Price As Decimal

		Public Property PictureFileName As String

		Public Property PictureUri As String

		Public Property CatalogTypeId As Integer

		'public CatalogType CatalogType { get; set; }

		Public Property CatalogBrandId As Integer

		'public CatalogBrand CatalogBrand { get; set; }

		' Quantity in stock
		Public Property AvailableStock As Integer

		' Available stock at which we should reorder
		Public Property RestockThreshold As Integer


		' Maximum number of units that can be in-stock at any time (due to physicial/logistical constraints in warehouses)
		Public Property MaxStockThreshold As Integer

		''' <summary>
		''' True if item is on reorder
		''' </summary>
		Public Property OnReorder As Boolean


		Public Property TagsJson As String

		Public ReadOnly Property Tags As CatalogFullTag
			Get
				If _tags IsNot Nothing Then
					Return _tags
				End If
				If _invalidTagJson Then
					Return Nothing
				End If

				Try
					If TagsJson Is Nothing Then
						Return Nothing
					Else
						_tags = JsonConvert.DeserializeObject(Of CatalogFullTag)(TagsJson)
						Return _tags
					End If
				Catch
					_invalidTagJson = True

					Return Nothing
				End Try
			End Get
		End Property
	End Class
End Namespace
