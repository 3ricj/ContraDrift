﻿
namespace ContraDrift
{
    partial class Form1
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.PID_Setting_Kd_DEC = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.PID_Setting_Ki_DEC = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.PID_Setting_Kp_DEC = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.PID_Setting_Kd_RA = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.PID_Setting_Ki_RA = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.PID_Setting_Kp_RA = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.PID_Setting_Nfilt_DEC = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.PID_Setting_Nfilt_RA = new System.Windows.Forms.TextBox();
            this.ProcessingFilter = new System.Windows.Forms.RadioButton();
            this.ProcessingTraditional = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.RA = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.PID_Setting_Kd_DEC_filter = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.PID_Setting_Ki_DEC_filter = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.PID_Setting_Kp_DEC_filter = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PID_Setting_Kd_RA_filter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PID_Setting_Ki_RA_filter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PID_Setting_Kp_RA_filter = new System.Windows.Forms.TextBox();
            this.PlatesolveGroupbox = new System.Windows.Forms.GroupBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Timestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Filename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlateRa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RaShift = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RaDrift = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewRaRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlateDec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DecShift = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DecDrift = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewDecRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Export = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.PlatesolveGroupbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(153, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(505, 20);
            this.textBox1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Location = new System.Drawing.Point(37, 85);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(702, 66);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Watch Folder";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(156, 23);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(505, 20);
            this.textBox2.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(16, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(131, 26);
            this.button2.TabIndex = 0;
            this.button2.Text = "Browse";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Location = new System.Drawing.Point(37, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(702, 67);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ASCOM";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select Telescope";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(776, 27);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(92, 29);
            this.button3.TabIndex = 5;
            this.button3.Text = "Start";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.SaveButton);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.PID_Setting_Kd_DEC);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.PID_Setting_Ki_DEC);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.PID_Setting_Kp_DEC);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.PID_Setting_Kd_RA);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.PID_Setting_Ki_RA);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.PID_Setting_Kp_RA);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.PID_Setting_Nfilt_DEC);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.PID_Setting_Nfilt_RA);
            this.groupBox3.Controls.Add(this.ProcessingFilter);
            this.groupBox3.Controls.Add(this.ProcessingTraditional);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.RA);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.PID_Setting_Kd_DEC_filter);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.PID_Setting_Ki_DEC_filter);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.PID_Setting_Kp_DEC_filter);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.PID_Setting_Kd_RA_filter);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.PID_Setting_Ki_RA_filter);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.PID_Setting_Kp_RA_filter);
            this.groupBox3.Location = new System.Drawing.Point(37, 235);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(702, 239);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "PID";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(588, 183);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 35;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(33, 189);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 13);
            this.label13.TabIndex = 34;
            this.label13.Text = "DEC";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(33, 142);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(22, 13);
            this.label14.TabIndex = 33;
            this.label14.Text = "RA";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(310, 189);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(26, 13);
            this.label15.TabIndex = 32;
            this.label15.Text = "Kd=";
            // 
            // PID_Setting_Kd_DEC
            // 
            this.PID_Setting_Kd_DEC.Location = new System.Drawing.Point(336, 186);
            this.PID_Setting_Kd_DEC.MaxLength = 10;
            this.PID_Setting_Kd_DEC.Name = "PID_Setting_Kd_DEC";
            this.PID_Setting_Kd_DEC.Size = new System.Drawing.Size(85, 20);
            this.PID_Setting_Kd_DEC.TabIndex = 31;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(198, 189);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(22, 13);
            this.label16.TabIndex = 30;
            this.label16.Text = "Ki=";
            // 
            // PID_Setting_Ki_DEC
            // 
            this.PID_Setting_Ki_DEC.Location = new System.Drawing.Point(224, 186);
            this.PID_Setting_Ki_DEC.MaxLength = 10;
            this.PID_Setting_Ki_DEC.Name = "PID_Setting_Ki_DEC";
            this.PID_Setting_Ki_DEC.Size = new System.Drawing.Size(80, 20);
            this.PID_Setting_Ki_DEC.TabIndex = 29;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(82, 189);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(26, 13);
            this.label17.TabIndex = 28;
            this.label17.Text = "Kp=";
            // 
            // PID_Setting_Kp_DEC
            // 
            this.PID_Setting_Kp_DEC.Location = new System.Drawing.Point(108, 186);
            this.PID_Setting_Kp_DEC.MaxLength = 10;
            this.PID_Setting_Kp_DEC.Name = "PID_Setting_Kp_DEC";
            this.PID_Setting_Kp_DEC.Size = new System.Drawing.Size(84, 20);
            this.PID_Setting_Kp_DEC.TabIndex = 27;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(310, 142);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(26, 13);
            this.label18.TabIndex = 26;
            this.label18.Text = "Kd=";
            // 
            // PID_Setting_Kd_RA
            // 
            this.PID_Setting_Kd_RA.Location = new System.Drawing.Point(336, 139);
            this.PID_Setting_Kd_RA.MaxLength = 10;
            this.PID_Setting_Kd_RA.Name = "PID_Setting_Kd_RA";
            this.PID_Setting_Kd_RA.Size = new System.Drawing.Size(85, 20);
            this.PID_Setting_Kd_RA.TabIndex = 25;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(198, 142);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(22, 13);
            this.label19.TabIndex = 24;
            this.label19.Text = "Ki=";
            // 
            // PID_Setting_Ki_RA
            // 
            this.PID_Setting_Ki_RA.Location = new System.Drawing.Point(224, 139);
            this.PID_Setting_Ki_RA.MaxLength = 10;
            this.PID_Setting_Ki_RA.Name = "PID_Setting_Ki_RA";
            this.PID_Setting_Ki_RA.Size = new System.Drawing.Size(80, 20);
            this.PID_Setting_Ki_RA.TabIndex = 23;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(82, 142);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(26, 13);
            this.label20.TabIndex = 22;
            this.label20.Text = "Kp=";
            // 
            // PID_Setting_Kp_RA
            // 
            this.PID_Setting_Kp_RA.Location = new System.Drawing.Point(108, 139);
            this.PID_Setting_Kp_RA.MaxLength = 10;
            this.PID_Setting_Kp_RA.Name = "PID_Setting_Kp_RA";
            this.PID_Setting_Kp_RA.Size = new System.Drawing.Size(84, 20);
            this.PID_Setting_Kp_RA.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(601, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Processing:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(427, 87);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Nfilt=";
            // 
            // PID_Setting_Nfilt_DEC
            // 
            this.PID_Setting_Nfilt_DEC.Location = new System.Drawing.Point(464, 84);
            this.PID_Setting_Nfilt_DEC.MaxLength = 10;
            this.PID_Setting_Nfilt_DEC.Name = "PID_Setting_Nfilt_DEC";
            this.PID_Setting_Nfilt_DEC.Size = new System.Drawing.Size(91, 20);
            this.PID_Setting_Nfilt_DEC.TabIndex = 18;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(427, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Nfilt=";
            // 
            // PID_Setting_Nfilt_RA
            // 
            this.PID_Setting_Nfilt_RA.Location = new System.Drawing.Point(464, 37);
            this.PID_Setting_Nfilt_RA.MaxLength = 10;
            this.PID_Setting_Nfilt_RA.Name = "PID_Setting_Nfilt_RA";
            this.PID_Setting_Nfilt_RA.Size = new System.Drawing.Size(91, 20);
            this.PID_Setting_Nfilt_RA.TabIndex = 16;
            // 
            // ProcessingFilter
            // 
            this.ProcessingFilter.AutoSize = true;
            this.ProcessingFilter.Location = new System.Drawing.Point(604, 63);
            this.ProcessingFilter.Name = "ProcessingFilter";
            this.ProcessingFilter.Size = new System.Drawing.Size(47, 17);
            this.ProcessingFilter.TabIndex = 15;
            this.ProcessingFilter.TabStop = true;
            this.ProcessingFilter.Text = "Filter";
            this.ProcessingFilter.UseVisualStyleBackColor = true;
            // 
            // ProcessingTraditional
            // 
            this.ProcessingTraditional.AutoSize = true;
            this.ProcessingTraditional.Location = new System.Drawing.Point(604, 138);
            this.ProcessingTraditional.Name = "ProcessingTraditional";
            this.ProcessingTraditional.Size = new System.Drawing.Size(74, 17);
            this.ProcessingTraditional.TabIndex = 14;
            this.ProcessingTraditional.TabStop = true;
            this.ProcessingTraditional.Text = "Traditional";
            this.ProcessingTraditional.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "DEC";
            // 
            // RA
            // 
            this.RA.AutoSize = true;
            this.RA.Location = new System.Drawing.Point(33, 40);
            this.RA.Name = "RA";
            this.RA.Size = new System.Drawing.Size(22, 13);
            this.RA.TabIndex = 12;
            this.RA.Text = "RA";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(310, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Kd=";
            // 
            // PID_Setting_Kd_DEC_filter
            // 
            this.PID_Setting_Kd_DEC_filter.Location = new System.Drawing.Point(336, 84);
            this.PID_Setting_Kd_DEC_filter.MaxLength = 10;
            this.PID_Setting_Kd_DEC_filter.Name = "PID_Setting_Kd_DEC_filter";
            this.PID_Setting_Kd_DEC_filter.Size = new System.Drawing.Size(85, 20);
            this.PID_Setting_Kd_DEC_filter.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(198, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Ki=";
            // 
            // PID_Setting_Ki_DEC_filter
            // 
            this.PID_Setting_Ki_DEC_filter.Location = new System.Drawing.Point(224, 84);
            this.PID_Setting_Ki_DEC_filter.MaxLength = 10;
            this.PID_Setting_Ki_DEC_filter.Name = "PID_Setting_Ki_DEC_filter";
            this.PID_Setting_Ki_DEC_filter.Size = new System.Drawing.Size(80, 20);
            this.PID_Setting_Ki_DEC_filter.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(82, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Kp=";
            // 
            // PID_Setting_Kp_DEC_filter
            // 
            this.PID_Setting_Kp_DEC_filter.Location = new System.Drawing.Point(108, 84);
            this.PID_Setting_Kp_DEC_filter.MaxLength = 10;
            this.PID_Setting_Kp_DEC_filter.Name = "PID_Setting_Kp_DEC_filter";
            this.PID_Setting_Kp_DEC_filter.Size = new System.Drawing.Size(84, 20);
            this.PID_Setting_Kp_DEC_filter.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(310, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Kd=";
            // 
            // PID_Setting_Kd_RA_filter
            // 
            this.PID_Setting_Kd_RA_filter.Location = new System.Drawing.Point(336, 37);
            this.PID_Setting_Kd_RA_filter.MaxLength = 10;
            this.PID_Setting_Kd_RA_filter.Name = "PID_Setting_Kd_RA_filter";
            this.PID_Setting_Kd_RA_filter.Size = new System.Drawing.Size(85, 20);
            this.PID_Setting_Kd_RA_filter.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(198, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Ki=";
            // 
            // PID_Setting_Ki_RA_filter
            // 
            this.PID_Setting_Ki_RA_filter.Location = new System.Drawing.Point(224, 37);
            this.PID_Setting_Ki_RA_filter.MaxLength = 10;
            this.PID_Setting_Ki_RA_filter.Name = "PID_Setting_Ki_RA_filter";
            this.PID_Setting_Ki_RA_filter.Size = new System.Drawing.Size(80, 20);
            this.PID_Setting_Ki_RA_filter.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(82, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Kp=";
            // 
            // PID_Setting_Kp_RA_filter
            // 
            this.PID_Setting_Kp_RA_filter.Location = new System.Drawing.Point(108, 37);
            this.PID_Setting_Kp_RA_filter.MaxLength = 10;
            this.PID_Setting_Kp_RA_filter.Name = "PID_Setting_Kp_RA_filter";
            this.PID_Setting_Kp_RA_filter.Size = new System.Drawing.Size(84, 20);
            this.PID_Setting_Kp_RA_filter.TabIndex = 0;
            // 
            // PlatesolveGroupbox
            // 
            this.PlatesolveGroupbox.Controls.Add(this.textBox3);
            this.PlatesolveGroupbox.Controls.Add(this.button4);
            this.PlatesolveGroupbox.Location = new System.Drawing.Point(37, 157);
            this.PlatesolveGroupbox.Name = "PlatesolveGroupbox";
            this.PlatesolveGroupbox.Size = new System.Drawing.Size(702, 72);
            this.PlatesolveGroupbox.TabIndex = 7;
            this.PlatesolveGroupbox.TabStop = false;
            this.PlatesolveGroupbox.Text = "Platesolve UCAC4 path";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(156, 23);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(505, 20);
            this.textBox3.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(16, 19);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(131, 26);
            this.button4.TabIndex = 2;
            this.button4.Text = "Browse";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Timestamp,
            this.Filename,
            this.PlateRa,
            this.RaShift,
            this.RaDrift,
            this.NewRaRate,
            this.PlateDec,
            this.DecShift,
            this.DecDrift,
            this.NewDecRate});
            this.dataGridView1.Location = new System.Drawing.Point(37, 491);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(1013, 255);
            this.dataGridView1.TabIndex = 8;
            // 
            // Timestamp
            // 
            this.Timestamp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Timestamp.HeaderText = "Timestamp";
            this.Timestamp.Name = "Timestamp";
            this.Timestamp.ReadOnly = true;
            this.Timestamp.Width = 83;
            // 
            // Filename
            // 
            this.Filename.HeaderText = "Filename";
            this.Filename.Name = "Filename";
            this.Filename.ReadOnly = true;
            // 
            // PlateRa
            // 
            this.PlateRa.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.PlateRa.HeaderText = "PlateRa";
            this.PlateRa.Name = "PlateRa";
            this.PlateRa.ReadOnly = true;
            this.PlateRa.Width = 70;
            // 
            // RaShift
            // 
            this.RaShift.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.RaShift.HeaderText = "RaShift";
            this.RaShift.Name = "RaShift";
            this.RaShift.ReadOnly = true;
            this.RaShift.Width = 67;
            // 
            // RaDrift
            // 
            this.RaDrift.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.RaDrift.HeaderText = "RaDrift";
            this.RaDrift.Name = "RaDrift";
            this.RaDrift.ReadOnly = true;
            this.RaDrift.Width = 65;
            // 
            // NewRaRate
            // 
            this.NewRaRate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.NewRaRate.HeaderText = "NewRaRate";
            this.NewRaRate.Name = "NewRaRate";
            this.NewRaRate.ReadOnly = true;
            this.NewRaRate.Width = 91;
            // 
            // PlateDec
            // 
            this.PlateDec.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.PlateDec.HeaderText = "PlateDec";
            this.PlateDec.Name = "PlateDec";
            this.PlateDec.ReadOnly = true;
            this.PlateDec.Width = 76;
            // 
            // DecShift
            // 
            this.DecShift.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.DecShift.HeaderText = "DecShift";
            this.DecShift.Name = "DecShift";
            this.DecShift.ReadOnly = true;
            this.DecShift.Width = 73;
            // 
            // DecDrift
            // 
            this.DecDrift.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.DecDrift.HeaderText = "DecDrift";
            this.DecDrift.Name = "DecDrift";
            this.DecDrift.ReadOnly = true;
            this.DecDrift.Width = 71;
            // 
            // NewDecRate
            // 
            this.NewDecRate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.NewDecRate.HeaderText = "NewDecRate";
            this.NewDecRate.Name = "NewDecRate";
            this.NewDecRate.ReadOnly = true;
            this.NewDecRate.Width = 97;
            // 
            // Export
            // 
            this.Export.Location = new System.Drawing.Point(967, 447);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(75, 23);
            this.Export.TabIndex = 9;
            this.Export.Text = "Export";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.Export_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1062, 758);
            this.Controls.Add(this.Export);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.PlatesolveGroupbox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "ContraDrift";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.PlatesolveGroupbox.ResumeLayout(false);
            this.PlatesolveGroupbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox PID_Setting_Kd_RA_filter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PID_Setting_Ki_RA_filter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PID_Setting_Kp_RA_filter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PID_Setting_Kd_DEC_filter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PID_Setting_Ki_DEC_filter;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox PID_Setting_Kp_DEC_filter;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label RA;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox PID_Setting_Nfilt_DEC;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox PID_Setting_Nfilt_RA;
        private System.Windows.Forms.RadioButton ProcessingFilter;
        private System.Windows.Forms.RadioButton ProcessingTraditional;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox PID_Setting_Kd_DEC;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox PID_Setting_Ki_DEC;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox PID_Setting_Kp_DEC;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox PID_Setting_Kd_RA;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox PID_Setting_Ki_RA;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox PID_Setting_Kp_RA;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.GroupBox PlatesolveGroupbox;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn Filename;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlateRa;
        private System.Windows.Forms.DataGridViewTextBoxColumn RaShift;
        private System.Windows.Forms.DataGridViewTextBoxColumn RaDrift;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewRaRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlateDec;
        private System.Windows.Forms.DataGridViewTextBoxColumn DecShift;
        private System.Windows.Forms.DataGridViewTextBoxColumn DecDrift;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewDecRate;
        private System.Windows.Forms.Button Export;
    }
}

