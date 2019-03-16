Imports Microsoft.ML
Imports System.IO
Imports Microsoft.ML.Data
Imports Microsoft.Data.DataView
Imports Common
Imports Microsoft.ML.TrainCatalogBase
Imports CreditCardFraudDetection.Common.DataModels
Imports System.IO.Compression
Imports Microsoft.ML.Transforms.NormalizingEstimator

Namespace CreditCardFraudDetection.Trainer
    Friend Class Program
        Shared Sub Main(args() As String)
            'File paths
            Dim AssetsRelativePath As String = "../../../assets"
            Dim assetsPath As String = GetAbsolutePath(AssetsRelativePath)
            Dim zipDataSet As String = Path.Combine(assetsPath, "input", "creditcardfraud-dataset.zip")
            Dim fullDataSetFilePath As String = Path.Combine(assetsPath, "input", "creditcard.csv")
            Dim trainDataSetFilePath As String = Path.Combine(assetsPath, "output", "trainData.csv")
            Dim testDataSetFilePath As String = Path.Combine(assetsPath, "output", "testData.csv")
            Dim modelFilePath As String = Path.Combine(assetsPath, "output", "fastTree.zip")
            '

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
            Dim model As ITransformer
            Dim trainerName As String
            With TrainModel(mlContext, trainingDataView)
                model = .model
                trainerName = .trainerName
            End With

            ' Evaluate quality of Model
            EvaluateModel(mlContext, model, testDataView, trainerName)

            ' Save model
            SaveModel(mlContext, model, modelFilePath)

            Console.WriteLine("=============== Press any key ===============")
            Console.ReadKey()
        End Sub

        Public Shared Sub PrepDatasets(mlContext As MLContext, fullDataSetFilePath As String, trainDataSetFilePath As String, testDataSetFilePath As String)
            'Only prep-datasets if train and test datasets don't exist yet

            If Not File.Exists(trainDataSetFilePath) AndAlso Not File.Exists(testDataSetFilePath) Then
                Console.WriteLine("===== Preparing train/test datasets =====")

                'Load the original single dataset
                Dim originalFullData As IDataView = mlContext.Data.LoadFromTextFile(Of TransactionObservation)(fullDataSetFilePath, separatorChar:=","c, hasHeader:=True)

                ' Split the data 80:20 into train and test sets, train and evaluate.
                Dim trainTestData As TrainTestData = mlContext.BinaryClassification.TrainTestSplit(originalFullData, testFraction:=0.2, seed:=1)
                Dim trainData As IDataView = trainTestData.TrainSet
                Dim testData As IDataView = trainTestData.TestSet

                'Inspect TestDataView to make sure there are true and false observations in test dataset, after spliting 
                InspectData(mlContext, testData, 4)

                ' save train split
                Using fileStream = File.Create(trainDataSetFilePath)
                    mlContext.Data.SaveAsText(trainData, fileStream, separatorChar:=","c, headerRow:=True, schema:=True)
                End Using

                ' save test split 
                Using fileStream = File.Create(testDataSetFilePath)
                    mlContext.Data.SaveAsText(testData, fileStream, separatorChar:=","c, headerRow:=True, schema:=True)
                End Using
            End If
        End Sub

        Public Shared Function TrainModel(mlContext As MLContext, trainDataView As IDataView) As (model As ITransformer, trainerName As String)
            'Get all the feature column names (All except the Label and the IdPreservationColumn)
            Dim featureColumnNames() As String = trainDataView.Schema.AsQueryable().Select(Function(column) column.Name).Where(Function(name) name <> NameOf(TransactionObservation.Label)).Where(Function(name) name <> "IdPreservationColumn").Where(Function(name) name <> "Time").ToArray()

            ' Create the data process pipeline
            Dim dataProcessPipeline As IEstimator(Of ITransformer) = mlContext.Transforms.Concatenate(DefaultColumnNames.Features, featureColumnNames).Append(mlContext.Transforms.DropColumns(New String() {"Time"})).Append(mlContext.Transforms.Normalize(inputColumnName:=DefaultColumnNames.Features, outputColumnName:="FeaturesNormalizedByMeanVar", mode:=NormalizerMode.MeanVariance))

            ' (OPTIONAL) Peek data (such as 2 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            ConsoleHelper.PeekDataViewInConsole(mlContext, trainDataView, dataProcessPipeline, 2)
            ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, DefaultColumnNames.Features, trainDataView, dataProcessPipeline, 1)

            ' Set the training algorithm
            Dim trainer As IEstimator(Of ITransformer) = mlContext.BinaryClassification.Trainers.FastTree(labelColumnName:=NameOf(TransactionObservation.Label), featureColumnName:="FeaturesNormalizedByMeanVar", numLeaves:=20, numTrees:=100, minDatapointsInLeaves:=10, learningRate:=0.2)

            Dim trainingPipeline As IEstimator(Of ITransformer) = dataProcessPipeline.Append(trainer)

            ConsoleHelper.ConsoleWriteHeader("=============== Training model ===============")

            Dim model As ITransformer = trainingPipeline.Fit(trainDataView)

            ConsoleHelper.ConsoleWriteHeader("=============== End of training process ===============")

            Return (model, DirectCast(trainer, Object).ToString())

        End Function

        Private Shared Sub EvaluateModel(mlContext As MLContext, model As ITransformer, testDataView As IDataView, trainerName As String)
            ' Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
            Dim predictions = model.Transform(testDataView)

            Dim metrics = mlContext.BinaryClassification.Evaluate(data:=predictions, label:=NameOf(TransactionObservation.Label), score:=DefaultColumnNames.Score)

            ConsoleHelper.PrintBinaryClassificationMetrics(trainerName, metrics)

        End Sub

        Public Shared Function GetAbsolutePath(relativePath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

            Return fullPath
        End Function

        Public Shared Sub InspectData(mlContext As MLContext, data As IDataView, records As Integer)
            'We want to make sure we have True and False observations

            Console.WriteLine("Show 4 fraud transactions (true)")
            ShowObservationsFilteredByLabel(mlContext, data, label:=True, count:=records)

            Console.WriteLine("Show 4 NOT-fraud transactions (false)")
            ShowObservationsFilteredByLabel(mlContext, data, label:=False, count:=records)
        End Sub

        Public Shared Sub ShowObservationsFilteredByLabel(mlContext As MLContext, dataView As IDataView, Optional label As Boolean = True, Optional count As Integer = 2)
            ' Convert to an enumerable of user-defined type. 
            Dim data = mlContext.Data.CreateEnumerable(Of TransactionObservation)(dataView, reuseRowObject:=False).Where(Function(x) x.Label = label).Take(count).ToList()

            ' print to console
            data.ForEach(Sub(row)
                             row.PrintToConsole()
                         End Sub)
        End Sub

        Public Shared Sub UnZipDataSet(zipDataSet As String, destinationFile As String)
            If Not File.Exists(destinationFile) Then
                Dim destinationDirectory = Path.GetDirectoryName(destinationFile)
                ZipFile.ExtractToDirectory(zipDataSet, $"{destinationDirectory}")
            End If
        End Sub

        Private Shared Sub SaveModel(mlContext As MLContext, model As ITransformer, modelFilePath As String)
            Using fs = New FileStream(modelFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)
                mlContext.Model.Save(model, fs)
            End Using

            Console.WriteLine("Saved model to " & modelFilePath)
        End Sub
    End Class
End Namespace
