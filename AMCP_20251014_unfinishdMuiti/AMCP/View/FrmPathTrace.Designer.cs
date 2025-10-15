namespace AMCP
{
    partial class FrmPathTrace
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint7 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint8 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, "0,0");
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint5 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint6 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, "0,0");
            this.positionChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.lblMouseAxisXY = new System.Windows.Forms.Label();
            this.btnClearChart = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnLocate = new System.Windows.Forms.Button();
            this.positionChart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lblCurrentLayer = new System.Windows.Forms.Label();
            this.lblCurrentCmd = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.positionChart)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.positionChart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // positionChart
            // 
            chartArea4.AlignmentOrientation = ((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations)((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Vertical | System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Horizontal)));
            chartArea4.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea4.AxisX.IsLabelAutoFit = false;
            chartArea4.AxisX.LabelAutoFitMaxFontSize = 8;
            chartArea4.AxisX.LabelAutoFitMinFontSize = 8;
            chartArea4.AxisX.LabelStyle.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea4.AxisX.LabelStyle.Format = "0";
            chartArea4.AxisX.LabelStyle.Interval = 0D;
            chartArea4.AxisX.LabelStyle.IntervalOffset = 0D;
            chartArea4.AxisX.LabelStyle.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea4.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea4.AxisX.LabelStyle.IsEndLabelVisible = false;
            chartArea4.AxisX.LabelStyle.TruncatedLabels = true;
            chartArea4.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gainsboro;
            chartArea4.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea4.AxisX.MajorTickMark.Interval = 0D;
            chartArea4.AxisX.Maximum = 200D;
            chartArea4.AxisX.MaximumAutoSize = 50F;
            chartArea4.AxisX.Minimum = 0D;
            chartArea4.AxisX.Title = "X";
            chartArea4.AxisX.TitleFont = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea4.AxisX2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea4.AxisX2.LabelStyle.Enabled = false;
            chartArea4.AxisY.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea4.AxisY.IsLabelAutoFit = false;
            chartArea4.AxisY.IsReversed = true;
            chartArea4.AxisY.LabelAutoFitMaxFontSize = 8;
            chartArea4.AxisY.LabelAutoFitMinFontSize = 8;
            chartArea4.AxisY.LabelStyle.Angle = -90;
            chartArea4.AxisY.LabelStyle.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea4.AxisY.LabelStyle.Format = "0";
            chartArea4.AxisY.LabelStyle.Interval = 0D;
            chartArea4.AxisY.LabelStyle.IntervalOffset = 0D;
            chartArea4.AxisY.LabelStyle.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea4.AxisY.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea4.AxisY.LabelStyle.IsEndLabelVisible = false;
            chartArea4.AxisY.LabelStyle.TruncatedLabels = true;
            chartArea4.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gainsboro;
            chartArea4.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea4.AxisY.MajorTickMark.Interval = 0D;
            chartArea4.AxisY.Maximum = 200D;
            chartArea4.AxisY.MaximumAutoSize = 50F;
            chartArea4.AxisY.Minimum = 0D;
            chartArea4.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            chartArea4.AxisY.Title = "Y";
            chartArea4.AxisY.TitleFont = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea4.AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea4.AxisY2.LabelStyle.Enabled = false;
            chartArea4.InnerPlotPosition.Auto = false;
            chartArea4.InnerPlotPosition.Height = 92F;
            chartArea4.InnerPlotPosition.Width = 90F;
            chartArea4.InnerPlotPosition.X = 9F;
            chartArea4.InnerPlotPosition.Y = 1F;
            chartArea4.IsSameFontSizeForAllAxes = true;
            chartArea4.Name = "ChartArea1";
            chartArea4.Position.Auto = false;
            chartArea4.Position.Height = 93F;
            chartArea4.Position.Width = 97F;
            chartArea4.Position.Y = 7F;
            this.positionChart.ChartAreas.Add(chartArea4);
            this.positionChart.ContextMenuStrip = this.contextMenuStrip1;
            this.positionChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionChart.Location = new System.Drawing.Point(0, 0);
            this.positionChart.Name = "positionChart";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Name = "Series0";
            series7.Points.Add(dataPoint7);
            series8.BorderColor = System.Drawing.Color.Red;
            series8.BorderWidth = 5;
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series8.Color = System.Drawing.Color.Red;
            series8.MarkerBorderColor = System.Drawing.Color.Lime;
            series8.MarkerBorderWidth = 0;
            series8.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            series8.MarkerSize = 10;
            series8.MarkerStep = 2;
            series8.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series8.Name = "Series1";
            series8.Points.Add(dataPoint8);
            series8.YValuesPerPoint = 2;
            this.positionChart.Series.Add(series7);
            this.positionChart.Series.Add(series8);
            this.positionChart.Size = new System.Drawing.Size(663, 566);
            this.positionChart.TabIndex = 9;
            this.positionChart.Text = "chart1";
            this.positionChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.positionChart_MouseDown);
            this.positionChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.positionChart_MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem1,
            this.ToolStripMenuItem2,
            this.ToolStripMenuItem3});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 70);
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.Checked = true;
            this.ToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItem1.Text = "X-Y平面+Z轴";
            this.ToolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // ToolStripMenuItem2
            // 
            this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            this.ToolStripMenuItem2.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItem2.Text = "X-Z平面+Y轴";
            this.ToolStripMenuItem2.Click += new System.EventHandler(this.ToolStripMenuItem2_Click);
            // 
            // ToolStripMenuItem3
            // 
            this.ToolStripMenuItem3.Name = "ToolStripMenuItem3";
            this.ToolStripMenuItem3.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItem3.Text = "Y-Z平面+X轴";
            this.ToolStripMenuItem3.Click += new System.EventHandler(this.ToolStripMenuItem3_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.BackColor = System.Drawing.Color.White;
            this.btnZoomOut.Location = new System.Drawing.Point(89, 18);
            this.btnZoomOut.Margin = new System.Windows.Forms.Padding(2);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(26, 26);
            this.btnZoomOut.TabIndex = 129;
            this.btnZoomOut.Text = "-";
            this.btnZoomOut.UseVisualStyleBackColor = false;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.BackColor = System.Drawing.Color.White;
            this.btnZoomIn.Location = new System.Drawing.Point(40, 18);
            this.btnZoomIn.Margin = new System.Windows.Forms.Padding(2);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(26, 26);
            this.btnZoomIn.TabIndex = 128;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.UseVisualStyleBackColor = false;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // lblMouseAxisXY
            // 
            this.lblMouseAxisXY.AutoSize = true;
            this.lblMouseAxisXY.Location = new System.Drawing.Point(231, 25);
            this.lblMouseAxisXY.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMouseAxisXY.Name = "lblMouseAxisXY";
            this.lblMouseAxisXY.Size = new System.Drawing.Size(23, 12);
            this.lblMouseAxisXY.TabIndex = 127;
            this.lblMouseAxisXY.Text = "0,0";
            // 
            // btnClearChart
            // 
            this.btnClearChart.BackColor = System.Drawing.Color.White;
            this.btnClearChart.Location = new System.Drawing.Point(427, 18);
            this.btnClearChart.Margin = new System.Windows.Forms.Padding(2);
            this.btnClearChart.Name = "btnClearChart";
            this.btnClearChart.Size = new System.Drawing.Size(62, 26);
            this.btnClearChart.TabIndex = 126;
            this.btnClearChart.Text = "清除轨迹";
            this.btnClearChart.UseVisualStyleBackColor = false;
            this.btnClearChart.Click += new System.EventHandler(this.btnClearChart_Click);
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(324, 18);
            this.btnReset.Margin = new System.Windows.Forms.Padding(2);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(62, 26);
            this.btnReset.TabIndex = 125;
            this.btnReset.Text = "复位";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnLocate
            // 
            this.btnLocate.BackColor = System.Drawing.Color.White;
            this.btnLocate.Location = new System.Drawing.Point(158, 18);
            this.btnLocate.Margin = new System.Windows.Forms.Padding(2);
            this.btnLocate.Name = "btnLocate";
            this.btnLocate.Size = new System.Drawing.Size(62, 26);
            this.btnLocate.TabIndex = 124;
            this.btnLocate.Text = "定位";
            this.btnLocate.UseVisualStyleBackColor = false;
            this.btnLocate.Click += new System.EventHandler(this.btnLocate_Click);
            // 
            // positionChart2
            // 
            this.positionChart2.BorderlineWidth = 0;
            chartArea3.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea3.AxisX.IsReversed = true;
            chartArea3.AxisX.LabelAutoFitMaxFontSize = 8;
            chartArea3.AxisX.LabelAutoFitMinFontSize = 8;
            chartArea3.AxisX.LabelStyle.Enabled = false;
            chartArea3.AxisX.MajorGrid.Enabled = false;
            chartArea3.AxisX.MajorGrid.Interval = 10D;
            chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea3.AxisX.MajorTickMark.Enabled = false;
            chartArea3.AxisX.Maximum = 10D;
            chartArea3.AxisX.Minimum = 0D;
            chartArea3.AxisX.TitleFont = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea3.AxisY.IsLabelAutoFit = false;
            chartArea3.AxisY.IsReversed = true;
            chartArea3.AxisY.LabelAutoFitMaxFontSize = 8;
            chartArea3.AxisY.LabelAutoFitMinFontSize = 8;
            chartArea3.AxisY.LabelStyle.Angle = -90;
            chartArea3.AxisY.LabelStyle.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea3.AxisY.LabelStyle.Format = "0";
            chartArea3.AxisY.LabelStyle.Interval = 25D;
            chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gainsboro;
            chartArea3.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea3.AxisY.MajorTickMark.Size = 5F;
            chartArea3.AxisY.Maximum = 150D;
            chartArea3.AxisY.Minimum = 0D;
            chartArea3.AxisY.ScaleBreakStyle.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea3.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            chartArea3.AxisY.Title = "Z";
            chartArea3.AxisY.TitleFont = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea3.InnerPlotPosition.Auto = false;
            chartArea3.InnerPlotPosition.Height = 94F;
            chartArea3.InnerPlotPosition.Width = 50F;
            chartArea3.InnerPlotPosition.Y = 6F;
            chartArea3.Name = "ChartArea1";
            chartArea3.Position.Auto = false;
            chartArea3.Position.Height = 90.5F;
            chartArea3.Position.Width = 100F;
            chartArea3.Position.Y = 2.5F;
            this.positionChart2.ChartAreas.Add(chartArea3);
            this.positionChart2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionChart2.Location = new System.Drawing.Point(0, 0);
            this.positionChart2.Name = "positionChart2";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Name = "Series0";
            series5.Points.Add(dataPoint5);
            series6.BorderColor = System.Drawing.Color.Red;
            series6.BorderWidth = 5;
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series6.Color = System.Drawing.Color.Red;
            series6.MarkerBorderColor = System.Drawing.Color.Lime;
            series6.MarkerBorderWidth = 0;
            series6.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            series6.MarkerSize = 10;
            series6.MarkerStep = 2;
            series6.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series6.Name = "Series1";
            series6.Points.Add(dataPoint6);
            series6.YValuesPerPoint = 2;
            this.positionChart2.Series.Add(series5);
            this.positionChart2.Series.Add(series6);
            this.positionChart2.Size = new System.Drawing.Size(90, 566);
            this.positionChart2.TabIndex = 139;
            this.positionChart2.Text = "chart1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.lblCurrentCmd);
            this.splitContainer1.Panel2.Controls.Add(this.lblCurrentLayer);
            this.splitContainer1.Panel2.Controls.Add(this.btnClearChart);
            this.splitContainer1.Panel2.Controls.Add(this.btnLocate);
            this.splitContainer1.Panel2.Controls.Add(this.btnReset);
            this.splitContainer1.Panel2.Controls.Add(this.lblMouseAxisXY);
            this.splitContainer1.Panel2.Controls.Add(this.btnZoomOut);
            this.splitContainer1.Panel2.Controls.Add(this.btnZoomIn);
            this.splitContainer1.Size = new System.Drawing.Size(757, 638);
            this.splitContainer1.SplitterDistance = 566;
            this.splitContainer1.TabIndex = 140;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.positionChart);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.positionChart2);
            this.splitContainer2.Size = new System.Drawing.Size(757, 566);
            this.splitContainer2.SplitterDistance = 663;
            this.splitContainer2.TabIndex = 0;
            // 
            // lblCurrentLayer
            // 
            this.lblCurrentLayer.AutoSize = true;
            this.lblCurrentLayer.Location = new System.Drawing.Point(525, 18);
            this.lblCurrentLayer.Name = "lblCurrentLayer";
            this.lblCurrentLayer.Size = new System.Drawing.Size(53, 12);
            this.lblCurrentLayer.TabIndex = 130;
            this.lblCurrentLayer.Text = "当前层数";
            // 
            // lblCurrentCmd
            // 
            this.lblCurrentCmd.AutoSize = true;
            this.lblCurrentCmd.Location = new System.Drawing.Point(527, 44);
            this.lblCurrentCmd.Name = "lblCurrentCmd";
            this.lblCurrentCmd.Size = new System.Drawing.Size(47, 12);
            this.lblCurrentCmd.TabIndex = 131;
            this.lblCurrentCmd.Text = "Command";
            // 
            // FrmPathTrace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 638);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(0, 218);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimizeBox = false;
            this.Name = "FrmPathTrace";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "路径跟踪";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.pathTrace_FormClosing);
            this.Load += new System.EventHandler(this.FrmImmStop_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmPathTrace_KeyDown);
            this.Resize += new System.EventHandler(this.FrmPathTrace_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.positionChart)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.positionChart2)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Label lblMouseAxisXY;
        private System.Windows.Forms.Button btnClearChart;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnLocate;
        private System.Windows.Forms.DataVisualization.Charting.Chart positionChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart positionChart2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label lblCurrentLayer;
        private System.Windows.Forms.Label lblCurrentCmd;
    }
}