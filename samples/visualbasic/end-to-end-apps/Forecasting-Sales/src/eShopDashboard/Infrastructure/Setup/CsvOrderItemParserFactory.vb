Imports eShopDashboard.EntityModels.Ordering
Imports TinyCsvParser
Imports TinyCsvParser.Mapping
Imports TinyCsvParser.Tokenizer.RFC4180

Namespace eShopDashboard.Infrastructure.Setup
	Public Class CsvOrderItemParserFactory
		Public Shared ReadOnly Property HeaderColumns As String()
			Get
				Return { "Id", "OrderId", "ProductId", "UnitPrice", "Units", "ProductName" }
			End Get
		End Property

		Public Shared Function CreateParser() As CsvParser(Of OrderItem)
			Dim tokenizerOptions = New Options(""""c, "\"c, ","c)
			Dim tokenizer = New RFC4180Tokenizer(tokenizerOptions)
			Dim parserOptions = New CsvParserOptions(True, tokenizer)
			Dim mapper = New CsvOrderItemMapper
			Dim parser = New CsvParser(Of OrderItem)(parserOptions, mapper)

			Return parser
		End Function

		Private Class CsvOrderItemMapper
			Inherits CsvMapping(Of OrderItem)

			Public Sub New()
				MapProperty(0, Function(m) m.Id)
				MapProperty(1, Function(m) m.OrderId)
				MapProperty(2, Function(m) m.ProductId)
				MapProperty(3, Function(m) m.UnitPrice)
				MapProperty(4, Function(m) m.Units)
				MapProperty(5, Function(m) m.ProductName)
			End Sub
		End Class
	End Class
End Namespace