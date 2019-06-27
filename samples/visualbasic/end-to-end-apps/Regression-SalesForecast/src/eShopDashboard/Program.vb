Imports eShopDashboard.Infrastructure.Data.Catalog
Imports eShopDashboard.Infrastructure.Data.Ordering
Imports eShopDashboard.Infrastructure.Setup
Imports Microsoft.AspNetCore
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Serilog
Imports Serilog.Events
Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks

Namespace eShopDashboard
	Public Class Program
		Private Shared _seedingProgress As Integer = 100

		Public Shared Function BuildWebHost(args() As String) As IWebHost
			Return WebHost.CreateDefaultBuilder(args).UseContentRoot(Directory.GetCurrentDirectory()).UseStartup(Of Startup)().UseSerilog().ConfigureAppConfiguration(Sub(builderContext, config)
			End Sub
				If True Then
					config.AddEnvironmentVariables()
				End If).Build()

'INSTANT VB TODO TASK: Local functions are not converted by Instant VB:
'		public static int GetSeedingProgress()
'		{
'			Return _seedingProgress;
'		}

'INSTANT VB TODO TASK: Local functions are not converted by Instant VB:
'		public static int Main(string[] args)
'		{
'			Log.Logger = New LoggerConfiguration().MinimumLevel.Debug().MinimumLevel.Override("Microsoft", LogEventLevel.Information).Enrich.FromLogContext().WriteTo.Console().WriteTo.Seq("http://localhost:5341/").CreateLogger();
'
'			Log.Information("----- Starting web host");
'
'			try
'			{
'				var host = BuildWebHost(args);
'
'				Log.Information("----- Seeding Database");
'
'				Task seeding = Task.Run(async() =>
'				{
'					await ConfigureDatabaseAsync(host);
'				}
'			   );
'
'				Log.Information("----- Running Host");
'
'				host.Run();
'
'				Log.Information("----- Web host stopped");
'
'				Return 0;
'			}
'			catch (Exception ex)
'			{
'				Log.Fatal(ex, "----- Host terminated unexpectedly");
'
'				Return 1;
'			}
'			finally
'			{
'				Log.CloseAndFlush();
'			}
'		}

'INSTANT VB TODO TASK: Local functions are not converted by Instant VB:
'		private static async Task ConfigureDatabaseAsync(IWebHost host)
'		{
'			_seedingProgress = 0;
'
'			using (var scope = host.Services.CreateScope())
'			{
'				var services = scope.ServiceProvider;
'
'				var catalogContext = services.GetService<CatalogContext>();
'				await catalogContext.Database.MigrateAsync();
'
'				var orderingContext = services.GetService<OrderingContext>();
'				await orderingContext.Database.MigrateAsync();
'			}
'
'			await SeedDatabaseAsync(host);
'
'			_seedingProgress = 100;
'		}

'INSTANT VB TODO TASK: Local functions are not converted by Instant VB:
'		private static async Task SeedDatabaseAsync(IWebHost host)
'		{
'			try
'			{
'				using (var scope = host.Services.CreateScope())
'				{
'					IServiceProvider services = scope.ServiceProvider;
'
'					Log.Information("----- Checking seeding status");
'
'					var catalogContextSetup = services.GetService<CatalogContextSetup>();
'					var orderingContextSetup = services.GetService<OrderingContextSetup>();
'
'					var catalogSeedingStatus = await catalogContextSetup.GetSeedingStatusAsync();
'					Log.Information("----- SeedingStatus ({Context}): {@SeedingStatus}", "Catalog", catalogSeedingStatus);
'
'					var orderingSeedingStatus = await orderingContextSetup.GetSeedingStatusAsync();
'					Log.Information("----- SeedingStatus ({Context}): {@SeedingStatus}", "Ordering", orderingSeedingStatus);
'
'					var seedingStatus = New SeedingStatus(catalogSeedingStatus, orderingSeedingStatus);
'					Log.Information("----- SeedingStatus ({Context}): {@SeedingStatus}", "Aggregated", seedingStatus);
'
'					if (!seedingStatus.NeedsSeeding)
'					{
'						Log.Information("----- No seeding needed");
'
'						Return;
'					}
'
'					Log.Information("----- Seeding database");
'
'					var sw = New Stopwatch();
'					sw.Start();
'
'					void ProgressAggregator()
'					{
'						seedingStatus.RecordsLoaded = catalogSeedingStatus.RecordsLoaded + orderingSeedingStatus.RecordsLoaded;
'
'						Log.Debug("----- Seeding {SeedingPercentComplete}% complete", seedingStatus.PercentComplete);
'						_seedingProgress = seedingStatus.PercentComplete;
'					}
'
'					var catalogProgressHandler = New Progress<int>(value =>
'					{
'						catalogSeedingStatus.RecordsLoaded = value;
'						ProgressAggregator();
'					}
'				   );
'
'					var orderingProgressHandler = New Progress<int>(value =>
'					{
'						orderingSeedingStatus.RecordsLoaded = value;
'						ProgressAggregator();
'					}
'				   );
'
'					Log.Information("----- Seeding CatalogContext");
'					Task catalogSeedingTask = Task.Run(async() => await catalogContextSetup.SeedAsync(catalogProgressHandler));
'
'					Log.Information("----- Seeding OrderingContext");
'					Task orderingSeedingTask = Task.Run(async() => await orderingContextSetup.SeedAsync(orderingProgressHandler));
'
'					await Task.WhenAll(catalogSeedingTask, orderingSeedingTask);
'
'					seedingStatus.SetAsComplete();
'					_seedingProgress = seedingStatus.PercentComplete;
'
'					Log.Information("----- Database Seeded ({ElapsedTime:n3}s)", sw.Elapsed.TotalSeconds);
'				}
'
'			}
'			catch (Exception ex)
'			{
'				Log.@Error(ex, "----- Exception seeding database");
'			}
'		}
		End Function
	End Class