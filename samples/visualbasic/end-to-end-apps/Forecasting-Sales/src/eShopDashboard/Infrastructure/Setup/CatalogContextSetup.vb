Imports eShopDashboard.EntityModels.Catalog
Imports eShopDashboard.Infrastructure.Data.Catalog
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports Newtonsoft.Json
Imports SqlBatchInsert
Imports System
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Options
Imports TinyCsvParser

Namespace eShopDashboard.Infrastructure.Setup
	Public Class CatalogContextSetup
		Private ReadOnly _dbContext As CatalogContext
		Private ReadOnly _logger As ILogger(Of CatalogContextSetup)
		Private ReadOnly _setupPath As String
		Private ReadOnly _connectionString As String

		Private _dataArray() As CatalogItem
		Private _status As SeedingStatus

		Public Sub New(dbContext As CatalogContext, env As IHostingEnvironment, logger As ILogger(Of CatalogContextSetup), configuration As IConfiguration)
			_dbContext = dbContext
			_logger = logger
			_setupPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Setup", "DataFiles")
			_connectionString = configuration.GetConnectionString("DefaultConnection")
		End Sub

		Public Async Function GetSeedingStatusAsync() As Task(Of SeedingStatus)
			If _status IsNot Nothing Then
				Return _status
			End If

			If Await _dbContext.CatalogItems.AnyAsync() Then
				_status = New SeedingStatus(False)
				Return _status
			End If

			Dim dataLinesCount As Integer = GetDataToLoad()

			_status = New SeedingStatus(dataLinesCount)
			Return _status
		End Function

		Public Async Function SeedAsync(catalogProgressHandler As IProgress(Of Integer)) As Task
			Dim seedingStatus = Await GetSeedingStatusAsync()

			If Not seedingStatus.NeedsSeeding Then
				Return
			End If

			_logger.LogInformation($"----- Seeding CatalogContext from ""{_setupPath}""")

			Await SeedCatalogItemsAsync(catalogProgressHandler)
		End Function

		Private Function GetDataToLoad() As Integer
			Dim parser As CsvParser(Of CatalogItem) = CsvCatalogItemParserFactory.CreateParser()
			Dim dataFile = Path.Combine(_setupPath, "CatalogItems.csv")

			Dim loadResult = parser.ReadFromFile(dataFile, Encoding.UTF8).ToList()

			If loadResult.Any(Function(r) Not r.IsValid) Then
				_logger.LogError("----- DATA PARSING ERRORS: {DataFile}" & vbLf & "{Details}", dataFile, String.Join(vbLf, loadResult.Where(Function(r) Not r.IsValid).Select(Function(r) r.Error)))

				Throw New InvalidOperationException($"Data parsing error loading ""{dataFile}""")
			End If

			_dataArray = loadResult.Select(Function(r) r.Result).ToArray()

			'---------------------------------------------
			' Times 2 to account for item tags processing
			'---------------------------------------------

			Return _dataArray.Length ' * 2 ; // Include times 2 if processing catalog tags
		End Function

		Private Async Function SeedCatalogItemsAsync(recordsProgressHandler As IProgress(Of Integer)) As Task
			Dim sw = New Stopwatch
			sw.Start()

			Dim itemCount = 0
			Dim tagCount = 0

'INSTANT VB TODO TASK: Local functions are not converted by Instant VB:
'			void Aggregator()
'			{
'				recordsProgressHandler.Report(itemCount + tagCount);
'			};

			Dim itemsProgressHandler = New Progress(Of Integer)(Sub(value)
				itemCount = value
				Aggregator()
			End Sub)

			Dim tagsProgressHandler = New Progress(Of Integer)(Sub(value)
				tagCount = value
				Aggregator()
			End Sub)

			_logger.LogInformation("----- Seeding CatalogItems")

			Dim batcher = New SqlBatcher(Of CatalogItem)(_dataArray, "Catalog.CatalogItems", CsvCatalogItemParserFactory.HeaderColumns)

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

			_logger.LogInformation("----- {TotalRows} {TableName} Inserted ({TotalSeconds:n3}s)", batcher.RowPointer, "CatalogItems", sw.Elapsed.TotalSeconds)


			'----------------------------------------------------------------------
			' Not needed now because CatalogItems.csv already includes CatalogTags
			' Could be needed later on in case the items or tags get updated
			'----------------------------------------------------------------------

			'await SeedCatalogTagsAsync(tagsProgressHandler);
		End Function

		Private Async Function SeedCatalogTagsAsync(recordsProgressHandler As IProgress(Of Integer)) As Task
			Dim sw = New Stopwatch
			sw.Start()

			_logger.LogInformation("----- Adding CatalogTags")
			Dim tagsText = Await File.ReadAllTextAsync(Path.Combine(_setupPath, "CatalogTags.txt"))

			Dim tags = JsonConvert.DeserializeObject(Of List(Of CatalogFullTag))(tagsText)

			_logger.LogInformation("----- Adding tags to CatalogItems")

			Dim i As Integer = 0

			For Each tag In tags
				Dim entity = Await _dbContext.CatalogItems.FirstOrDefaultAsync(Function(ci) ci.Id = tag.ProductId)

				If entity Is Nothing Then
					Continue For
				End If

				entity.TagsJson = JsonConvert.SerializeObject(tag)

				_dbContext.Update(entity)

				i += 1
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: recordsProgressHandler.Report(++i);
				recordsProgressHandler.Report(i)
			Next tag

			Await _dbContext.SaveChangesAsync()

			_logger.LogInformation("----- {TotalRows} CatalogTags Added ({TotalSeconds:n3}s)", i, sw.Elapsed.TotalSeconds)
		End Function
	End Class
End Namespace