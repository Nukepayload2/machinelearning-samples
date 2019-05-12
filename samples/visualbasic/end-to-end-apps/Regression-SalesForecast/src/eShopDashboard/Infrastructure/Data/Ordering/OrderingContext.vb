Imports eShopDashboard.EntityModels.Ordering
Imports Microsoft.EntityFrameworkCore

Namespace eShopDashboard.Infrastructure.Data.Ordering
	Public Class OrderingContext
		Inherits DbContext

		Public Sub New(options As DbContextOptions(Of OrderingContext))
			MyBase.New(options)
		End Sub

		Public Property OrderItems As DbSet(Of OrderItem)

		Public Property Orders As DbSet(Of Order)

		Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
			modelBuilder.ApplyConfiguration(New OrderConfiguration)
			modelBuilder.ApplyConfiguration(New OrderItemConfiguration)
		End Sub
	End Class
End Namespace