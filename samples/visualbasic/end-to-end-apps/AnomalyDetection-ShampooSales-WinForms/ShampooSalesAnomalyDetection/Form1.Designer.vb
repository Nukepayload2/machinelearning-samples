Namespace ShampooSalesAnomalyDetection
	Partial Public Class Form1
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container
			Dim chartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea
			Dim legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend
			Dim legendItem1 As System.Windows.Forms.DataVisualization.Charting.LegendItem = New System.Windows.Forms.DataVisualization.Charting.LegendItem
			Dim legendItem2 As System.Windows.Forms.DataVisualization.Charting.LegendItem = New System.Windows.Forms.DataVisualization.Charting.LegendItem
			Dim series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series
			Dim series2 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series
			Me.debugInstructionsLabel = New System.Windows.Forms.Label
			Me.button1 = New System.Windows.Forms.Button
			Me.helloWorldLabel = New System.Windows.Forms.Label
			Me.filePathTextbox = New System.Windows.Forms.TextBox
			Me.dataGridView1 = New System.Windows.Forms.DataGridView
			Me.button2 = New System.Windows.Forms.Button
			Me.performanceCounter1 = New System.Diagnostics.PerformanceCounter
			Me.commaSeparatedRadio = New System.Windows.Forms.RadioButton
			Me.tabSeparatedRadio = New System.Windows.Forms.RadioButton
			Me.backgroundWorker1 = New System.ComponentModel.BackgroundWorker
			Me.label1 = New System.Windows.Forms.Label
			Me.label2 = New System.Windows.Forms.Label
			Me.backgroundWorker2 = New System.ComponentModel.BackgroundWorker
			Me.contextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
			Me.contextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
			Me.anomalyText = New System.Windows.Forms.RichTextBox
			Me.label3 = New System.Windows.Forms.Label
			Me.graph = New System.Windows.Forms.DataVisualization.Charting.Chart
			Me.openFileExplorer = New System.Windows.Forms.OpenFileDialog
			Me.panel1 = New System.Windows.Forms.Panel
			Me.panel2 = New System.Windows.Forms.Panel
			Me.changePointDet = New System.Windows.Forms.CheckBox
			Me.spikeDet = New System.Windows.Forms.CheckBox
			CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me.performanceCounter1, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me.graph, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.panel1.SuspendLayout()
			Me.panel2.SuspendLayout()
			Me.SuspendLayout()
			' 
			' debugInstructionsLabel
			' 
			Me.debugInstructionsLabel.AutoSize = True
			Me.debugInstructionsLabel.Location = New System.Drawing.Point(17, 105)
			Me.debugInstructionsLabel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			Me.debugInstructionsLabel.Name = "debugInstructionsLabel"
			Me.debugInstructionsLabel.Size = New System.Drawing.Size(154, 25)
			Me.debugInstructionsLabel.TabIndex = 1
			Me.debugInstructionsLabel.Text = "Data File Path:"
			' 
			' button1
			' 
			Me.button1.Location = New System.Drawing.Point(423, 130)
			Me.button1.Margin = New System.Windows.Forms.Padding(4)
			Me.button1.Name = "button1"
			Me.button1.Size = New System.Drawing.Size(121, 46)
			Me.button1.TabIndex = 2
			Me.button1.Text = "Find"
			Me.button1.UseVisualStyleBackColor = True
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.button1.Click += new System.EventHandler(this.button1_Click);
			' 
			' helloWorldLabel
			' 
			Me.helloWorldLabel.AutoSize = True
			Me.helloWorldLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte(0)))
			Me.helloWorldLabel.Location = New System.Drawing.Point(13, 25)
			Me.helloWorldLabel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			Me.helloWorldLabel.Name = "helloWorldLabel"
			Me.helloWorldLabel.Size = New System.Drawing.Size(385, 51)
			Me.helloWorldLabel.TabIndex = 3
			Me.helloWorldLabel.Text = "Anomaly Detection"
			' 
			' filePathTextbox
			' 
			Me.filePathTextbox.Location = New System.Drawing.Point(22, 138)
			Me.filePathTextbox.Name = "filePathTextbox"
			Me.filePathTextbox.Size = New System.Drawing.Size(394, 31)
			Me.filePathTextbox.TabIndex = 4
			' 
			' dataGridView1
			' 
			Me.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			Me.dataGridView1.Location = New System.Drawing.Point(18, 388)
			Me.dataGridView1.Name = "dataGridView1"
			Me.dataGridView1.RowTemplate.Height = 33
			Me.dataGridView1.Size = New System.Drawing.Size(526, 812)
			Me.dataGridView1.TabIndex = 5
			' 
			' button2
			' 
			Me.button2.Location = New System.Drawing.Point(14, 310)
			Me.button2.Margin = New System.Windows.Forms.Padding(4)
			Me.button2.Name = "button2"
			Me.button2.Size = New System.Drawing.Size(544, 46)
			Me.button2.TabIndex = 6
			Me.button2.Text = "Go"
			Me.button2.UseVisualStyleBackColor = True
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.button2.Click += new System.EventHandler(this.button2_Click);
			' 
			' commaSeparatedRadio
			' 
			Me.commaSeparatedRadio.AutoSize = True
			Me.commaSeparatedRadio.Checked = True
			Me.commaSeparatedRadio.Location = New System.Drawing.Point(13, 19)
			Me.commaSeparatedRadio.Name = "commaSeparatedRadio"
			Me.commaSeparatedRadio.Size = New System.Drawing.Size(221, 29)
			Me.commaSeparatedRadio.TabIndex = 9
			Me.commaSeparatedRadio.TabStop = True
			Me.commaSeparatedRadio.Text = "Comma Separated"
			Me.commaSeparatedRadio.UseVisualStyleBackColor = True
			' 
			' tabSeparatedRadio
			' 
			Me.tabSeparatedRadio.AutoSize = True
			Me.tabSeparatedRadio.Location = New System.Drawing.Point(13, 66)
			Me.tabSeparatedRadio.Name = "tabSeparatedRadio"
			Me.tabSeparatedRadio.Size = New System.Drawing.Size(185, 29)
			Me.tabSeparatedRadio.TabIndex = 10
			Me.tabSeparatedRadio.Text = "Tab Separated"
			Me.tabSeparatedRadio.UseVisualStyleBackColor = True
			' 
			' label1
			' 
			Me.label1.AutoSize = True
			Me.label1.Location = New System.Drawing.Point(13, 360)
			Me.label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(197, 25)
			Me.label1.TabIndex = 11
			Me.label1.Text = "Data View Preview:"
			' 
			' label2
			' 
			Me.label2.AutoSize = True
			Me.label2.Location = New System.Drawing.Point(600, 130)
			Me.label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			Me.label2.Name = "label2"
			Me.label2.Size = New System.Drawing.Size(210, 25)
			Me.label2.TabIndex = 13
			Me.label2.Text = "Anomalies Detected:"
			' 
			' contextMenuStrip1
			' 
			Me.contextMenuStrip1.ImageScalingSize = New System.Drawing.Size(32, 32)
			Me.contextMenuStrip1.Name = "contextMenuStrip1"
			Me.contextMenuStrip1.Size = New System.Drawing.Size(61, 4)
			' 
			' contextMenuStrip2
			' 
			Me.contextMenuStrip2.ImageScalingSize = New System.Drawing.Size(32, 32)
			Me.contextMenuStrip2.Name = "contextMenuStrip2"
			Me.contextMenuStrip2.Size = New System.Drawing.Size(61, 4)
			' 
			' anomalyText
			' 
			Me.anomalyText.ForeColor = System.Drawing.Color.Black
			Me.anomalyText.Location = New System.Drawing.Point(605, 168)
			Me.anomalyText.Name = "anomalyText"
			Me.anomalyText.Size = New System.Drawing.Size(1194, 174)
			Me.anomalyText.TabIndex = 17
			Me.anomalyText.Text = ""
			' 
			' label3
			' 
			Me.label3.AutoSize = True
			Me.label3.Location = New System.Drawing.Point(600, 360)
			Me.label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			Me.label3.Name = "label3"
			Me.label3.Size = New System.Drawing.Size(77, 25)
			Me.label3.TabIndex = 18
			Me.label3.Text = "Graph:"
			' 
			' graph
			' 
			chartArea1.AxisX.Title = "Month"
			chartArea1.AxisY.Maximum = 700R
			chartArea1.AxisY.Minimum = 0R
			chartArea1.AxisY.Title = "Sales"
			chartArea1.Name = "ChartArea1"
			Me.graph.ChartAreas.Add(chartArea1)
			legendItem1.ImageStyle = System.Windows.Forms.DataVisualization.Charting.LegendImageStyle.Marker
			legendItem1.MarkerBorderColor = System.Drawing.Color.DarkRed
			legendItem1.MarkerBorderWidth = 0
			legendItem1.MarkerColor = System.Drawing.Color.DarkRed
			legendItem1.MarkerSize = 15
			legendItem1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4
			legendItem1.Name = "Spike"
			legendItem2.ImageStyle = System.Windows.Forms.DataVisualization.Charting.LegendImageStyle.Marker
			legendItem2.MarkerBorderColor = System.Drawing.Color.DarkBlue
			legendItem2.MarkerBorderWidth = 0
			legendItem2.MarkerColor = System.Drawing.Color.DarkBlue
			legendItem2.MarkerSize = 15
			legendItem2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4
			legendItem2.Name = "Change point"
			legend1.CustomItems.Add(legendItem1)
			legend1.CustomItems.Add(legendItem2)
			legend1.Enabled = False
			legend1.Name = "Legend1"
			Me.graph.Legends.Add(legend1)
			Me.graph.Location = New System.Drawing.Point(605, 388)
			Me.graph.Name = "graph"
			Me.graph.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None
			series1.ChartArea = "ChartArea1"
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
			series1.Color = System.Drawing.Color.DimGray
			series1.IsVisibleInLegend = False
			series1.IsXValueIndexed = True
			series1.Legend = "Legend1"
			series1.Name = "Series1"
			series2.ChartArea = "ChartArea1"
			series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point
			series2.IsVisibleInLegend = False
			series2.Legend = "Legend1"
			series2.Name = "Series2"
			Me.graph.Series.Add(series1)
			Me.graph.Series.Add(series2)
			Me.graph.Size = New System.Drawing.Size(1194, 812)
			Me.graph.TabIndex = 19
			Me.graph.Text = "graph"
			' 
			' panel1
			' 
			Me.panel1.Controls.Add(Me.commaSeparatedRadio)
			Me.panel1.Controls.Add(Me.tabSeparatedRadio)
			Me.panel1.Location = New System.Drawing.Point(14, 183)
			Me.panel1.Name = "panel1"
			Me.panel1.Size = New System.Drawing.Size(241, 116)
			Me.panel1.TabIndex = 22
			' 
			' panel2
			' 
			Me.panel2.Controls.Add(Me.changePointDet)
			Me.panel2.Controls.Add(Me.spikeDet)
			Me.panel2.Location = New System.Drawing.Point(261, 183)
			Me.panel2.Name = "panel2"
			Me.panel2.Size = New System.Drawing.Size(297, 116)
			Me.panel2.TabIndex = 23
			' 
			' changePointDet
			' 
			Me.changePointDet.AutoSize = True
			Me.changePointDet.Location = New System.Drawing.Point(24, 66)
			Me.changePointDet.Name = "changePointDet"
			Me.changePointDet.Size = New System.Drawing.Size(271, 29)
			Me.changePointDet.TabIndex = 25
			Me.changePointDet.Text = "Change Point Detection"
			Me.changePointDet.UseVisualStyleBackColor = True
			' 
			' spikeDet
			' 
			Me.spikeDet.AutoSize = True
			Me.spikeDet.Location = New System.Drawing.Point(24, 20)
			Me.spikeDet.Name = "spikeDet"
			Me.spikeDet.Size = New System.Drawing.Size(195, 29)
			Me.spikeDet.TabIndex = 24
			Me.spikeDet.Text = "Spike Detection"
			Me.spikeDet.UseVisualStyleBackColor = True
			' 
			' Form1
			' 
			Me.AutoScaleDimensions = New System.Drawing.SizeF(12F, 25F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.AutoSize = True
			Me.ClientSize = New System.Drawing.Size(1841, 1231)
			Me.Controls.Add(Me.panel2)
			Me.Controls.Add(Me.panel1)
			Me.Controls.Add(Me.graph)
			Me.Controls.Add(Me.label3)
			Me.Controls.Add(Me.anomalyText)
			Me.Controls.Add(Me.label2)
			Me.Controls.Add(Me.label1)
			Me.Controls.Add(Me.button2)
			Me.Controls.Add(Me.dataGridView1)
			Me.Controls.Add(Me.filePathTextbox)
			Me.Controls.Add(Me.helloWorldLabel)
			Me.Controls.Add(Me.button1)
			Me.Controls.Add(Me.debugInstructionsLabel)
			Me.Margin = New System.Windows.Forms.Padding(4)
			Me.Name = "Form1"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Anomaly Detection"
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.Load += new System.EventHandler(this.Form1_Load);
			CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
			'((System.ComponentModel.ISupportInitialize)(this.performanceCounter1)).EndInit();
			CType(Me.graph, System.ComponentModel.ISupportInitialize).EndInit()
			Me.panel1.ResumeLayout(False)
			Me.panel1.PerformLayout()
			Me.panel2.ResumeLayout(False)
			Me.panel2.PerformLayout()
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

		#End Region
		Private debugInstructionsLabel As System.Windows.Forms.Label
		Private WithEvents button1 As System.Windows.Forms.Button
		Private helloWorldLabel As System.Windows.Forms.Label
		Private filePathTextbox As System.Windows.Forms.TextBox
		Private dataGridView1 As System.Windows.Forms.DataGridView
		Private WithEvents button2 As System.Windows.Forms.Button
		Private performanceCounter1 As System.Diagnostics.PerformanceCounter
		Private commaSeparatedRadio As System.Windows.Forms.RadioButton
		Private tabSeparatedRadio As System.Windows.Forms.RadioButton
		Private backgroundWorker1 As System.ComponentModel.BackgroundWorker
		Private label1 As System.Windows.Forms.Label
		Private label2 As System.Windows.Forms.Label
		Private backgroundWorker2 As System.ComponentModel.BackgroundWorker
		Private contextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
		Private contextMenuStrip2 As System.Windows.Forms.ContextMenuStrip
		Private anomalyText As System.Windows.Forms.RichTextBox
		Private label3 As System.Windows.Forms.Label
		Private graph As System.Windows.Forms.DataVisualization.Charting.Chart
		Private openFileExplorer As System.Windows.Forms.OpenFileDialog
		Private panel1 As System.Windows.Forms.Panel
		Private panel2 As System.Windows.Forms.Panel
		Private spikeDet As System.Windows.Forms.CheckBox
		Private changePointDet As System.Windows.Forms.CheckBox
	End Class
End Namespace

