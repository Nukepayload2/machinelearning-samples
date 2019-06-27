﻿Imports System
Imports System.IO
Imports System.Threading.Tasks
Imports ImageClassification.Model
Imports ImageClassification.Model.ConsoleHelpers

Namespace ImageClassification.Train
	Public Class Program
		Shared Sub Main(args() As String)
			Dim assetsRelativePath As String = "../../../assets"
			Dim assetsPath As String = GetAbsolutePath(assetsRelativePath)

			Dim tagsTsv = Path.Combine(assetsPath, "inputs", "data", "tags.tsv")
			Dim imagesFolder = Path.Combine(assetsPath, "inputs", "data")
			Dim inceptionPb = Path.Combine(assetsPath, "inputs", "inception", "tensorflow_inception_graph.pb")
			Dim imageClassifierZip = Path.Combine(assetsPath, "outputs", "imageClassifier.zip")

			Try
				Dim modelBuilder = New ModelBuilder(tagsTsv, imagesFolder, inceptionPb, imageClassifierZip)
				modelBuilder.BuildAndTrain()
			Catch ex As Exception
				ConsoleWriteException(ex.ToString())
			End Try

			ConsolePressAnyKey()
		End Sub

		Public Shared Function GetAbsolutePath(relativePath As String) As String
			Dim _dataRoot As New FileInfo(GetType(Program).Assembly.Location)
			Dim assemblyFolderPath As String = _dataRoot.Directory.FullName

			Dim fullPath As String = Path.Combine(assemblyFolderPath, relativePath)

			Return fullPath
		End Function
	End Class
End Namespace
