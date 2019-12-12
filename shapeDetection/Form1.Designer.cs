namespace shapeDetection
{
    partial class Roulette_main
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
            this.pic_videoDisplay = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.lbl_round = new System.Windows.Forms.Label();
            this.lbl_statue = new System.Windows.Forms.Label();
            this.pic_visualAngle = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pic_videoDisplay)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_visualAngle)).BeginInit();
            this.SuspendLayout();
            // 
            // pic_videoDisplay
            // 
            this.pic_videoDisplay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pic_videoDisplay.Location = new System.Drawing.Point(8, 15);
            this.pic_videoDisplay.Name = "pic_videoDisplay";
            this.pic_videoDisplay.Size = new System.Drawing.Size(256, 198);
            this.pic_videoDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic_videoDisplay.TabIndex = 1;
            this.pic_videoDisplay.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.btn_close);
            this.groupBox1.Controls.Add(this.lbl_round);
            this.groupBox1.Controls.Add(this.lbl_statue);
            this.groupBox1.Controls.Add(this.pic_videoDisplay);
            this.groupBox1.Controls.Add(this.pic_visualAngle);
            this.groupBox1.Location = new System.Drawing.Point(6, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(277, 499);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(16, 250);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(235, 20);
            this.textBox1.TabIndex = 8;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(97, 221);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Start";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(7, 221);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Screen Rect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_close
            // 
            this.btn_close.Location = new System.Drawing.Point(187, 221);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(75, 23);
            this.btn_close.TabIndex = 5;
            this.btn_close.Text = "Close";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // lbl_round
            // 
            this.lbl_round.AutoSize = true;
            this.lbl_round.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbl_round.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_round.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lbl_round.Location = new System.Drawing.Point(202, 278);
            this.lbl_round.Name = "lbl_round";
            this.lbl_round.Size = new System.Drawing.Size(60, 16);
            this.lbl_round.TabIndex = 4;
            this.lbl_round.Text = "0: Round";
            // 
            // lbl_statue
            // 
            this.lbl_statue.AutoSize = true;
            this.lbl_statue.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbl_statue.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_statue.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lbl_statue.Location = new System.Drawing.Point(12, 279);
            this.lbl_statue.Name = "lbl_statue";
            this.lbl_statue.Size = new System.Drawing.Size(38, 16);
            this.lbl_statue.TabIndex = 4;
            this.lbl_statue.Text = "Initial";
            // 
            // pic_visualAngle
            // 
            this.pic_visualAngle.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pic_visualAngle.Location = new System.Drawing.Point(4, 274);
            this.pic_visualAngle.Name = "pic_visualAngle";
            this.pic_visualAngle.Size = new System.Drawing.Size(264, 219);
            this.pic_visualAngle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic_visualAngle.TabIndex = 1;
            this.pic_visualAngle.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 40;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Roulette_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(286, 502);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Roulette_main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Roulette detection ";
            this.Load += new System.EventHandler(this.Roulette_main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pic_videoDisplay)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_visualAngle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pic_videoDisplay;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pic_visualAngle;
        private System.Windows.Forms.Label lbl_statue;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Label lbl_round;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}

