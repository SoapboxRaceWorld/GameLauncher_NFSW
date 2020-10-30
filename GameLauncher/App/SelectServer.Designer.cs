namespace GameLauncher.App
{
    partial class SelectServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectServer));
            this.ServerListRenderer = new System.Windows.Forms.ListView();
            this.loading = new System.Windows.Forms.Label();
            this.BtnAddServer = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ServerListRenderer
            // 
            this.ServerListRenderer.AutoArrange = false;
            this.ServerListRenderer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ServerListRenderer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ServerListRenderer.HideSelection = false;
            this.ServerListRenderer.Location = new System.Drawing.Point(12, 12);
            this.ServerListRenderer.MultiSelect = false;
            this.ServerListRenderer.Name = "ServerListRenderer";
            this.ServerListRenderer.Size = new System.Drawing.Size(546, 256);
            this.ServerListRenderer.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ServerListRenderer.TabIndex = 0;
            this.ServerListRenderer.UseCompatibleStateImageBehavior = false;
            // 
            // loading
            // 
            this.loading.AutoSize = true;
            this.loading.Location = new System.Drawing.Point(12, 278);
            this.loading.Name = "loading";
            this.loading.Size = new System.Drawing.Size(91, 13);
            this.loading.TabIndex = 4;
            this.loading.Text = "Loading servers...";
            // 
            // BtnAddServer
            // 
            this.BtnAddServer.Location = new System.Drawing.Point(402, 273);
            this.BtnAddServer.Name = "BtnAddServer";
            this.BtnAddServer.Size = new System.Drawing.Size(75, 23);
            this.BtnAddServer.TabIndex = 6;
            this.BtnAddServer.Text = "Add Server";
            this.BtnAddServer.UseVisualStyleBackColor = true;
            this.BtnAddServer.Click += new System.EventHandler(this.BtnAddServer_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(483, 273);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 5;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // SelectServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 300);
            this.Controls.Add(this.BtnAddServer);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.loading);
            this.Controls.Add(this.ServerListRenderer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectServer";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please select a server before continuing...";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView ServerListRenderer;
        private System.Windows.Forms.Label loading;
        private System.Windows.Forms.Button BtnAddServer;
        private System.Windows.Forms.Button BtnClose;
    }
}