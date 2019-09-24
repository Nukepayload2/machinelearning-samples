Imports eShopDashboard.EntityModels.Ordering
Imports TinyCsvParser
Imports TinyCsvParser.Mapping
Imports TinyCsvParser.Tokenizer.RFC4180

Namespace eShopDashboard.Infrastructure.Setup
	Public Class CsvOrderParserFactory
		Public Shared ReadOnly Property HeaderColumns As String()
			Get
				Return { "Id", "Address_Country", "OrderDate", "Description" }
			End Get
		End Property

		Public Shared Function CreateParser() As CsvParser(Of Order)
			Dim tokenizerOptions = New Options(""""c, "\"c, ","c)
			Dim tokenizer = New RFC4180Tokenizer(tokenizerOptions)
			Dim parserOptions = New CsvParserOptions(True, tokenizer)
			Dim mapper = New CsvOrderMapper
			Dim parser = New CsvParser(Of Order)(parserOptions, mapper)

			Return parser
		End Function

		Private Class CsvOrderMapper
			Inherits CsvMapping(Of Order)

			Public Sub New()
				MapProperty(0, Function(m) m.Id)
				MapProperty(1, Function(m) m.Address_Country)
				MapProperty(2, Function(m) m.OrderDate)
				MapProperty(3, Function(m) m.Description)
			End Sub
		End Class
	End Class
End Namespace