using System.Windows.Forms;
namespace GameLauncher.App.UI_Forms.SelectServer_Screen
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
            this.Loading = new System.Windows.Forms.Label();
            this.BtnAddServer = new System.Windows.Forms.Button();
            this.BtnSelectServer = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.Version = new System.Windows.Forms.Label();
            this.BtnRemoveServer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ServerListRenderer
            // 
            this.ServerListRenderer.AutoArrange = false;
            this.ServerListRenderer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.ServerListRenderer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ServerListRenderer.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerListRenderer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.ServerListRenderer.GridLines = true;
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
            // Loading
            // 
            this.Loading.BackColor = System.Drawing.Color.Transparent;
            this.Loading.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Loading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.Loading.Location = new System.Drawing.Point(234, 242);
            this.Loading.Name = "Loading";
            this.Loading.Size = new System.Drawing.Size(123, 14);
            this.Loading.TabIndex = 4;
            this.Loading.Text = "Loading Servers ...";
            // 
            // BtnAddServer
            // 
            this.BtnAddServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.BtnAddServer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.BtnAddServer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.BtnAddServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnAddServer.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnAddServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BtnAddServer.Location = new System.Drawing.Point(15, 247);
            this.BtnAddServer.Name = "BtnAddServer";
            this.BtnAddServer.Size = new System.Drawing.Size(96, 23);
            this.BtnAddServer.TabIndex = 4;
            this.BtnAddServer.Text = "Add Server";
            this.BtnAddServer.UseVisualStyleBackColor = false;
            this.BtnAddServer.Click += new System.EventHandler(this.BtnAddServer_Click);
            // 
            // BtnSelectServer
            // 
            this.BtnSelectServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.BtnSelectServer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.BtnSelectServer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.BtnSelectServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSelectServer.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSelectServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BtnSelectServer.Location = new System.Drawing.Point(399, 247);
            this.BtnSelectServer.Name = "BtnSelectServer";
            this.BtnSelectServer.Size = new System.Drawing.Size(75, 23);
            this.BtnSelectServer.TabIndex = 2;
            this.BtnSelectServer.Text = "Select";
            this.BtnSelectServer.UseVisualStyleBackColor = false;
            // 
            // BtnClose
            // 
            this.BtnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.BtnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnClose.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BtnClose.Location = new System.Drawing.Point(480, 247);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 8;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // Version
            // 
            this.Version.BackColor = System.Drawing.Color.Transparent;
            this.Version.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Version.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Version.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Version.Location = new System.Drawing.Point(213, 260);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(164, 14);
            this.Version.TabIndex = 9;
            this.Version.Text = "Version : vXX.XX.XX.XXXX";
            this.Version.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // BtnRemoveServer
            // 
            this.BtnRemoveServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.BtnRemoveServer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.BtnRemoveServer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.BtnRemoveServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnRemoveServer.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnRemoveServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BtnRemoveServer.Location = new System.Drawing.Point(117, 247);
            this.BtnRemoveServer.Name = "BtnRemoveServer";
            this.BtnRemoveServer.Size = new System.Drawing.Size(75, 23);
            this.BtnRemoveServer.TabIndex = 6;
            this.BtnRemoveServer.Text = "Remove";
            this.BtnRemoveServer.UseVisualStyleBackColor = false;
            // 
            // SelectServer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(36)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(570, 275);
            this.Controls.Add(this.BtnRemoveServer);
            this.Controls.Add(this.Version);
            this.Controls.Add(this.BtnAddServer);
            this.Controls.Add(this.BtnSelectServer);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.Loading);
            this.Controls.Add(this.ServerListRenderer);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please Select a Server - SBRW Launcher";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ServerListRenderer;
        private System.Windows.Forms.Label Loading;
        private System.Windows.Forms.Button BtnAddServer;
        private System.Windows.Forms.Button BtnSelectServer;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Label Version;
        private Button BtnRemoveServer;
    }
}