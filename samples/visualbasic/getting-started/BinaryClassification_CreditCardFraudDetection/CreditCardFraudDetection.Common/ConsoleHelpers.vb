Imports CreditCardFraudDetection.Common.DataModels
Imports Microsoft.Data.DataView
Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports System.IO
Imports System.IO.Compression

Namespace CreditCardFraudDetection.Common
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

        Public Sub ConsoleWriterSection(ParamArray lines() As String)
            Dim defaultColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Blue
            Console.WriteLine(" ")
            For Each line In lines
                Console.WriteLine(line)
            Next line
            Dim maxLength = lines.Select(Function(x) x.Length).Max()
            Console.WriteLine(New String("-"c, maxLength))
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

        Public Sub ConsoleWriteWarning(ParamArray lines() As String)
            Dim defaultColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.DarkMagenta
            Const warningTitle As String = "WARNING"
            Console.WriteLine(" ")
            Console.WriteLine(warningTitle)
            Console.WriteLine(New String("#"c, warningTitle.Length))
            Console.ForegroundColor = defaultColor
            For Each line In lines
                Console.WriteLine(line)
            Next line
        End Sub

        Public Function GetAssetsPath(ParamArray paths() As String) As String

            Dim _dataRoot As New FileInfo(GetType(ConsoleHelpers).Assembly.Location)
            If paths Is Nothing OrElse paths.Length = 0 Then
                Return Nothing
            End If

            Return Path.Combine(paths.Prepend(_dataRoot.Directory.FullName).ToArray())
        End Function

        Public Function DeleteAssets(ParamArray paths() As String) As String
            Dim location = GetAssetsPath(paths)

            If Not String.IsNullOrWhiteSpace(location) AndAlso File.Exists(location) Then
                File.Delete(location)
            End If
            Return location
        End Function

        Public Sub InspectData(env As MLContext, data As IDataView, records As Integer)
            ShowObservations(env, data, label:=True, count:=records)
            ShowObservations(env, data, label:=False, count:=records)
        End Sub

        Public Sub InspectScoredData(env As MLContext, data As IDataView, records As Integer)
            ShowPredictions(env, data, label:=True, count:=records)
            ShowPredictions(env, data, label:=False, count:=records)
        End Sub

        Public Sub ShowObservations(env As MLContext, data As IDataView, Optional label As Boolean = True, Optional count As Integer = 2)
            env.CreateEnumerable(Of TransactionObservation)(data, reuseRowObject:=False).
                Where(Function(x) x.Label = label).
                Take(count).ToList().
                ForEach(Sub(row)
                            row.PrintToConsole()
                        End Sub)
        End Sub

        Public Sub ShowPredictions(env As MLContext, data As IDataView, Optional label As Boolean = True, Optional count As Integer = 2)
            env.CreateEnumerable(Of TransactionFraudPrediction)(data, reuseRowObject:=False).
                Where(Function(x) x.PredictedLabel = label).
                Take(count).ToList().
                ForEach(Sub(row)
                            row.PrintToConsole()
                        End Sub)
        End Sub

        Public Sub UnZipDataSet(zipDataSet As String, destinationFile As String)
            If Not File.Exists(destinationFile) Then
                Dim destinationDirectory = Path.GetDirectoryName(destinationFile)
                ZipFile.ExtractToDirectory(zipDataSet, $"{destinationDirectory}")
            End If
        End Sub
    End Module

End Namespace
