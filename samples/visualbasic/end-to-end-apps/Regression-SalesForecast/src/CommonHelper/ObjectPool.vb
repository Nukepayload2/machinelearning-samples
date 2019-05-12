Imports System
Imports System.Collections.Concurrent

Namespace CommonHelpers
	Public Class ObjectPool(Of T)
		Private _objects As ConcurrentBag(Of Tuple(Of T, DateTime))
		Private _objectGenerator As Func(Of T)
		Private _maxPoolSize As Integer
		Private _minPoolSize As Integer
		Private _expirationTime As Double

		Public ReadOnly Property CurrentPoolSize As Integer
			Get
				Return _objects.Count
			End Get
		End Property

		''' <param name="objectGenerator"></param>
		''' <param name="minPoolSize">Minimum number of objects in pool, as goal. Could be less but eventually it'll tend to that number</param>
		''' <param name="maxPoolSize">Maximum number of objects in pool</param>
		''' <param name="expirationTime">Expiration Time (mlSecs) of object since added to the pool</param>
		Public Sub New(objectGenerator As Func(Of T), Optional minPoolSize As Integer = 5, Optional maxPoolSize As Integer = 1000, Optional expirationTime As Double = 30000)
			If objectGenerator Is Nothing Then
				Throw New ArgumentNullException("objectGenerator")
			End If
			If minPoolSize > maxPoolSize Then
				Throw New Exception("minPoolSize cannot be higher than maxPoolSize")
			End If
			If minPoolSize <= 0 Then
				Throw New Exception("minPoolSize cannot be equal or lower than cero")
			End If
			If maxPoolSize <= 0 Then
				Throw New Exception("maxPoolSize cannot be equal or lower than cero")
			End If
			If expirationTime <= 0 Then
				Throw New Exception("expirationTime cannot be equal or lower than cero")
			End If

			_objects = New ConcurrentBag(Of Tuple(Of T, DateTime))
			_objectGenerator = objectGenerator
			_maxPoolSize = maxPoolSize
			_minPoolSize = minPoolSize
			_expirationTime = expirationTime

			'Measure total time of minimum objects creation 
			Dim watch = System.Diagnostics.Stopwatch.StartNew()

			'Create minimum number of objects in pool
			For i As Integer = 0 To minPoolSize - 1
				Dim tuple As New Tuple(Of T, DateTime)(_objectGenerator(), DateTime.UtcNow)
				_objects.Add(tuple)
			Next i

			'Stop measuring time
			watch.Stop()
			Dim elapsedMs As Long = watch.ElapsedMilliseconds
		End Sub

		Public Function GetObject() As T
			Dim now As DateTime = DateTime.UtcNow

			Do While Not _objects.IsEmpty
				Dim tuple As Tuple(Of T, DateTime) = Nothing
				If _objects.TryTake(tuple) Then
					If DateTime.UtcNow.Subtract(tuple.Item2) < TimeSpan.FromMilliseconds(_expirationTime) Then
						'object has NOT expired, so return it
						Return tuple.Item1
					Else
					'If object is expired, but we have less or equal the threadshold of minPoolSize, then use it    
						If _objects.Count <= _minPoolSize Then
							Return tuple.Item1
						End If
					End If

					'If it gets here, do nothing with the expired-object and try to get another non-expired object
				End If
			Loop

			' If there are no objects available in the pool, create one and return it   
			Return _objectGenerator()
		End Function

		Public Sub PutObject(item As T)
			'Only add objects to the pool if maxPoolSize has not been reached
			If _objects.Count < _maxPoolSize Then
				'Expiration time starts when adding an object to the pool, no matter if it was originally older
				Dim tuple As New Tuple(Of T, DateTime)(item, DateTime.UtcNow)
				_objects.Add(tuple)
			End If
		End Sub
	End Class
End Namespace
