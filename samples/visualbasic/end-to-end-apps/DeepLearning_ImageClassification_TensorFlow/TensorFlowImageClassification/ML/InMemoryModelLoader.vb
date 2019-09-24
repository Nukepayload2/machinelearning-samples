Imports Microsoft.Extensions.ML
Imports Microsoft.Extensions.Primitives
Imports Microsoft.ML
Imports System.Threading

Namespace TensorFlowImageClassification.ML
	Public Class InMemoryModelLoader
		Inherits ModelLoader

		Private ReadOnly _model As ITransformer

		Public Sub New(model As ITransformer)
			_model = model
		End Sub

		Public Overrides Function GetModel() As ITransformer
			Return _model
		End Function

		Public Overrides Function GetReloadToken() As IChangeToken
			Return New CancellationChangeToken(CancellationToken.None)
		End Function
	End Class
End Namespace
