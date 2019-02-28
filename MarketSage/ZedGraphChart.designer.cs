#region Copyright
/*
 * Software: TickZoom Trading Platform
 * Copyright 2009 M. Wayne Walter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, see <http://www.tickzoom.org/wiki/Licenses>
 * or write to Free Software Foundation, Inc., 51 Franklin Street,
 * Fifth Floor, Boston, MA  02110-1301, USA.
 * 
 */
#endregion

using System;
using ZedGraph;
using System.Drawing;

namespace MarketSage
{
	partial class ZedGraphChart
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.dataGraph = new ZedGraph.ZedGraphControl();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusXY = new System.Windows.Forms.ToolStripStatusLabel();
			this.refreshTimer = new System.Windows.Forms.Timer(this.components);
			this.checkBoxOnTop = new System.Windows.Forms.CheckBox();
			this.audioNotify = new System.Windows.Forms.CheckBox();
			this.logTextBox = new System.Windows.Forms.TextBox();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGraph
			// 
			this.dataGraph.AutoScroll = true;
			this.dataGraph.Location = new System.Drawing.Point(10, 32);
			this.dataGraph.Name = "dataGraph";
			this.dataGraph.ScrollGrace = 0;
			this.dataGraph.ScrollMaxX = 0;
			this.dataGraph.ScrollMaxY = 0;
			this.dataGraph.ScrollMaxY2 = 0;
			this.dataGraph.ScrollMinX = 0;
			this.dataGraph.ScrollMinY = 0;
			this.dataGraph.ScrollMinY2 = 0;
			this.dataGraph.Size = new System.Drawing.Size(767, 328);
			this.dataGraph.TabIndex = 0;
			this.dataGraph.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.DataGraphMouseMoveEvent);
			this.dataGraph.ContextMenuBuilder += new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(this.DataGraphContextMenuBuilder);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripStatusXY});
			this.statusStrip1.Location = new System.Drawing.Point(0, 430);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(791, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusXY
			// 
			this.toolStripStatusXY.Name = "toolStripStatusXY";
			this.toolStripStatusXY.Size = new System.Drawing.Size(90, 17);
			this.toolStripStatusXY.Text = "toolStripStatusXY";
			// 
			// refreshTimer
			// 
			this.refreshTimer.Enabled = true;
			this.refreshTimer.Tick += new System.EventHandler(this.refreshTick);
			// 
			// checkBoxOnTop
			// 
			this.checkBoxOnTop.BackColor = System.Drawing.SystemColors.Control;
			this.checkBoxOnTop.Location = new System.Drawing.Point(717, 6);
			this.checkBoxOnTop.Name = "checkBoxOnTop";
			this.checkBoxOnTop.Size = new System.Drawing.Size(62, 20);
			this.checkBoxOnTop.TabIndex = 3;
			this.checkBoxOnTop.Text = "On Top";
			this.checkBoxOnTop.UseVisualStyleBackColor = false;
			this.checkBoxOnTop.CheckStateChanged += new System.EventHandler(this.CheckBoxOnTopCheckStateChanged);
			// 
			// audioNotify
			// 
			this.audioNotify.Location = new System.Drawing.Point(626, 4);
			this.audioNotify.Name = "audioNotify";
			this.audioNotify.Size = new System.Drawing.Size(85, 24);
			this.audioNotify.TabIndex = 4;
			this.audioNotify.Text = "Notify Audio";
			this.audioNotify.UseVisualStyleBackColor = true;
			this.audioNotify.CheckStateChanged += new System.EventHandler(this.AudioNotifyCheckStateChanged);
			// 
			// logTextBox
			// 
			this.logTextBox.Location = new System.Drawing.Point(10, 366);
			this.logTextBox.Multiline = true;
			this.logTextBox.Name = "logTextBox";
			this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logTextBox.Size = new System.Drawing.Size(767, 61);
			this.logTextBox.TabIndex = 5;
			this.logTextBox.Text = "Chart Log";
			// 
			// Chart
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(791, 452);
			this.Controls.Add(this.logTextBox);
			this.Controls.Add(this.audioNotify);
			this.Controls.Add(this.checkBoxOnTop);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.dataGraph);
			this.KeyPreview = true;
			this.Name = "Chart";
			this.Text = "TestRun";
			this.Load += new System.EventHandler(this.ChartLoad);
			this.Resize += new System.EventHandler(this.ChartResize);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TextBox logTextBox;
		private System.Windows.Forms.CheckBox audioNotify;
		private System.Windows.Forms.CheckBox checkBoxOnTop;
		private System.Windows.Forms.Timer refreshTimer;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusXY;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private ZedGraph.ZedGraphControl dataGraph;
		
		public ZedGraphControl DataGraph {
			get { return dataGraph; }
		}

	}
}
