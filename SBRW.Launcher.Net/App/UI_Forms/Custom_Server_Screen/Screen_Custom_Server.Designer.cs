namespace SBRW.Launcher.App.UI_Forms.Custom_Server_Screen
{
    partial class Screen_Custom_Server
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
            this.ListView_Server_List = new System.Windows.Forms.ListView();
            this.Button_Server_Add = new System.Windows.Forms.Button();
            this.Button_Server_Remove = new System.Windows.Forms.Button();
            this.Button_Server_Select = new System.Windows.Forms.Button();
            this.Button_Close = new System.Windows.Forms.Button();
            this.Label_Loading = new System.Windows.Forms.Label();
            this.Label_Version = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ListView_Server_List
            // 
            this.ListView_Server_List.Location = new System.Drawing.Point(13, 13);
            this.ListView_Server_List.MultiSelect = false;
            this.ListView_Server_List.Name = "ListView_Server_List";
            this.ListView_Server_List.Size = new System.Drawing.Size(544, 229);
            this.ListView_Server_List.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ListView_Server_List.TabIndex = 0;
            this.ListView_Server_List.UseCompatibleStateImageBehavior = false;
            // 
            // Button_Server_Add
            // 
            this.Button_Server_Add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Server_Add.Location = new System.Drawing.Point(15, 247);
            this.Button_Server_Add.Name = "Button_Server_Add";
            this.Button_Server_Add.Size = new System.Drawing.Size(96, 23);
            this.Button_Server_Add.TabIndex = 1;
            this.Button_Server_Add.Text = "Add Server";
            this.Button_Server_Add.UseVisualStyleBackColor = true;
            // 
            // Button_Server_Remove
            // 
            this.Button_Server_Remove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Server_Remove.Location = new System.Drawing.Point(117, 247);
            this.Button_Server_Remove.Name = "Button_Server_Remove";
            this.Button_Server_Remove.Size = new System.Drawing.Size(75, 23);
            this.Button_Server_Remove.TabIndex = 2;
            this.Button_Server_Remove.Text = "Remove";
            this.Button_Server_Remove.UseVisualStyleBackColor = true;
            // 
            // Button_Server_Select
            // 
            this.Button_Server_Select.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Server_Select.Location = new System.Drawing.Point(399, 247);
            this.Button_Server_Select.Name = "Button_Server_Select";
            this.Button_Server_Select.Size = new System.Drawing.Size(75, 23);
            this.Button_Server_Select.TabIndex = 3;
            this.Button_Server_Select.Text = "Select";
            this.Button_Server_Select.UseVisualStyleBackColor = true;
            // 
            // Button_Close
            // 
            this.Button_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Close.Location = new System.Drawing.Point(480, 247);
            this.Button_Close.Name = "Button_Close";
            this.Button_Close.Size = new System.Drawing.Size(75, 23);
            this.Button_Close.TabIndex = 4;
            this.Button_Close.Text = "Close";
            this.Button_Close.UseVisualStyleBackColor = true;
            // 
            // Label_Loading
            // 
            this.Label_Loading.BackColor = System.Drawing.Color.Transparent;
            this.Label_Loading.ForeColor = System.Drawing.Color.Black;
            this.Label_Loading.Location = new System.Drawing.Point(234, 242);
            this.Label_Loading.Name = "Label_Loading";
            this.Label_Loading.Size = new System.Drawing.Size(123, 14);
            this.Label_Loading.TabIndex = 25;
            this.Label_Loading.Text = "Loading Servers ...";
            this.Label_Loading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_Version
            // 
            this.Label_Version.BackColor = System.Drawing.Color.Transparent;
            this.Label_Version.ForeColor = System.Drawing.Color.Black;
            this.Label_Version.Location = new System.Drawing.Point(213, 260);
            this.Label_Version.Name = "Label_Version";
            this.Label_Version.Size = new System.Drawing.Size(164, 14);
            this.Label_Version.TabIndex = 26;
            this.Label_Version.Text = "Version : vXX.XX.XX.XXXX";
            this.Label_Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Screen_Custom_Server
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(570, 275);
            this.Controls.Add(this.Label_Version);
            this.Controls.Add(this.Label_Loading);
            this.Controls.Add(this.Button_Close);
            this.Controls.Add(this.Button_Server_Select);
            this.Controls.Add(this.Button_Server_Remove);
            this.Controls.Add(this.Button_Server_Add);
            this.Controls.Add(this.ListView_Server_List);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Screen_Custom_Server";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please Select a Server - SBRW Launcher";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ListView_Server_List;
        private System.Windows.Forms.Button Button_Server_Add;
        private System.Windows.Forms.Button Button_Server_Remove;
        private System.Windows.Forms.Button Button_Server_Select;
        private System.Windows.Forms.Button Button_Close;
        private System.Windows.Forms.Label Label_Loading;
        private System.Windows.Forms.Label Label_Version;
    }
}