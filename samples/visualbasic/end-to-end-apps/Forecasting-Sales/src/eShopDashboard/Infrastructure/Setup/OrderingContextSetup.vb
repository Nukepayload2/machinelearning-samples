Imports eShopDashboard.EntityModels.Ordering
Imports eShopDashboard.Infrastructure.Data.Ordering
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging
Imports SqlBatchInsert
Imports System
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports TinyCsvParser

Namespace eShopDashboard.Infrastructure.Setup
	Public Class OrderingContextSetup
		Private ReadOnly _connectionString As String
		Private ReadOnly _dbContext As OrderingContext
		Private ReadOnly _logger As ILogger(Of OrderingContextSetup)
		Private ReadOnly _setupPath As String
		Private _orderDataArray() As Order
		Private _orderItemDataArray() As OrderItem
		Private _status As SeedingStatus

		Public Sub New(dbContext As OrderingContext, env As IHostingEnvironment, logger As ILogger(Of OrderingContextSetup), configuration As IConfiguration)
			_dbContext = dbContext
			_logger = logger
			_setupPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Setup", "DataFiles")
			_connectionString = configuration.GetConnectionString("DefaultConnection")
		End Sub

		Public Async Function GetSeedingStatusAsync() As Task(Of SeedingStatus)
			If _status IsNot Nothing Then
				Return _status
			End If

			If Await _dbContext.Orders.AnyAsync() Then
				_status = New SeedingStatus(False)
				Return _status
			End If

			Dim dataLinesCount As Integer = GetOrdersDataToLoad() + GetOrderItemsDataToLoad()

			_status = New SeedingStatus(dataLinesCount)
			Return _status
		End Function

		Public Async Function SeedAsync(orderingProgressHandler As IProgress(Of Integer)) As Task
			Dim seedingStatus = Await GetSeedingStatusAsync()

			If Not seedingStatus.NeedsSeeding Then
				Return
			End If

			_logger.LogInformation($"----- Seeding OrderingContext from ""{_setupPath}""")

			Dim ordersLoaded = 0
			Dim orderItemsLoaded = 0

			Dim ordersProgressHandler = New Progress(Of Integer)(Sub(value)
				ordersLoaded = value
				orderingProgressHandler.Report(ordersLoaded + orderItemsLoaded)
			End Sub)

			Dim orderItemsProgressHandler = New Progress(Of Integer)(Sub(value)
				orderItemsLoaded = value
				orderingProgressHandler.Report(ordersLoaded + orderItemsLoaded)
			End Sub)

			Await SeedOrdersAsync(ordersProgressHandler)
			Await SeedOrderItemsAsync(orderItemsProgressHandler)
		End Function

		Private Function GetOrderItemsDataToLoad() As Integer
			Dim parser As CsvParser(Of OrderItem) = CsvOrderItemParserFactory.CreateParser()
			Dim dataFile = Path.Combine(_setupPath, "OrderItems.csv")

			Dim loadResult = parser.ReadFromFile(dataFile, Encoding.UTF8).ToList()

			If loadResult.Any(Function(r) Not r.IsValid) Then
				_logger.LogError("----- DATA PARSING ERRORS: {DataFile}" & vbLf & "{Details}", dataFile, String.Join(vbLf, loadResult.Where(Function(r) Not r.IsValid).Select(Function(r) r.Error)))

				Throw New InvalidOperationException($"Data parsing error loading ""{dataFile}""")
			End If

			_orderItemDataArray = loadResult.Select(Function(r) r.Result).ToArray()

			Return _orderItemDataArray.Length
		End Function

		Private Function GetOrdersDataToLoad() As Integer
			Dim parser As CsvParser(Of Order) = CsvOrderParserFactory.CreateParser()
			Dim dataFile = Path.Combine(_setupPath, "Orders.csv")

			Dim loadResult = parser.ReadFromFile(dataFile, Encoding.UTF8).ToList()

			If loadResult.Any(Function(r) Not r.IsValid) Then
				_logger.LogError("----- DATA PARSING ERRORS: {DataFile}" & vbLf & "{Details}", dataFile, String.Join(vbLf, loadResult.Where(Function(r) Not r.IsValid).Select(Function(r) r.Error)))

				Throw New InvalidOperationException($"Data parsing error loading ""{dataFile}""")
			End If

			_orderDataArray = loadResult.Select(Function(r) r.Result).ToArray()

			Return _orderDataArray.Length
		End Function

		Private Async Function SeedOrderItemsAsync(recordsProgressHandler As IProgress(Of Integer)) As Task
			Dim sw = New Stopwatch
			sw.Start()

			_logger.LogInformation("----- Seeding OrderItems")

			Dim batcher = New SqlBatcher(Of OrderItem)(_orderItemDataArray, "Ordering.OrderItems", CsvOrderItemParserFactory.HeaderColumns)

			Using connection = New SqlConnection(_connectionString)
				connection.Open()

				Dim sqlInsert As String

				sqlInsert = batcher.GetInsertCommand()
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((sqlInsert = batcher.GetInsertCommand()) != string.Empty)
				Do While sqlInsert <> String.Empty
					Dim sqlCommand = New SqlCommand(sqlInsert, connection)
					Await sqlCommand.ExecuteNonQueryAsync()

					recordsProgressHandler.Report(batcher.RowPointer)
					sqlInsert = batcher.GetInsertCommand()
				Loop
			End Using

			_logger.LogInformation("----- {TotalRows} {TableName} Inserted ({TotalSeconds:n3}s)", batcher.RowPointer, "OrderItems", sw.Elapsed.TotalSeconds)
		End Function

		Private Async Function SeedOrdersAsync(recordsProgressHandler As IProgress(Of Integer)) As Task
			Dim sw = New Stopwatch
			sw.Start()

			_logger.LogInformation("----- Seeding Orders")

			Dim batcher = New SqlBatcher(Of Order)(_orderDataArray, "Ordering.Orders", CsvOrderParserFactory.HeaderColumns)

			Using connection = New SqlConnection(_connectionString)
				connection.Open()

				Dim sqlInsert As String

				sqlInsert = batcher.GetInsertCommand()
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((sqlInsert = batcher.GetInsertCommand()) != string.Empty)
				Do While sqlInsert <> String.Empty
					Dim sqlCommand = New SqlCommand(sqlInsert, connection)
					Await sqlCommand.ExecuteNonQueryAsync()

					recordsProgressHandler.Report(batcher.RowPointer)
					sqlInsert = batcher.GetInsertCommand()
				Loop
			End Using

			_logger.LogInformation("----- {TotalRows} {TableName} Inserted ({TotalSeconds:n3}s)", batcher.RowPointer, "Orders", sw.Elapsed.TotalSeconds)
		End Function
	End Class
End Namespace