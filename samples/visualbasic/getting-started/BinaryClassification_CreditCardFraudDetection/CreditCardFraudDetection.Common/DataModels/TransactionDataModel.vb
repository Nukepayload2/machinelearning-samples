'using static Microsoft.ML.Runtime.Data.RoleMappedSchema;

Namespace CreditCardFraudDetection.Common.DataModels

    Public Interface IModelEntity
        Sub PrintToConsole()
    End Interface

    Public Class TransactionObservation
        Implements IModelEntity

        Public Label As Boolean
        Public V1 As Single
        Public V2 As Single
        Public V3 As Single
        Public V4 As Single
        Public V5 As Single
        Public V6 As Single
        Public V7 As Single
        Public V8 As Single
        Public V9 As Single
        Public V10 As Single
        Public V11 As Single
        Public V12 As Single
        Public V13 As Single
        Public V14 As Single
        Public V15 As Single
        Public V16 As Single
        Public V17 As Single
        Public V18 As Single
        Public V19 As Single
        Public V20 As Single
        Public V21 As Single
        Public V22 As Single
        Public V23 As Single
        Public V24 As Single
        Public V25 As Single
        Public V26 As Single
        Public V27 As Single
        Public V28 As Single
        Public Amount As Single

        Public Sub PrintToConsole() Implements IModelEntity.PrintToConsole
            Console.WriteLine($"Label: {Label}")
            Console.WriteLine($"Features: [V1] {V1} [V2] {V2} [V3] {V3} ... [V28] {V28} Amount: {Amount}")
        End Sub

        'public static List<KeyValuePair<ColumnRole, string>>  Roles() {
        '    return new List<KeyValuePair<ColumnRole, string>>() {
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Label, "Label"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V1"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V2"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V3"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V4"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V5"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V6"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V7"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V8"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V9"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V10"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V11"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V12"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V13"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V14"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V15"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V16"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V17"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V18"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V19"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V20"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V21"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V22"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V23"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V24"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V25"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V26"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V27"),
        '            new KeyValuePair<ColumnRole, string>(ColumnRole.Feature, "V28"),
        '            new KeyValuePair<ColumnRole, string>(new ColumnRole("Amount"), ""),

        '        };
        '}
    End Class

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
