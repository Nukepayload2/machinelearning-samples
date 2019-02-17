Imports Microsoft.ML

Imports MovieRecommendationConsoleApp.DataStructures
Imports MovieRecommendation.DataStructures
Imports Microsoft.ML.Data
Imports Microsoft.Data.DataView
Imports Microsoft.ML.Trainers
Imports Microsoft.ML.Core.Data

Namespace MovieRecommendation
    Friend Class Program
        ' Using the ml-latest-small.zip as dataset from https://grouplens.org/datasets/movielens/. 
        Private Shared ModelsLocation As String = "../../../../MLModels"
        Public Shared DatasetsLocation As String = "../../../../Data"
        Private Shared TrainingDataLocation As String = $"{DatasetsLocation}/recommendation-ratings-train.csv"
        Private Shared TestDataLocation As String = $"{DatasetsLocation}/recommendation-ratings-test.csv"
        Private Shared MoviesDataLocation As String = $"{DatasetsLocation}/movies.csv"
        Private Const predictionuserId As Single = 6
        Private Const predictionmovieId As Integer = 10
        Private Shared userIdEncoded As String = NameOf(userIdEncoded)
        Private Shared movieIdEncoded As String = NameOf(movieIdEncoded)

        Shared Sub Main(ByVal args() As String)
            'STEP 1: Create MLContext to be shared across the model creation workflow objects 
            Dim mlcontext As New MLContext()

            'STEP 2: Read the training data which will be used to train the movie recommendation model    
            'The schema for training data is defined by type 'TInput' in ReadFromTextFile<TInput>() method.
            Dim trainingDataView As IDataView = mlcontext.Data.ReadFromTextFile(Of MovieRating)(TrainingDataLocation, hasHeader:=True, separatorChar:=","c)

            'STEP 3: Transform your data by encoding the two features userId and movieID. These encoded features will be provided as input
            '        to our MatrixFactorizationTrainer.
            Dim dataProcessingPipeline = mlcontext.Transforms.Conversion.MapValueToKey(outputColumnName:=userIdEncoded, inputColumnName:=NameOf(MovieRating.userId)).Append(mlcontext.Transforms.Conversion.MapValueToKey(outputColumnName:=movieIdEncoded, inputColumnName:=NameOf(MovieRating.movieId)))

            'Specify the options for MatrixFactorization trainer
            Dim options As New MatrixFactorizationTrainer.Options()
            options.MatrixColumnIndexColumnName = userIdEncoded
            options.MatrixRowIndexColumnName = movieIdEncoded
            options.LabelColumnName = DefaultColumnNames.Label
            options.NumIterations = 20
            options.K = 100

            'STEP 4: Create the training pipeline 
            Dim trainingPipeLine = dataProcessingPipeline.Append(mlcontext.Recommendation().Trainers.MatrixFactorization(options))

            'STEP 5: Train the model fitting to the DataSet
            Console.WriteLine("=============== Training the model ===============")
            Dim model As ITransformer = trainingPipeLine.Fit(trainingDataView)

            'STEP 6: Evaluate the model performance 
            Console.WriteLine("=============== Evaluating the model ===============")
            Dim testDataView As IDataView = mlcontext.Data.ReadFromTextFile(Of MovieRating)(TestDataLocation, hasHeader:=True)
            Dim prediction = model.Transform(testDataView)
            Dim metrics = mlcontext.Regression.Evaluate(prediction, label:=DefaultColumnNames.Label, score:=DefaultColumnNames.Score)
            'Console.WriteLine("The model evaluation metrics rms:" + Math.Round(float.Parse(metrics.Rms.ToString()), 1));

            'STEP 7:  Try/test a single prediction by predicting a single movie rating for a specific user
            Dim predictionengine = model.CreatePredictionEngine(Of MovieRating, MovieRatingPrediction)(mlcontext)
            '             Make a single movie rating prediction, the scores are for a particular user and will range from 1 - 5. 
            '               The higher the score the higher the likelyhood of a user liking a particular movie.
            '               You can recommend a movie to a user if say rating > 3.5.
            Dim movieratingprediction = predictionengine.Predict(New MovieRating() With {
                .userId = predictionuserId,
                .movieId = predictionmovieId
            })

            Dim movieService As New Movie()
            Console.WriteLine("For userId:" & predictionuserId & " movie rating prediction (1 - 5 stars) for movie:" & movieService.Get(predictionmovieId).movieTitle & " is:" & Math.Round(movieratingprediction.Score, 1))

            Console.WriteLine("=============== End of process, hit any key to finish ===============")
            Console.ReadLine()
        End Sub
    End Class

End Namespace
