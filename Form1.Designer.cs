using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WindowsFormsApp1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Install = new System.Windows.Forms.Button();
            this.siso = new System.Windows.Forms.Button();
            this.killpgrm = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.saving = new System.Windows.Forms.Label();
            this.usblabel = new System.Windows.Forms.Label();
            this.usbpick = new System.Windows.Forms.ComboBox();
            this.writeusb = new System.Windows.Forms.Button();
            this.Formatting = new System.Windows.Forms.Label();
            this.savedpercent = new System.Windows.Forms.Label();
            this.ProgressGif = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ProgressGif)).BeginInit();
            this.SuspendLayout();
            // 
            // Install
            // 
            this.Install.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Install.Location = new System.Drawing.Point(338, 299);
            this.Install.MinimumSize = new System.Drawing.Size(200, 100);
            this.Install.Name = "Install";
            this.Install.Size = new System.Drawing.Size(200, 100);
            this.Install.TabIndex = 0;
            this.Install.Text = "Install";
            this.Install.UseVisualStyleBackColor = true;
            this.Install.Click += new System.EventHandler(this.Imstall_Click);
            // 
            // siso
            // 
            this.siso.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.siso.Location = new System.Drawing.Point(338, 410);
            this.siso.MinimumSize = new System.Drawing.Size(200, 100);
            this.siso.Name = "siso";
            this.siso.Size = new System.Drawing.Size(200, 100);
            this.siso.TabIndex = 1;
            this.siso.Text = "Save ISO";
            this.siso.UseVisualStyleBackColor = true;
            this.siso.Click += new System.EventHandler(this.button1_Click);
            // 
            // killpgrm
            // 
            this.killpgrm.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.killpgrm.Location = new System.Drawing.Point(338, 521);
            this.killpgrm.MinimumSize = new System.Drawing.Size(200, 100);
            this.killpgrm.Name = "killpgrm";
            this.killpgrm.Size = new System.Drawing.Size(200, 100);
            this.killpgrm.TabIndex = 2;
            this.killpgrm.Text = "Exit";
            this.killpgrm.UseVisualStyleBackColor = true;
            this.killpgrm.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(376, 276);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Pear OS Installer";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // saving
            // 
            this.saving.AutoSize = true;
            this.saving.Location = new System.Drawing.Point(415, 624);
            this.saving.Name = "saving";
            this.saving.Size = new System.Drawing.Size(49, 13);
            this.saving.TabIndex = 4;
            this.saving.Text = "Saving...";
            this.saving.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.saving.Visible = false;
            this.saving.Click += new System.EventHandler(this.saving_Click);
            // 
            // usblabel
            // 
            this.usblabel.AutoSize = true;
            this.usblabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usblabel.Location = new System.Drawing.Point(363, 193);
            this.usblabel.Name = "usblabel";
            this.usblabel.Size = new System.Drawing.Size(165, 20);
            this.usblabel.TabIndex = 5;
            this.usblabel.Text = "Pick Your USB Drive";
            this.usblabel.Visible = false;
            // 
            // usbpick
            // 
            this.usbpick.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usbpick.FormattingEnabled = true;
            this.usbpick.Location = new System.Drawing.Point(309, 216);
            this.usbpick.Name = "usbpick";
            this.usbpick.Size = new System.Drawing.Size(259, 28);
            this.usbpick.TabIndex = 6;
            this.usbpick.Visible = false;
            this.usbpick.SelectedIndexChanged += new System.EventHandler(this.usbpick_SelectedIndexChanged);
            // 
            // writeusb
            // 
            this.writeusb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.writeusb.Location = new System.Drawing.Point(402, 250);
            this.writeusb.Name = "writeusb";
            this.writeusb.Size = new System.Drawing.Size(75, 23);
            this.writeusb.TabIndex = 7;
            this.writeusb.Text = "Write USB";
            this.writeusb.UseVisualStyleBackColor = true;
            this.writeusb.Visible = false;
            this.writeusb.Click += new System.EventHandler(this.writeusb_Click);
            // 
            // Formatting
            // 
            this.Formatting.AutoSize = true;
            this.Formatting.Location = new System.Drawing.Point(412, 281);
            this.Formatting.Name = "Formatting";
            this.Formatting.Size = new System.Drawing.Size(65, 13);
            this.Formatting.TabIndex = 8;
            this.Formatting.Text = "Formatting...";
            this.Formatting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Formatting.Visible = false;
            // 
            // savedpercent
            // 
            this.savedpercent.AutoSize = true;
            this.savedpercent.Location = new System.Drawing.Point(306, 299);
            this.savedpercent.Name = "savedpercent";
            this.savedpercent.Size = new System.Drawing.Size(65, 13);
            this.savedpercent.TabIndex = 9;
            this.savedpercent.Text = "Formatting...";
            this.savedpercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.savedpercent.Visible = false;
            // 
            // ProgressGif
            // 
            this.ProgressGif.Image = ((System.Drawing.Image)(resources.GetObject("ProgressGif.Image")));
            this.ProgressGif.InitialImage = ((System.Drawing.Image)(resources.GetObject("ProgressGif.InitialImage")));
            this.ProgressGif.Location = new System.Drawing.Point(400, 327);
            this.ProgressGif.Name = "ProgressGif";
            this.ProgressGif.Size = new System.Drawing.Size(80, 61);
            this.ProgressGif.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ProgressGif.TabIndex = 10;
            this.ProgressGif.TabStop = false;
            this.ProgressGif.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 904);
            this.Controls.Add(this.ProgressGif);
            this.Controls.Add(this.savedpercent);
            this.Controls.Add(this.Formatting);
            this.Controls.Add(this.writeusb);
            this.Controls.Add(this.usbpick);
            this.Controls.Add(this.usblabel);
            this.Controls.Add(this.saving);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.killpgrm);
            this.Controls.Add(this.siso);
            this.Controls.Add(this.Install);
            this.MinimumSize = new System.Drawing.Size(200, 100);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ProgressGif)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Install;
        private System.Windows.Forms.Button siso;
        private System.Windows.Forms.Button killpgrm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label saving;
        private System.Windows.Forms.Label usblabel;
        private System.Windows.Forms.ComboBox usbpick;
        private System.Windows.Forms.Button writeusb;
        private System.Windows.Forms.Label Formatting;
        private System.Windows.Forms.Label savedpercent;
        private System.Windows.Forms.PictureBox ProgressGif;
    }
}

