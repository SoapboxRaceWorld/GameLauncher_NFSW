namespace SBRW.Launcher.App.UI_Forms.Welcome_Screen
{
    partial class Screen_Welcome
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
            this.Label_Introduction = new System.Windows.Forms.Label();
            this.Label_CDN_Status_List = new System.Windows.Forms.Label();
            this.Label_Game_Language = new System.Windows.Forms.Label();
            this.ComboBox_Game_Language = new System.Windows.Forms.ComboBox();
            this.Label_CDN_Source = new System.Windows.Forms.Label();
            this.Button_API_Bypass = new System.Windows.Forms.Button();
            this.Button_Save = new System.Windows.Forms.Button();
            this.Label_Version = new System.Windows.Forms.Label();
            this.ComboBox_CDN_Sources = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // Label_Introduction
            // 
            this.Label_Introduction.BackColor = System.Drawing.Color.Transparent;
            this.Label_Introduction.ForeColor = System.Drawing.Color.Black;
            this.Label_Introduction.Location = new System.Drawing.Point(9, 6);
            this.Label_Introduction.Name = "Label_Introduction";
            this.Label_Introduction.Size = new System.Drawing.Size(392, 80);
            this.Label_Introduction.TabIndex = 25;
            this.Label_Introduction.Text = "Checking API Status";
            this.Label_Introduction.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Label_CDN_Status_List
            // 
            this.Label_CDN_Status_List.BackColor = System.Drawing.Color.Transparent;
            this.Label_CDN_Status_List.ForeColor = System.Drawing.Color.Black;
            this.Label_CDN_Status_List.Location = new System.Drawing.Point(140, 95);
            this.Label_CDN_Status_List.Name = "Label_CDN_Status_List";
            this.Label_CDN_Status_List.Size = new System.Drawing.Size(129, 14);
            this.Label_CDN_Status_List.TabIndex = 26;
            this.Label_CDN_Status_List.Text = "API Status - Pinging";
            this.Label_CDN_Status_List.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_Game_Language
            // 
            this.Label_Game_Language.BackColor = System.Drawing.Color.Transparent;
            this.Label_Game_Language.ForeColor = System.Drawing.Color.Black;
            this.Label_Game_Language.Location = new System.Drawing.Point(117, 127);
            this.Label_Game_Language.Name = "Label_Game_Language";
            this.Label_Game_Language.Size = new System.Drawing.Size(175, 14);
            this.Label_Game_Language.TabIndex = 27;
            this.Label_Game_Language.Text = "Select Game Language:";
            this.Label_Game_Language.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ComboBox_Game_Language
            // 
            this.ComboBox_Game_Language.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ComboBox_Game_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Game_Language.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboBox_Game_Language.FormattingEnabled = true;
            this.ComboBox_Game_Language.Location = new System.Drawing.Point(95, 150);
            this.ComboBox_Game_Language.Name = "ComboBox_Game_Language";
            this.ComboBox_Game_Language.Size = new System.Drawing.Size(220, 24);
            this.ComboBox_Game_Language.TabIndex = 28;
            // 
            // Label_CDN_Source
            // 
            this.Label_CDN_Source.BackColor = System.Drawing.Color.Transparent;
            this.Label_CDN_Source.ForeColor = System.Drawing.Color.Black;
            this.Label_CDN_Source.Location = new System.Drawing.Point(116, 187);
            this.Label_CDN_Source.Name = "Label_CDN_Source";
            this.Label_CDN_Source.Size = new System.Drawing.Size(177, 14);
            this.Label_CDN_Source.TabIndex = 29;
            this.Label_CDN_Source.Text = "CDN / Download Source:";
            this.Label_CDN_Source.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Button_API_Bypass
            // 
            this.Button_API_Bypass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_API_Bypass.Location = new System.Drawing.Point(143, 255);
            this.Button_API_Bypass.Name = "Button_API_Bypass";
            this.Button_API_Bypass.Size = new System.Drawing.Size(126, 26);
            this.Button_API_Bypass.TabIndex = 31;
            this.Button_API_Bypass.Text = "Manual Bypass";
            this.Button_API_Bypass.UseVisualStyleBackColor = true;
            // 
            // Button_Save
            // 
            this.Button_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Save.Location = new System.Drawing.Point(5, 287);
            this.Button_Save.Name = "Button_Save";
            this.Button_Save.Size = new System.Drawing.Size(400, 32);
            this.Button_Save.TabIndex = 32;
            this.Button_Save.Text = "Save Settings and Select Download / Install Location";
            this.Button_Save.UseVisualStyleBackColor = true;
            // 
            // Label_Version
            // 
            this.Label_Version.BackColor = System.Drawing.Color.Transparent;
            this.Label_Version.ForeColor = System.Drawing.Color.Black;
            this.Label_Version.Location = new System.Drawing.Point(123, 337);
            this.Label_Version.Name = "Label_Version";
            this.Label_Version.Size = new System.Drawing.Size(164, 14);
            this.Label_Version.TabIndex = 33;
            this.Label_Version.Text = "Version: vXX.XX.XX.XXXX";
            this.Label_Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ComboBox_CDN_Sources
            // 
            this.ComboBox_CDN_Sources.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ComboBox_CDN_Sources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_CDN_Sources.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboBox_CDN_Sources.FormattingEnabled = true;
            this.ComboBox_CDN_Sources.Location = new System.Drawing.Point(95, 210);
            this.ComboBox_CDN_Sources.Name = "ComboBox_CDN_Sources";
            this.ComboBox_CDN_Sources.Size = new System.Drawing.Size(220, 24);
            this.ComboBox_CDN_Sources.TabIndex = 34;
            // 
            // Screen_Welcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 364);
            this.Controls.Add(this.ComboBox_CDN_Sources);
            this.Controls.Add(this.Label_Version);
            this.Controls.Add(this.Button_Save);
            this.Controls.Add(this.Button_API_Bypass);
            this.Controls.Add(this.Label_CDN_Source);
            this.Controls.Add(this.ComboBox_Game_Language);
            this.Controls.Add(this.Label_Game_Language);
            this.Controls.Add(this.Label_CDN_Status_List);
            this.Controls.Add(this.Label_Introduction);
            this.Name = "Screen_Welcome";
            this.Text = "Welcome - SBRW Laucher";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Label_Introduction;
        private System.Windows.Forms.Label Label_CDN_Status_List;
        private System.Windows.Forms.Label Label_Game_Language;
        private System.Windows.Forms.ComboBox ComboBox_Game_Language;
        private System.Windows.Forms.Label Label_CDN_Source;
        private System.Windows.Forms.Button Button_API_Bypass;
        private System.Windows.Forms.Button Button_Save;
        private System.Windows.Forms.Label Label_Version;
        private System.Windows.Forms.ComboBox ComboBox_CDN_Sources;
    }
}