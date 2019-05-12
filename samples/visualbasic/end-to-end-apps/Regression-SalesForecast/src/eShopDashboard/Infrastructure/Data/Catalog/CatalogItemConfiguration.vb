Imports eShopDashboard.EntityModels.Catalog
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace eShopDashboard.Infrastructure.Data.Catalog
	Public Class CatalogItemConfiguration
		Implements IEntityTypeConfiguration(Of CatalogItem)

		Public Sub Configure(builder As EntityTypeBuilder(Of CatalogItem))
			builder.ToTable("CatalogItems", schema:= "Catalog")

			builder.Property(Function(ci) ci.Id).IsRequired(True).ValueGeneratedNever()

			builder.Property(Function(ci) ci.Name).IsRequired(True).HasMaxLength(50)

			builder.Property(Function(ci) ci.Price).IsRequired(True)

			builder.Property(Function(ci) ci.PictureUri).IsRequired(False)

			builder.Property(Function(ci) ci.TagsJson).IsRequired(False).HasMaxLength(4000)
		End Sub
	End Class
End Namespace