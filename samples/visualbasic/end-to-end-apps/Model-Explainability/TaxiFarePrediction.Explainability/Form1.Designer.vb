Namespace TaxiFareRegression.Explainability
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
			Me.plot1 = New OxyPlot.WindowsForms.PlotView
			Me.button1 = New System.Windows.Forms.Button
			Me.lblTrip = New System.Windows.Forms.Label
			Me.lblTripID = New System.Windows.Forms.Label
			Me.label1 = New System.Windows.Forms.Label
			Me.lblFare = New System.Windows.Forms.Label
			Me.button2 = New System.Windows.Forms.Button
			Me.SuspendLayout()
			' 
			' plot1
			' 
			Me.plot1.Location = New System.Drawing.Point(16, 53)
			Me.plot1.Name = "plot1"
			Me.plot1.PanCursor = System.Windows.Forms.Cursors.Hand
			Me.plot1.Size = New System.Drawing.Size(550, 357)
			Me.plot1.TabIndex = 0
			Me.plot1.Text = "plot1"
			Me.plot1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE
			Me.plot1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE
			Me.plot1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.plot1.Click += new System.EventHandler(this.Plot1_Click);
			' 
			' button1
			' 
			Me.button1.Location = New System.Drawing.Point(491, 437)
			Me.button1.Name = "button1"
			Me.button1.Size = New System.Drawing.Size(75, 23)
			Me.button1.TabIndex = 1
			Me.button1.Text = "Next"
			Me.button1.UseVisualStyleBackColor = True
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.button1.Click += new System.EventHandler(this.Button1_Click);
			' 
			' lblTrip
			' 
			Me.lblTrip.AutoSize = True
			Me.lblTrip.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte(0)))
			Me.lblTrip.Location = New System.Drawing.Point(13, 13)
			Me.lblTrip.Name = "lblTrip"
			Me.lblTrip.Size = New System.Drawing.Size(58, 24)
			Me.lblTrip.TabIndex = 2
			Me.lblTrip.Text = "Trip #"
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.lblTrip.Click += new System.EventHandler(this.Label1_Click);
			' 
			' lblTripID
			' 
			Me.lblTripID.AutoSize = True
			Me.lblTripID.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte(0)))
			Me.lblTripID.Location = New System.Drawing.Point(66, 13)
			Me.lblTripID.Name = "lblTripID"
			Me.lblTripID.Size = New System.Drawing.Size(20, 24)
			Me.lblTripID.TabIndex = 3
			Me.lblTripID.Text = "1"
			' 
			' label1
			' 
			Me.label1.AutoSize = True
			Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte(0)))
			Me.label1.Location = New System.Drawing.Point(92, 13)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(134, 24)
			Me.label1.TabIndex = 4
			Me.label1.Text = "Predicted Fare"
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.label1.Click += new System.EventHandler(this.Label1_Click_1);
			' 
			' lblFare
			' 
			Me.lblFare.AutoSize = True
			Me.lblFare.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte(0)))
			Me.lblFare.Location = New System.Drawing.Point(232, 13)
			Me.lblFare.Name = "lblFare"
			Me.lblFare.Size = New System.Drawing.Size(45, 24)
			Me.lblFare.TabIndex = 5
			Me.lblFare.Text = "1.20"
			' 
			' button2
			' 
			Me.button2.Location = New System.Drawing.Point(410, 437)
			Me.button2.Name = "button2"
			Me.button2.Size = New System.Drawing.Size(75, 23)
			Me.button2.TabIndex = 6
			Me.button2.Text = "Previous"
			Me.button2.UseVisualStyleBackColor = True
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.button2.Click += new System.EventHandler(this.Button2_Click);
			' 
			' Form1
			' 
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.ClientSize = New System.Drawing.Size(593, 472)
			Me.Controls.Add(Me.button2)
			Me.Controls.Add(Me.lblFare)
			Me.Controls.Add(Me.label1)
			Me.Controls.Add(Me.lblTripID)
			Me.Controls.Add(Me.lblTrip)
			Me.Controls.Add(Me.button1)
			Me.Controls.Add(Me.plot1)
			Me.Name = "Form1"
			Me.Text = "Example 1 (WindowsForms)"
'INSTANT VB NOTE: The following InitializeComponent event wireup was converted to a 'Handles' clause:
'ORIGINAL LINE: this.Load += new System.EventHandler(this.Form1_Load_1);
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

		#End Region
		Private WithEvents button1 As System.Windows.Forms.Button
		Public WithEvents plot1 As OxyPlot.WindowsForms.PlotView
		Private WithEvents lblTrip As System.Windows.Forms.Label
		Private lblTripID As System.Windows.Forms.Label
		Private WithEvents label1 As System.Windows.Forms.Label
		Private lblFare As System.Windows.Forms.Label
		Private WithEvents button2 As System.Windows.Forms.Button
	End Class
End Namespace

