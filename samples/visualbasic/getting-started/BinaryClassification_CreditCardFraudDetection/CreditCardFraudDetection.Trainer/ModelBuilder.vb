Imports CreditCardFraudDetection.Common
Imports Microsoft.ML
Imports Microsoft.ML.Transforms.Normalizers.NormalizingEstimator
Imports System.IO
Imports Microsoft.ML.Data
Imports Microsoft.Data.DataView

Namespace CreditCardFraudDetection.Trainer
    Public Class ModelBuilder
        Private ReadOnly _assetsPath As String
        Private ReadOnly _dataSetFile As String
        Private ReadOnly _outputPath As String

        Private _context As BinaryClassificationCatalog
        Private _reader As TextLoader
        Private _trainData As IDataView
        Private _testData As IDataView
        Private _mlContext As MLContext

        Public Sub New(mlContext As MLContext, assetsPath As String, dataSetFile As String)
            _mlContext = mlContext
            _assetsPath = assetsPath
            _dataSetFile = dataSetFile
            _outputPath = Path.Combine(_assetsPath, "output")
        End Sub

        Public Function PreProcessData(mlContext As MLContext) As ModelBuilder
            With PrepareData(_mlContext)
                _context = .context
                _reader = .Item2
                _trainData = .trainData
                _testData = .testData
            End With
            Return Me
        End Function

        Public Sub TrainFastTreeAndSaveModels(Optional cvNumFolds As Integer = 2, Optional numLeaves As Integer = 20, Optional numTrees As Integer = 100, Optional minDocumentsInLeafs As Integer = 10, Optional learningRate As Double = 0.2)
            'Create a flexible pipeline (composed by a chain of estimators) for building/traing the model.

            'Get all the column names for the Features (All except the Label and the StratificationColumn)
            Dim featureColumnNames = _trainData.Schema.AsQueryable().Select(Function(column) column.Name).
                Where(Function(name) name <> "Label").
                Where(Function(name) name <> "StratificationColumn").ToArray()

            Dim pipeline = _mlContext.Transforms.Concatenate(DefaultColumnNames.Features, featureColumnNames).
                Append(_mlContext.Transforms.Normalize(inputColumnName:="Features", outputColumnName:="FeaturesNormalizedByMeanVar", mode:=NormalizerMode.MeanVariance)).
                Append(_mlContext.BinaryClassification.Trainers.FastTree(labelColumn:="Label", featureColumn:="Features", numLeaves:=20, numTrees:=100, minDatapointsInLeaves:=10, learningRate:=0.2))

            Dim model = pipeline.Fit(_trainData)

            Dim metrics = _context.Evaluate(model.Transform(_testData), label:="Label")

            ConsoleWriteHeader($"Test Metrics:")
            Console.WriteLine("Acuracy: " & metrics.Accuracy)
            metrics.ToConsole()

            Using fs = New FileStream(Path.Combine(_outputPath, "fastTree.zip"), FileMode.Create, FileAccess.Write, FileShare.Write)
                _mlContext.Model.Save(model, fs)
            End Using

            Console.WriteLine("Saved model to " & Path.Combine(_outputPath, "fastTree.zip"))
        End Sub

        Private Function PrepareData(mlContext As MLContext) As (context As BinaryClassificationCatalog, TextLoader, trainData As IDataView, testData As IDataView)

            Dim data As IDataView = Nothing
            Dim trainData As IDataView = Nothing
            Dim testData As IDataView = Nothing

            Dim columns() As TextLoader.Column = {
                New TextLoader.Column("Label", DataKind.BL, 30),
                New TextLoader.Column("V1", DataKind.R4, 1),
                New TextLoader.Column("V2", DataKind.R4, 2),
                New TextLoader.Column("V3", DataKind.R4, 3),
                New TextLoader.Column("V4", DataKind.R4, 4),
                New TextLoader.Column("V5", DataKind.R4, 5),
                New TextLoader.Column("V6", DataKind.R4, 6),
                New TextLoader.Column("V7", DataKind.R4, 7),
                New TextLoader.Column("V8", DataKind.R4, 8),
                New TextLoader.Column("V9", DataKind.R4, 9),
                New TextLoader.Column("V10", DataKind.R4, 10),
                New TextLoader.Column("V11", DataKind.R4, 11),
                New TextLoader.Column("V12", DataKind.R4, 12),
                New TextLoader.Column("V13", DataKind.R4, 13),
                New TextLoader.Column("V14", DataKind.R4, 14),
                New TextLoader.Column("V15", DataKind.R4, 15),
                New TextLoader.Column("V16", DataKind.R4, 16),
                New TextLoader.Column("V17", DataKind.R4, 17),
                New TextLoader.Column("V18", DataKind.R4, 18),
                New TextLoader.Column("V19", DataKind.R4, 19),
                New TextLoader.Column("V20", DataKind.R4, 20),
                New TextLoader.Column("V21", DataKind.R4, 21),
                New TextLoader.Column("V22", DataKind.R4, 22),
                New TextLoader.Column("V23", DataKind.R4, 23),
                New TextLoader.Column("V24", DataKind.R4, 24),
                New TextLoader.Column("V25", DataKind.R4, 25),
                New TextLoader.Column("V26", DataKind.R4, 26),
                New TextLoader.Column("V27", DataKind.R4, 27),
                New TextLoader.Column("V28", DataKind.R4, 28),
                New TextLoader.Column("Amount", DataKind.R4, 29)
            }

            Dim txtLoaderArgs As TextLoader.Arguments = New TextLoader.Arguments With {
                .Column = columns,
                .HasHeader = True,
                .Separators = {","c}
            }

            ' Step one: read the data as an IDataView.
            ' Create the reader: define the data columns 
            ' and where to find them in the text file.
            Dim reader = New TextLoader(mlContext, txtLoaderArgs)


            ' We know that this is a Binary Classification task,
            ' so we create a Binary Classification context:
            ' it will give us the algorithms we need,
            ' as well as the evaluation procedure.
            Dim classification = New BinaryClassificationCatalog(mlContext)

            If Not File.Exists(Path.Combine(_outputPath, "testData.idv")) AndAlso Not File.Exists(Path.Combine(_outputPath, "trainData.idv")) Then
                ' Split the data 80:20 into train and test sets, train and evaluate.

                data = reader.Read(New MultiFileSource(_dataSetFile))
                ConsoleWriteHeader("Show 4 transactions fraud (true) and 4 transactions not fraud (false) -  (source)")
                InspectData(mlContext, data, 4)

                ' Can't do stratification when column type is a boolean, is this an issue?
                With classification.TrainTestSplit(data, testFraction:=0.2)
                    trainData = .trainSet
                    testData = .testSet
                End With
                ' save test split
                Using fileStream = File.Create(Path.Combine(_outputPath, "testData.csv"))
                    mlContext.Data.SaveAsText(testData, fileStream, separatorChar:=","c, headerRow:=True, schema:=True)
                End Using

                ' save train split 
                Using fileStream = File.Create(Path.Combine(_outputPath, "trainData.csv"))
                    mlContext.Data.SaveAsText(trainData, fileStream, separatorChar:=","c, headerRow:=True, schema:=True)
                End Using

            Else
                'Add the "StratificationColumn" that was added by classification.TrainTestSplit()
                ' And Label is moved to column 0

                Dim columnsPlus() As TextLoader.Column = {
                    New TextLoader.Column("Label", DataKind.BL, 0),
                    New TextLoader.Column("V1", DataKind.R4, 1),
                    New TextLoader.Column("V2", DataKind.R4, 2),
                    New TextLoader.Column("V3", DataKind.R4, 3),
                    New TextLoader.Column("V4", DataKind.R4, 4),
                    New TextLoader.Column("V5", DataKind.R4, 5),
                    New TextLoader.Column("V6", DataKind.R4, 6),
                    New TextLoader.Column("V7", DataKind.R4, 7),
                    New TextLoader.Column("V8", DataKind.R4, 8),
                    New TextLoader.Column("V9", DataKind.R4, 9),
                    New TextLoader.Column("V10", DataKind.R4, 10),
                    New TextLoader.Column("V11", DataKind.R4, 11),
                    New TextLoader.Column("V12", DataKind.R4, 12),
                    New TextLoader.Column("V13", DataKind.R4, 13),
                    New TextLoader.Column("V14", DataKind.R4, 14),
                    New TextLoader.Column("V15", DataKind.R4, 15),
                    New TextLoader.Column("V16", DataKind.R4, 16),
                    New TextLoader.Column("V17", DataKind.R4, 17),
                    New TextLoader.Column("V18", DataKind.R4, 18),
                    New TextLoader.Column("V19", DataKind.R4, 19),
                    New TextLoader.Column("V20", DataKind.R4, 20),
                    New TextLoader.Column("V21", DataKind.R4, 21),
                    New TextLoader.Column("V22", DataKind.R4, 22),
                    New TextLoader.Column("V23", DataKind.R4, 23),
                    New TextLoader.Column("V24", DataKind.R4, 24),
                    New TextLoader.Column("V25", DataKind.R4, 25),
                    New TextLoader.Column("V26", DataKind.R4, 26),
                    New TextLoader.Column("V27", DataKind.R4, 27),
                    New TextLoader.Column("V28", DataKind.R4, 28),
                    New TextLoader.Column("Amount", DataKind.R4, 29),
                    New TextLoader.Column("StratificationColumn", DataKind.R4, 30)
                }

                ' Load splited data
                trainData = mlContext.Data.ReadFromTextFile(Path.Combine(_outputPath, "trainData.csv"), columnsPlus, hasHeader:=txtLoaderArgs.HasHeader, separatorChar:=txtLoaderArgs.Separators(0))


                testData = mlContext.Data.ReadFromTextFile(Path.Combine(_outputPath, "testData.csv"), columnsPlus, hasHeader:=txtLoaderArgs.HasHeader, separatorChar:=txtLoaderArgs.Separators(0))
            End If

            ConsoleWriteHeader("Show 4 transactions fraud (true) and 4 transactions not fraud (false) -  (traindata)")
            InspectData(mlContext, trainData, 4)

            ConsoleWriteHeader("Show 4 transactions fraud (true) and 4 transactions not fraud (false) -  (testData)")
            InspectData(mlContext, testData, 4)

            Return (classification, reader, trainData, testData)
        End Function
    End Class

End Namespace
