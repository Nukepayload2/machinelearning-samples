Imports Microsoft.ML
Imports MovieRecommendationConsoleApp.DataStructures
Imports MovieRecommendation.DataStructures
Imports System.IO
Imports Microsoft.ML.Trainers

Namespace MovieRecommendation
    Friend Class Program
        ' Using the ml-latest-small.zip as dataset from https://grouplens.org/datasets/movielens/. 
        Private Shared ModelsRelativePath As String = "../../../../MLModels"
        Public Shared DatasetsRelativePath As String = "../../../../Data"

        Private Shared TrainingDataRelativePath As String = $"{DatasetsRelativePath}/recommendation-ratings-train.csv"
        Private Shared TestDataRelativePath As String = $"{DatasetsRelativePath}/recommendation-ratings-test.csv"
        Private Shared MoviesDataLocation As String = $"{DatasetsRelativePath}/movies.csv"

        Private Shared TrainingDataLocation As String = GetAbsolutePath(TrainingDataRelativePath)
        Private Shared TestDataLocation As String = GetAbsolutePath(TestDataRelativePath)

        Private Shared ModelPath As String = GetAbsolutePath(ModelsRelativePath)

        Private Const predictionuserId As Single = 6
        Private Const predictionmovieId As Integer = 10

        Shared Sub Main(args() As String)
            'STEP 1: Create MLContext to be shared across the model creation workflow objects 
            Dim mlcontext As MLContext = New MLContext

            'STEP 2: Read the training data which will be used to train the movie recommendation model    
            'The schema for training data is defined by type 'TInput' in LoadFromTextFile<TInput>() method.
            Dim trainingDataView As IDataView = mlcontext.Data.LoadFromTextFile(Of MovieRating)(TrainingDataLocation, hasHeader:=True, separatorChar:=","c)

            'STEP 3: Transform your data by encoding the two features userId and movieID. These encoded features will be provided as input
            '        to our MatrixFactorizationTrainer.
            Dim dataProcessingPipeline = mlcontext.Transforms.Conversion.MapValueToKey(outputColumnName:="userIdEncoded", inputColumnName:=NameOf(MovieRating.userId)).Append(mlcontext.Transforms.Conversion.MapValueToKey(outputColumnName:="movieIdEncoded", inputColumnName:=NameOf(MovieRating.movieId)))

            'Specify the options for MatrixFactorization trainer            
            Dim options As MatrixFactorizationTrainer.Options = New MatrixFactorizationTrainer.Options
            options.MatrixColumnIndexColumnName = "userIdEncoded"
            options.MatrixRowIndexColumnName = "movieIdEncoded"
            options.LabelColumnName = "Label"
            options.NumberOfIterations = 20
            options.ApproximationRank = 100

            'STEP 4: Create the training pipeline 
            Dim trainingPipeLine = dataProcessingPipeline.Append(mlcontext.Recommendation().Trainers.MatrixFactorization(options))

            'STEP 5: Train the model fitting to the DataSet
            Console.WriteLine("=============== Training the model ===============")
            Dim model As ITransformer = trainingPipeLine.Fit(trainingDataView)

            'STEP 6: Evaluate the model performance 
            Console.WriteLine("=============== Evaluating the model ===============")
            Dim testDataView As IDataView = mlcontext.Data.LoadFromTextFile(Of MovieRating)(TestDataLocation, hasHeader:=True, separatorChar:=","c)
            Dim prediction = model.Transform(testDataView)
            Dim metrics = mlcontext.Regression.Evaluate(prediction, labelColumnName:="Label", scoreColumnName:="Score")
            Console.WriteLine("The model evaluation metrics RootMeanSquaredError:" & metrics.RootMeanSquaredError)

            'STEP 7:  Try/test a single prediction by predicting a single movie rating for a specific user
            Dim predictionengine = mlcontext.Model.CreatePredictionEngine(Of MovieRating, MovieRatingPrediction)(model)
            '             Make a single movie rating prediction, the scores are for a particular user and will range from 1 - 5. 
            '               The higher the score the higher the likelyhood of a user liking a particular movie.
            '               You can recommend a movie to a user if say rating > 3.5.
            Dim movieratingprediction = predictionengine.Predict(New MovieRating With {
                .userId = predictionuserId,
                .movieId = predictionmovieId
            })

            Dim movieService As Movie = New Movie
            Console.WriteLine("For userId:" & predictionuserId & " movie rating prediction (1 - 5 stars) for movie:" & movieService.Get(predictionmovieId).movieTitle & " is:" & Math.Round(movieratingprediction.Score, 1))

            Console.WriteLine("=============== End of process, hit any key to finish ===============")
            Console.ReadLine()
        End Sub

        Public Shared Function GetAbsolutePath(relativePath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

            Return fullPath
        End Function
    End Class
End Namespace
