Imports System.IO

Imports Microsoft.ML
Imports Common
Imports Clustering_Iris.DataStructures
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Data

Namespace Clustering_Iris
    Friend Module Program
        Private ReadOnly Property AppPath As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private BaseDatasetsLocation As String = "../../../../Data"
        Private DataPath As String = $"{BaseDatasetsLocation}/iris-full.txt"

        Private BaseModelsPath As String = "../../../../MLModels"
        Private ModelPath As String = $"{BaseModelsPath}/IrisModel.zip"

        Public Sub Main(args() As String)
            'Create the MLContext to share across components for deterministic results
            Dim mlContext As New MLContext(seed:=1) 'Seed set to any number so you have a deterministic environment

            ' STEP 1: Common data loading configuration
            Dim textLoader As TextLoader = mlContext.Data.CreateTextReader({
                    New TextLoader.Column("Label", DataKind.R4, 0),
                    New TextLoader.Column("SepalLength", DataKind.R4, 1),
                    New TextLoader.Column("SepalWidth", DataKind.R4, 2),
                    New TextLoader.Column("PetalLength", DataKind.R4, 3),
                    New TextLoader.Column("PetalWidth", DataKind.R4, 4)
                },
                separatorChar:=vbTab,
                hasHeader:=True
            )

            Dim fullData As IDataView = textLoader.Read(DataPath)

            'Split dataset in two parts: TrainingDataset (80%) and TestDataset (20%)
            With mlContext.Clustering.TrainTestSplit(fullData, testFraction:=0.2)
                Dim trainingDataView = .trainSet
                Dim testingDataView = .testSet

                'STEP 2: Process data transformations in pipeline
                Dim dataProcessPipeline = mlContext.Transforms.Concatenate("Features", "SepalLength", "SepalWidth", "PetalLength", "PetalWidth")

                ' (Optional) Peek data in training DataView after applying the ProcessPipeline's transformations  
                Common.ConsoleHelper.PeekDataViewInConsole(Of IrisData)(mlContext, trainingDataView, dataProcessPipeline, 10)
                Common.ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", trainingDataView, dataProcessPipeline, 10)

                ' STEP 3: Create and train the model     
                Dim trainer = mlContext.Clustering.Trainers.KMeans(features:="Features", clustersCount:=3)
                Dim trainingPipeline = dataProcessPipeline.Append(trainer)
                Dim trainedModel = trainingPipeline.Fit(trainingDataView)

                ' STEP4: Evaluate accuracy of the model
                Dim predictions As IDataView = trainedModel.Transform(testingDataView)
                Dim metrics = mlContext.Clustering.Evaluate(predictions, score:="Score", features:="Features")

                ConsoleHelper.PrintClusteringMetrics(trainer.ToString(), metrics)

                ' STEP5: Save/persist the model as a .ZIP file
                Using fs = New FileStream(ModelPath, FileMode.Create, FileAccess.Write, FileShare.Write)
                    mlContext.Model.Save(trainedModel, fs)
                End Using

                Console.WriteLine("=============== End of training process ===============")

                Console.WriteLine("=============== Predict a cluster for a single case (Single Iris data sample) ===============")

                ' Test with one sample text 
                Dim sampleIrisData = New IrisData() With {
                    .SepalLength = 3.3F,
                    .SepalWidth = 1.6F,
                    .PetalLength = 0.2F,
                    .PetalWidth = 5.1F
                }

                Using stream = New FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Dim model As ITransformer = mlContext.Model.Load(stream)
                    ' Create prediction engine related to the loaded trained model
                    Dim predEngine = model.CreatePredictionEngine(Of IrisData, IrisPrediction)(mlContext)

                    'Score
                    Dim resultprediction = predEngine.Predict(sampleIrisData)

                    Console.WriteLine($"Cluster assigned for setosa flowers:" & resultprediction.SelectedClusterId)
                End Using
            End With

            Console.WriteLine("=============== End of process, hit any key to finish ===============")
            Console.ReadKey()
        End Sub
    End Module

End Namespace
