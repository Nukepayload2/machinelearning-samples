Imports Microsoft.ML
Imports Octokit
Imports System.IO
Imports GitHubLabeler.DataStructures
Imports Microsoft.ML.Data

Namespace GitHubLabeler
    'This "Labeler" class could be used in a different End-User application (Web app, other console app, desktop app, etc.)
    Friend Class Labeler
        Private ReadOnly _client As GitHubClient
        Private ReadOnly _repoOwner As String
        Private ReadOnly _repoName As String
        Private ReadOnly _modelPath As String
        Private ReadOnly _mlContext As MLContext

        Private ReadOnly _predEngine As PredictionEngine(Of GitHubIssue, GitHubIssuePrediction)
        Private ReadOnly _trainedModel As ITransformer

        Private _fullPredictions() As FullPrediction

        Public Sub New(modelPath As String, Optional repoOwner As String = "", Optional repoName As String = "", Optional accessToken As String = "")
            _modelPath = modelPath
            _repoOwner = repoOwner
            _repoName = repoName

            _mlContext = New MLContext()

            'Load model from file

            Using stream = New FileStream(_modelPath, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read)
                _trainedModel = _mlContext.Model.Load(stream)
            End Using

            ' Create prediction engine related to the loaded trained model
            _predEngine = _trainedModel.CreatePredictionEngine(Of GitHubIssue, GitHubIssuePrediction)(_mlContext)

            'Configure Client to access a GitHub repo
            If accessToken <> String.Empty Then
                Dim productInformation = New ProductHeaderValue("MLGitHubLabeler")
                _client = New GitHubClient(productInformation) With {.Credentials = New Credentials(accessToken)}
            End If
        End Sub

        Public Sub TestPredictionForSingleIssue()
            Dim singleIssue As New GitHubIssue() With {
                .ID = "Any-ID",
                .Title = "Crash in SqlConnection when using TransactionScope",
                .Description = "I'm using SqlClient in netcoreapp2.0. Sqlclient.Close() crashes in Linux but works on Windows"
            }

            'Predict labels and scores for single hard-coded issue
            Dim prediction = _predEngine.Predict(singleIssue)

            _fullPredictions = GetBestThreePredictions(prediction)

            Console.WriteLine("==== Displaying prediction of Issue with Title = {0} and Description = {1} ====", singleIssue.Title, singleIssue.Description)

            Console.WriteLine("1st Label: " & _fullPredictions(0).PredictedLabel & " with score: " & _fullPredictions(0).Score)
            Console.WriteLine("2nd Label: " & _fullPredictions(1).PredictedLabel & " with score: " & _fullPredictions(1).Score)
            Console.WriteLine("3rd Label: " & _fullPredictions(2).PredictedLabel & " with score: " & _fullPredictions(2).Score)

            Console.WriteLine($"=============== Single Prediction - Result: {prediction.Area} ===============")
        End Sub

        Private Function GetBestThreePredictions(prediction As GitHubIssuePrediction) As FullPrediction()
            Dim scores() As Single = prediction.Score
            Dim size As Integer = scores.Length
            Dim index0 As Integer = Nothing, index1 As Integer = Nothing, index2 As Integer = 0

            Dim slotNames As VBuffer(Of ReadOnlyMemory(Of Char)) = Nothing
            _predEngine.OutputSchema(NameOf(GitHubIssuePrediction.Score)).GetSlotNames(slotNames)

            GetIndexesOfTopThreeScores(scores, size, index0, index1, index2)

            _fullPredictions = New FullPrediction() {
                New FullPrediction(slotNames.GetItemOrDefault(index0).ToString(), scores(index0), index0),
                New FullPrediction(slotNames.GetItemOrDefault(index1).ToString(), scores(index1), index1),
                New FullPrediction(slotNames.GetItemOrDefault(index2).ToString(), scores(index2), index2)
            }

            Return _fullPredictions
        End Function

        Private Sub GetIndexesOfTopThreeScores(scores() As Single, n As Integer, ByRef index0 As Integer, ByRef index1 As Integer, ByRef index2 As Integer)
            Dim i As Integer
            Dim first, second, third As Single
            index2 = 0
            index1 = index2
            index0 = index1
            If n < 3 Then
                Console.WriteLine("Invalid Input")
                Return
            End If
            second = 0
            first = second
            third = first
            For i = 0 To n - 1
                ' If current element is  
                ' smaller than first 
                If scores(i) > first Then
                    third = second
                    second = first
                    first = scores(i)
                    ' If arr[i] is in between first 
                    ' and second then update second 
                ElseIf scores(i) > second Then
                    third = second
                    second = scores(i)

                ElseIf scores(i) > third Then
                    third = scores(i)
                End If
            Next i
            Dim scoresList = scores.ToList()
            index0 = scoresList.IndexOf(first)
            index1 = scoresList.IndexOf(second)
            index2 = scoresList.IndexOf(third)
        End Sub

        ' Label all issues that are not labeled yet
        Public Async Function LabelAllNewIssuesInGitHubRepo() As Task
            Dim newIssues = Await GetNewIssues()
            For Each issue In newIssues.Where(Function(issue1) Not issue1.Labels.Any())
                Dim label = PredictLabels(issue)
                ApplyLabels(issue, label)
            Next issue
        End Function

        Private Async Function GetNewIssues() As Task(Of IReadOnlyList(Of Issue))
            Dim issueRequest = New RepositoryIssueRequest With {
                .State = ItemStateFilter.Open,
                .Filter = IssueFilter.All,
                .Since = Date.Now.AddMinutes(-10)
            }

            Dim allIssues = Await _client.Issue.GetAllForRepository(_repoOwner, _repoName, issueRequest)

            ' Filter out pull requests and issues that are older than minId
            Return allIssues.Where(Function(i) Not i.HtmlUrl.Contains("/pull/")).ToList()
        End Function

        Private Function PredictLabels(issue As Octokit.Issue) As FullPrediction()
            Dim corefxIssue = New GitHubIssue With {
                .ID = issue.Number.ToString(),
                .Title = issue.Title,
                .Description = issue.Body
            }

            _fullPredictions = Predict(corefxIssue)

            Return _fullPredictions
        End Function

        Public Function Predict(issue As GitHubIssue) As FullPrediction()
            Dim prediction = _predEngine.Predict(issue)

            Dim fullPredictions = GetBestThreePredictions(prediction)

            Return fullPredictions
        End Function

        Private Sub ApplyLabels(issue As Issue, fullPredictions() As FullPrediction)
            Dim issueUpdate = New IssueUpdate()

            'assign labels in GITHUB only if predicted score of all predictions is > 30%
            For Each fullPrediction In fullPredictions
                If fullPrediction.Score >= 0.3 Then
                    issueUpdate.AddLabel(fullPrediction.PredictedLabel)
                    _client.Issue.Update(_repoOwner, _repoName, issue.Number, issueUpdate)

                    Console.WriteLine($"Issue {issue.Number} : ""{issue.Title}"" " & vbTab & " was labeled as: {fullPredictions[0].PredictedLabel}")
                End If
            Next fullPrediction
        End Sub
    End Class
End Namespace
