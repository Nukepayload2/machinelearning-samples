Imports Microsoft.EntityFrameworkCore.Migrations
Imports System
Imports System.Collections.Generic

Namespace eShopDashboard.Infrastructure.Migrations.Catalog
	Partial Public Class InitialMigration_CatalogContext
		Inherits Migration

		Protected Overrides Sub Up(migrationBuilder As MigrationBuilder)
			migrationBuilder.EnsureSchema(name:= "Catalog")

			migrationBuilder.CreateTable(name:= "CatalogItems", schema:= "Catalog", columns:= Function(table) New With {
				Key .Id = table.Column(Of Integer)(nullable:= False),
				Key .AvailableStock = table.Column(Of Integer)(nullable:= False),
				Key .CatalogBrandId = table.Column(Of Integer)(nullable:= False),
				Key .CatalogTypeId = table.Column(Of Integer)(nullable:= False),
				Key .Description = table.Column(Of String)(nullable:= True),
				Key .MaxStockThreshold = table.Column(Of Integer)(nullable:= False),
				Key .Name = table.Column(Of String)(maxLength:= 50, nullable:= False),
				Key .OnReorder = table.Column(Of Boolean)(nullable:= False),
				Key .PictureFileName = table.Column(Of String)(nullable:= True),
				Key .PictureUri = table.Column(Of String)(nullable:= True),
				Key .Price = table.Column(Of Decimal)(nullable:= False),
				Key .RestockThreshold = table.Column(Of Integer)(nullable:= False),
				Key .TagsJson = table.Column(Of String)(maxLength:= 4000, nullable:= True)
			}, constraints:= Sub(table)
					table.PrimaryKey("PK_CatalogItems", Function(x) x.Id)
			End Sub)
		End Sub

		Protected Overrides Sub Down(migrationBuilder As MigrationBuilder)
			migrationBuilder.DropTable(name:= "CatalogItems", schema:= "Catalog")
		End Sub
	End Class
End Namespace
