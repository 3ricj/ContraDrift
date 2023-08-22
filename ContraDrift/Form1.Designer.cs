
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
            this.WatchFolderBrowseButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SelectTelescopeButton = new System.Windows.Forms.Button();
            this.StartStopButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.BufferFitsCount = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.DecRateLimitTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.RaRateLimitTextBox = new System.Windows.Forms.TextBox();
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
            this.PlaceSolvePathBrowseButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Export = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.Timestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Filename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dt_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlateRa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RaP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RaI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RaD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewRaRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlateDec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DecP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DecI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DecD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewDecRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Messages = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.PlatesolveGroupbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(306, 46);
            this.textBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(1006, 31);
            this.textBox1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.WatchFolderBrowseButton);
            this.groupBox1.Location = new System.Drawing.Point(37, 85);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(1404, 127);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Watch Folder";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(312, 44);
            this.textBox2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(1006, 31);
            this.textBox2.TabIndex = 1;
            // 
            // WatchFolderBrowseButton
            // 
            this.WatchFolderBrowseButton.Location = new System.Drawing.Point(16, 19);
            this.WatchFolderBrowseButton.Name = "WatchFolderBrowseButton";
            this.WatchFolderBrowseButton.Size = new System.Drawing.Size(131, 26);
            this.WatchFolderBrowseButton.TabIndex = 0;
            this.WatchFolderBrowseButton.Text = "Browse";
            this.WatchFolderBrowseButton.UseVisualStyleBackColor = true;
            this.WatchFolderBrowseButton.Click += new System.EventHandler(this.WatchFolderBrowseButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.SelectTelescopeButton);
            this.groupBox2.Location = new System.Drawing.Point(37, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Size = new System.Drawing.Size(1404, 129);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ASCOM";
            // 
            // SelectTelescopeButton
            // 
            this.SelectTelescopeButton.Location = new System.Drawing.Point(16, 19);
            this.SelectTelescopeButton.Name = "SelectTelescopeButton";
            this.SelectTelescopeButton.Size = new System.Drawing.Size(131, 29);
            this.SelectTelescopeButton.TabIndex = 0;
            this.SelectTelescopeButton.Text = "Select Telescope";
            this.SelectTelescopeButton.UseVisualStyleBackColor = true;
            this.SelectTelescopeButton.Click += new System.EventHandler(this.SelectTelescopeButton_Click);
            // 
            // StartStopButton
            // 
            this.StartStopButton.Location = new System.Drawing.Point(776, 27);
            this.StartStopButton.Name = "StartStopButton";
            this.StartStopButton.Size = new System.Drawing.Size(92, 29);
            this.StartStopButton.TabIndex = 5;
            this.StartStopButton.Text = "Start";
            this.StartStopButton.UseVisualStyleBackColor = true;
            this.StartStopButton.Click += new System.EventHandler(this.StartStopButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.BufferFitsCount);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.DecRateLimitTextBox);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.RaRateLimitTextBox);
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
            this.groupBox3.Location = new System.Drawing.Point(74, 452);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox3.Size = new System.Drawing.Size(1404, 460);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "PID";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(32, 23);
            this.label21.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(173, 25);
            this.label21.TabIndex = 41;
            this.label21.Text = "BufferFitsCount=";
            // 
            // BufferFitsCount
            // 
            this.BufferFitsCount.Location = new System.Drawing.Point(214, 17);
            this.BufferFitsCount.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.BufferFitsCount.MaxLength = 10;
            this.BufferFitsCount.Name = "BufferFitsCount";
            this.BufferFitsCount.Size = new System.Drawing.Size(166, 31);
            this.BufferFitsCount.TabIndex = 40;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(864, 363);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(114, 25);
            this.label11.TabIndex = 39;
            this.label11.Text = "RateLimit=";
            // 
            // DecRateLimitTextBox
            // 
            this.DecRateLimitTextBox.Location = new System.Drawing.Point(992, 358);
            this.DecRateLimitTextBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.DecRateLimitTextBox.MaxLength = 10;
            this.DecRateLimitTextBox.Name = "DecRateLimitTextBox";
            this.DecRateLimitTextBox.Size = new System.Drawing.Size(166, 31);
            this.DecRateLimitTextBox.TabIndex = 38;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(864, 273);
            this.label12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(114, 25);
            this.label12.TabIndex = 37;
            this.label12.Text = "RateLimit=";
            // 
            // RaRateLimitTextBox
            // 
            this.RaRateLimitTextBox.Location = new System.Drawing.Point(992, 267);
            this.RaRateLimitTextBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.RaRateLimitTextBox.MaxLength = 10;
            this.RaRateLimitTextBox.Name = "RaRateLimitTextBox";
            this.RaRateLimitTextBox.Size = new System.Drawing.Size(166, 31);
            this.RaRateLimitTextBox.TabIndex = 36;
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(1176, 352);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(150, 44);
            this.SaveButton.TabIndex = 35;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(66, 363);
            this.label13.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 25);
            this.label13.TabIndex = 34;
            this.label13.Text = "DEC";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(66, 273);
            this.label14.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 25);
            this.label14.TabIndex = 33;
            this.label14.Text = "RA";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(620, 363);
            this.label15.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(50, 25);
            this.label15.TabIndex = 32;
            this.label15.Text = "Kd=";
            // 
            // PID_Setting_Kd_DEC
            // 
            this.PID_Setting_Kd_DEC.Location = new System.Drawing.Point(672, 358);
            this.PID_Setting_Kd_DEC.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Kd_DEC.MaxLength = 10;
            this.PID_Setting_Kd_DEC.Name = "PID_Setting_Kd_DEC";
            this.PID_Setting_Kd_DEC.Size = new System.Drawing.Size(166, 31);
            this.PID_Setting_Kd_DEC.TabIndex = 31;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(396, 363);
            this.label16.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(43, 25);
            this.label16.TabIndex = 30;
            this.label16.Text = "Ki=";
            // 
            // PID_Setting_Ki_DEC
            // 
            this.PID_Setting_Ki_DEC.Location = new System.Drawing.Point(448, 358);
            this.PID_Setting_Ki_DEC.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Ki_DEC.MaxLength = 10;
            this.PID_Setting_Ki_DEC.Name = "PID_Setting_Ki_DEC";
            this.PID_Setting_Ki_DEC.Size = new System.Drawing.Size(156, 31);
            this.PID_Setting_Ki_DEC.TabIndex = 29;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(164, 363);
            this.label17.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(50, 25);
            this.label17.TabIndex = 28;
            this.label17.Text = "Kp=";
            // 
            // PID_Setting_Kp_DEC
            // 
            this.PID_Setting_Kp_DEC.Location = new System.Drawing.Point(216, 358);
            this.PID_Setting_Kp_DEC.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Kp_DEC.MaxLength = 10;
            this.PID_Setting_Kp_DEC.Name = "PID_Setting_Kp_DEC";
            this.PID_Setting_Kp_DEC.Size = new System.Drawing.Size(164, 31);
            this.PID_Setting_Kp_DEC.TabIndex = 27;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(620, 273);
            this.label18.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 25);
            this.label18.TabIndex = 26;
            this.label18.Text = "Kd=";
            // 
            // PID_Setting_Kd_RA
            // 
            this.PID_Setting_Kd_RA.Location = new System.Drawing.Point(672, 267);
            this.PID_Setting_Kd_RA.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Kd_RA.MaxLength = 10;
            this.PID_Setting_Kd_RA.Name = "PID_Setting_Kd_RA";
            this.PID_Setting_Kd_RA.Size = new System.Drawing.Size(166, 31);
            this.PID_Setting_Kd_RA.TabIndex = 25;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(396, 273);
            this.label19.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(43, 25);
            this.label19.TabIndex = 24;
            this.label19.Text = "Ki=";
            // 
            // PID_Setting_Ki_RA
            // 
            this.PID_Setting_Ki_RA.Location = new System.Drawing.Point(448, 267);
            this.PID_Setting_Ki_RA.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Ki_RA.MaxLength = 10;
            this.PID_Setting_Ki_RA.Name = "PID_Setting_Ki_RA";
            this.PID_Setting_Ki_RA.Size = new System.Drawing.Size(156, 31);
            this.PID_Setting_Ki_RA.TabIndex = 23;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(164, 273);
            this.label20.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(50, 25);
            this.label20.TabIndex = 22;
            this.label20.Text = "Kp=";
            // 
            // PID_Setting_Kp_RA
            // 
            this.PID_Setting_Kp_RA.Location = new System.Drawing.Point(216, 267);
            this.PID_Setting_Kp_RA.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Kp_RA.MaxLength = 10;
            this.PID_Setting_Kp_RA.Name = "PID_Setting_Kp_RA";
            this.PID_Setting_Kp_RA.Size = new System.Drawing.Size(164, 31);
            this.PID_Setting_Kp_RA.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1202, 31);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(125, 25);
            this.label10.TabIndex = 20;
            this.label10.Text = "Processing:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(854, 167);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 25);
            this.label8.TabIndex = 19;
            this.label8.Text = "Nfilt=";
            // 
            // PID_Setting_Nfilt_DEC
            // 
            this.PID_Setting_Nfilt_DEC.Location = new System.Drawing.Point(928, 162);
            this.PID_Setting_Nfilt_DEC.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Nfilt_DEC.MaxLength = 10;
            this.PID_Setting_Nfilt_DEC.Name = "PID_Setting_Nfilt_DEC";
            this.PID_Setting_Nfilt_DEC.Size = new System.Drawing.Size(178, 31);
            this.PID_Setting_Nfilt_DEC.TabIndex = 18;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(854, 77);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 25);
            this.label9.TabIndex = 17;
            this.label9.Text = "Nfilt=";
            // 
            // PID_Setting_Nfilt_RA
            // 
            this.PID_Setting_Nfilt_RA.Location = new System.Drawing.Point(928, 71);
            this.PID_Setting_Nfilt_RA.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Nfilt_RA.MaxLength = 10;
            this.PID_Setting_Nfilt_RA.Name = "PID_Setting_Nfilt_RA";
            this.PID_Setting_Nfilt_RA.Size = new System.Drawing.Size(178, 31);
            this.PID_Setting_Nfilt_RA.TabIndex = 16;
            // 
            // ProcessingFilter
            // 
            this.ProcessingFilter.AutoSize = true;
            this.ProcessingFilter.Location = new System.Drawing.Point(1208, 121);
            this.ProcessingFilter.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ProcessingFilter.Name = "ProcessingFilter";
            this.ProcessingFilter.Size = new System.Drawing.Size(91, 29);
            this.ProcessingFilter.TabIndex = 15;
            this.ProcessingFilter.TabStop = true;
            this.ProcessingFilter.Text = "Filter";
            this.ProcessingFilter.UseVisualStyleBackColor = true;
            // 
            // ProcessingTraditional
            // 
            this.ProcessingTraditional.AutoSize = true;
            this.ProcessingTraditional.Location = new System.Drawing.Point(1208, 265);
            this.ProcessingTraditional.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ProcessingTraditional.Name = "ProcessingTraditional";
            this.ProcessingTraditional.Size = new System.Drawing.Size(144, 29);
            this.ProcessingTraditional.TabIndex = 14;
            this.ProcessingTraditional.TabStop = true;
            this.ProcessingTraditional.Text = "Traditional";
            this.ProcessingTraditional.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(66, 167);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 25);
            this.label7.TabIndex = 13;
            this.label7.Text = "DEC";
            // 
            // RA
            // 
            this.RA.AutoSize = true;
            this.RA.Location = new System.Drawing.Point(66, 77);
            this.RA.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.RA.Name = "RA";
            this.RA.Size = new System.Drawing.Size(41, 25);
            this.RA.TabIndex = 12;
            this.RA.Text = "RA";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(620, 167);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 25);
            this.label4.TabIndex = 11;
            this.label4.Text = "Kd=";
            // 
            // PID_Setting_Kd_DEC_filter
            // 
            this.PID_Setting_Kd_DEC_filter.Location = new System.Drawing.Point(672, 162);
            this.PID_Setting_Kd_DEC_filter.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Kd_DEC_filter.MaxLength = 10;
            this.PID_Setting_Kd_DEC_filter.Name = "PID_Setting_Kd_DEC_filter";
            this.PID_Setting_Kd_DEC_filter.Size = new System.Drawing.Size(166, 31);
            this.PID_Setting_Kd_DEC_filter.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(396, 167);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 25);
            this.label5.TabIndex = 9;
            this.label5.Text = "Ki=";
            // 
            // PID_Setting_Ki_DEC_filter
            // 
            this.PID_Setting_Ki_DEC_filter.Location = new System.Drawing.Point(448, 162);
            this.PID_Setting_Ki_DEC_filter.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Ki_DEC_filter.MaxLength = 10;
            this.PID_Setting_Ki_DEC_filter.Name = "PID_Setting_Ki_DEC_filter";
            this.PID_Setting_Ki_DEC_filter.Size = new System.Drawing.Size(156, 31);
            this.PID_Setting_Ki_DEC_filter.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(164, 167);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 25);
            this.label6.TabIndex = 7;
            this.label6.Text = "Kp=";
            // 
            // PID_Setting_Kp_DEC_filter
            // 
            this.PID_Setting_Kp_DEC_filter.Location = new System.Drawing.Point(216, 162);
            this.PID_Setting_Kp_DEC_filter.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Kp_DEC_filter.MaxLength = 10;
            this.PID_Setting_Kp_DEC_filter.Name = "PID_Setting_Kp_DEC_filter";
            this.PID_Setting_Kp_DEC_filter.Size = new System.Drawing.Size(164, 31);
            this.PID_Setting_Kp_DEC_filter.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(620, 77);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 25);
            this.label3.TabIndex = 5;
            this.label3.Text = "Kd=";
            // 
            // PID_Setting_Kd_RA_filter
            // 
            this.PID_Setting_Kd_RA_filter.Location = new System.Drawing.Point(672, 71);
            this.PID_Setting_Kd_RA_filter.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Kd_RA_filter.MaxLength = 10;
            this.PID_Setting_Kd_RA_filter.Name = "PID_Setting_Kd_RA_filter";
            this.PID_Setting_Kd_RA_filter.Size = new System.Drawing.Size(166, 31);
            this.PID_Setting_Kd_RA_filter.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(396, 77);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Ki=";
            // 
            // PID_Setting_Ki_RA_filter
            // 
            this.PID_Setting_Ki_RA_filter.Location = new System.Drawing.Point(448, 71);
            this.PID_Setting_Ki_RA_filter.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Ki_RA_filter.MaxLength = 10;
            this.PID_Setting_Ki_RA_filter.Name = "PID_Setting_Ki_RA_filter";
            this.PID_Setting_Ki_RA_filter.Size = new System.Drawing.Size(156, 31);
            this.PID_Setting_Ki_RA_filter.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(164, 77);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Kp=";
            // 
            // PID_Setting_Kp_RA_filter
            // 
            this.PID_Setting_Kp_RA_filter.Location = new System.Drawing.Point(216, 71);
            this.PID_Setting_Kp_RA_filter.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PID_Setting_Kp_RA_filter.MaxLength = 10;
            this.PID_Setting_Kp_RA_filter.Name = "PID_Setting_Kp_RA_filter";
            this.PID_Setting_Kp_RA_filter.Size = new System.Drawing.Size(164, 31);
            this.PID_Setting_Kp_RA_filter.TabIndex = 0;
            // 
            // PlatesolveGroupbox
            // 
            this.PlatesolveGroupbox.Controls.Add(this.textBox3);
            this.PlatesolveGroupbox.Controls.Add(this.PlaceSolvePathBrowseButton);
            this.PlatesolveGroupbox.Location = new System.Drawing.Point(37, 157);
            this.PlatesolveGroupbox.Name = "PlatesolveGroupbox";
            this.PlatesolveGroupbox.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PlatesolveGroupbox.Size = new System.Drawing.Size(1404, 138);
            this.PlatesolveGroupbox.TabIndex = 7;
            this.PlatesolveGroupbox.TabStop = false;
            this.PlatesolveGroupbox.Text = "Platesolve UCAC4 path";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(312, 44);
            this.textBox3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(1006, 31);
            this.textBox3.TabIndex = 3;
            // 
            // PlaceSolvePathBrowseButton
            // 
            this.PlaceSolvePathBrowseButton.Location = new System.Drawing.Point(16, 19);
            this.PlaceSolvePathBrowseButton.Name = "PlaceSolvePathBrowseButton";
            this.PlaceSolvePathBrowseButton.Size = new System.Drawing.Size(131, 26);
            this.PlaceSolvePathBrowseButton.TabIndex = 2;
            this.PlaceSolvePathBrowseButton.Text = "Browse";
            this.PlaceSolvePathBrowseButton.UseVisualStyleBackColor = true;
            this.PlaceSolvePathBrowseButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Timestamp,
            this.Filename,
            this.dt_time,
            this.PlateRa,
            this.RaP,
            this.RaI,
            this.RaD,
            this.NewRaRate,
            this.PlateDec,
            this.DecP,
            this.DecI,
            this.DecD,
            this.NewDecRate,
            this.Messages});
            this.dataGridView1.Location = new System.Drawing.Point(37, 492);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 82;
            this.dataGridView1.Size = new System.Drawing.Size(2116, 488);
            this.dataGridView1.TabIndex = 8;
            // 
            // Export
            // 
            this.Export.Location = new System.Drawing.Point(1006, 446);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(75, 23);
            this.Export.TabIndex = 9;
            this.Export.Text = "CSV";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.Export_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(919, 446);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 10;
            this.button5.Text = "Excel";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Timestamp
            // 
            this.Timestamp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Timestamp.HeaderText = "Timestamp";
            this.Timestamp.MinimumWidth = 10;
            this.Timestamp.Name = "Timestamp";
            this.Timestamp.ReadOnly = true;
            this.Timestamp.Width = 162;
            // 
            // Filename
            // 
            this.Filename.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Filename.HeaderText = "Filename";
            this.Filename.MinimumWidth = 10;
            this.Filename.Name = "Filename";
            this.Filename.ReadOnly = true;
            this.Filename.Width = 145;
            // 
            // dt_time
            // 
            this.dt_time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dt_time.HeaderText = "dt_sec";
            this.dt_time.MinimumWidth = 10;
            this.dt_time.Name = "dt_time";
            this.dt_time.ReadOnly = true;
            this.dt_time.Width = 121;
            // 
            // PlateRa
            // 
            this.PlateRa.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.PlateRa.HeaderText = "PlateRa";
            this.PlateRa.MinimumWidth = 10;
            this.PlateRa.Name = "PlateRa";
            this.PlateRa.ReadOnly = true;
            this.PlateRa.Width = 133;
            // 
            // RaP
            // 
            this.RaP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.RaP.HeaderText = "RaP";
            this.RaP.MinimumWidth = 10;
            this.RaP.Name = "RaP";
            this.RaP.ReadOnly = true;
            this.RaP.Width = 98;
            // 
            // RaI
            // 
            this.RaI.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.RaI.HeaderText = "RaI";
            this.RaI.MinimumWidth = 10;
            this.RaI.Name = "RaI";
            this.RaI.ReadOnly = true;
            this.RaI.Width = 89;
            // 
            // RaD
            // 
            this.RaD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.RaD.HeaderText = "RaD";
            this.RaD.MinimumWidth = 10;
            this.RaD.Name = "RaD";
            this.RaD.ReadOnly = true;
            this.RaD.Width = 99;
            // 
            // NewRaRate
            // 
            this.NewRaRate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.NewRaRate.HeaderText = "NewRaRate";
            this.NewRaRate.MinimumWidth = 10;
            this.NewRaRate.Name = "NewRaRate";
            this.NewRaRate.ReadOnly = true;
            this.NewRaRate.Width = 171;
            // 
            // PlateDec
            // 
            this.PlateDec.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.PlateDec.HeaderText = "PlateDec";
            this.PlateDec.MinimumWidth = 10;
            this.PlateDec.Name = "PlateDec";
            this.PlateDec.ReadOnly = true;
            this.PlateDec.Width = 144;
            // 
            // DecP
            // 
            this.DecP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.DecP.HeaderText = "DecP";
            this.DecP.MinimumWidth = 10;
            this.DecP.Name = "DecP";
            this.DecP.ReadOnly = true;
            this.DecP.Width = 109;
            // 
            // DecI
            // 
            this.DecI.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.DecI.HeaderText = "DecI";
            this.DecI.MinimumWidth = 10;
            this.DecI.Name = "DecI";
            this.DecI.ReadOnly = true;
            // 
            // DecD
            // 
            this.DecD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.DecD.HeaderText = "DecD";
            this.DecD.MinimumWidth = 10;
            this.DecD.Name = "DecD";
            this.DecD.ReadOnly = true;
            this.DecD.Width = 110;
            // 
            // NewDecRate
            // 
            this.NewDecRate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.NewDecRate.HeaderText = "NewDecRate";
            this.NewDecRate.MinimumWidth = 10;
            this.NewDecRate.Name = "NewDecRate";
            this.NewDecRate.ReadOnly = true;
            this.NewDecRate.Width = 182;
            // 
            // Messages
            // 
            this.Messages.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Messages.HeaderText = "Messages";
            this.Messages.Name = "Messages";
            this.Messages.ReadOnly = true;
            this.Messages.Width = 80;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(1537, 858);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(135, 41);
            this.button6.TabIndex = 11;
            this.button6.Text = "Reset PID";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(2884, 1421);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.Export);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.PlatesolveGroupbox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.StartStopButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MinimumSize = new System.Drawing.Size(2868, 1369);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
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
        private System.Windows.Forms.Button WatchFolderBrowseButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button StartStopButton;
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
        private System.Windows.Forms.Button PlaceSolvePathBrowseButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.Button SelectTelescopeButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button Export;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox DecRateLimitTextBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox RaRateLimitTextBox;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox BufferFitsCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn Filename;
        private System.Windows.Forms.DataGridViewTextBoxColumn dt_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlateRa;
        private System.Windows.Forms.DataGridViewTextBoxColumn RaP;
        private System.Windows.Forms.DataGridViewTextBoxColumn RaI;
        private System.Windows.Forms.DataGridViewTextBoxColumn RaD;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewRaRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlateDec;
        private System.Windows.Forms.DataGridViewTextBoxColumn DecP;
        private System.Windows.Forms.DataGridViewTextBoxColumn DecI;
        private System.Windows.Forms.DataGridViewTextBoxColumn DecD;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewDecRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Messages;
    }
}

