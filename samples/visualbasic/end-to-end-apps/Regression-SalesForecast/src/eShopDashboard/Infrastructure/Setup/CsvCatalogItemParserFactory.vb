Imports System
Imports eShopDashboard.EntityModels.Catalog
Imports TinyCsvParser
Imports TinyCsvParser.Mapping
Imports TinyCsvParser.Tokenizer.RFC4180
Imports TinyCsvParser.TypeConverter

Namespace eShopDashboard.Infrastructure.Setup
	Public Class CsvCatalogItemParserFactory
		Public Shared ReadOnly Property HeaderColumns As String()
			Get
				Return { "Id", "AvailableStock", "CatalogBrandId", "CatalogTypeId", "Description", "MaxStockThreshold", "Name", "OnReorder", "PictureFileName", "PictureUri", "Price", "RestockThreshold", "TagsJson" }
			End Get
		End Property

		Public Shared Function CreateParser() As CsvParser(Of CatalogItem)
			Dim tokenizerOptions = New Options(""""c, "\"c, ","c)
			Dim tokenizer = New RFC4180Tokenizer(tokenizerOptions)
			Dim parserOptions = New CsvParserOptions(True, tokenizer)
			Dim mapper = New CsvCatalogItemMapper
			Dim parser = New CsvParser(Of CatalogItem)(parserOptions, mapper)

			Return parser
		End Function

		Private Class CsvCatalogItemMapper
			Inherits CsvMapping(Of CatalogItem)

			Public Sub New()
				MapProperty(0, Function(m) m.Id)
				MapProperty(1, Function(m) m.AvailableStock)
				MapProperty(2, Function(m) m.CatalogBrandId)
				MapProperty(3, Function(m) m.CatalogTypeId)
				MapProperty(4, Function(m) m.Description)
				MapProperty(5, Function(m) m.MaxStockThreshold)
				MapProperty(6, Function(m) m.Name)
				MapProperty(7, Function(m) m.OnReorder, New BoolConverter("1", "0", StringComparison.InvariantCulture))
				MapProperty(8, Function(m) m.PictureFileName)
				MapProperty(9, Function(m) m.PictureUri)
				MapProperty(10, Function(m) m.Price)
				MapProperty(11, Function(m) m.RestockThreshold)
				MapProperty(12, Function(m) m.TagsJson)
			End Sub
		End Class
	End Class
End Namespace