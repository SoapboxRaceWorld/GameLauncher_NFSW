namespace GameLauncher.App.UI_Forms.UpdatePopup_Screen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdatePopup));
            this.ChangelogBox = new System.Windows.Forms.GroupBox();
            this.ChangelogText = new System.Windows.Forms.TextBox();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.UpdateIcon = new System.Windows.Forms.PictureBox();
            this.UpdateText = new System.Windows.Forms.Label();
            this.SkipButton = new System.Windows.Forms.Button();
            this.IgnoreButton = new System.Windows.Forms.Button();
            this.ChangelogBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // ChangelogBox
            // 
            this.ChangelogBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.ChangelogBox.Controls.Add(this.ChangelogText);
            this.ChangelogBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChangelogBox.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChangelogBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ChangelogBox.Location = new System.Drawing.Point(12, 82);
            this.ChangelogBox.Margin = new System.Windows.Forms.Padding(0);
            this.ChangelogBox.Name = "ChangelogBox";
            this.ChangelogBox.Padding = new System.Windows.Forms.Padding(0);
            this.ChangelogBox.Size = new System.Drawing.Size(374, 158);
            this.ChangelogBox.TabIndex = 0;
            this.ChangelogBox.TabStop = false;
            this.ChangelogBox.Text = "Changelog:";
            // 
            // ChangelogText
            // 
            this.ChangelogText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.ChangelogText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ChangelogText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChangelogText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.ChangelogText.Location = new System.Drawing.Point(6, 13);
            this.ChangelogText.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ChangelogText.Multiline = true;
            this.ChangelogText.Name = "ChangelogText";
            this.ChangelogText.ReadOnly = true;
            this.ChangelogText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ChangelogText.Size = new System.Drawing.Size(368, 139);
            this.ChangelogText.TabIndex = 3;
            this.ChangelogText.Text = "This is a Test";
            // 
            // UpdateButton
            // 
            this.UpdateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.UpdateButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.UpdateButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.UpdateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UpdateButton.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.UpdateButton.Location = new System.Drawing.Point(82, 252);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(75, 23);
            this.UpdateButton.TabIndex = 1;
            this.UpdateButton.Text = "Update";
            this.UpdateButton.UseVisualStyleBackColor = false;
            // 
            // UpdateIcon
            // 
            this.UpdateIcon.Location = new System.Drawing.Point(20, 20);
            this.UpdateIcon.Name = "UpdateIcon";
            this.UpdateIcon.Size = new System.Drawing.Size(32, 32);
            this.UpdateIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.UpdateIcon.TabIndex = 3;
            this.UpdateIcon.TabStop = false;
            // 
            // UpdateText
            // 
            this.UpdateText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateText.Location = new System.Drawing.Point(64, 17);
            this.UpdateText.Name = "UpdateText";
            this.UpdateText.Size = new System.Drawing.Size(322, 62);
            this.UpdateText.TabIndex = 4;
            this.UpdateText.Text = "An update is available. Would you like to update?Your version: ";
            // 
            // SkipButton
            // 
            this.SkipButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.SkipButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.SkipButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.SkipButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SkipButton.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SkipButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SkipButton.Location = new System.Drawing.Point(244, 252);
            this.SkipButton.Name = "SkipButton";
            this.SkipButton.Size = new System.Drawing.Size(75, 23);
            this.SkipButton.TabIndex = 6;
            this.SkipButton.Text = "Skip";
            this.SkipButton.UseVisualStyleBackColor = false;
            // 
            // IgnoreButton
            // 
            this.IgnoreButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.IgnoreButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.IgnoreButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.IgnoreButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.IgnoreButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IgnoreButton.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IgnoreButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.IgnoreButton.Location = new System.Drawing.Point(163, 252);
            this.IgnoreButton.Name = "IgnoreButton";
            this.IgnoreButton.Size = new System.Drawing.Size(75, 23);
            this.IgnoreButton.TabIndex = 2;
            this.IgnoreButton.Text = "Ignore";
            this.IgnoreButton.UseVisualStyleBackColor = false;
            // 
            // UpdatePopup
            // 
            this.AcceptButton = this.UpdateButton;
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuPopup;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(36)))), ((int)(((byte)(45)))));
            this.CancelButton = this.IgnoreButton;
            this.ClientSize = new System.Drawing.Size(398, 292);
            this.Controls.Add(this.UpdateText);
            this.Controls.Add(this.UpdateIcon);
            this.Controls.Add(this.ChangelogBox);
            this.Controls.Add(this.SkipButton);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.IgnoreButton);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdatePopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update Available - SBRW Launcher";
            this.ChangelogBox.ResumeLayout(false);
            this.ChangelogBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ChangelogBox;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.TextBox ChangelogText;
        private System.Windows.Forms.PictureBox UpdateIcon;
        private System.Windows.Forms.Label UpdateText;
        private System.Windows.Forms.Button SkipButton;
        private System.Windows.Forms.Button IgnoreButton;
    }
}