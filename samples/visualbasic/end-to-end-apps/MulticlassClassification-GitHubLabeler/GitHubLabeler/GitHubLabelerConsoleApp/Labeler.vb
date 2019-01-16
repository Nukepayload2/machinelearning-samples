Imports Microsoft.ML.Core.Data
Imports Microsoft.ML
Imports Octokit
Imports System.IO
Imports GitHubLabeler.DataStructures

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

        Public Sub New(modelPath As String, Optional repoOwner As String = "", Optional repoName As String = "", Optional accessToken As String = "")
            _modelPath = modelPath
            _repoOwner = repoOwner
            _repoName = repoName

            _mlContext = New MLContext(seed:=1)

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
                .Title = "Entity Framework crashes",
                .Description = "When connecting to the database, EF is crashing"
            }

            'Predict label for single hard-coded issue
            'Score
            Dim prediction = _predEngine.Predict(singleIssue)
            Console.WriteLine($"=============== Single Prediction - Result: {prediction.Area} ===============")
        End Sub

        ' Label all issues that are not labeled yet
        Public Async Function LabelAllNewIssuesInGitHubRepo() As Task
            Dim newIssues = Await GetNewIssues()
            For Each issue In newIssues.Where(Function(issue1) Not issue1.Labels.Any())
                Dim label = PredictLabel(issue)
                ApplyLabel(issue, label)
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

        Private Function PredictLabel(issue As Octokit.Issue) As String
            Dim corefxIssue = New GitHubIssue With {
                .ID = issue.Number.ToString(),
                .Title = issue.Title,
                .Description = issue.Body
            }

            Dim predictedLabel = Predict(corefxIssue)

            Return predictedLabel
        End Function

        Public Function Predict(issue As GitHubIssue) As String
            Dim prediction = _predEngine.Predict(issue)

            Return prediction.Area
        End Function

        Private Sub ApplyLabel(issue As Issue, label As String)
            Dim issueUpdate = New IssueUpdate()
            issueUpdate.AddLabel(label)

            _client.Issue.Update(_repoOwner, _repoName, issue.Number, issueUpdate)

            Console.WriteLine($"Issue {issue.Number} : ""{issue.Title}"" " & vbTab & " was labeled as: {label}")
        End Sub
    End Class
End Namespace