'using static Microsoft.ML.Runtime.Data.RoleMappedSchema;

Namespace CreditCardFraudDetection.Common.DataModels
    Public Class TransactionFraudPrediction
        Implements IModelEntity

        Public Label As Boolean
        Public PredictedLabel As Boolean
        Public Score As Single
        Public Probability As Single

        Public Sub PrintToConsole() Implements IModelEntity.PrintToConsole
            Console.WriteLine($"Predicted Label: {PredictedLabel}")
            Console.WriteLine($"Probability: {Probability}  ({Score})")
        End Sub
    End Class
End Namespace
