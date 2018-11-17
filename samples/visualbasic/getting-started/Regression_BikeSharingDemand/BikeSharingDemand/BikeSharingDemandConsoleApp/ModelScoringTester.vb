﻿Imports System.IO

Imports Microsoft.ML

Imports BikeSharingDemand.DataStructures
Imports Common

Namespace BikeSharingDemand
    Public Module ModelScoringTester
        Public Sub VisualizeSomePredictions(mlContext As MLContext, modelName As String, testDataLocation As String, modelScorer As ModelScorer(Of DemandObservation, DemandPrediction), numberOfPredictions As Integer)
            'Make a few prediction tests 
            ' Make the provided number of predictions and compare with observed data from the test dataset
            Dim testData = ReadSampleDataFromCsvFile(testDataLocation, numberOfPredictions)

            For i As Integer = 0 To numberOfPredictions - 1
                Dim prediction = modelScorer.PredictSingle(testData(i))

                PrintRegressionPredictionVersusObserved(prediction.PredictedCount.ToString(), testData(i).Count.ToString())
            Next i

        End Sub

        'This method is using regular .NET System.IO.File and LinQ to read just some sample data to test/predict with 
        Public Function ReadSampleDataFromCsvFile(dataLocation As String, numberOfRecordsToRead As Integer) As List(Of DemandObservation)
            Return File.ReadLines(dataLocation).Skip(1).Where(Function(x) Not String.IsNullOrWhiteSpace(x)).Select(Function(x) x.Split(","c)).Select(Function(x) New DemandObservation() With {
                .Season = Single.Parse(x(2)),
                .Year = Single.Parse(x(3)),
                .Month = Single.Parse(x(4)),
                .Hour = Single.Parse(x(5)),
                .Holiday = Single.Parse(x(6)),
                .Weekday = Single.Parse(x(7)),
                .WorkingDay = Single.Parse(x(8)),
                .Weather = Single.Parse(x(9)),
                .Temperature = Single.Parse(x(10)),
                .NormalizedTemperature = Single.Parse(x(11)),
                .Humidity = Single.Parse(x(12)),
                .Windspeed = Single.Parse(x(13)),
                .Count = Single.Parse(x(16))
            }).Take(numberOfRecordsToRead).ToList()
        End Function
    End Module
End Namespace
