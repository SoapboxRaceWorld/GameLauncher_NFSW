using System.Windows.Forms;
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
            this.version = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ServerListRenderer
            // 
            this.ServerListRenderer.AutoArrange = false;
            this.ServerListRenderer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.ServerListRenderer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.ServerListRenderer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ServerListRenderer.HideSelection = false;
            this.ServerListRenderer.Location = new System.Drawing.Point(13, 13);
            this.ServerListRenderer.MultiSelect = false;
            this.ServerListRenderer.Name = "ServerListRenderer";
            this.ServerListRenderer.Size = new System.Drawing.Size(544, 229);
            this.ServerListRenderer.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ServerListRenderer.TabIndex = 0;
            this.ServerListRenderer.UseCompatibleStateImageBehavior = false;
            // 
            // loading
            // 
            this.loading.AutoSize = true;
            this.loading.Location = new System.Drawing.Point(232, 242);
            this.loading.Name = "loading";
            this.loading.Size = new System.Drawing.Size(91, 13);
            this.loading.TabIndex = 4;
            this.loading.Text = "Loading servers...";
            // 
            // btnAddServer
            // 
            this.btnAddServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.btnAddServer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.btnAddServer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.btnAddServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAddServer.Location = new System.Drawing.Point(15, 247);
            this.btnAddServer.Name = "btnAddServer";
            this.btnAddServer.Size = new System.Drawing.Size(75, 23);
            this.btnAddServer.TabIndex = 8;
            this.btnAddServer.Text = "Add Server";
            this.btnAddServer.UseVisualStyleBackColor = false;
            this.btnAddServer.Click += new System.EventHandler(this.BtnAddServer_Click);
            // 
            // btnSelectServer
            // 
            this.btnSelectServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.btnSelectServer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.btnSelectServer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.btnSelectServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSelectServer.Location = new System.Drawing.Point(399, 247);
            this.btnSelectServer.Name = "btnSelectServer";
            this.btnSelectServer.Size = new System.Drawing.Size(75, 23);
            this.btnSelectServer.TabIndex = 6;
            this.btnSelectServer.Text = "Select";
            this.btnSelectServer.UseVisualStyleBackColor = false;
            this.btnSelectServer.Click += new System.EventHandler(this.BtnSelectServer_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnClose.Location = new System.Drawing.Point(480, 247);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // version
            // 
            this.version.AutoSize = true;
            this.version.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.version.Location = new System.Drawing.Point(232, 257);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(90, 13);
            this.version.TabIndex = 9;
            this.version.Text = "Version : v" + Application.ProductVersion;
            this.version.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SelectServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(36)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(570, 275);
            this.Controls.Add(this.version);
            this.Controls.Add(this.btnAddServer);
            this.Controls.Add(this.btnSelectServer);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.loading);
            this.Controls.Add(this.ServerListRenderer);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher - Please select a Server";
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
        private System.Windows.Forms.Label version;
    }
}