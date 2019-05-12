Imports Microsoft.EntityFrameworkCore.Migrations
Imports System
Imports System.Collections.Generic

Namespace eShopDashboard.Infrastructure.Migrations.Ordering
	Partial Public Class AddDescriptionsMigration_OrderingContext
		Inherits Migration

		Protected Overrides Sub Up(migrationBuilder As MigrationBuilder)
			migrationBuilder.AlterColumn(Of String)(name:= "Address_Country", schema:= "Ordering", table:= "Orders", maxLength:= 100, nullable:= True, oldClrType:= GetType(String), oldMaxLength:= 100)

			migrationBuilder.AddColumn(Of String)(name:= "Description", schema:= "Ordering", table:= "Orders", maxLength:= 1000, nullable:= True)

			migrationBuilder.AddColumn(Of String)(name:= "ProductName", schema:= "Ordering", table:= "OrderItems", maxLength:= 100, nullable:= True)

			migrationBuilder.Sql("update oi set ProductName = ci.Name from Ordering.OrderItems oi join Catalog.CatalogItems ci on ci.Id = oi.ProductId")
		End Sub

		Protected Overrides Sub Down(migrationBuilder As MigrationBuilder)
			migrationBuilder.DropColumn(name:= "Description", schema:= "Ordering", table:= "Orders")

			migrationBuilder.DropColumn(name:= "ProductName", schema:= "Ordering", table:= "OrderItems")

			migrationBuilder.AlterColumn(Of String)(name:= "Address_Country", schema:= "Ordering", table:= "Orders", maxLength:= 100, nullable:= False, oldClrType:= GetType(String), oldMaxLength:= 100, oldNullable:= True)
		End Sub
	End Class
End Namespace
