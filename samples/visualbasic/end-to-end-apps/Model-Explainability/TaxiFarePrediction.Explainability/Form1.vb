Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports OxyPlot
Imports OxyPlot.Axes
Imports OxyPlot.Series
Imports System.IO


Namespace TaxiFareRegression.Explainability
	Partial Public Class Form1
		Inherits Form

		Private _predictionIndex As Integer = 0

		Private Shared predictions As List(Of DataStructures.TaxiFarePrediction) = GetTaxiFare.Predictions()
		Private _resultCount As Integer= predictions.Count()-1

		Public Sub New()
			Me.InitializeComponent()
			PaintChart()
		End Sub

		Private Sub PaintChart()
'INSTANT VB NOTE: The variable chart was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim chart_Renamed As PlotModel = TaxiFareRegression.Explainability.Form1.Chart.GetPlotModel(predictions(_predictionIndex))
			lblTripID.Text = (_predictionIndex + 1).ToString()
			Dim predictedAmount As String = String.Format("{0:C}", Convert.ToDecimal(predictions(_predictionIndex).FareAmount))
			lblFare.Text = predictedAmount
			Me.plot1.Model = chart_Renamed
		End Sub

		Friend NotInheritable Class Chart

			Private Sub New()
			End Sub

			Public Shared Function GetPlotModel(prediction As DataStructures.TaxiFarePrediction) As PlotModel
				Dim model = New PlotModel With {.Title = "Taxi Fare Prediction Impact per Feature"}

				Dim barSeries = New BarSeries With {
					.ItemsSource = New List(Of BarItem)( {
						New BarItem With {.Value = (prediction.Features(0).Value)},
						New BarItem With {.Value = (prediction.Features(1).Value)},
						New BarItem With {.Value = (prediction.Features(2).Value)}
					}),
					.LabelPlacement = LabelPlacement.Inside,
					.LabelFormatString = "{0:.00}"
				}

				model.Series.Add(barSeries)

				model.Axes.Add(New CategoryAxis With {
					.Position = AxisPosition.Left,
					.Key = "FeatureAxis",
					.ItemsSource = { prediction.Features(0).Name, prediction.Features(1).Name, prediction.Features(2).Name }
				})

				Return model
			End Function


		End Class
		Friend NotInheritable Class GetTaxiFare

			Private Sub New()
			End Sub

			Private Shared BaseRelativePath As String = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "Model-Explainability", "TaxiFarePrediction", "TaxiFarePredictionConsoleApp")
			Private Shared BaseDataPath As String = Path.Combine(Path.GetFullPath(BaseRelativePath), "inputs")
			Private Shared TestDataPath As String = Path.Combine(BaseDataPath, "taxi-fare-test.csv")
			Private Shared ModelPath As String = Path.Combine(BaseRelativePath, "outputs", "TaxiFareModel.zip")

			Public Shared Function Predictions() As List(Of DataStructures.TaxiFarePrediction)
				Dim modelPredictor = New Predictor(ModelPath, TestDataPath)
'INSTANT VB NOTE: The local variable predictions was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim predictions_Renamed As List(Of DataStructures.TaxiFarePrediction) = modelPredictor.RunMultiplePredictions(numberOfPredictions:= 20)
				Return predictions_Renamed
				'Console.WriteLine(JsonConvert.SerializeObject(predictions, Formatting.Indented));

			End Function


		End Class
		Private Sub Form1_Load(sender As Object, e As EventArgs)

		End Sub

		Private Sub Plot1_Click(sender As Object, e As EventArgs) Handles plot1.Click

		End Sub

		Private Sub Button1_Click(sender As Object, e As EventArgs) Handles button1.Click
			If _predictionIndex < _resultCount Then
				_predictionIndex += 1
				PaintChart()
			End If
		End Sub

		Private Sub Label1_Click(sender As Object, e As EventArgs) Handles lblTrip.Click

		End Sub

		Private Sub Form1_Load_1(sender As Object, e As EventArgs) Handles Me.Load

		End Sub

		Private Sub Label1_Click_1(sender As Object, e As EventArgs) Handles label1.Click

		End Sub

		Private Sub Button2_Click(sender As Object, e As EventArgs) Handles button2.Click
			If _predictionIndex > 0 Then
				_predictionIndex -= 1
				PaintChart()
			End If
		End Sub
	End Class
End Namespace
