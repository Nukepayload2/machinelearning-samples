Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports Microsoft.ML.Trainers

Namespace ProductRecommender
    Friend Class Program
        '1. Do remember to replace amazon0302.txt with dataset from https://snap.stanford.edu/data/amazon0302.html
        '2. Replace column names with ProductID and CoPurchaseProductID. It should look like this:
        '   ProductID	ProductID_Copurchased
        '   0	1
        '   0  2
        Private Shared TrainingDataLocation As String = $"./Data/Amazon0302.txt"

        Private Shared ModelPath As String = $"./Model/model.zip"

        Shared Sub Main(ByVal args() As String)
            'STEP 1: Create MLContext to be shared across the model creation workflow objects 
            Dim ctx = New MLContext()

            'STEP 2: Create a reader by defining the schema for reading the product co-purchase dataset
            '        Do remember to replace amazon0302.txt with dataset from https://snap.stanford.edu/data/amazon0302.html
            Dim reader = ctx.Data.CreateTextReader({
                    New TextLoader.Column("Label", DataKind.R4, 0),
                    New TextLoader.Column("ProductID", DataKind.U4, New TextLoader.Range() {New TextLoader.Range(0)}, New KeyRange(0, 262110)),
                    New TextLoader.Column("CoPurchaseProductID", DataKind.U4, New TextLoader.Range() {New TextLoader.Range(1)}, New KeyRange(0, 262110))
                },
                separatorChar:=vbTab,
                hasHeader:=True
            )

            'STEP 3: Read the training data which will be used to train the movie recommendation model
            Dim traindata = reader.Read(New MultiFileSource(TrainingDataLocation))


            'STEP 4: Your data is already encoded so all you need to do is call the MatrixFactorization Trainer with a few extra hyperparameters
            '        LossFunction, Alpa, Lambda and a few others like K and C as shown below. 
            Dim est = ctx.Recommendation().Trainers.MatrixFactorization("ProductID", "CoPurchaseProductID", labelColumn:="Label", advancedSettings:=Sub(s)
                                                                                                                                                        s.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass
                                                                                                                                                        s.Alpha = 0.01
                                                                                                                                                        s.Lambda = 0.025
                                                                                                                                                        ' For better results use the following parameters
                                                                                                                                                        's.K = 100;
                                                                                                                                                        's.C = 0.00001;
                                                                                                                                                    End Sub)

            'STEP 5: Train the model fitting to the DataSet
            'Please add Amazon0302.txt dataset from https://snap.stanford.edu/data/amazon0302.html to Data folder if FileNotFoundException is thrown.
            Dim model = est.Fit(traindata)


            'STEP 6: Create prediction engine and predict the score for Product 63 being co-purchased with Product 3.
            '        The higher the score the higher the probability for this particular productID being co-purchased 
            Dim predictionengine = model.CreatePredictionEngine(Of ProductEntry, Copurchase_prediction)(ctx)
            Dim prediction = predictionengine.Predict(New ProductEntry() With {
                .ProductID = 3,
                .CoPurchaseProductID = 63
            })
        End Sub

        Public Class Copurchase_prediction
            Public Property Score() As Single
        End Class

        Public Class ProductEntry
            <KeyType(Contiguous:=True, Count:=262111, Min:=0)>
            Public Property ProductID() As UInteger

            <KeyType(Contiguous:=True, Count:=262111, Min:=0)>
            Public Property CoPurchaseProductID() As UInteger
        End Class
    End Class
End Namespace
