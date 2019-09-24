Imports Common
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.ML
Imports Microsoft.ML.Transforms
Imports System.IO
Imports System.Net

Namespace DatabaseIntegration
    Public Class Program
        ' The url for the dataset that will be downloaded
        Public Shared datasetUrl As String = "https://raw.githubusercontent.com/dotnet/machinelearning/244a8c2ac832657af282aa312d568211698790aa/test/data/adult.train"

        Public Shared Iterator Function ReadRemoteDataset(url As String) As IEnumerable(Of String)
            Using client = New WebClient
                Using stream = client.OpenRead(url)
                    Using reader = New StreamReader(stream)
                        Dim line As String
                        line = reader.ReadLine()
                        'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
                        'ORIGINAL LINE: while ((line = reader.ReadLine()) != null)
                        Do While line IsNot Nothing
                            Yield line
                            line = reader.ReadLine()
                        Loop
                    End Using
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Wrapper function that performs the database query and returns an IEnumerable, creating
        ''' a database context each time.
        ''' </summary>
        ''' <remarks>
        ''' ML.Net can traverse an IEnumerable with multiple threads. This will result in Entity Core Framwork throwing an exception
        ''' as multiple threads cannot access the same database context. To work around this, create a database context
        ''' each time a IEnumerable is requested.
        ''' </remarks>
        ''' <returns>An IEnumerable of the resulting data.</returns>
        Private Shared Iterator Function QueryData() As IEnumerable(Of AdultCensus)
            Using db = New AdultCensusContext
                ' Query our training data from the database. This query is selecting everything from the AdultCensus table. The
                ' result is then loaded by ML.Net through the LoadFromEnumerable. LoadFromEnumerable returns an IDataView which
                ' can be consumed by an ML.Net pipeline.
                ' NOTE: For training, ML.Net requires that the training data is processed in the same order to produce consistent results.
                ' Therefore we are sorting the data by the AdultCensusId, which is an auto-generated id.
                ' NOTE: That the query used here sets the query tracking behavior to be NoTracking, this is particularly useful because
                ' our scenarios only require read-only access.
                For Each adult In db.AdultCensus.AsNoTracking().OrderBy(Function(x) x.AdultCensusId)
                    Yield adult
                Next adult
            End Using
        End Function

        ''' <summary>
        ''' Populates the database with the specified dataset url.
        ''' </summary>
        Public Shared Sub CreateDatabase(url As String)
            Dim dataset = ReadRemoteDataset(url)
            Using db = New AdultCensusContext
                ' Ensure that we have a clean database to start with.
                db.Database.EnsureDeleted()
                db.Database.EnsureCreated()
                Console.WriteLine($"Database created, populating...")

                ' Parse the dataset.
                Dim data = dataset.Skip(1).Select(Function(l) l.Split(","c)).Where(Function(row) row.Length > 1).Select(Function(row) New AdultCensus With {
                    .Age = Integer.Parse(row(0)),
                    .Workclass = row(1),
                    .Education = row(3),
                    .MaritalStatus = row(5),
                    .Occupation = row(6),
                    .Relationship = row(7),
                    .Race = row(8),
                    .Sex = row(9),
                    .CapitalGain = row(10),
                    .CapitalLoss = row(11),
                    .HoursPerWeek = Integer.Parse(row(12)),
                    .NativeCountry = row(13),
                    .Label = If(Integer.Parse(row(14)) = 1, True, False)
                })

                ' Add the data into the database
                db.AdultCensus.AddRange(data)

                Dim count = db.SaveChanges()
                Console.WriteLine($"Total count of items saved to database: {count}")
            End Using
        End Sub

        Public Shared Sub Main()
            ' Seed the database with the dataset.
            CreateDatabase(datasetUrl)
            Dim mlContext = New MLContext(seed:=1)

            ''' Query the data from the database, please see <see cref="QueryData"/> for more information.
            Dim dataView = mlContext.Data.LoadFromEnumerable(QueryData())
            ''' Creates the training and testing data sets.
            Dim trainTestData = mlContext.Data.TrainTestSplit(dataView)

            Dim pipeline = mlContext.Transforms.Categorical.OneHotEncoding({
                New InputOutputColumnPair("MsOHE", "MaritalStatus"),
                New InputOutputColumnPair("OccOHE", "Occupation"),
                New InputOutputColumnPair("RelOHE", "Relationship"),
                New InputOutputColumnPair("SOHE", "Sex"),
                New InputOutputColumnPair("NatOHE", "NativeCountry")
            }, OneHotEncodingEstimator.OutputKind.Binary).Append(mlContext.Transforms.Concatenate("Features", "MsOHE", "OccOHE", "RelOHE", "SOHE", "NatOHE")).Append(mlContext.BinaryClassification.Trainers.LightGbm())

            Console.WriteLine("Training model...")
            Dim model = pipeline.Fit(trainTestData.TrainSet)

            Console.WriteLine("Predicting...")

            ' Now that the model is trained, we want to test it's prediction results, which is done by using a test dataset
            Dim predictions = model.Transform(trainTestData.TestSet)

            ' Now that we have the predictions, calculate the metrics of those predictions and output the results.
            Dim metrics = mlContext.BinaryClassification.Evaluate(predictions)
            ConsoleHelper.PrintBinaryClassificationMetrics("Database Example", metrics)
            ConsoleHelper.ConsolePressAnyKey()
        End Sub
    End Class
End Namespace