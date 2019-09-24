Imports System
Imports System.IO
Imports Newtonsoft.Json
Imports System.Collections.Generic

Namespace TaxiFareRegression
	Friend Module Program
		Private BaseRelativePath As String = "../../../../TaxiFarePrediction"
		Private BaseDataPath As String = Path.Combine(Path.GetFullPath(BaseRelativePath), "inputs")
		Private TestDataPath As String = Path.Combine(BaseDataPath, "taxi-fare-test.csv")
		Private ModelPath As String = Path.Combine(BaseRelativePath, "outputs", "TaxiFareModel.zip")

		Sub Main(args() As String)
			Dim modelPredictor = New Predictor(ModelPath, TestDataPath)
			Dim predictions As List(Of DataStructures.TaxiFarePrediction) = modelPredictor.RunMultiplePredictions(numberOfPredictions:= 5)
			Console.WriteLine(JsonConvert.SerializeObject(predictions, Formatting.Indented))

		End Sub


	End Module
End Namespace
