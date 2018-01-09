namespace GameLauncher.App
{
    partial class AddServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddServer));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.serverNameLabel = new System.Windows.Forms.Label();
            this.serverName = new System.Windows.Forms.TextBox();
            this.serverAddress = new System.Windows.Forms.TextBox();
            this.serverAddressLabel = new System.Windows.Forms.Label();
            this.error = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(312, 106);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(231, 106);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // serverNameLabel
            // 
            this.serverNameLabel.AutoSize = true;
            this.serverNameLabel.Location = new System.Drawing.Point(13, 13);
            this.serverNameLabel.Name = "serverNameLabel";
            this.serverNameLabel.Size = new System.Drawing.Size(70, 13);
            this.serverNameLabel.TabIndex = 2;
            this.serverNameLabel.Text = "Server name:";
            this.serverNameLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // serverName
            // 
            this.serverName.Location = new System.Drawing.Point(16, 29);
            this.serverName.Name = "serverName";
            this.serverName.Size = new System.Drawing.Size(371, 20);
            this.serverName.TabIndex = 3;
            // 
            // serverAddress
            // 
            this.serverAddress.Location = new System.Drawing.Point(16, 78);
            this.serverAddress.Name = "serverAddress";
            this.serverAddress.Size = new System.Drawing.Size(371, 20);
            this.serverAddress.TabIndex = 5;
            // 
            // serverAddressLabel
            // 
            this.serverAddressLabel.AutoSize = true;
            this.serverAddressLabel.Location = new System.Drawing.Point(13, 62);
            this.serverAddressLabel.Name = "serverAddressLabel";
            this.serverAddressLabel.Size = new System.Drawing.Size(81, 13);
            this.serverAddressLabel.TabIndex = 4;
            this.serverAddressLabel.Text = "Server address:";
            this.serverAddressLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // error
            // 
            this.error.AutoSize = true;
            this.error.ForeColor = System.Drawing.Color.Red;
            this.error.Location = new System.Drawing.Point(13, 111);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(111, 13);
            this.error.TabIndex = 6;
            this.error.Text = "Please check better...";
            this.error.Visible = false;
            // 
            // AddServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 141);
            this.Controls.Add(this.error);
            this.Controls.Add(this.serverAddress);
            this.Controls.Add(this.serverAddressLabel);
            this.Controls.Add(this.serverName);
            this.Controls.Add(this.serverNameLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddServer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Server";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label serverNameLabel;
        private System.Windows.Forms.TextBox serverName;
        private System.Windows.Forms.TextBox serverAddress;
        private System.Windows.Forms.Label serverAddressLabel;
        private System.Windows.Forms.Label error;
    }
}