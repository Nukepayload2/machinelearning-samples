Imports Microsoft.ML.Data

Namespace MulticlassClassification_HeartDisease.DataStructure
	Public Class HeartData
		Public Age As Single
		Public Sex As Single
		Public Cp As Single
		Public TrestBps As Single
		Public Chol As Single
		Public Fbs As Single
		Public RestEcg As Single
		Public Thalac As Single
		Public Exang As Single
		Public OldPeak As Single
		Public Slope As Single
		Public Ca As Single
		Public Thal As Single
	End Class

	Public Class HeartDataImport
        <LoadColumn(0)>
        Public Property Age As Single
        <LoadColumn(1)>
        Public Property Sex As Single
        <LoadColumn(2)>
        Public Property Cp As Single
        <LoadColumn(3)>
        Public Property TrestBps As Single
        <LoadColumn(4)>
        Public Property Chol As Single
        <LoadColumn(5)>
        Public Property Fbs As Single
        <LoadColumn(6)>
        Public Property RestEcg As Single
        <LoadColumn(7)>
        Public Property Thalac As Single
        <LoadColumn(8)>
        Public Property Exang As Single
        <LoadColumn(9)>
        Public Property OldPeak As Single
        <LoadColumn(10)>
        Public Property Slope As Single
        <LoadColumn(11)>
        Public Property Ca As Single
        <LoadColumn(12)>
        Public Property Thal As Single
        <LoadColumn(13)>
        Public Property Label As Single


        'new TextLoader.Column("Age", DataKind.R4, 0),
        'new TextLoader.Column("Sex", DataKind.R4, 1),
        'new TextLoader.Column("Cp", DataKind.R4, 2),
        'new TextLoader.Column("TrestBps", DataKind.R4, 3),
        'new TextLoader.Column("Chol", DataKind.R4, 4),
        'new TextLoader.Column("Fbs", DataKind.R4, 5),
        'new TextLoader.Column("RestEcg", DataKind.R4, 6),
        'new TextLoader.Column("Thalac", DataKind.R4, 7),
        'new TextLoader.Column("Exang", DataKind.R4, 8),
        'new TextLoader.Column("OldPeak", DataKind.R4, 9),
        'new TextLoader.Column("Slope", DataKind.R4, 10),
        'new TextLoader.Column("Ca", DataKind.R4, 11),
        'new TextLoader.Column("Thal", DataKind.R4, 12),
        'new TextLoader.Column("Label", DataKind.R4, 13)
    End Class
End Namespace
