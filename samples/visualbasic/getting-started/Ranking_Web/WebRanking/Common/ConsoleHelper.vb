Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports WebRanking.DataStructures

Namespace WebRanking.Common
    Public Class ConsoleHelper
        Public Shared Sub EvaluateMetrics(ByVal mlContext As MLContext, ByVal predictions As IDataView)
            Dim metrics As RankingMetrics = mlContext.Ranking.Evaluate(predictions)
            Console.WriteLine($"DCG: {String.Join(", ", metrics.DiscountedCumulativeGains.[Select](Function(d, i) $"@{i + 1}:{d}").ToArray())}")
            Console.WriteLine($"NDCG: {String.Join(", ", metrics.NormalizedDiscountedCumulativeGains.[Select](Function(d, i) $"@{i + 1}:{d}").ToArray())}\n")
        End Sub

        Public Shared Sub EvaluateMetrics(ByVal mlContext As MLContext, ByVal predictions As IDataView, ByVal truncationLevel As Integer)
            If truncationLevel < 1 OrElse truncationLevel > 10 Then
                Throw New InvalidOperationException("Currently metrics are only supported for 1 to 10 truncation levels.")
            End If

            Dim mlAssembly = GetType(TextLoader).Assembly
            Dim rankEvalType = mlAssembly.DefinedTypes.Where(Function(t) t.Name.Contains("RankingEvaluator")).First()
            Dim evalArgsType = rankEvalType.GetNestedType("Arguments")
            Dim evalArgs = Activator.CreateInstance(rankEvalType.GetNestedType("Arguments"))
            Dim dcgLevel = evalArgsType.GetField("DcgTruncationLevel")
            dcgLevel.SetValue(evalArgs, truncationLevel)
            Dim ctor = rankEvalType.GetConstructors().First()
            Dim evaluator = ctor.Invoke(New Object() {mlContext, evalArgs})
            Dim evaluateMethod = rankEvalType.GetMethod("Evaluate")
            Dim metrics As RankingMetrics = CType(evaluateMethod.Invoke(evaluator, New Object() {predictions, "Label", "GroupId", "Score"}), RankingMetrics)
            Console.WriteLine($"DCG: {String.Join(", ", metrics.DiscountedCumulativeGains.[Select](Function(d, i) $"@{i + 1}:{d}").ToArray())}")
            Console.WriteLine($"NDCG: {String.Join(", ", metrics.NormalizedDiscountedCumulativeGains.[Select](Function(d, i) $"@{i + 1}:{d}").ToArray())}\n")
        End Sub

        Public Shared Sub PrintScores(ByVal predictions As IEnumerable(Of SearchResultPrediction))
            For Each prediction In predictions
                Console.WriteLine($"GroupId: {prediction.GroupId}, Score: {prediction.Score}")
            Next
        End Sub
    End Class
End Namespace
