Namespace OnnxObjectDetection
	Public Interface IOnnxModel
		ReadOnly Property ModelPath As String

		' To check Model input and output parameter names, you can
		' use tools like Netron: https://github.com/lutzroeder/netron
		ReadOnly Property ModelInput As String
		ReadOnly Property ModelOutput As String

		ReadOnly Property Labels As String()

        ReadOnly Property Anchors As (Single, Single)()
    End Interface

	Public Interface IOnnxObjectPrediction
		Property PredictedLabels As Single()
	End Interface
End Namespace
