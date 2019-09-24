Imports System

Namespace CreditCardFraudDetection.Common.DataModels
	Public Class TransactionFraudPrediction
		Implements IModelEntity

		Public Label As Single

		''' <summary>
		''' The non-negative, unbounded score that was calculated by the anomaly detection model.
		''' Fraudulent transactions (Anomalies) will have higher scores than normal transactions
		''' </summary>
		Public Score As Single

		''' <summary>
		''' The predicted label, based on the score. A value of true indicates an anomaly.
		''' </summary>
		Public PredictedLabel As Boolean

		Public Sub PrintToConsole() Implements IModelEntity.PrintToConsole
			' There is currently an issue where PredictedLabel is always set to true
			' Due to this issue, we'll manually choose the treshold that will indicate an anomaly
			' Issue: https://github.com/dotnet/machinelearning/issues/3990
			'Console.WriteLine($"Predicted Label: {Score > 0.2f}  (Score: {Score})");

			Console.WriteLine($"Predicted Label: {PredictedLabel}  (Score: {Score})")
		End Sub
	End Class
End Namespace
