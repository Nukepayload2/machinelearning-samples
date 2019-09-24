Imports eShopDashboard.EntityModels.Ordering
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace eShopDashboard.Infrastructure.Data.Ordering
	Public Class OrderItemConfiguration
		Implements IEntityTypeConfiguration(Of OrderItem)

		Public Sub Configure(builder As EntityTypeBuilder(Of OrderItem))
			builder.ToTable("OrderItems", schema:= "Ordering")

			builder.Property(Function(oi) oi.Id).IsRequired(True).ValueGeneratedNever()

			builder.Property(Function(oi) oi.ProductId).IsRequired()

			builder.Property(Function(oi) oi.OrderId).IsRequired()

			builder.Property(Function(oi) oi.UnitPrice).IsRequired()

			builder.Property(Function(oi) oi.Units).IsRequired()

			builder.Property(Function(oi) oi.ProductName).HasMaxLength(100)
		End Sub
	End Class
End Namespace