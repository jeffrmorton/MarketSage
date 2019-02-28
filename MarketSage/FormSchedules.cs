using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using MarketSage.Library;

/* MarketSage
   Copyright © 2008, 2009 Jeffrey Morton
 
   This program is free software; you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation; either version 2 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.
 
   You should have received a copy of the GNU General Public License
   along with this program; if not, write to the Free Software
   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA */

namespace MarketSage
{
    /// <summary>
    /// Summary description for SchedulerUI.
    /// </summary>
    public class FormSchedules : System.Windows.Forms.Form
    {
        private System.Windows.Forms.ListView SchedulesView;
        private System.Windows.Forms.ColumnHeader NameCol;
        private System.Windows.Forms.ColumnHeader TypeCol;
        private System.Windows.Forms.Button DeleteBtn;
        private System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker m_startTimePicker;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox m_sat;
        private System.Windows.Forms.CheckBox m_fri;
        private System.Windows.Forms.CheckBox m_thu;
        private System.Windows.Forms.CheckBox m_wed;
        private System.Windows.Forms.CheckBox m_tue;
        private System.Windows.Forms.CheckBox m_mon;
        private System.Windows.Forms.CheckBox m_sun;
        private System.Windows.Forms.Button UpdateBtn;
        private System.Windows.Forms.RadioButton RunWeekly;
        private System.Windows.Forms.RadioButton RunDaily;
        private System.Windows.Forms.RadioButton RunIntervals;
        private System.Windows.Forms.RadioButton RunOnce;
        private System.Windows.Forms.RadioButton RunMonthly;
        private System.Windows.Forms.GroupBox GroupBox_RunOnlyOn;
        private System.Windows.Forms.GroupBox GroupBox_RunBetween;
        private System.Windows.Forms.Label SecsLabel;
        private System.Windows.Forms.Label IntervalLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox NameTxt;
        private System.Windows.Forms.TextBox IntervalTxt;
        private System.Windows.Forms.Button CreateBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker m_fromTime;
        private System.Windows.Forms.DateTimePicker m_toTime;
        private System.Windows.Forms.ColumnHeader NextTime;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private TextBox FilenameTxt;
        private Label label4;
        private static FormSchedules singletonInstance = null;
        public FormSchedules()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            m_fromTime.Value = DateTime.Today.AddSeconds(1); // 12:00 AM
            m_toTime.Value = DateTime.Today.AddMinutes(24 * 60 - 1); // 11:59 PM
            Scheduler.OnSchedulerEvent += new Scheduler.SchedulerEventDelegate(OnSchedulerEvent);
            m_startTimePicker.Value = DateTime.Now.AddHours(1);
            RunIntervals.Checked = true;
            RunOnce.Checked = true;
            SchedulesView.Items.Clear();
            for (int i = 0; i < Scheduler.Count(); i++)
                OnSchedulerEvent(Scheduler.SchedulerEventType.CREATED, Scheduler.GetScheduleAt(i).Name);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSchedules));
            this.SchedulesView = new System.Windows.Forms.ListView();
            this.NameCol = new System.Windows.Forms.ColumnHeader();
            this.TypeCol = new System.Windows.Forms.ColumnHeader();
            this.NextTime = new System.Windows.Forms.ColumnHeader();
            this.DeleteBtn = new System.Windows.Forms.Button();
            this.AddBtn = new System.Windows.Forms.Button();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SecsLabel = new System.Windows.Forms.Label();
            this.IntervalTxt = new System.Windows.Forms.TextBox();
            this.IntervalLabel = new System.Windows.Forms.Label();
            this.m_sat = new System.Windows.Forms.CheckBox();
            this.m_fri = new System.Windows.Forms.CheckBox();
            this.m_thu = new System.Windows.Forms.CheckBox();
            this.m_wed = new System.Windows.Forms.CheckBox();
            this.m_tue = new System.Windows.Forms.CheckBox();
            this.m_mon = new System.Windows.Forms.CheckBox();
            this.m_sun = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.RunMonthly = new System.Windows.Forms.RadioButton();
            this.RunWeekly = new System.Windows.Forms.RadioButton();
            this.RunDaily = new System.Windows.Forms.RadioButton();
            this.RunIntervals = new System.Windows.Forms.RadioButton();
            this.RunOnce = new System.Windows.Forms.RadioButton();
            this.m_startTimePicker = new System.Windows.Forms.DateTimePicker();
            this.UpdateBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GroupBox_RunOnlyOn = new System.Windows.Forms.GroupBox();
            this.GroupBox_RunBetween = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_toTime = new System.Windows.Forms.DateTimePicker();
            this.m_fromTime = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.NameTxt = new System.Windows.Forms.TextBox();
            this.CreateBtn = new System.Windows.Forms.Button();
            this.FilenameTxt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.GroupBox_RunOnlyOn.SuspendLayout();
            this.GroupBox_RunBetween.SuspendLayout();
            this.SuspendLayout();
            // 
            // SchedulesView
            // 
            this.SchedulesView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameCol,
            this.TypeCol,
            this.NextTime});
            this.SchedulesView.Dock = System.Windows.Forms.DockStyle.Top;
            this.SchedulesView.FullRowSelect = true;
            this.SchedulesView.GridLines = true;
            this.SchedulesView.Location = new System.Drawing.Point(0, 0);
            this.SchedulesView.MultiSelect = false;
            this.SchedulesView.Name = "SchedulesView";
            this.SchedulesView.Size = new System.Drawing.Size(398, 192);
            this.SchedulesView.TabIndex = 0;
            this.SchedulesView.UseCompatibleStateImageBehavior = false;
            this.SchedulesView.View = System.Windows.Forms.View.Details;
            this.SchedulesView.SelectedIndexChanged += new System.EventHandler(this.SchedulesView_SelectedIndexChanged);
            this.SchedulesView.DoubleClick += new System.EventHandler(this.OnScheduleDblClk);
            // 
            // NameCol
            // 
            this.NameCol.Text = "Name";
            this.NameCol.Width = 80;
            // 
            // TypeCol
            // 
            this.TypeCol.Text = "Type";
            this.TypeCol.Width = 90;
            // 
            // NextTime
            // 
            this.NextTime.Text = "Next Invoke Time";
            this.NextTime.Width = 200;
            // 
            // DeleteBtn
            // 
            this.DeleteBtn.Location = new System.Drawing.Point(119, 198);
            this.DeleteBtn.Name = "DeleteBtn";
            this.DeleteBtn.Size = new System.Drawing.Size(78, 22);
            this.DeleteBtn.TabIndex = 1;
            this.DeleteBtn.Text = "Delete";
            this.DeleteBtn.Click += new System.EventHandler(this.OnDeleteSchedule);
            // 
            // AddBtn
            // 
            this.AddBtn.Location = new System.Drawing.Point(20, 14);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(80, 22);
            this.AddBtn.TabIndex = 4;
            this.AddBtn.Text = "Add";
            // 
            // CloseBtn
            // 
            this.CloseBtn.Location = new System.Drawing.Point(312, 470);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(76, 22);
            this.CloseBtn.TabIndex = 21;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.Click += new System.EventHandler(this.OnCloseClick);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(216, 202);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(172, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "Double click on a schedule to edit";
            // 
            // SecsLabel
            // 
            this.SecsLabel.Location = new System.Drawing.Point(352, 328);
            this.SecsLabel.Name = "SecsLabel";
            this.SecsLabel.Size = new System.Drawing.Size(34, 14);
            this.SecsLabel.TabIndex = 22;
            this.SecsLabel.Text = "secs";
            // 
            // IntervalTxt
            // 
            this.IntervalTxt.Location = new System.Drawing.Point(296, 325);
            this.IntervalTxt.Name = "IntervalTxt";
            this.IntervalTxt.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.IntervalTxt.Size = new System.Drawing.Size(48, 20);
            this.IntervalTxt.TabIndex = 11;
            this.IntervalTxt.Text = "60";
            // 
            // IntervalLabel
            // 
            this.IntervalLabel.Location = new System.Drawing.Point(244, 328);
            this.IntervalLabel.Name = "IntervalLabel";
            this.IntervalLabel.Size = new System.Drawing.Size(48, 14);
            this.IntervalLabel.TabIndex = 20;
            this.IntervalLabel.Text = "Interval :";
            // 
            // m_sat
            // 
            this.m_sat.Location = new System.Drawing.Point(330, 22);
            this.m_sat.Name = "m_sat";
            this.m_sat.Size = new System.Drawing.Size(48, 18);
            this.m_sat.TabIndex = 18;
            this.m_sat.Text = "Sat";
            // 
            // m_fri
            // 
            this.m_fri.Checked = true;
            this.m_fri.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_fri.Location = new System.Drawing.Point(279, 22);
            this.m_fri.Name = "m_fri";
            this.m_fri.Size = new System.Drawing.Size(48, 18);
            this.m_fri.TabIndex = 17;
            this.m_fri.Text = "Fri";
            // 
            // m_thu
            // 
            this.m_thu.Checked = true;
            this.m_thu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_thu.Location = new System.Drawing.Point(228, 22);
            this.m_thu.Name = "m_thu";
            this.m_thu.Size = new System.Drawing.Size(48, 18);
            this.m_thu.TabIndex = 16;
            this.m_thu.Text = "Thu";
            // 
            // m_wed
            // 
            this.m_wed.Checked = true;
            this.m_wed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_wed.Location = new System.Drawing.Point(177, 22);
            this.m_wed.Name = "m_wed";
            this.m_wed.Size = new System.Drawing.Size(48, 18);
            this.m_wed.TabIndex = 15;
            this.m_wed.Text = "Wed";
            // 
            // m_tue
            // 
            this.m_tue.Checked = true;
            this.m_tue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_tue.Location = new System.Drawing.Point(126, 22);
            this.m_tue.Name = "m_tue";
            this.m_tue.Size = new System.Drawing.Size(48, 18);
            this.m_tue.TabIndex = 14;
            this.m_tue.Text = "Tue";
            // 
            // m_mon
            // 
            this.m_mon.Checked = true;
            this.m_mon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_mon.Location = new System.Drawing.Point(75, 22);
            this.m_mon.Name = "m_mon";
            this.m_mon.Size = new System.Drawing.Size(48, 18);
            this.m_mon.TabIndex = 13;
            this.m_mon.Text = "Mon";
            // 
            // m_sun
            // 
            this.m_sun.Location = new System.Drawing.Point(24, 22);
            this.m_sun.Name = "m_sun";
            this.m_sun.Size = new System.Drawing.Size(48, 18);
            this.m_sun.TabIndex = 12;
            this.m_sun.Text = "Sun";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 326);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 14);
            this.label3.TabIndex = 11;
            this.label3.Text = "Start Time :";
            // 
            // RunMonthly
            // 
            this.RunMonthly.Location = new System.Drawing.Point(160, 40);
            this.RunMonthly.Name = "RunMonthly";
            this.RunMonthly.Size = new System.Drawing.Size(62, 16);
            this.RunMonthly.TabIndex = 9;
            this.RunMonthly.Text = "Monthly";
            this.RunMonthly.CheckedChanged += new System.EventHandler(this.OnScheduleTypeChange);
            // 
            // RunWeekly
            // 
            this.RunWeekly.Location = new System.Drawing.Point(279, 19);
            this.RunWeekly.Name = "RunWeekly";
            this.RunWeekly.Size = new System.Drawing.Size(75, 16);
            this.RunWeekly.TabIndex = 7;
            this.RunWeekly.Text = "Weekly";
            this.RunWeekly.CheckedChanged += new System.EventHandler(this.OnScheduleTypeChange);
            // 
            // RunDaily
            // 
            this.RunDaily.Location = new System.Drawing.Point(160, 20);
            this.RunDaily.Name = "RunDaily";
            this.RunDaily.Size = new System.Drawing.Size(48, 16);
            this.RunDaily.TabIndex = 6;
            this.RunDaily.Text = "Daily";
            this.RunDaily.CheckedChanged += new System.EventHandler(this.OnScheduleTypeChange);
            // 
            // RunIntervals
            // 
            this.RunIntervals.Enabled = false;
            this.RunIntervals.Location = new System.Drawing.Point(22, 40);
            this.RunIntervals.Name = "RunIntervals";
            this.RunIntervals.Size = new System.Drawing.Size(120, 16);
            this.RunIntervals.TabIndex = 8;
            this.RunIntervals.Text = "At regular intervals";
            this.RunIntervals.CheckedChanged += new System.EventHandler(this.OnScheduleTypeChange);
            // 
            // RunOnce
            // 
            this.RunOnce.Checked = true;
            this.RunOnce.Location = new System.Drawing.Point(22, 20);
            this.RunOnce.Name = "RunOnce";
            this.RunOnce.Size = new System.Drawing.Size(94, 16);
            this.RunOnce.TabIndex = 5;
            this.RunOnce.TabStop = true;
            this.RunOnce.Text = "Run only once";
            this.RunOnce.CheckedChanged += new System.EventHandler(this.OnScheduleTypeChange);
            // 
            // m_startTimePicker
            // 
            this.m_startTimePicker.CustomFormat = "MM\'/\'dd\'/\'yyyy   hh:mm:ss tt";
            this.m_startTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.m_startTimePicker.Location = new System.Drawing.Point(76, 322);
            this.m_startTimePicker.Name = "m_startTimePicker";
            this.m_startTimePicker.Size = new System.Drawing.Size(142, 20);
            this.m_startTimePicker.TabIndex = 10;
            this.m_startTimePicker.Value = new System.DateTime(2004, 9, 21, 0, 0, 0, 0);
            this.m_startTimePicker.ValueChanged += new System.EventHandler(this.m_startTimePicker_ValueChanged);
            // 
            // UpdateBtn
            // 
            this.UpdateBtn.Location = new System.Drawing.Point(227, 470);
            this.UpdateBtn.Name = "UpdateBtn";
            this.UpdateBtn.Size = new System.Drawing.Size(76, 22);
            this.UpdateBtn.TabIndex = 2;
            this.UpdateBtn.Text = "Update";
            this.UpdateBtn.Click += new System.EventHandler(this.OnUpdateSchedule);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RunMonthly);
            this.groupBox1.Controls.Add(this.RunWeekly);
            this.groupBox1.Controls.Add(this.RunDaily);
            this.groupBox1.Controls.Add(this.RunIntervals);
            this.groupBox1.Controls.Add(this.RunOnce);
            this.groupBox1.Location = new System.Drawing.Point(4, 228);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(390, 64);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Schedule Type";
            // 
            // GroupBox_RunOnlyOn
            // 
            this.GroupBox_RunOnlyOn.Controls.Add(this.m_thu);
            this.GroupBox_RunOnlyOn.Controls.Add(this.m_sat);
            this.GroupBox_RunOnlyOn.Controls.Add(this.m_sun);
            this.GroupBox_RunOnlyOn.Controls.Add(this.m_mon);
            this.GroupBox_RunOnlyOn.Controls.Add(this.m_wed);
            this.GroupBox_RunOnlyOn.Controls.Add(this.m_tue);
            this.GroupBox_RunOnlyOn.Controls.Add(this.m_fri);
            this.GroupBox_RunOnlyOn.Location = new System.Drawing.Point(4, 350);
            this.GroupBox_RunOnlyOn.Name = "GroupBox_RunOnlyOn";
            this.GroupBox_RunOnlyOn.Size = new System.Drawing.Size(390, 50);
            this.GroupBox_RunOnlyOn.TabIndex = 23;
            this.GroupBox_RunOnlyOn.TabStop = false;
            this.GroupBox_RunOnlyOn.Text = "   Run only on";
            // 
            // GroupBox_RunBetween
            // 
            this.GroupBox_RunBetween.Controls.Add(this.label1);
            this.GroupBox_RunBetween.Controls.Add(this.m_toTime);
            this.GroupBox_RunBetween.Controls.Add(this.m_fromTime);
            this.GroupBox_RunBetween.Location = new System.Drawing.Point(4, 410);
            this.GroupBox_RunBetween.Name = "GroupBox_RunBetween";
            this.GroupBox_RunBetween.Size = new System.Drawing.Size(390, 50);
            this.GroupBox_RunBetween.TabIndex = 24;
            this.GroupBox_RunBetween.TabStop = false;
            this.GroupBox_RunBetween.Text = "  Run only between these times";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(181, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "and";
            // 
            // m_toTime
            // 
            this.m_toTime.CustomFormat = "hh:mm tt";
            this.m_toTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.m_toTime.Location = new System.Drawing.Point(223, 22);
            this.m_toTime.Name = "m_toTime";
            this.m_toTime.Size = new System.Drawing.Size(74, 20);
            this.m_toTime.TabIndex = 1;
            // 
            // m_fromTime
            // 
            this.m_fromTime.CustomFormat = "hh:mm tt";
            this.m_fromTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.m_fromTime.Location = new System.Drawing.Point(89, 22);
            this.m_fromTime.Name = "m_fromTime";
            this.m_fromTime.Size = new System.Drawing.Size(76, 20);
            this.m_fromTime.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(7, 300);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 16);
            this.label5.TabIndex = 25;
            this.label5.Text = "Name :";
            // 
            // NameTxt
            // 
            this.NameTxt.Location = new System.Drawing.Point(55, 297);
            this.NameTxt.Name = "NameTxt";
            this.NameTxt.Size = new System.Drawing.Size(114, 20);
            this.NameTxt.TabIndex = 26;
            // 
            // CreateBtn
            // 
            this.CreateBtn.Location = new System.Drawing.Point(26, 198);
            this.CreateBtn.Name = "CreateBtn";
            this.CreateBtn.Size = new System.Drawing.Size(78, 22);
            this.CreateBtn.TabIndex = 27;
            this.CreateBtn.Text = "Create";
            this.CreateBtn.Click += new System.EventHandler(this.OnCreateSchedule);
            // 
            // FilenameTxt
            // 
            this.FilenameTxt.Location = new System.Drawing.Point(232, 297);
            this.FilenameTxt.Name = "FilenameTxt";
            this.FilenameTxt.Size = new System.Drawing.Size(156, 20);
            this.FilenameTxt.TabIndex = 29;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(178, 300);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 16);
            this.label4.TabIndex = 28;
            this.label4.Text = "Filename:";
            // 
            // FormSchedules
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(398, 501);
            this.Controls.Add(this.FilenameTxt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CreateBtn);
            this.Controls.Add(this.NameTxt);
            this.Controls.Add(this.IntervalTxt);
            this.Controls.Add(this.CloseBtn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.GroupBox_RunBetween);
            this.Controls.Add(this.GroupBox_RunOnlyOn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DeleteBtn);
            this.Controls.Add(this.SchedulesView);
            this.Controls.Add(this.UpdateBtn);
            this.Controls.Add(this.SecsLabel);
            this.Controls.Add(this.IntervalLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.m_startTimePicker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSchedules";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = " Schedules";
            this.Closed += new System.EventHandler(this.OnClosed);
            this.groupBox1.ResumeLayout(false);
            this.GroupBox_RunOnlyOn.ResumeLayout(false);
            this.GroupBox_RunBetween.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void OnCloseClick(object sender, System.EventArgs e)
        {
            Close();
        }

        private void OnScheduleTypeChange(object sender, System.EventArgs e)
        {
            GroupBox_RunOnlyOn.Enabled = GroupBox_RunBetween.Enabled =
            IntervalLabel.Enabled = IntervalTxt.Enabled = SecsLabel.Enabled = false;
            if (sender == RunDaily || sender == RunIntervals)
                GroupBox_RunOnlyOn.Enabled = ((RadioButton)sender).Checked;
            if (sender == RunIntervals)
            {
                GroupBox_RunBetween.Enabled = ((RadioButton)sender).Checked;
                IntervalLabel.Enabled = IntervalTxt.Enabled = SecsLabel.Enabled = true;
            }
        }

        private void OnCreateSchedule(object sender, System.EventArgs e)
        {
            try
            {
                Schedule sc = null;
                if (RunOnce.Checked) sc = new Schedule(NameTxt.Text, m_startTimePicker.Value, ScheduleType.ONETIME);
                if (RunDaily.Checked) sc = new Schedule(NameTxt.Text, m_startTimePicker.Value, ScheduleType.DAILY);
                if (RunWeekly.Checked) sc = new Schedule(NameTxt.Text, m_startTimePicker.Value, ScheduleType.WEEKLY);
                //if (RunIntervals.Checked) sc = new Schedule(NameTxt.Text, m_startTimePicker.Value,
                //							  Int32.Parse(IntervalTxt.Text),
                //							  m_fromTime.Value.TimeOfDay,
                //							  m_toTime.Value.TimeOfDay);
                if (RunMonthly.Checked) sc = new Schedule(NameTxt.Text, m_startTimePicker.Value, ScheduleType.MONTHLY);
                sc.StartTime = m_startTimePicker.Value;
                sc.Path = FilenameTxt.Text;
                SetScheduleWeekDays(sc);
                Scheduler.AddSchedule(sc);
                System.Xml.Serialization.XmlSerializer x1 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextWriter xw1 = new XmlTextWriter(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + sc.Path, null);
                x1.Serialize(xw1, sc);
                xw1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void ShowSchedules()
        {
            if (singletonInstance == null)
                singletonInstance = new FormSchedules();
            singletonInstance.Show();
            singletonInstance.Focus();
        }

        private void OnClosed(object sender, System.EventArgs e)
        {
            singletonInstance = null;
        }

        // transfer values from controls to schedule object
        private void SetScheduleWeekDays(Schedule sc)
        {
            sc.SetWeekDay(DayOfWeek.Sunday, m_sun.Checked);
            sc.SetWeekDay(DayOfWeek.Monday, m_mon.Checked);
            sc.SetWeekDay(DayOfWeek.Tuesday, m_tue.Checked);
            sc.SetWeekDay(DayOfWeek.Wednesday, m_wed.Checked);
            sc.SetWeekDay(DayOfWeek.Thursday, m_thu.Checked);
            sc.SetWeekDay(DayOfWeek.Friday, m_fri.Checked);
            sc.SetWeekDay(DayOfWeek.Saturday, m_sat.Checked);
        }

        // transfer values from schedule object to controls
        private void GetScheduleWeekDays(Schedule sc)
        {
            m_sun.Checked = sc.WeekDayActive(DayOfWeek.Sunday);
            m_mon.Checked = sc.WeekDayActive(DayOfWeek.Monday);
            m_tue.Checked = sc.WeekDayActive(DayOfWeek.Tuesday);
            m_wed.Checked = sc.WeekDayActive(DayOfWeek.Wednesday);
            m_thu.Checked = sc.WeekDayActive(DayOfWeek.Thursday);
            m_fri.Checked = sc.WeekDayActive(DayOfWeek.Friday);
            m_sat.Checked = sc.WeekDayActive(DayOfWeek.Saturday);
        }

        private void OnScheduleDblClk(object sender, System.EventArgs e)
        {
            Schedule s = Scheduler.GetSchedule(SchedulesView.SelectedItems[0].Text);
            if (s == null)
            {
                MessageBox.Show("Schedule not found !");
                return;
            }
            NameTxt.Text = s.Name;
            FilenameTxt.Text = s.Path;
            m_startTimePicker.Value = s.NextInvokeTime;
            SetScheduleWeekDays(s);
            IntervalTxt.Text = s.Interval.ToString();
            switch (s.Type)
            {
                case ScheduleType.DAILY: RunDaily.Checked = true; break;
                case ScheduleType.INTERVAL: RunIntervals.Checked = true; break;
                case ScheduleType.MONTHLY: RunMonthly.Checked = true; break;
                case ScheduleType.ONETIME: RunOnce.Checked = true; break;
                case ScheduleType.WEEKLY: RunWeekly.Checked = true; break;
            }
        }

        public void OnSchedulerEvent(Scheduler.SchedulerEventType type, string scheduleName)
        {
            switch (type)
            {
                case Scheduler.SchedulerEventType.CREATED:
                    ListViewItem lv = SchedulesView.Items.Add(scheduleName);
                    Schedule s = Scheduler.GetSchedule(scheduleName);
                    lv.SubItems.Add(s.Type.ToString());
                    lv.SubItems.Add(s.NextInvokeTime.ToString("MM/dd/yyyy hh:mm:ss tt"));
                    break;
                case Scheduler.SchedulerEventType.DELETED:
                    for (int i = 0; i < SchedulesView.Items.Count; i++)
                        if (SchedulesView.Items[i].Text == scheduleName)
                            SchedulesView.Items.RemoveAt(i);
                    break;
                case Scheduler.SchedulerEventType.INVOKED:
                    for (int i = 0; i < SchedulesView.Items.Count; i++)
                        if (SchedulesView.Items[i].Text == scheduleName)
                        {
                            Schedule si = Scheduler.GetSchedule(scheduleName);
                            SchedulesView.Items[i].SubItems[2].Text =
                                si.NextInvokeTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                        }
                    break;
            }
            SchedulesView.Refresh();
        }

        private void OnDeleteSchedule(object sender, System.EventArgs e)
        {
            Schedule s = Scheduler.GetSchedule(SchedulesView.SelectedItems[0].Text);
            if (s == null)
            {
                MessageBox.Show("No schedule is selected for deletion");
                return;
            }
            Scheduler.RemoveSchedule(s);
        }

        private void OnUpdateSchedule(object sender, EventArgs e)
        {
            Schedule sc = Scheduler.GetSchedule(SchedulesView.SelectedItems[0].Text);
            //string path = sc.Path;
            if (RunOnce.Checked) sc = new Schedule(NameTxt.Text, m_startTimePicker.Value, ScheduleType.ONETIME);
            if (RunDaily.Checked) sc = new Schedule(NameTxt.Text, m_startTimePicker.Value, ScheduleType.DAILY);
            if (RunWeekly.Checked) sc = new Schedule(NameTxt.Text, m_startTimePicker.Value, ScheduleType.WEEKLY);
            //if (RunIntervals.Checked) sc = new Schedule(NameTxt.Text, m_startTimePicker.Value,
            //							  Int32.Parse(IntervalTxt.Text),
            //							  m_fromTime.Value.TimeOfDay,
            //							  m_toTime.Value.TimeOfDay);
            if (RunMonthly.Checked) sc = new Schedule(NameTxt.Text, m_startTimePicker.Value, ScheduleType.MONTHLY);
            sc.StartTime = m_startTimePicker.Value;
            sc.Path = FilenameTxt.Text;
            SetScheduleWeekDays(sc);
            System.Xml.Serialization.XmlSerializer x1 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
            XmlTextWriter xw1 = new XmlTextWriter(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + sc.Path, null);
            x1.Serialize(xw1, sc);
            xw1.Close();

            //Scheduler.UpdateSchedule(sc);
        }

        private void m_startTimePicker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void SchedulesView_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
