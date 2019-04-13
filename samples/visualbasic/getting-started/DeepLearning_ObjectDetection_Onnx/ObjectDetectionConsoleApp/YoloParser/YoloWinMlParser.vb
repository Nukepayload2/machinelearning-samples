Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ObjectDetection
	Friend Class YoloWinMlParser
		Public Const ROW_COUNT As Integer = 13
		Public Const COL_COUNT As Integer = 13
		Public Const CHANNEL_COUNT As Integer = 125
		Public Const BOXES_PER_CELL As Integer = 5
		Public Const BOX_INFO_FEATURE_COUNT As Integer = 5
		Public Const CLASS_COUNT As Integer = 20
		Public Const CELL_WIDTH As Single = 32
		Public Const CELL_HEIGHT As Single = 32

		Private channelStride As Integer = ROW_COUNT * COL_COUNT

		Private anchors() As Single = { 1.08F, 1.19F, 3.42F, 4.41F, 6.63F, 11.38F, 9.42F, 5.11F, 16.62F, 10.52F }

		Private labels() As String = { "aeroplane", "bicycle", "bird", "boat", "bottle", "bus", "car", "cat", "chair", "cow", "diningtable", "dog", "horse", "motorbike", "person", "pottedplant", "sheep", "sofa", "train", "tvmonitor" }

		Public Function ParseOutputs(yoloModelOutputs() As Single, Optional threshold As Single = .3F) As IList(Of YoloBoundingBox)
			Dim boxes = New List(Of YoloBoundingBox)

			Dim featuresPerBox = BOX_INFO_FEATURE_COUNT + CLASS_COUNT
			Dim stride = featuresPerBox * BOXES_PER_CELL

			For cy As Integer = 0 To ROW_COUNT - 1
				For cx As Integer = 0 To COL_COUNT - 1
					For b As Integer = 0 To BOXES_PER_CELL - 1
						Dim channel = (b * (CLASS_COUNT + BOX_INFO_FEATURE_COUNT))

						Dim tx = yoloModelOutputs(GetOffset(cx, cy, channel))
						Dim ty = yoloModelOutputs(GetOffset(cx, cy, channel + 1))
						Dim tw = yoloModelOutputs(GetOffset(cx, cy, channel + 2))
						Dim th = yoloModelOutputs(GetOffset(cx, cy, channel + 3))
						Dim tc = yoloModelOutputs(GetOffset(cx, cy, channel + 4))

						Dim x = (CSng(cx) + Sigmoid(tx)) * CELL_WIDTH
						Dim y = (CSng(cy) + Sigmoid(ty)) * CELL_HEIGHT
						Dim width = CSng(Math.Exp(tw)) * CELL_WIDTH * Me.anchors(b * 2)
						Dim height = CSng(Math.Exp(th)) * CELL_HEIGHT * Me.anchors(b * 2 + 1)

						Dim confidence = Sigmoid(tc)

						If confidence < threshold Then
							Continue For
						End If

						Dim classes = New Single(CLASS_COUNT - 1){}
						Dim classOffset = channel + BOX_INFO_FEATURE_COUNT

						For i As Integer = 0 To CLASS_COUNT - 1
							classes(i) = yoloModelOutputs(GetOffset(cx, cy, i + classOffset))
						Next i

						Dim results = Softmax(classes).Select(Function(v, ik) New With {
							Key .Value = v,
							Key .Index = ik
						})

						Dim topClass = results.OrderByDescending(Function(r) r.Value).First().Index
						Dim topScore = results.OrderByDescending(Function(r) r.Value).First().Value * confidence
						Dim testSum = results.Sum(Function(r) r.Value)

						If topScore < threshold Then
							Continue For
						End If

						boxes.Add(New YoloBoundingBox With {
							.Confidence = topScore,
							.X = (x - width / 2),
							.Y = (y - height / 2),
							.Width = width,
							.Height = height,
							.Label = Me.labels(topClass)
						})
					Next b
				Next cx
			Next cy

			Return boxes
		End Function

		Public Function NonMaxSuppress(boxes As IList(Of YoloBoundingBox), limit As Integer, threshold As Single) As IList(Of YoloBoundingBox)
			Dim activeCount = boxes.Count
			Dim isActiveBoxes = New Boolean(boxes.Count - 1){}

			For i As Integer = 0 To isActiveBoxes.Length - 1
				isActiveBoxes(i) = True
			Next i

			Dim sortedBoxes = boxes.Select(Function(b, i) New With {
				Key .Box = b,
				Key .Index = i
			}).OrderByDescending(Function(b) b.Box.Confidence).ToList()

			Dim results = New List(Of YoloBoundingBox)

			For i As Integer = 0 To boxes.Count - 1
				If isActiveBoxes(i) Then
					Dim boxA = sortedBoxes(i).Box
					results.Add(boxA)

					If results.Count >= limit Then
						Exit For
					End If

					For j = i + 1 To boxes.Count - 1
						If isActiveBoxes(j) Then
							Dim boxB = sortedBoxes(j).Box

							If IntersectionOverUnion(boxA.Rect, boxB.Rect) > threshold Then
								isActiveBoxes(j) = False
								activeCount -= 1

								If activeCount <= 0 Then
									Exit For
								End If
							End If
						End If
					Next j

					If activeCount <= 0 Then
						Exit For
					End If
				End If
			Next i

			Return results
		End Function

		Private Function IntersectionOverUnion(a As RectangleF, b As RectangleF) As Single
			Dim areaA = a.Width * a.Height

			If areaA <= 0 Then
				Return 0
			End If

			Dim areaB = b.Width * b.Height

			If areaB <= 0 Then
				Return 0
			End If

			Dim minX = Math.Max(a.Left, b.Left)
			Dim minY = Math.Max(a.Top, b.Top)
			Dim maxX = Math.Min(a.Right, b.Right)
			Dim maxY = Math.Min(a.Bottom, b.Bottom)

			Dim intersectionArea = Math.Max(maxY - minY, 0) * Math.Max(maxX - minX, 0)

			Return intersectionArea / (areaA + areaB - intersectionArea)
		End Function

		Private Function GetOffset(x As Integer, y As Integer, channel As Integer) As Integer
			' YOLO outputs a tensor that has a shape of 125x13x13, which 
			' WinML flattens into a 1D array.  To access a specific channel 
			' for a given (x,y) cell position, we need to calculate an offset
			' into the array
			Return (channel * Me.channelStride) + (y * COL_COUNT) + x
		End Function

		Private Function Sigmoid(value As Single) As Single
			Dim k = CSng(Math.Exp(value))

			Return k / (1.0F + k)
		End Function

		Private Function Softmax(values() As Single) As Single()
			Dim maxVal = values.Max()
			Dim exp = values.Select(Function(v) Math.Exp(v - maxVal))
			Dim sumExp = exp.Sum()

'INSTANT VB WARNING: Instant VB cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Return exp.Select(Function(v) CSng(v / sumExp)).ToArray()
		End Function
	End Class
End Namespace