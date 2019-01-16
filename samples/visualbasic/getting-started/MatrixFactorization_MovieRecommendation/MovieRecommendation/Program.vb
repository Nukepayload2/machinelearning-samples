Imports System
Imports Microsoft.ML
Imports Microsoft.ML.Trainers.Recommender
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Trainers

Imports MovieRecommendationConsoleApp.DataStructures
Imports MovieRecommendation.DataStructures
Imports Microsoft.ML.Data

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

        Shared Sub Main(ByVal args() As String)
            'STEP 1: Create MLContext to be shared across the model creation workflow objects 
            Dim mlcontext = New MLContext()

            'STEP 2: Create a reader by defining the schema for reading the movie recommendation datasets
            Dim reader = mlcontext.Data.CreateTextReader({
                    New TextLoader.Column("userId", DataKind.R4, 0),
                    New TextLoader.Column("movieId", DataKind.R4, 1),
                    New TextLoader.Column("Label", DataKind.R4, 2)
                },
                separatorChar:=",",
                hasHeader:=True
            )

            'STEP 3: Read the training data which will be used to train the movie recommendation model
            Dim trainingDataView As IDataView = reader.Read(TrainingDataLocation)

            'STEP 4: Transform your data by encoding the two features userId and movieID. These encoded features will be provided as input
            '        to our MatrixFactorizationTrainer.
            Dim pipeline = mlcontext.Transforms.Conversion.MapValueToKey("userId", "userIdEncoded").
                Append(mlcontext.Transforms.Conversion.MapValueToKey("movieId", "movieIdEncoded").
                Append(mlcontext.Recommendation.Trainers.MatrixFactorization("userIdEncoded", "movieIdEncoded", "Label",
                advancedSettings:=Sub(s)
                                      s.NumIterations = 20
                                      s.K = 100
                                  End Sub)))

            'STEP 5: Train the model fitting to the DataSet
            Console.WriteLine("=============== Training the model ===============")
            Dim model = pipeline.Fit(trainingDataView)

            'STEP 6: Evaluate the model performance 
            Console.WriteLine("=============== Evaluating the model ===============")
            Dim testDataView As IDataView = reader.Read(TestDataLocation)
            Dim prediction = model.Transform(testDataView)
            Dim metrics = mlcontext.Regression.Evaluate(prediction, label:="Label", score:="Score")
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
        End Sub

    End Class
End Namespace
