Imports Microsoft.ML
Imports System.IO
Imports Microsoft.ML.Data
Imports Console = Colorful.Console
Imports System.Drawing

Namespace MovieRecommenderModel
    '     This movie recommendation model is built on the http://files.grouplens.org/datasets/movielens/ml-latest-small.zip dataset
    '       for improved model performance use the https://grouplens.org/datasets/movielens/1m/ dataset instead. 

    Friend Class Program
        Private Shared BaseModelRelativePath As String = "../../../Model"
        Private Shared ModelRelativePath As String = $"{BaseModelRelativePath}/model.zip"

        Private Shared BaseDataSetRelativepath As String = "../../../Data"
        Private Shared TrainingDataRelativePath As String = $"{BaseDataSetRelativepath}/ratings_train.csv"
        Private Shared TestDataRelativePath As String = $"{BaseDataSetRelativepath}/ratings_test.csv"

        Private Shared TrainingDataLocation As String = GetAbsolutePath(TrainingDataRelativePath)
        Private Shared TestDataLocation As String = GetAbsolutePath(TestDataRelativePath)
        Private Shared ModelPath As String = GetAbsolutePath(ModelRelativePath)

        Shared Sub Main(args() As String)
            Dim color As Color = Color.FromArgb(130, 150, 115)

            'Call the following piece of code for splitting the ratings.csv into ratings_train.csv and ratings.test.csv.
            ' Program.DataPrep();

            'STEP 1: Create MLContext to be shared across the model creation workflow objects
            Dim mlContext As MLContext = New MLContext

            'STEP 2: Read data from text file using TextLoader by defining the schema for reading the movie recommendation datasets and return dataview.
            Dim trainingDataView = mlContext.Data.LoadFromTextFile(Of MovieRating)(path:=TrainingDataLocation, hasHeader:=True, separatorChar:=","c)

            Console.WriteLine("=============== Reading Input Files ===============", color)
            Console.WriteLine()

            ' ML.NET doesn't cache data set by default. Therefore, if one reads a data set from a file and accesses it many times, it can be slow due to
            ' expensive featurization and disk operations. When the considered data can fit into memory, a solution is to cache the data in memory. Caching is especially
            ' helpful when working with iterative algorithms which needs many data passes. Since SDCA is the case, we cache. Inserting a
            ' cache step in a pipeline is also possible, please see the construction of pipeline below.
            trainingDataView = mlContext.Data.Cache(trainingDataView)

            Console.WriteLine("=============== Transform Data And Preview ===============", color)
            Console.WriteLine()

            'STEP 4: Transform your data by encoding the two features userId and movieID.
            '        These encoded features will be provided as input to FieldAwareFactorizationMachine learner
            Dim dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName:="userIdFeaturized", inputColumnName:=NameOf(MovieRating.userId)).Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName:="movieIdFeaturized", inputColumnName:=NameOf(MovieRating.movieId)).Append(mlContext.Transforms.Concatenate("Features", "userIdFeaturized", "movieIdFeaturized")))
            Common.ConsoleHelper.PeekDataViewInConsole(mlContext, trainingDataView, dataProcessPipeline, 10)

            ' STEP 5: Train the model fitting to the DataSet
            Console.WriteLine("=============== Training the model ===============", color)
            Console.WriteLine()
            Dim trainingPipeLine = dataProcessPipeline.Append(mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine(New String() {"Features"}))
            Dim model = trainingPipeLine.Fit(trainingDataView)

            'STEP 6: Evaluate the model performance
            Console.WriteLine("=============== Evaluating the model ===============", color)
            Console.WriteLine()
            Dim testDataView = mlContext.Data.LoadFromTextFile(Of MovieRating)(path:=TestDataLocation, hasHeader:=True, separatorChar:=","c)

            Dim prediction = model.Transform(testDataView)

            Dim metrics = mlContext.BinaryClassification.Evaluate(data:=prediction, labelColumnName:="Label", scoreColumnName:="Score", predictedLabelColumnName:="PredictedLabel")
            Console.WriteLine("Evaluation Metrics: acc:" & Math.Round(metrics.Accuracy, 2) & " AreaUnderRocCurve(AUC):" & Math.Round(metrics.AreaUnderRocCurve, 2), color)

            'STEP 7:  Try/test a single prediction by predicting a single movie rating for a specific user
            Console.WriteLine("=============== Test a single prediction ===============", color)
            Console.WriteLine()
            Dim predictionEngine = mlContext.Model.CreatePredictionEngine(Of MovieRating, MovieRatingPrediction)(model)
            Dim testData As MovieRating = New MovieRating With {
                .userId = "6",
                .movieId = "10"
            }

            Dim movieRatingPrediction = predictionEngine.Predict(testData)
            Console.WriteLine($"UserId:{testData.userId} with movieId: {testData.movieId} Score:{Sigmoid(movieRatingPrediction.Score)} and Label {movieRatingPrediction.PredictedLabel}", Color.YellowGreen)
            Console.WriteLine()

            'STEP 8:  Save model to disk
            Console.WriteLine("=============== Writing model to the disk ===============", color)
            Console.WriteLine()
            mlContext.Model.Save(model, trainingDataView.Schema, ModelPath)

            Console.WriteLine("=============== Re-Loading model from the disk ===============", color)
            Console.WriteLine()
            Dim trainedModel As ITransformer
            Using stream As New FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim modelInputSchema As Object
                trainedModel = mlContext.Model.Load(stream, modelInputSchema)
            End Using

            Console.WriteLine("Press any key ...")
            Console.Read()
        End Sub

        '        
        '         * FieldAwareFactorizationMachine the learner used in this example requires the problem to setup as a binary classification problem.
        '         * The DataPrep method performs two tasks:
        '         * 1. It goes through all the ratings and replaces the ratings > 3 as 1, suggesting a movie is recommended and ratings < 3 as 0, suggesting
        '              a movie is not recommended
        '           2. This piece of code also splits the ratings.csv into rating-train.csv and ratings-test.csv used for model training and testing respectively.
        '         
        Public Shared Sub DataPrep()

            Dim dataset() As String = File.ReadAllLines(".\Data\ratings.csv")

            Dim new_dataset(dataset.Length - 1) As String
            new_dataset(0) = dataset(0)
            For i As Integer = 1 To dataset.Length - 1
                Dim line As String = dataset(i)
                Dim lineSplit() As String = line.Split(","c)
                Dim rating As Double = Double.Parse(lineSplit(2))
                rating = If(rating > 3, 1, 0)
                lineSplit(2) = rating.ToString()
                Dim new_line As String = String.Join(","c, lineSplit)
                new_dataset(i) = new_line
            Next i
            dataset = new_dataset
            Dim numLines As Integer = dataset.Length
            Dim body = dataset.Skip(1)
            Dim sorted = body.Select(Function(line) New With {
                Key .SortKey = Int32.Parse(line.Split(","c)(3)),
                Key .Line = line
            }).OrderBy(Function(x) x.SortKey).Select(Function(x) x.Line)
            File.WriteAllLines("../../../Data\ratings_train.csv", dataset.Take(1).Concat(sorted.Take(CInt(Math.Truncate(numLines * 0.9)))))
            File.WriteAllLines("../../../Data\ratings_test.csv", dataset.Take(1).Concat(sorted.TakeLast(CInt(Math.Truncate(numLines * 0.1)))))
        End Sub

        Public Shared Function Sigmoid(x As Single) As Single
            Return CSng(100 / (1 + Math.Exp(-x)))
        End Function

        Public Shared Function GetAbsolutePath(relativeDatasetPath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativeDatasetPath)

            Return fullPath
        End Function
    End Class


    Public Class MovieRating
        <LoadColumn(0)>
        Public userId As String

        <LoadColumn(1)>
        Public movieId As String

        <LoadColumn(2)>
        Public Label As Boolean
    End Class

    Public Class MovieRatingPrediction
        Public PredictedLabel As Boolean

        Public Score As Single
    End Class
End Namespace
