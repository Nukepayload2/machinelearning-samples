Imports Microsoft.ML.Core.Data
Imports OxyPlot
Imports OxyPlot.Series
Imports System.IO

Imports Common
Imports CustomerSegmentation.DataStructures
Imports Microsoft.ML
Imports Microsoft.ML.Data

Namespace CustomerSegmentation.Model
    Public Class ClusteringModelScorer
        Private ReadOnly _pivotDataLocation As String

        Private ReadOnly _plotLocation As String
        Private ReadOnly _csvlocation As String
        Private ReadOnly _mlContext As MLContext
        Private _trainedModel As ITransformer

        Public Sub New(mlContext As MLContext, pivotDataLocation As String, plotLocation As String, csvlocation As String)
            _pivotDataLocation = pivotDataLocation
            _plotLocation = plotLocation
            _csvlocation = csvlocation
            _mlContext = mlContext
        End Sub

        Public Function LoadModelFromZipFile(modelPath As String) As ITransformer
            Using stream = New FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                _trainedModel = _mlContext.Model.Load(stream)
            End Using

            Return _trainedModel
        End Function

        Public Sub CreateCustomerClusters()
            Dim data = _mlContext.Data.ReadFromTextFile(path:=_pivotDataLocation, columns:={
                New TextLoader.Column("Features", DataKind.R4, {New TextLoader.Range(0, 31)}),
                New TextLoader.Column(NameOf(PivotData.LastName), DataKind.Text, 32)
            }, hasHeader:=True, separatorChar:=","c)

            'Apply data transformation to create predictions/clustering
            Dim tranfomedDataView = _trainedModel.Transform(data)
            Dim predictions = _mlContext.CreateEnumerable(Of ClusteringPrediction)(tranfomedDataView, False).ToArray()

            'Generate data files with customer data grouped by clusters
            SaveCustomerSegmentationCSV(predictions, _csvlocation)

            'Plot/paint the clusters in a chart and open it with the by-default image-tool in Windows
            SaveCustomerSegmentationPlotChart(predictions, _plotLocation)
            OpenChartInDefaultWindow(_plotLocation)
        End Sub

        Private Shared Sub SaveCustomerSegmentationCSV(predictions As IEnumerable(Of ClusteringPrediction), csvlocation As String)
            ConsoleHelper.ConsoleWriteHeader("CSV Customer Segmentation")
            Using w = New System.IO.StreamWriter(csvlocation)
                w.WriteLine($"LastName,SelectedClusterId")
                w.Flush()
                predictions.ToList().ForEach(Sub(prediction)
                                                 w.WriteLine($"{prediction.LastName},{prediction.SelectedClusterId}")
                                                 w.Flush()
                                             End Sub)
            End Using

            Console.WriteLine($"CSV location: {csvlocation}")
        End Sub

        Private Shared Sub SaveCustomerSegmentationPlotChart(predictions As IEnumerable(Of ClusteringPrediction), plotLocation As String)
            Common.ConsoleHelper.ConsoleWriteHeader("Plot Customer Segmentation")

            Dim plot = New PlotModel With {
                .Title = "Customer Segmentation",
                .IsLegendVisible = True
            }

            Dim clusters = predictions.Select(Function(p) p.SelectedClusterId).Distinct().OrderBy(Function(x) x)

            For Each cluster In clusters
                Dim scatter = New ScatterSeries With {
                    .MarkerType = MarkerType.Circle,
                    .MarkerStrokeThickness = 2,
                    .Title = $"Cluster: {cluster}",
                    .RenderInLegend = True
                }
                Dim series = predictions.Where(Function(p) p.SelectedClusterId = cluster).Select(Function(p) New ScatterPoint(p.Location(0), p.Location(1))).ToArray()
                scatter.Points.AddRange(series)
                plot.Series.Add(scatter)
            Next cluster

            plot.DefaultColors = OxyPalettes.HueDistinct(plot.Series.Count).Colors

            Dim exporter = New SvgExporter With {
                .Width = 600,
                .Height = 400
            }
            Using fs = New System.IO.FileStream(plotLocation, System.IO.FileMode.Create)
                exporter.Export(plot, fs)
            End Using

            Console.WriteLine($"Plot location: {plotLocation}")
        End Sub

        Private Shared Sub OpenChartInDefaultWindow(plotLocation As String)
            Console.WriteLine("Showing chart...")
            Dim p = New Process()
            p.StartInfo = New ProcessStartInfo(plotLocation) With {.UseShellExecute = True}
            p.Start()
        End Sub
    End Class
End Namespace
