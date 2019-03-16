Imports System.IO

Namespace ImageClassification.Model
    Public Module ConsoleHelpers
        Public Sub ConsoleWriteHeader(ParamArray lines() As String)
            Dim defaultColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine(" ")
            For Each line In lines
                Console.WriteLine(line)
            Next line
            Dim maxLength = lines.Select(Function(x) x.Length).Max()
            Console.WriteLine(New String("#"c, maxLength))
            Console.ForegroundColor = defaultColor
        End Sub

        Public Sub ConsolePressAnyKey()
            Dim defaultColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine(" ")
            Console.WriteLine("Press any key to finish.")
            Console.ReadKey()
        End Sub

        Public Sub ConsoleWriteException(ParamArray lines() As String)
            Dim defaultColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Red
            Const exceptionTitle As String = "EXCEPTION"
            Console.WriteLine(" ")
            Console.WriteLine(exceptionTitle)
            Console.WriteLine(New String("#"c, exceptionTitle.Length))
            Console.ForegroundColor = defaultColor
            For Each line In lines
                Console.WriteLine(line)
            Next line
        End Sub

        Public Sub ConsoleWriteImagePrediction(ImagePath As String, PredictedLabel As String, Probability As Single)
            Dim defaultForeground = Console.ForegroundColor
            Dim labelColor = ConsoleColor.Magenta
            Dim probColor = ConsoleColor.Blue

            Console.Write("ImagePath: ")
            Console.ForegroundColor = labelColor
            Console.Write($"{Path.GetFileName(ImagePath)}")
            Console.ForegroundColor = defaultForeground
            Console.Write(" predicted as ")
            Console.ForegroundColor = labelColor
            Console.Write(PredictedLabel)
            Console.ForegroundColor = defaultForeground
            Console.Write(" with score ")
            Console.ForegroundColor = probColor
            Console.Write(Probability)
            Console.ForegroundColor = defaultForeground
            Console.WriteLine("")
        End Sub
    End Module
End Namespace
