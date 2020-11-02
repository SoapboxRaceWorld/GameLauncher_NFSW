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
            this.btnAddServer = new System.Windows.Forms.Button();
            this.btnSelectServer = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
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
            this.ServerListRenderer.Size = new System.Drawing.Size(546, 230);
            this.ServerListRenderer.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ServerListRenderer.TabIndex = 0;
            this.ServerListRenderer.UseCompatibleStateImageBehavior = false;
            // 
            // loading
            // 
            this.loading.AutoSize = true;
            this.loading.Location = new System.Drawing.Point(220, 253);
            this.loading.Name = "loading";
            this.loading.Size = new System.Drawing.Size(91, 13);
            this.loading.TabIndex = 4;
            this.loading.Text = "Loading servers...";
            // 
            // btnAddServer
            // 
            this.btnAddServer.Location = new System.Drawing.Point(15, 248);
            this.btnAddServer.Name = "btnAddServer";
            this.btnAddServer.Size = new System.Drawing.Size(75, 23);
            this.btnAddServer.TabIndex = 5;
            this.btnAddServer.Text = "Add Server";
            this.btnAddServer.UseVisualStyleBackColor = true;
            this.btnAddServer.Click += new System.EventHandler(this.BtnAddServer_Click);
            // 
            // btnSelectServer
            // 
            this.btnSelectServer.Location = new System.Drawing.Point(403, 248);
            this.btnSelectServer.Name = "btnSelectServer";
            this.btnSelectServer.Size = new System.Drawing.Size(75, 23);
            this.btnSelectServer.TabIndex = 6;
            this.btnSelectServer.Text = "Select";
            this.btnSelectServer.UseVisualStyleBackColor = true;
            this.btnSelectServer.Click += new System.EventHandler(this.BtnSelectServer_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(483, 248);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // SelectServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 275);
            this.Controls.Add(this.btnAddServer);
            this.Controls.Add(this.btnSelectServer);
            this.Controls.Add(this.btnClose);
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
        private System.Windows.Forms.Button btnAddServer;
        private System.Windows.Forms.Button btnSelectServer;
        private System.Windows.Forms.Button btnClose;
    }
}