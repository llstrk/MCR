﻿namespace MCR
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.picTopBar = new System.Windows.Forms.PictureBox();
            this.picTypeBar = new System.Windows.Forms.PictureBox();
            this.picArt = new System.Windows.Forms.PictureBox();
            this.picFull = new System.Windows.Forms.PictureBox();
            this.picOrg = new System.Windows.Forms.PictureBox();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.txtThreshold = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTopBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTypeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picArt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFull)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOrg)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(547, 480);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(444, 548);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(589, 548);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // picTopBar
            // 
            this.picTopBar.Location = new System.Drawing.Point(1133, 14);
            this.picTopBar.Name = "picTopBar";
            this.picTopBar.Size = new System.Drawing.Size(220, 16);
            this.picTopBar.TabIndex = 3;
            this.picTopBar.TabStop = false;
            // 
            // picTypeBar
            // 
            this.picTypeBar.Location = new System.Drawing.Point(1133, 55);
            this.picTypeBar.Name = "picTypeBar";
            this.picTypeBar.Size = new System.Drawing.Size(220, 16);
            this.picTypeBar.TabIndex = 4;
            this.picTypeBar.TabStop = false;
            // 
            // picArt
            // 
            this.picArt.Location = new System.Drawing.Point(1133, 98);
            this.picArt.Name = "picArt";
            this.picArt.Size = new System.Drawing.Size(220, 162);
            this.picArt.TabIndex = 5;
            this.picArt.TabStop = false;
            // 
            // picFull
            // 
            this.picFull.Location = new System.Drawing.Point(1133, 318);
            this.picFull.Name = "picFull";
            this.picFull.Size = new System.Drawing.Size(241, 346);
            this.picFull.TabIndex = 6;
            this.picFull.TabStop = false;
            // 
            // picOrg
            // 
            this.picOrg.Location = new System.Drawing.Point(565, 12);
            this.picOrg.Name = "picOrg";
            this.picOrg.Size = new System.Drawing.Size(547, 480);
            this.picOrg.TabIndex = 7;
            this.picOrg.TabStop = false;
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(444, 577);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(220, 20);
            this.txtFilename.TabIndex = 8;
            this.txtFilename.Text = "test8.jpg";
            // 
            // txtThreshold
            // 
            this.txtThreshold.Location = new System.Drawing.Point(444, 604);
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(43, 20);
            this.txtThreshold.TabIndex = 9;
            this.txtThreshold.Text = "100";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1395, 678);
            this.Controls.Add(this.txtThreshold);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.picOrg);
            this.Controls.Add(this.picFull);
            this.Controls.Add(this.picArt);
            this.Controls.Add(this.picTypeBar);
            this.Controls.Add(this.picTopBar);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTopBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTypeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picArt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFull)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOrg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox picTopBar;
        private System.Windows.Forms.PictureBox picTypeBar;
        private System.Windows.Forms.PictureBox picArt;
        private System.Windows.Forms.PictureBox picFull;
        private System.Windows.Forms.PictureBox picOrg;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.TextBox txtThreshold;
    }
}

