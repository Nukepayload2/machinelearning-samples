Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Data
Imports Microsoft.ML.Trainers

Namespace ProductRecommender
    Friend Module Program
        '1. Do remember to replace amazon0302.txt with dataset from https://snap.stanford.edu/data/amazon0302.html
        '2. Replace column names with ProductID and CoPurchaseProductID. It should look like this:
        '   ProductID	ProductID_Copurchased
        '   0	1
        '   0  2
        Private TrainingDataLocation As String = $"./Data/Amazon0302.txt"

        Private ModelPath As String = $"./Model/model.zip"

        Sub Main(ByVal args() As String)
            'STEP 1: Create MLContext to be shared across the model creation workflow objects 
            Dim mlContext As New MLContext()

            'STEP 2: Read the trained data using TextLoader by defining the schema for reading the product co-purchase dataset
            '        Do remember to replace amazon0302.txt with dataset from https://snap.stanford.edu/data/amazon0302.html
            Dim traindata = mlContext.Data.ReadFromTextFile(path:=TrainingDataLocation, columns:={
                New TextLoader.Column(DefaultColumnNames.Label, DataKind.R4, 0),
                New TextLoader.Column(name:=NameOf(ProductEntry.ProductID), type:=DataKind.U4, source:=New TextLoader.Range() {New TextLoader.Range(0)}, keyCount:=New KeyCount(262111)),
                New TextLoader.Column(name:=NameOf(ProductEntry.CoPurchaseProductID), type:=DataKind.U4, source:=New TextLoader.Range() {New TextLoader.Range(1)}, keyCount:=New KeyCount(262111))
            }, hasHeader:=True, separatorChar:=vbTab)

            'STEP 3: Your data is already encoded so all you need to do is specify options for MatrxiFactorizationTrainer with a few extra hyperparameters
            '        LossFunction, Alpa, Lambda and a few others like K and C as shown below and call the trainer. 
            Dim options As New MatrixFactorizationTrainer.Options()
            options.MatrixColumnIndexColumnName = NameOf(ProductEntry.ProductID)
            options.MatrixRowIndexColumnName = NameOf(ProductEntry.CoPurchaseProductID)
            options.LabelColumnName = DefaultColumnNames.Label
            options.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass
            options.Alpha = 0.01
            options.Lambda = 0.025
            ' For better results use the following parameters
            'options.K = 100;
            'options.C = 0.00001;

            'Step 4: Call the MatrixFactorization trainer by passing options.
            Dim est = mlContext.Recommendation().Trainers.MatrixFactorization(options)

            'STEP 5: Train the model fitting to the DataSet
            'Please add Amazon0302.txt dataset from https://snap.stanford.edu/data/amazon0302.html to Data folder if FileNotFoundException is thrown.
            Dim model As ITransformer = est.Fit(traindata)

            'STEP 6: Create prediction engine and predict the score for Product 63 being co-purchased with Product 3.
            '        The higher the score the higher the probability for this particular productID being co-purchased 
            Dim predictionengine = model.CreatePredictionEngine(Of ProductEntry, Copurchase_prediction)(mlContext)
            Dim prediction = predictionengine.Predict(New ProductEntry() With {
                .ProductID = 3,
                .CoPurchaseProductID = 63
            })

            Console.WriteLine(vbLf & " For ProductID = 3 and  CoPurchaseProductID = 63 the predicted score is " & Math.Round(prediction.Score, 1))
            Console.WriteLine("=============== End of process, hit any key to finish ===============")
            Console.ReadKey()
        End Sub

        Public Class Copurchase_prediction
            Public Property Score() As Single
        End Class

        Public Class ProductEntry
            <KeyType(Count:=262111)>
            Public Property ProductID() As UInteger

            <KeyType(Count:=262111)>
            Public Property CoPurchaseProductID() As UInteger
        End Class
    End Module

End Namespace
