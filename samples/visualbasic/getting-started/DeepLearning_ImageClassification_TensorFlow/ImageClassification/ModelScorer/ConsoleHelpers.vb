Imports ImageClassification.ImageDataStructures
Imports System.IO

Namespace ImageClassification.ModelScorer
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
            Const exceptionTitle As String = "EXCEPTION"

            Console.WriteLine(" ")
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine(exceptionTitle)
            Console.WriteLine(New String("#"c, exceptionTitle.Length))
            Console.ForegroundColor = defaultColor
            For Each line In lines
                Console.WriteLine(line)
            Next line
        End Sub

        <System.Runtime.CompilerServices.Extension> _
        Public Sub ConsoleWrite(self As ImageNetDataProbability)
            Dim defaultForeground = Console.ForegroundColor
            Dim labelColor = ConsoleColor.Magenta
            Dim probColor = ConsoleColor.Blue
            Dim exactLabel = ConsoleColor.Green
            Dim failLabel = ConsoleColor.Red

            Console.Write("ImagePath: ")
            Console.ForegroundColor = labelColor
            Console.Write($"{Path.GetFileName(self.ImagePath)}")
            Console.ForegroundColor = defaultForeground
            Console.Write(" labeled as ")
            Console.ForegroundColor = labelColor
            Console.Write(self.Label)
            Console.ForegroundColor = defaultForeground
            Console.Write(" predicted as ")
            If self.Label.Equals(self.PredictedLabel) Then
                Console.ForegroundColor = exactLabel
                Console.Write($"{self.PredictedLabel}")
            Else
                Console.ForegroundColor = failLabel
                Console.Write($"{self.PredictedLabel}")
            End If
            Console.ForegroundColor = defaultForeground
            Console.Write(" with probability ")
            Console.ForegroundColor = probColor
            Console.Write(self.Probability)
            Console.ForegroundColor = defaultForeground
            Console.WriteLine("")
        End Sub

    End Module

End Namespace
