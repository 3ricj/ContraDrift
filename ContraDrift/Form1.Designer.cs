
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.RA = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.PID_Setting_Kd_DEC = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.PID_Setting_Ki_DEC = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.PID_Setting_Kp_DEC = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PID_Setting_Kd_RA = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PID_Setting_Ki_RA = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PID_Setting_Kp_RA = new System.Windows.Forms.TextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.PID_Setting_Nfilt_DEC = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.PID_Setting_Nfilt_RA = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(33, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select Telescope";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(198, 40);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(289, 20);
            this.textBox1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Location = new System.Drawing.Point(37, 120);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(702, 93);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Watch Folder";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(198, 43);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(289, 20);
            this.textBox2.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(33, 43);
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
            this.groupBox2.Size = new System.Drawing.Size(702, 91);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ASCOM";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(53, 456);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(92, 29);
            this.button3.TabIndex = 5;
            this.button3.Text = "Start";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.PID_Setting_Nfilt_DEC);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.PID_Setting_Nfilt_RA);
            this.groupBox3.Controls.Add(this.radioButton2);
            this.groupBox3.Controls.Add(this.radioButton1);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.RA);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.PID_Setting_Kd_DEC);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.PID_Setting_Ki_DEC);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.PID_Setting_Kp_DEC);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.PID_Setting_Kd_RA);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.PID_Setting_Ki_RA);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.PID_Setting_Kp_RA);
            this.groupBox3.Location = new System.Drawing.Point(37, 219);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(702, 197);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "PID";
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
            // PID_Setting_Kd_DEC
            // 
            this.PID_Setting_Kd_DEC.Location = new System.Drawing.Point(336, 84);
            this.PID_Setting_Kd_DEC.Name = "PID_Setting_Kd_DEC";
            this.PID_Setting_Kd_DEC.Size = new System.Drawing.Size(62, 20);
            this.PID_Setting_Kd_DEC.TabIndex = 10;
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
            // PID_Setting_Ki_DEC
            // 
            this.PID_Setting_Ki_DEC.Location = new System.Drawing.Point(224, 84);
            this.PID_Setting_Ki_DEC.Name = "PID_Setting_Ki_DEC";
            this.PID_Setting_Ki_DEC.Size = new System.Drawing.Size(62, 20);
            this.PID_Setting_Ki_DEC.TabIndex = 8;
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
            // PID_Setting_Kp_DEC
            // 
            this.PID_Setting_Kp_DEC.Location = new System.Drawing.Point(108, 84);
            this.PID_Setting_Kp_DEC.Name = "PID_Setting_Kp_DEC";
            this.PID_Setting_Kp_DEC.Size = new System.Drawing.Size(62, 20);
            this.PID_Setting_Kp_DEC.TabIndex = 6;
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
            // PID_Setting_Kd_RA
            // 
            this.PID_Setting_Kd_RA.Location = new System.Drawing.Point(336, 37);
            this.PID_Setting_Kd_RA.Name = "PID_Setting_Kd_RA";
            this.PID_Setting_Kd_RA.Size = new System.Drawing.Size(62, 20);
            this.PID_Setting_Kd_RA.TabIndex = 4;
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
            // PID_Setting_Ki_RA
            // 
            this.PID_Setting_Ki_RA.Location = new System.Drawing.Point(224, 37);
            this.PID_Setting_Ki_RA.Name = "PID_Setting_Ki_RA";
            this.PID_Setting_Ki_RA.Size = new System.Drawing.Size(62, 20);
            this.PID_Setting_Ki_RA.TabIndex = 2;
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
            // PID_Setting_Kp_RA
            // 
            this.PID_Setting_Kp_RA.Location = new System.Drawing.Point(108, 37);
            this.PID_Setting_Kp_RA.Name = "PID_Setting_Kp_RA";
            this.PID_Setting_Kp_RA.Size = new System.Drawing.Size(62, 20);
            this.PID_Setting_Kp_RA.TabIndex = 0;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(108, 125);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(74, 17);
            this.radioButton1.TabIndex = 14;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Traditional";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(201, 125);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(44, 17);
            this.radioButton2.TabIndex = 15;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "fitdir";
            this.radioButton2.UseVisualStyleBackColor = true;
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
            this.PID_Setting_Nfilt_DEC.Name = "PID_Setting_Nfilt_DEC";
            this.PID_Setting_Nfilt_DEC.Size = new System.Drawing.Size(62, 20);
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
            this.PID_Setting_Nfilt_RA.Name = "PID_Setting_Nfilt_RA";
            this.PID_Setting_Nfilt_RA.Size = new System.Drawing.Size(62, 20);
            this.PID_Setting_Nfilt_RA.TabIndex = 16;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(33, 127);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Processing:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 758);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox PID_Setting_Kd_RA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PID_Setting_Ki_RA;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PID_Setting_Kp_RA;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PID_Setting_Kd_DEC;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PID_Setting_Ki_DEC;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox PID_Setting_Kp_DEC;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label RA;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox PID_Setting_Nfilt_DEC;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox PID_Setting_Nfilt_RA;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}

