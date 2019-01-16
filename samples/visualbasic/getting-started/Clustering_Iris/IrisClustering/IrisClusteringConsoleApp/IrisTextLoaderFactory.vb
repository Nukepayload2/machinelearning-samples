Imports Microsoft.ML
Imports Microsoft.ML.Data

Namespace Clustering_Iris
    Public Module IrisTextLoaderFactory
        Public Function CreateTextLoader(mlContext As MLContext) As TextLoader
            Dim textLoader As TextLoader = mlContext.Data.CreateTextReader({
                    New TextLoader.Column("Label", DataKind.R4, 0),
                    New TextLoader.Column("SepalLength", DataKind.R4, 1),
                    New TextLoader.Column("SepalWidth", DataKind.R4, 2),
                    New TextLoader.Column("PetalLength", DataKind.R4, 3),
                    New TextLoader.Column("PetalWidth", DataKind.R4, 4)
                },
                separatorChar:=vbTab,
                hasHeader:=True
            )
            Return textLoader
        End Function
    End Module
End Namespace
