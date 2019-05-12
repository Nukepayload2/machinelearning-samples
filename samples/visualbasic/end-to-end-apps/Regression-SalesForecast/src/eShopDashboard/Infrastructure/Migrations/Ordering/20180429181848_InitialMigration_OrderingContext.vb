Imports Microsoft.EntityFrameworkCore.Migrations
Imports System
Imports System.Collections.Generic

Namespace eShopDashboard.Infrastructure.Migrations.Ordering
	Partial Public Class InitialMigration_OrderingContext
		Inherits Migration

		Protected Overrides Sub Up(migrationBuilder As MigrationBuilder)
			migrationBuilder.EnsureSchema(name:= "Ordering")

			migrationBuilder.CreateTable(name:= "Orders", schema:= "Ordering", columns:= Function(table) New With {
				Key .Id = table.Column(Of Integer)(nullable:= False),
				Key .Address_Country = table.Column(Of String)(maxLength:= 100, nullable:= False),
				Key .OrderDate = table.Column(Of DateTime)(nullable:= False)
			}, constraints:= Sub(table)
					table.PrimaryKey("PK_Orders", Function(x) x.Id)
			End Sub)

			migrationBuilder.CreateTable(name:= "OrderItems", schema:= "Ordering", columns:= Function(table) New With {
				Key .Id = table.Column(Of Integer)(nullable:= False),
				Key .OrderId = table.Column(Of Integer)(nullable:= False),
				Key .ProductId = table.Column(Of Integer)(nullable:= False),
				Key .UnitPrice = table.Column(Of Decimal)(nullable:= False),
				Key .Units = table.Column(Of Integer)(nullable:= False)
			}, constraints:= Sub(table)
					table.PrimaryKey("PK_OrderItems", Function(x) x.Id)
					table.ForeignKey(name:= "FK_OrderItems_Orders_OrderId", column:= Function(x) x.OrderId, principalSchema:= "Ordering", principalTable:= "Orders", principalColumn:= "Id", onDelete:= ReferentialAction.Cascade)
			End Sub)

			migrationBuilder.CreateIndex(name:= "IX_OrderItems_OrderId", schema:= "Ordering", table:= "OrderItems", column:= "OrderId")
		End Sub

		Protected Overrides Sub Down(migrationBuilder As MigrationBuilder)
			migrationBuilder.DropTable(name:= "OrderItems", schema:= "Ordering")

			migrationBuilder.DropTable(name:= "Orders", schema:= "Ordering")
		End Sub
	End Class
End Namespace
