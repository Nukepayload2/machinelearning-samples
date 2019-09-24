Imports eShopDashboard.Infrastructure.Data.Catalog
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design

Namespace Scripts.Cli
	Public Class CatalogContextDesignTimeFactory
		Implements IDesignTimeDbContextFactory(Of CatalogContext)

		Public Function CreateDbContext(args() As String) As CatalogContext
			Dim builder = New DbContextOptionsBuilder(Of CatalogContext)

			builder.UseSqlServer("x")

			Return New CatalogContext(builder.Options)
		End Function
	End Class
End Namespace