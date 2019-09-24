Imports System.IO
Imports ImageClassification.Model
Imports ImageClassification.Model.ConsoleHelpers
Imports Common
Imports ImageClassification.DataModels

Namespace ImageClassification.Train
    Public Class Program
        Shared Sub Main(args() As String)
            Dim assetsRelativePath As String = "../../../assets"
            Dim assetsPath As String = GetAbsolutePath(assetsRelativePath)

            'Inception v3
            Dim inceptionPb = Path.Combine(assetsPath, "inputs", "tensorflow-pretrained-models", "inception-v3", "inception_v3_2016_08_28_frozen.pb")

            'Inception v1 (Second alternative)
            'var inceptionPb = Path.Combine(assetsPath, "inputs", "tensorflow-pretrained-models", "inception-v1", "tensorflow_inception_graph.pb");

            Dim imageClassifierZip = Path.Combine(assetsPath, "outputs", "imageClassifier.zip")

            Dim tagsTsv = Path.Combine(assetsPath, "inputs", "data", "tags.tsv")

            Dim imagesDownloadFolderPath As String = Path.Combine(assetsPath, "inputs", "images")
            Dim finalImagesFolderName As String = DownloadImageSet(imagesDownloadFolderPath)
            Dim fullImagesetFolderPath = Path.Combine(imagesDownloadFolderPath, finalImagesFolderName)
            Console.WriteLine($"Images folder: {fullImagesetFolderPath}")

            ' Single full dataset
            Dim allImages As IEnumerable(Of ImageData) = LoadImagesFromDirectory(folder:=fullImagesetFolderPath, useFolderNameasLabel:=True)
            Try
                Dim modelBuilder = New ModelBuilder(inceptionPb, imageClassifierZip)

                modelBuilder.BuildAndTrain(allImages)
            Catch ex As Exception
                ConsoleWriteException(ex.ToString())
            End Try

            ConsolePressAnyKey()
        End Sub

        Public Shared Iterator Function LoadImagesFromDirectory(folder As String, Optional useFolderNameasLabel As Boolean = True) As IEnumerable(Of ImageData)
            Dim files = Directory.GetFiles(folder, "*", searchOption:=SearchOption.AllDirectories)

            For Each file In files
                If (Path.GetExtension(file) <> ".jpg") AndAlso (Path.GetExtension(file) <> ".png") Then
                    Continue For
                End If

                Dim label = Path.GetFileName(file)
                If useFolderNameasLabel Then
                    label = Directory.GetParent(file).Name
                Else
                    Dim index As Integer = 0
                    Do While index < label.Length
                        If Not Char.IsLetter(label.Chars(index)) Then
                            label = label.Substring(0, index)
                            Exit Do
                        End If
                        index += 1
                    Loop
                End If

                Yield New ImageData With {
                    .ImagePath = file,
                    .Label = label
                }

            Next file
        End Function


        Public Shared Function DownloadImageSet(imagesDownloadFolder As String) As String
            ' Download a set of images to teach the network about the new classes

            'SMALL FLOWERS IMAGESET (200 files)
            Dim fileName As String = "flower_photos_small_set.zip"
            Dim url As String = $"https://mlnetfilestorage.file.core.windows.net/imagesets/flower_images/flower_photos_small_set.zip?st=2019-08-07T21%3A27%3A44Z&se=2030-08-08T21%3A27%3A00Z&sp=rl&sv=2018-03-28&sr=f&sig=SZ0UBX47pXD0F1rmrOM%2BfcwbPVob8hlgFtIlN89micM%3D"
            Web.Download(url, imagesDownloadFolder, fileName)
            Compress.UnZip(Path.Join(imagesDownloadFolder, fileName), imagesDownloadFolder)

            'FULL FLOWERS IMAGESET (3,600 files)
            'string fileName = "flower_photos.tgz";
            'string url = $"http://download.tensorflow.org/example_images/{fileName}";
            'Web.Download(url, imagesDownloadFolder, fileName);
            'Compress.ExtractTGZ(Path.Join(imagesDownloadFolder, fileName), imagesDownloadFolder);

            Return Path.GetFileNameWithoutExtension(fileName)
        End Function

        Public Shared Function GetAbsolutePath(relativePath As String) As String
            Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
            Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

            Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

            Return fullPath
        End Function
    End Class
End Namespace
