Option Compare Text

Imports System.IO

Module Program
    Sub Main()
        Dim curDir = String.Empty
        If Not TryGetVBSamplesFolder(curDir) Then
            Console.WriteLine("Please run this tool under the visual basic sample folder")
            Environment.Exit(1)
            Return
        End If

        Dim vbSampleFolder As New DirectoryInfo(curDir)
        FixVBProjectFiles(vbSampleFolder)
        MergeDirectories(vbSampleFolder)
        FixControlChars(vbSampleFolder)
        Console.WriteLine("Done")
    End Sub

    Private Sub FixControlChars(vbSampleFolder As DirectoryInfo)
        ' ControlChars.Tab
        Dim vbFiles = vbSampleFolder.GetFiles("*.vb", SearchOption.AllDirectories)
        For Each vb In vbFiles
            Dim content = File.ReadAllText(vb.FullName)
            Dim newContent = content.Replace("ControlChars.Tab", "vbTab")
            If content.Length <> newContent.Length Then
                Console.WriteLine("Fix code file " & vb.FullName)
                File.WriteAllText(vb.FullName, newContent)
            End If
        Next
    End Sub

    Private Sub MergeDirectories(vbSampleFolder As DirectoryInfo)
        Dim duplicateFolders = vbSampleFolder.GetDirectories("* - VB", SearchOption.AllDirectories)
        For Each dupFolder In duplicateFolders
            Dim srcDirName = dupFolder.Name
            Console.WriteLine("Merging " & dupFolder.FullName)
            Dim targetDirName = srcDirName.Substring(0, srcDirName.Length - " - VB".Length)
            Dim parentDir = dupFolder.Parent.FullName
            Dim targetDir = Path.Combine(parentDir, targetDirName)
            If Not Directory.Exists(targetDir) Then
                Directory.CreateDirectory(targetDir)
            End If
            MoveFilesRecursive(dupFolder.FullName, targetDir)
        Next
    End Sub

    Private Sub MoveFilesRecursive(fullFromDir As String, fullToDir As String, Optional depth As Integer = 8)
        For Each fromFile In Directory.GetFiles(fullFromDir)
            Dim relativeFile As String = fromFile.Substring(fullFromDir.Length + 1)
            Dim destFile = Path.Combine(fullToDir, relativeFile)
            If File.Exists(destFile) Then
                File.Delete(destFile)
            End If
            File.Move(fromFile, destFile)
        Next
        If depth > 0 Then
            For Each fromDir In Directory.GetDirectories(fullFromDir)
                Dim relativeDir As String = fromDir.Substring(fullFromDir.Length + 1)
                Dim destDir = Path.Combine(fullToDir, relativeDir)
                MoveFilesRecursive(fromDir, destDir, depth - 1)
            Next
        End If
    End Sub

    Private Sub FixVBProjectFiles(vbSampleFolder As DirectoryInfo)
        Dim vbprojFiles = vbSampleFolder.GetFiles("*.vbproj", SearchOption.AllDirectories)

        For Each vbprojFile In vbprojFiles
            FixVBProjectFile(vbprojFile)
        Next
    End Sub

    Private Sub FixVBProjectFile(vbprojFile As FileInfo)
        Dim fullName As String = vbprojFile.FullName
        Console.WriteLine("Fix project file " & fullName)
        Dim content = File.ReadAllText(fullName)
        content = FixXmlFormat(content)
        content = RemoveDefaultImports(content)
        content = MergeDirectoryReference(content)
        content = ConvertLangVersion(content)
        content = EmptyRootNamespace(content)
        File.WriteAllText(fullName, content)
    End Sub

    Private Function FixXmlFormat(content As String) As String
        content = content.Replace("<EmbeddedResource Include=""**\*.resx"" />
<Compile Include=""**\*.vb"" />
", String.Empty)
        Return content
    End Function

    Private Function RemoveDefaultImports(content As String) As String
        content = content.Replace("<ItemGroup>
    <Import Include=""Microsoft.VisualBasic"" />
    <Import Include=""System.Collections.Generic"" />
    <Import Include=""System.Collections"" />
    <Import Include=""System.Diagnostics"" />
    <Import Include=""System.Linq"" />
    <Import Include=""System"" />
  </ItemGroup>
", String.Empty)
        Return content
    End Function

    Private Function MergeDirectoryReference(content As String) As String
        content = content.Replace(" - VB", String.Empty).Replace(" \", "\")
        Return content
    End Function

    Private Function ConvertLangVersion(content As String) As String
        content = content.Replace("<LangVersion>7.2</LangVersion>", "<LangVersion>15.5</LangVersion>")
        content = content.Replace("<LangVersion>7.1</LangVersion>", "<LangVersion>15.5</LangVersion>")
        Return content
    End Function

    Private Function EmptyRootNamespace(content As String) As String
        If content.Contains("Microsoft.Common.props") Then
            ' .NET Framework vbproj
            Return content
        End If
        Dim xmlView = XDocument.Load(New StringReader(content))
        Dim propGroups = xmlView.Root.<PropertyGroup>.ToArray
        If Not propGroups.<RootNamespace>.Any Then
            propGroups.First.Add(<RootNamespace></RootNamespace>)
        End If
        content = xmlView.ToString
        Return content
    End Function

    Private Function TryGetVBSamplesFolder(ByRef curDir As String) As Boolean
        curDir = Directory.GetCurrentDirectory
        Do
            Dim dirName = Path.GetFileName(curDir)
            If dirName = Nothing Then
                Return False
            End If
            If dirName = "visualbasic" Then Exit Do
            curDir = Directory.GetParent(curDir).FullName
        Loop
        Return True
    End Function
End Module
