Imports eShopDashboard.EntityModels.Ordering
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace eShopDashboard.Infrastructure.Data.Ordering
	Public Class OrderConfiguration
		Implements IEntityTypeConfiguration(Of Order)

		Public Sub Configure(builder As EntityTypeBuilder(Of Order))
			builder.ToTable("Orders", schema:= "Ordering")

			builder.Property(Function(o) o.Id).IsRequired(True).ValueGeneratedNever()

			builder.Property(Function(o) o.OrderDate).IsRequired(True)

			builder.Property(Function(o) o.Address_Country).HasMaxLength(100)

			builder.Property(Function(o) o.Description).HasMaxLength(1000)
		End Sub
	End Class
End Namespace