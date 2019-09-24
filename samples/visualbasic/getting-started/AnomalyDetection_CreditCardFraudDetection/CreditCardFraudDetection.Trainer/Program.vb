Imports System.IO
Imports System.IO.Compression

Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports Microsoft.ML.Trainers

Imports Common

Imports CreditCardFraudDetection.Common.DataModels

Imports Microsoft.ML.DataOperationsCatalog


Namespace CreditCardFraudDetection.Trainer
    Friend Class Program
        Shared Sub Main(args() As String)
            ' File paths
            Dim AssetsRelativePath As String = "../../../assets"
            Dim assetsPath As String = GetAbsolutePath(AssetsRelativePath)
            Dim zipDataSet As String = Path.Combine(assetsPath, "input", "creditcardfraud-dataset.zip")
            Dim fullDataSetFilePath As String = Path.Combine(assetsPath, "input", "creditcard.csv")
            Dim trainDataSetFilePath As String = Path.Combine(assetsPath, "output", "trainData.csv")
            Dim testDataSetFilePath As String = Path.Combine(assetsPath, "output", "testData.csv")
            Dim modelFilePath As String = Path.Combine(assetsPath, "output", "randomizedPca.zip")

            ' Unzip the original dataset as it is too large for GitHub repo if not zipped
            UnZipDataSet(zipDataSet, fullDataSetFilePath)

            ' Create a common ML.NET context.
            ' Seed set to any number so you have a deterministic environment for repeateable results
            Dim mlContext As New MLContext(seed:=1)

            ' Prepare data and create Train/Test split datasets
            PrepDatasets(mlContext, fullDataSetFilePath, trainDataSetFilePath, testDataSetFilePath)

            ' Load Datasets
            Dim trainingDataView As IDataView = mlContext.Data.LoadFromTextFile(Of TransactionObservation)(trainDataSetFilePath, separatorChar:=","c, hasHeader:=True)
            Dim testDataView As IDataView = mlContext.Data.LoadFromTextFile(Of TransactionObservation)(testDataSetFilePath, separatorChar:=","c, hasHeader:=True)

            ' Train Model
            Dim model As ITransformer = TrainModel(mlContext, trainingDataView)

            ' Evaluate quality of Model
            EvaluateModel(mlContext, model, testDataView)

            ' Save model
            SaveModel(mlContext, model, modelFilePath, trainingDataView.Schema)

            Console.WriteLine("=============== Press any key ===============")
            Console.ReadKey()
        End Sub


        Public Shared Sub PrepDatasets(mlContext As MLContext, fullDataSetFilePath As String, trainDataSetFilePath As String, testDataSetFilePath As String)
            ' Only prep-datasets if train and test datasets don't exist yet
            If Not File.Exists(trainDataSetFilePath) AndAlso Not File.Exists(testDataSetFilePath) Then
                Console.WriteLine("===== Preparing train/test datasets =====")

                ' Load the original single dataset
                Dim originalFullData As IDataView = mlContext.Data.LoadFromTextFile(Of TransactionObservation)(fullDataSetFilePath, separatorChar:=","c, hasHeader:=True)

                ' Split the data 80:20 into train and test sets, train and evaluate.
                Dim trainTestData As TrainTestData = mlContext.Data.TrainTestSplit(originalFullData, testFraction:=0.2, seed:=1)

                ' 80% of original dataset
                Dim trainData As IDataView = trainTestData.TrainSet

                ' 20% of original dataset
                Dim testData As IDataView = trainTestData.TestSet

                ' Inspect TestDataView to make sure there are true and false observations in test dataset, after spliting 
                InspectData(mlContext, testData, 4)

                ' Save train split
                Using fileStream = File.Create(trainDataSetFilePath)
                    mlContext.Data.SaveAsText(trainData, fileStream, separatorChar:=","c, headerRow:=True, schema:=True)
                End Using

                ' Save test split 
                Using fileStream = File.Create(testDataSetFilePath)
                    mlContext.Data.SaveAsText(testData, fileStream, separatorChar:=","c, headerRow:=True, schema:=True)
                End Using
            End If
        End Sub


        Public Shared Function TrainModel(mlContext As MLContext, trainDataView As IDataView) As ITransformer

            ' Get all the feature column names (All except the Label and the IdPreservationColumn)
            Dim featureColumnNames() As String = trainDataView.Schema.AsQueryable().Select(Function(column) column.Name).Where(Function(name) name <> NameOf(TransactionObservation.Label)).Where(Function(name) name <> "IdPreservationColumn").Where(Function(name) name <> NameOf(TransactionObservation.Time)).ToArray()


            ' Create the data process pipeline
            Dim dataProcessPipeline As IEstimator(Of ITransformer) = mlContext.Transforms.Concatenate("Features", featureColumnNames).Append(mlContext.Transforms.DropColumns(New String() {NameOf(TransactionObservation.Time)})).Append(mlContext.Transforms.NormalizeLpNorm(outputColumnName:="NormalizedFeatures", inputColumnName:="Features"))

            ' In Anomaly Detection, the learner assumes all training examples have label 0, as it only learns from normal examples.
            ' If any of the training examples has label 1, it is recommended to use a Filter transform to filter them out before training:
            Dim normalTrainDataView As IDataView = mlContext.Data.FilterRowsByColumn(trainDataView, columnName:=NameOf(TransactionObservation.Label), lowerBound:=0, upperBound:=1)


            ' (OPTIONAL) Peek data (such as 2 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            ConsoleHelper.PeekDataViewInConsole(mlContext, normalTrainDataView, dataProcessPipeline, 2)
            ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "NormalizedFeatures", normalTrainDataView, dataProcessPipeline, 2)


            Dim options = New RandomizedPcaTrainer.Options With {
                .FeatureColumnName = "NormalizedFeatures",
                .ExampleWeightColumnName = Nothing,
                .Rank = 28,
                .Oversampling = 20,
                .EnsureZeroMean = True,
                .Seed = 1
            }


            ' Create an anomaly detector. Its underlying algorithm is randomized PCA.
            Dim trainer As IEstimator(Of ITransformer) = mlContext.AnomalyDetection.Trainers.RandomizedPca(options:=options)

            Dim trainingPipeline As EstimatorChain(Of ITransformer) = dataProcessPipeline.Append(trainer)

            ConsoleHelper.ConsoleWriteHeader("=============== Training model ===============")

            Dim model As TransformerChain(Of ITransformer) = trainingPipeline.Fit(normalTrainDataView)

            ConsoleHelper.ConsoleWriteHeader("=============== End of training process ===============")

            Return model
        End Function


        Private Shared Sub EvaluateModel(mlContext As MLContext, model As ITransformer, testDataView As IDataView)
            ' Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")

            Dim predictions = model.Transform(testDataView)

            Dim metrics As AnomalyDetectionMetrics = mlContext.AnomalyDetection.Evaluate(predictions)

            ConsoleHelper.PrintAnomalyDetectionMetrics("RandomizedPca", metrics)
        End Sub


        Public Shared Sub InspectData(mlContext As MLContext, data As IDataView, records As Integer)
            ' We want to make sure we have both True and False observations
            Console.WriteLine("Show 4 fraud transactions (true)")
            ShowObservationsFilteredByLabel(mlContext, data, label:=True, count:=records)

            Console.WriteLine("Show 4 NOT-fraud transactions (false)")
            ShowObservationsFilteredByLabel(mlContext, data, label:=False, count:=records)
        End Sub


        Public Shared Sub ShowObservationsFilteredByLabel(mlContext As MLContext, dataView As IDataView, Optional label As Boolean = True, Optional count As Integer = 2)
            ' Convert to an enumerable of user-defined type. 
            Dim data = mlContext.Data.CreateEnumerable(Of TransactionObservation)(dataView, reuseRowObject:=False).Where(Function(x) Math.Abs(x.Label - (If(label, 1, 0))) < Single.Epsilon).Take(count).ToList()

            ' Print to console
            data.ForEach(Sub(row)
                             row.PrintToConsole()
                         End Sub)
        End Sub


#Region "file handeling"

        Public Shared Function GetAbsolutePath(relativePath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)

            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

            Return fullPath
        End Function



        Public Shared Sub UnZipDataSet(zipDataSet As String, destinationFile As String)
            If Not File.Exists(destinationFile) Then
                Dim destinationDirectory = Path.GetDirectoryName(destinationFile)
                ZipFile.ExtractToDirectory(zipDataSet, $"{destinationDirectory}")
            End If
        End Sub


        Private Shared Sub SaveModel(mlContext As MLContext, model As ITransformer, modelFilePath As String, trainingDataSchema As DataViewSchema)
            mlContext.Model.Save(model, trainingDataSchema, modelFilePath)

            Console.WriteLine("Saved model to " & modelFilePath)
        End Sub

#End Region
    End Class
End Namespace
