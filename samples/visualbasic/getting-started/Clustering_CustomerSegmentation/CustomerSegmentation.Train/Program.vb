Imports System.IO

Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Transforms.Categorical
Imports Microsoft.ML.Transforms.Projections

Imports CustomerSegmentation.DataStructures
Imports Common
Imports Microsoft.ML.Data

Namespace CustomerSegmentation
    Public Module Program
        Sub Main(args() As String)
            Dim assetsPath = PathHelper.GetAssetsPath("..\..\..\assets")

            Dim transactionsCsv = Path.Combine(assetsPath, "inputs", "transactions.csv")
            Dim offersCsv = Path.Combine(assetsPath, "inputs", "offers.csv")
            Dim pivotCsv = Path.Combine(assetsPath, "inputs", "pivot.csv")
            Dim modelZip = Path.Combine(assetsPath, "outputs", "retailClustering.zip")

            Try
                'STEP 0: Special data pre-process in this sample creating the PivotTable csv file
                DataHelpers.PreProcessAndSave(offersCsv, transactionsCsv, pivotCsv)

                'Create the MLContext to share across components for deterministic results
                Dim mlContext As New MLContext(seed:=1) 'Seed set to any number so you have a deterministic environment

                ' STEP 1: Common data loading configuration
                Dim pivotDataView = mlContext.Data.ReadFromTextFile(path:=pivotCsv, columns:={
                    New TextLoader.Column("Features", DataKind.R4, New TextLoader.Range() {New TextLoader.Range(0, 31)}),
                    New TextLoader.Column(NameOf(PivotData.LastName), DataKind.Text, 32)
                }, hasHeader:=True, separatorChar:=","c)

                'STEP 2: Configure data transformations in pipeline
                Dim dataProcessPipeline = (New PrincipalComponentAnalysisEstimator(env:=mlContext, outputColumnName:="PCAFeatures", inputColumnName:="Features", rank:=2)).
                    Append(New OneHotEncodingEstimator(mlContext, {New OneHotEncodingEstimator.ColumnInfo(name:="LastNameKey", inputColumnName:=NameOf(PivotData.LastName), OneHotEncodingTransformer.OutputKind.Ind)}))

                ' (Optional) Peek data in training DataView after applying the ProcessPipeline's transformations  
                ConsoleHelper.PeekDataViewInConsole(mlContext, pivotDataView, dataProcessPipeline, 10)
                ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", pivotDataView, dataProcessPipeline, 10)

                'STEP 3: Create the training pipeline                
                Dim trainer = mlContext.Clustering.Trainers.KMeans(featureColumn:=DefaultColumnNames.Features, clustersCount:=3)
                Dim trainingPipeline = dataProcessPipeline.Append(trainer)

                'STEP 4: Train the model fitting to the pivotDataView
                Console.WriteLine("=============== Training the model ===============")
                Dim trainedModel As ITransformer = trainingPipeline.Fit(pivotDataView)

                'STEP 5: Evaluate the model and show accuracy stats
                Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
                Dim predictions = trainedModel.Transform(pivotDataView)
                Dim metrics = mlContext.Clustering.Evaluate(predictions, score:=DefaultColumnNames.Score, features:=DefaultColumnNames.Features)

                ConsoleHelper.PrintClusteringMetrics(trainer.ToString(), metrics)

                'STEP 6: Save/persist the trained model to a .ZIP file
                Using fs = New FileStream(modelZip, FileMode.Create, FileAccess.Write, FileShare.Write)
                    mlContext.Model.Save(trainedModel, fs)
                End Using

                Console.WriteLine("The model is saved to {0}", modelZip)
            Catch ex As Exception
                ConsoleHelper.ConsoleWriteException(ex.Message)
            End Try

            ConsoleHelper.ConsolePressAnyKey()
        End Sub

    End Module
End Namespace
