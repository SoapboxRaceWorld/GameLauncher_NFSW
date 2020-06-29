﻿namespace GameLauncher.App
{
    partial class UpdatePopup
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.changelogText = new System.Windows.Forms.TextBox();
            this.ignore = new System.Windows.Forms.Button();
            this.update = new System.Windows.Forms.Button();
            this.icon = new System.Windows.Forms.PictureBox();
            this.updateLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.changelogText);
            this.groupBox1.Location = new System.Drawing.Point(12, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(374, 158);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Changelog:";
            // 
            // changelogText
            // 
            this.changelogText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.changelogText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.changelogText.Location = new System.Drawing.Point(6, 19);
            this.changelogText.Multiline = true;
            this.changelogText.Name = "changelogText";
            this.changelogText.ReadOnly = true;
            this.changelogText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.changelogText.Size = new System.Drawing.Size(362, 133);
            this.changelogText.TabIndex = 0;
            // 
            // ignore
            // 
            this.ignore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ignore.Location = new System.Drawing.Point(311, 230);
            this.ignore.Name = "ignore";
            this.ignore.Size = new System.Drawing.Size(75, 23);
            this.ignore.TabIndex = 1;
            this.ignore.Text = "Ignore";
            this.ignore.UseVisualStyleBackColor = true;
            // 
            // update
            // 
            this.update.Location = new System.Drawing.Point(230, 230);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(75, 23);
            this.update.TabIndex = 2;
            this.update.Text = "Update";
            this.update.UseVisualStyleBackColor = true;
            // 
            // icon
            // 
            this.icon.Location = new System.Drawing.Point(20, 20);
            this.icon.Name = "icon";
            this.icon.Size = new System.Drawing.Size(32, 32);
            this.icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.icon.TabIndex = 3;
            this.icon.TabStop = false;
            // 
            // updateLabel
            // 
            this.updateLabel.AutoSize = true;
            this.updateLabel.Location = new System.Drawing.Point(63, 19);
            this.updateLabel.Name = "updateLabel";
            this.updateLabel.Size = new System.Drawing.Size(35, 13);
            this.updateLabel.TabIndex = 4;
            this.updateLabel.Text = "label1";
            // 
            // UpdatePopup
            // 
            this.AcceptButton = this.update;
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuPopup;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.CancelButton = this.ignore;
            this.ClientSize = new System.Drawing.Size(398, 262);
            this.Controls.Add(this.updateLabel);
            this.Controls.Add(this.icon);
            this.Controls.Add(this.update);
            this.Controls.Add(this.ignore);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdatePopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update Available";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ignore;
        private System.Windows.Forms.Button update;
        private System.Windows.Forms.TextBox changelogText;
        private System.Windows.Forms.PictureBox icon;
        private System.Windows.Forms.Label updateLabel;
    }
}