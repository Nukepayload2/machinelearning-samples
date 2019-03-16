Option Compare Text

Imports System.IO

Module Program
    Sub Main(args As String())
        Dim sampleDir As String = Nothing
        If Not TryGetSamplesFolder(sampleDir) Then
            Console.WriteLine("Please run this program under the ""samples"" folder or its subfolder.")
            Return
        End If
        Console.WriteLine("Coping markdown")
        CopyFilesRecursive(rootDir:=sampleDir, fromDir:="csharp", toDir:="visualbasic", depth:=8, pattern:="*.md",
                           copiedCallback:=
                           Sub(copyInfo As (src As String, dest As String))
                               Console.WriteLine($"Copied {copyInfo.src} to {copyInfo.dest}.")
                           End Sub)
        Console.WriteLine("Finished")
        Console.ReadKey()
    End Sub

    Private Sub CopyFilesRecursive(rootDir As String, fromDir As String, toDir As String, depth As Integer,
                                   pattern As String, copiedCallback As Action(Of (src$, dest$)))
        Dim fullFromDir As String = Path.Combine(rootDir, fromDir)
        Dim fullToDir As String = Path.Combine(rootDir, toDir)
        CopyRecursiveInternal(pattern, copiedCallback, fullFromDir, fullToDir, depth)
    End Sub

    Private Sub CopyRecursiveInternal(pattern As String, copiedCallback As Action(Of (src As String, dest As String)),
                                      fullFromDir As String, fullToDir As String, depth As Integer)
        For Each fromFile In Directory.GetFiles(fullFromDir, pattern)
            Dim relativeFile As String = fromFile.Substring(fullFromDir.Length + 1)
            Dim destFile = Path.Combine(fullToDir, relativeFile)
            MakeContainingDirRecursive(destFile)
            File.Copy(fromFile, destFile, True)
            copiedCallback?((fromFile, destFile))
        Next
        If depth > 0 Then
            For Each fromDir In Directory.GetDirectories(fullFromDir)
                Dim relativeDir As String = fromDir.Substring(fullFromDir.Length + 1)
                Dim destDir = Path.Combine(fullToDir, relativeDir)
                CopyRecursiveInternal(pattern, copiedCallback, fromDir, destDir, depth - 1)
            Next
        End If
    End Sub

    Private Sub MakeContainingDirRecursive(destFile As String)
        Dim dir = Directory.GetParent(destFile).FullName
        If Not Directory.Exists(dir) Then
            Directory.CreateDirectory(dir)
        End If
    End Sub

    Private Function TryGetSamplesFolder(ByRef curDir As String) As Boolean
        curDir = Directory.GetCurrentDirectory
        Do
            Dim dirName = Path.GetFileName(curDir)
            If dirName = Nothing Then
                Return False
            End If
            If dirName = "samples" Then Exit Do
            curDir = Directory.GetParent(curDir).FullName
        Loop
        Return True
    End Function
End Module
