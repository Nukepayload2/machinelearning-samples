Namespace OnnxObjectDetection
	Public Interface IOnnxModel
		ReadOnly Property ModelPath As String

		' To check Model input and output parameter names, you can
		' use tools like Netron: https://github.com/lutzroeder/netron
		ReadOnly Property ModelInput As String
		ReadOnly Property ModelOutput As String

		ReadOnly Property Labels As String()
'INSTANT VB TODO TASK: There is no equivalent in VB to C# default interface methods:
'		(float, float)[] Anchors
'		{
'			get;
'		}
	End Interface

	Public Interface IOnnxObjectPrediction
		Property PredictedLabels As Single()
	End Interface
End Namespace
