namespace HotelSim
{
    partial class Simulation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Simulation));
            this.MainSimDisplay = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelPauze = new System.Windows.Forms.Panel();
            this.labelGamePauzed = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            this.MainSimDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelPauze.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainSimDisplay
            // 
            this.MainSimDisplay.Controls.Add(this.panelPauze);
            this.MainSimDisplay.Controls.Add(this.pictureBox1);
            this.MainSimDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSimDisplay.Location = new System.Drawing.Point(0, 0);
            this.MainSimDisplay.Name = "MainSimDisplay";
            this.MainSimDisplay.Size = new System.Drawing.Size(946, 547);
            this.MainSimDisplay.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(946, 547);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panelPauze
            // 
            this.panelPauze.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(182)))), ((int)(((byte)(244)))));
            this.panelPauze.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPauze.Controls.Add(this.labelInfo);
            this.panelPauze.Controls.Add(this.labelGamePauzed);
            this.panelPauze.Location = new System.Drawing.Point(12, 36);
            this.panelPauze.Name = "panelPauze";
            this.panelPauze.Size = new System.Drawing.Size(164, 416);
            this.panelPauze.TabIndex = 1;
            // 
            // labelGamePauzed
            // 
            this.labelGamePauzed.Font = new System.Drawing.Font("Roboto Cn", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGamePauzed.Location = new System.Drawing.Point(0, 0);
            this.labelGamePauzed.Name = "labelGamePauzed";
            this.labelGamePauzed.Size = new System.Drawing.Size(163, 23);
            this.labelGamePauzed.TabIndex = 0;
            this.labelGamePauzed.Text = "- Simulation Pauzed -";
            this.labelGamePauzed.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelInfo
            // 
            this.labelInfo.Location = new System.Drawing.Point(3, 32);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(100, 69);
            this.labelInfo.TabIndex = 1;
            this.labelInfo.Text = "label1";
            // 
            // Simulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(129)))), ((int)(((byte)(209)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(946, 547);
            this.Controls.Add(this.MainSimDisplay);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Simulation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Simulation_KeyDown);
            this.MainSimDisplay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelPauze.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainSimDisplay;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panelPauze;
        private System.Windows.Forms.Label labelGamePauzed;
        private System.Windows.Forms.Label labelInfo;
    }
}

