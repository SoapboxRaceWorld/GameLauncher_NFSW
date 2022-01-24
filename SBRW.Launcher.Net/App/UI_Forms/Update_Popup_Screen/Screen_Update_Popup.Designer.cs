namespace SBRW.Launcher.App.UI_Forms.Update_Popup_Screen
{
    partial class Screen_Update_Popup
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
            this.Icon_Information = new System.Windows.Forms.PictureBox();
            this.Label_Text_Update = new System.Windows.Forms.Label();
            this.GroupBox_Changelog = new System.Windows.Forms.GroupBox();
            this.TextBox_Changelog = new System.Windows.Forms.TextBox();
            this.Button_Update = new System.Windows.Forms.Button();
            this.Button_Ignore = new System.Windows.Forms.Button();
            this.Button_Skip = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Icon_Information)).BeginInit();
            this.GroupBox_Changelog.SuspendLayout();
            this.SuspendLayout();
            // 
            // Icon_Information
            // 
            this.Icon_Information.Location = new System.Drawing.Point(20, 20);
            this.Icon_Information.Name = "Icon_Information";
            this.Icon_Information.Size = new System.Drawing.Size(32, 32);
            this.Icon_Information.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Icon_Information.TabIndex = 0;
            this.Icon_Information.TabStop = false;
            // 
            // Label_Text_Update
            // 
            this.Label_Text_Update.BackColor = System.Drawing.Color.Transparent;
            this.Label_Text_Update.ForeColor = System.Drawing.Color.Black;
            this.Label_Text_Update.Location = new System.Drawing.Point(64, 17);
            this.Label_Text_Update.Name = "Label_Text_Update";
            this.Label_Text_Update.Size = new System.Drawing.Size(322, 62);
            this.Label_Text_Update.TabIndex = 26;
            this.Label_Text_Update.Text = "An update is available. Would you like to update? Your version: ";
            this.Label_Text_Update.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // GroupBox_Changelog
            // 
            this.GroupBox_Changelog.Controls.Add(this.TextBox_Changelog);
            this.GroupBox_Changelog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GroupBox_Changelog.Location = new System.Drawing.Point(12, 82);
            this.GroupBox_Changelog.Name = "GroupBox_Changelog";
            this.GroupBox_Changelog.Size = new System.Drawing.Size(374, 158);
            this.GroupBox_Changelog.TabIndex = 27;
            this.GroupBox_Changelog.TabStop = false;
            this.GroupBox_Changelog.Text = "Changelog:";
            // 
            // TextBox_Changelog
            // 
            this.TextBox_Changelog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox_Changelog.Location = new System.Drawing.Point(6, 13);
            this.TextBox_Changelog.Multiline = true;
            this.TextBox_Changelog.Name = "TextBox_Changelog";
            this.TextBox_Changelog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBox_Changelog.Size = new System.Drawing.Size(368, 139);
            this.TextBox_Changelog.TabIndex = 35;
            this.TextBox_Changelog.Text = "This is a Test";
            // 
            // Button_Update
            // 
            this.Button_Update.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Update.Location = new System.Drawing.Point(82, 252);
            this.Button_Update.Name = "Button_Update";
            this.Button_Update.Size = new System.Drawing.Size(75, 23);
            this.Button_Update.TabIndex = 32;
            this.Button_Update.Text = "Update";
            this.Button_Update.UseVisualStyleBackColor = true;
            // 
            // Button_Ignore
            // 
            this.Button_Ignore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Ignore.Location = new System.Drawing.Point(163, 252);
            this.Button_Ignore.Name = "Button_Ignore";
            this.Button_Ignore.Size = new System.Drawing.Size(75, 23);
            this.Button_Ignore.TabIndex = 33;
            this.Button_Ignore.Text = "Ignore";
            this.Button_Ignore.UseVisualStyleBackColor = true;
            // 
            // Button_Skip
            // 
            this.Button_Skip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Skip.Location = new System.Drawing.Point(244, 252);
            this.Button_Skip.Name = "Button_Skip";
            this.Button_Skip.Size = new System.Drawing.Size(75, 23);
            this.Button_Skip.TabIndex = 34;
            this.Button_Skip.Text = "Skip";
            this.Button_Skip.UseVisualStyleBackColor = true;
            // 
            // Screen_Update_Popup
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuPopup;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.Button_Ignore;
            this.ClientSize = new System.Drawing.Size(398, 292);
            this.Controls.Add(this.Button_Skip);
            this.Controls.Add(this.Button_Ignore);
            this.Controls.Add(this.Button_Update);
            this.Controls.Add(this.GroupBox_Changelog);
            this.Controls.Add(this.Label_Text_Update);
            this.Controls.Add(this.Icon_Information);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Screen_Update_Popup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update Available - SBRW Launcher";
            ((System.ComponentModel.ISupportInitialize)(this.Icon_Information)).EndInit();
            this.GroupBox_Changelog.ResumeLayout(false);
            this.GroupBox_Changelog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Icon_Information;
        private System.Windows.Forms.Label Label_Text_Update;
        private System.Windows.Forms.GroupBox GroupBox_Changelog;
        private System.Windows.Forms.Button Button_Update;
        private System.Windows.Forms.Button Button_Ignore;
        private System.Windows.Forms.Button Button_Skip;
        private System.Windows.Forms.TextBox TextBox_Changelog;
    }
}