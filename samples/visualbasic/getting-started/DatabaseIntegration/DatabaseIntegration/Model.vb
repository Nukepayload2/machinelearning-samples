Imports Microsoft.EntityFrameworkCore
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Namespace DatabaseIntegration
    Public Class AdultCensusContext
        Inherits DbContext

        Public Property AdultCensus As DbSet(Of AdultCensus)

        Protected Overrides Sub OnConfiguring(optionsBuilder As DbContextOptionsBuilder)
            optionsBuilder.UseSqlite("Data Source=mlexample.db")
        End Sub
    End Class
    Public Class AdultCensus
        <Key>
        <DatabaseGenerated(DatabaseGeneratedOption.Identity)>
        Public Property AdultCensusId As Integer
        Public Property Age As Integer
        Public Property Workclass As String
        Public Property Education As String
        Public Property MaritalStatus As String
        Public Property Occupation As String
        Public Property Relationship As String
        Public Property Race As String
        Public Property Sex As String
        Public Property CapitalGain As String
        Public Property CapitalLoss As String
        Public Property HoursPerWeek As Integer
        Public Property NativeCountry As String
        Public Property Label As Boolean
    End Class
End Namespace
