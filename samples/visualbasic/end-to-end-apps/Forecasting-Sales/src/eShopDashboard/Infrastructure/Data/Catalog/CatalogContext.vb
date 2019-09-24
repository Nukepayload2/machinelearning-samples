Imports eShopDashboard.EntityModels.Catalog
Imports Microsoft.EntityFrameworkCore

Namespace eShopDashboard.Infrastructure.Data.Catalog
	Public Class CatalogContext
		Inherits DbContext

		Public Sub New(options As DbContextOptions(Of CatalogContext))
			MyBase.New(options)
		End Sub

		Public Property CatalogItems As DbSet(Of CatalogItem)

		Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
			modelBuilder.ApplyConfiguration(New CatalogItemConfiguration)
		End Sub
	End Class
End Namespace