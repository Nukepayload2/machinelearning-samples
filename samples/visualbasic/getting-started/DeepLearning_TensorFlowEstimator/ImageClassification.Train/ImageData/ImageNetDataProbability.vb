Namespace ImageClassification.ImageData
    Public Class ImageNetDataProbability
        Inherits ImageNetData

        Public Property Probability As Single


        Public Sub ConsoleWriteLine()
            Dim defaultForeground = Console.ForegroundColor
            Dim labelColor = ConsoleColor.Green

            Console.Write($"ImagePath: {ImagePath} predicted as ")
            Console.ForegroundColor = labelColor
            Console.Write(Label)
            Console.ForegroundColor = defaultForeground
            Console.Write(" with probability ")
            Console.ForegroundColor = labelColor
            Console.Write(Probability)
            Console.ForegroundColor = defaultForeground
            Console.WriteLine("")
        End Sub
    End Class
End Namespace
