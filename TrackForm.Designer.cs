﻿using Emgu.CV.UI;

namespace VCD_Demo
{
    partial class TrackForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrackForm));
            this.resultImage = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.resultImage)).BeginInit();
            this.SuspendLayout();
            // 
            // resultImage
            // 
            this.resultImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultImage.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.resultImage.Location = new System.Drawing.Point(0, 0);
            this.resultImage.Name = "resultImage";
            this.resultImage.Size = new System.Drawing.Size(854, 480);
            this.resultImage.TabIndex = 2;
            this.resultImage.TabStop = false;
            // 
            // TrackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 480);
            this.Controls.Add(this.resultImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrackForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TrackForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrackForm_FormClosing);
            this.Load += new System.EventHandler(this.TrackForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.resultImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ImageBox resultImage;
    }
}