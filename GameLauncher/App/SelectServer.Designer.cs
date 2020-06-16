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
            this.ServerListRenderer = new System.Windows.Forms.ListView();
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
            this.ServerListRenderer.Size = new System.Drawing.Size(546, 276);
            this.ServerListRenderer.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ServerListRenderer.TabIndex = 0;
            this.ServerListRenderer.UseCompatibleStateImageBehavior = false;
            // 
            // SelectServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 300);
            this.Controls.Add(this.ServerListRenderer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectServer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please select the server before continuing...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ServerListRenderer;
    }
}