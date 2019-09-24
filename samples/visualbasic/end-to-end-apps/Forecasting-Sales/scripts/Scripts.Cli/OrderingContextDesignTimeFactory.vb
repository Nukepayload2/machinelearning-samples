Imports eShopDashboard.Infrastructure.Data.Ordering
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design

Namespace Scripts.Cli
	Public Class OrderingContextDesignTimeFactory
		Implements IDesignTimeDbContextFactory(Of OrderingContext)

		Public Function CreateDbContext(args() As String) As OrderingContext
			Dim builder = New DbContextOptionsBuilder(Of OrderingContext)

			builder.UseSqlServer("x")

			Return New OrderingContext(builder.Options)
		End Function
	End Class
End Namespace