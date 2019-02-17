Imports System.ComponentModel.Composition
Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Data
Imports SpamDetectionConsoleApp.MLDataStructures

Namespace SpamDetectionConsoleApp
    Friend Module Program
        Private ReadOnly Property AppPath As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private ReadOnly Property DataDirectoryPath As String
            Get
                Return Path.Combine(AppPath, "..", "..", "..", "Data", "spamfolder")
            End Get
        End Property

        Private ReadOnly Property TrainDataPath As String
            Get
                Return Path.Combine(AppPath, "..", "..", "..", "Data", "spamfolder", "SMSSpamCollection")
            End Get
        End Property

        Sub Main(args() As String)
            ' Download the dataset if it doesn't exist.
            If Not File.Exists(TrainDataPath) Then
                Using client = New WebClient()
                    client.DownloadFile("https://archive.ics.uci.edu/ml/machine-learning-databases/00228/smsspamcollection.zip", "spam.zip")
                End Using

                ZipFile.ExtractToDirectory("spam.zip", DataDirectoryPath)
            End If

            ' Set up the MLContext, which is a catalog of components in ML.NET.
            Dim mlContext As New MLContext()

            ' Specify the schema for spam data and read it into DataView.
            Dim data = mlContext.Data.ReadFromTextFile(Of SpamInput)(path:=TrainDataPath, hasHeader:=True, separatorChar:=vbTab)

            ' Create the estimator which converts the text label to boolean, featurizes the text, and adds a linear trainer.
            Dim dataProcessPipeLine = mlContext.Transforms.CustomMapping(Of MyInput, MyOutput)(mapAction:=AddressOf MyLambda.MyAction, contractName:="MyLambda").Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName:=DefaultColumnNames.Features, inputColumnName:=NameOf(SpamInput.Message)))

            'Create the training pipeline
            Console.WriteLine("=============== Training the model ===============")
            Dim trainingPipeLine = dataProcessPipeLine.Append(mlContext.BinaryClassification.Trainers.StochasticDualCoordinateAscent())

            ' Evaluate the model using cross-validation.
            ' Cross-validation splits our dataset into 'folds', trains a model on some folds and 
            ' evaluates it on the remaining fold. We are using 5 folds so we get back 5 sets of scores.
            ' Let's compute the average AUC, which should be between 0.5 and 1 (higher is better).
            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============")
            Dim crossValidationResults = mlContext.BinaryClassification.CrossValidate(data:=data, estimator:=trainingPipeLine, numFolds:=5)
            Dim aucs = crossValidationResults.Select(Function(r) r.metrics.Auc)
            Console.WriteLine("The AUC is {0}", aucs.Average())

            ' Now let's train a model on the full dataset to help us get better results
            Dim model = trainingPipeLine.Fit(data)

            ' The dataset we have is skewed, as there are many more non-spam messages than spam messages.
            ' While our model is relatively good at detecting the difference, this skewness leads it to always
            ' say the message is not spam. We deal with this by lowering the threshold of the predictor. In reality,
            ' it is useful to look at the precision-recall curve to identify the best possible threshold.
            Dim inPipe = New TransformerChain(Of ITransformer)(model.Take(model.Count() - 1).ToArray())
            Dim lastTransformer = New BinaryPredictionTransformer(Of IPredictorProducing(Of Single))(mlContext, model.LastTransformer.Model, inPipe.GetOutputSchema(data.Schema), model.LastTransformer.FeatureColumn, threshold:=0.15F, thresholdColumn:=DefaultColumnNames.Probability)

            Dim parts() As ITransformer = model.ToArray()
            parts(parts.Length - 1) = lastTransformer
            Dim newModel As ITransformer = New TransformerChain(Of ITransformer)(parts)

            ' Create a PredictionFunction from our model 
            Dim predictor = newModel.CreatePredictionEngine(Of SpamInput, SpamPrediction)(mlContext)

            Console.WriteLine("=============== Predictions for below data===============")
            ' Test a few examples
            ClassifyMessage(predictor, "That's a great idea. It should work.")
            ClassifyMessage(predictor, "free medicine winner! congratulations")
            ClassifyMessage(predictor, "Yes we should meet over the weekend!")
            ClassifyMessage(predictor, "you win pills and free entry vouchers")

            Console.WriteLine("=============== End of process, hit any key to finish =============== ")
            Console.ReadLine()
        End Sub

        Public Class MyInput
            Public Property Label() As String
        End Class

        Public Class MyOutput
            Public Property Label() As Boolean
        End Class

        Public Class MyLambda
            <Export("MyLambda")>
            Public ReadOnly Property MyTransformer As ITransformer
                Get
                    Return ML.Transforms.CustomMappingTransformer(Of MyInput, MyOutput)(AddressOf MyAction, "MyLambda")
                End Get
            End Property

            <Import>
            Public Property ML() As MLContext

            Public Shared Sub MyAction(input As MyInput, output As MyOutput)
                output.Label = If(input.Label = "spam", True, False)
            End Sub
        End Class

        Public Sub ClassifyMessage(predictor As PredictionEngine(Of SpamInput, SpamPrediction), message As String)
            Dim input = New SpamInput With {.Message = message}
            Dim prediction = predictor.Predict(input)

            Console.WriteLine("The message '{0}' is {1}", input.Message, If(prediction.isSpam, "spam", "not spam"))
        End Sub
    End Module
End Namespace
