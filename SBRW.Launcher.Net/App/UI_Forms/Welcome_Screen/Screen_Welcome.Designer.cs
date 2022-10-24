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
            this.components = new System.ComponentModel.Container();
            this.Label_Introduction = new System.Windows.Forms.Label();
            this.Label_CDN_Status_List = new System.Windows.Forms.Label();
            this.Label_Game_Language = new System.Windows.Forms.Label();
            this.ComboBox_Game_Language = new System.Windows.Forms.ComboBox();
            this.Label_CDN_Source = new System.Windows.Forms.Label();
            this.Button_Save = new System.Windows.Forms.Button();
            this.Label_Version = new System.Windows.Forms.Label();
            this.Button_CDN_Sources = new System.Windows.Forms.Button();
            this.CheckBox_LZMA_Downloader = new System.Windows.Forms.CheckBox();
            this.Label_Download_Method = new System.Windows.Forms.Label();
            this.ToolTip_Hover = new System.Windows.Forms.ToolTip(this.components);
            this.CheckBox_Alt_WebCalls = new System.Windows.Forms.CheckBox();
            this.Label_WebClient_Settings = new System.Windows.Forms.Label();
            this.NumericUpDown_WebClient_Timeout = new System.Windows.Forms.NumericUpDown();
            this.Label_WebClient_Timeout = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_WebClient_Timeout)).BeginInit();
            this.SuspendLayout();
            // 
            // Label_Introduction
            // 
            this.Label_Introduction.BackColor = System.Drawing.Color.Transparent;
            this.Label_Introduction.ForeColor = System.Drawing.Color.Black;
            this.Label_Introduction.Location = new System.Drawing.Point(51, 9);
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
            this.Label_CDN_Status_List.Location = new System.Drawing.Point(135, 98);
            this.Label_CDN_Status_List.Name = "Label_CDN_Status_List";
            this.Label_CDN_Status_List.Size = new System.Drawing.Size(222, 16);
            this.Label_CDN_Status_List.TabIndex = 26;
            this.Label_CDN_Status_List.Text = "API Status - Pinging";
            this.Label_CDN_Status_List.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_Game_Language
            // 
            this.Label_Game_Language.BackColor = System.Drawing.Color.Transparent;
            this.Label_Game_Language.ForeColor = System.Drawing.Color.Black;
            this.Label_Game_Language.Location = new System.Drawing.Point(14, 147);
            this.Label_Game_Language.Name = "Label_Game_Language";
            this.Label_Game_Language.Size = new System.Drawing.Size(220, 16);
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
            this.ComboBox_Game_Language.Location = new System.Drawing.Point(14, 171);
            this.ComboBox_Game_Language.Name = "ComboBox_Game_Language";
            this.ComboBox_Game_Language.Size = new System.Drawing.Size(220, 24);
            this.ComboBox_Game_Language.TabIndex = 28;
            // 
            // Label_CDN_Source
            // 
            this.Label_CDN_Source.BackColor = System.Drawing.Color.Transparent;
            this.Label_CDN_Source.ForeColor = System.Drawing.Color.Black;
            this.Label_CDN_Source.Location = new System.Drawing.Point(14, 211);
            this.Label_CDN_Source.Name = "Label_CDN_Source";
            this.Label_CDN_Source.Size = new System.Drawing.Size(220, 14);
            this.Label_CDN_Source.TabIndex = 29;
            this.Label_CDN_Source.Text = "CDN / Download Source:";
            this.Label_CDN_Source.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Button_Save
            // 
            this.Button_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Save.Location = new System.Drawing.Point(51, 300);
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
            this.Label_Version.Location = new System.Drawing.Point(170, 349);
            this.Label_Version.Name = "Label_Version";
            this.Label_Version.Size = new System.Drawing.Size(164, 14);
            this.Label_Version.TabIndex = 33;
            this.Label_Version.Text = "Version: XX.XX.XX.XXXX";
            this.Label_Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Button_CDN_Sources
            // 
            this.Button_CDN_Sources.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_CDN_Sources.Location = new System.Drawing.Point(14, 234);
            this.Button_CDN_Sources.Name = "Button_CDN_Sources";
            this.Button_CDN_Sources.Size = new System.Drawing.Size(220, 26);
            this.Button_CDN_Sources.TabIndex = 35;
            this.Button_CDN_Sources.Text = "CDN Selector Screen";
            this.Button_CDN_Sources.UseVisualStyleBackColor = true;
            // 
            // CheckBox_LZMA_Downloader
            // 
            this.CheckBox_LZMA_Downloader.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_LZMA_Downloader.ForeColor = System.Drawing.Color.White;
            this.CheckBox_LZMA_Downloader.Location = new System.Drawing.Point(272, 147);
            this.CheckBox_LZMA_Downloader.Name = "CheckBox_LZMA_Downloader";
            this.CheckBox_LZMA_Downloader.Size = new System.Drawing.Size(222, 18);
            this.CheckBox_LZMA_Downloader.TabIndex = 81;
            this.CheckBox_LZMA_Downloader.Text = "Enable LZMA Downloader";
            this.CheckBox_LZMA_Downloader.UseVisualStyleBackColor = false;
            // 
            // Label_Download_Method
            // 
            this.Label_Download_Method.BackColor = System.Drawing.Color.Transparent;
            this.Label_Download_Method.ForeColor = System.Drawing.Color.Black;
            this.Label_Download_Method.Location = new System.Drawing.Point(272, 129);
            this.Label_Download_Method.Name = "Label_Download_Method";
            this.Label_Download_Method.Size = new System.Drawing.Size(222, 15);
            this.Label_Download_Method.TabIndex = 83;
            this.Label_Download_Method.Text = "Download Method:";
            this.Label_Download_Method.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CheckBox_Alt_WebCalls
            // 
            this.CheckBox_Alt_WebCalls.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox_Alt_WebCalls.ForeColor = System.Drawing.Color.White;
            this.CheckBox_Alt_WebCalls.Location = new System.Drawing.Point(272, 209);
            this.CheckBox_Alt_WebCalls.Name = "CheckBox_Alt_WebCalls";
            this.CheckBox_Alt_WebCalls.Size = new System.Drawing.Size(222, 18);
            this.CheckBox_Alt_WebCalls.TabIndex = 84;
            this.CheckBox_Alt_WebCalls.Text = "Enable Alternative WebCalls";
            this.CheckBox_Alt_WebCalls.UseVisualStyleBackColor = false;
            // 
            // Label_WebClient_Settings
            // 
            this.Label_WebClient_Settings.BackColor = System.Drawing.Color.Transparent;
            this.Label_WebClient_Settings.ForeColor = System.Drawing.Color.Black;
            this.Label_WebClient_Settings.Location = new System.Drawing.Point(272, 177);
            this.Label_WebClient_Settings.Name = "Label_WebClient_Settings";
            this.Label_WebClient_Settings.Size = new System.Drawing.Size(222, 18);
            this.Label_WebClient_Settings.TabIndex = 85;
            this.Label_WebClient_Settings.Text = "WebClient Settings:";
            this.Label_WebClient_Settings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NumericUpDown_WebClient_Timeout
            // 
            this.NumericUpDown_WebClient_Timeout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.NumericUpDown_WebClient_Timeout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NumericUpDown_WebClient_Timeout.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.NumericUpDown_WebClient_Timeout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.NumericUpDown_WebClient_Timeout.Location = new System.Drawing.Point(272, 258);
            this.NumericUpDown_WebClient_Timeout.Maximum = new decimal(new int[] {
            179,
            0,
            0,
            0});
            this.NumericUpDown_WebClient_Timeout.Name = "NumericUpDown_WebClient_Timeout";
            this.NumericUpDown_WebClient_Timeout.Size = new System.Drawing.Size(61, 17);
            this.NumericUpDown_WebClient_Timeout.TabIndex = 147;
            this.NumericUpDown_WebClient_Timeout.Tag = "WebClientTimeNumeric";
            this.NumericUpDown_WebClient_Timeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label_WebClient_Timeout
            // 
            this.Label_WebClient_Timeout.BackColor = System.Drawing.Color.Transparent;
            this.Label_WebClient_Timeout.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Label_WebClient_Timeout.ForeColor = System.Drawing.Color.DarkGray;
            this.Label_WebClient_Timeout.Location = new System.Drawing.Point(272, 240);
            this.Label_WebClient_Timeout.Name = "Label_WebClient_Timeout";
            this.Label_WebClient_Timeout.Size = new System.Drawing.Size(222, 15);
            this.Label_WebClient_Timeout.TabIndex = 146;
            this.Label_WebClient_Timeout.Text = "Web Client Timeout:";
            this.Label_WebClient_Timeout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Screen_Welcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 368);
            this.Controls.Add(this.NumericUpDown_WebClient_Timeout);
            this.Controls.Add(this.Label_WebClient_Timeout);
            this.Controls.Add(this.Label_WebClient_Settings);
            this.Controls.Add(this.CheckBox_Alt_WebCalls);
            this.Controls.Add(this.Label_Download_Method);
            this.Controls.Add(this.CheckBox_LZMA_Downloader);
            this.Controls.Add(this.Button_CDN_Sources);
            this.Controls.Add(this.Label_Version);
            this.Controls.Add(this.Button_Save);
            this.Controls.Add(this.Label_CDN_Source);
            this.Controls.Add(this.ComboBox_Game_Language);
            this.Controls.Add(this.Label_Game_Language);
            this.Controls.Add(this.Label_CDN_Status_List);
            this.Controls.Add(this.Label_Introduction);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Screen_Welcome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Welcome - SBRW Laucher";
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_WebClient_Timeout)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label Label_Introduction;
        public System.Windows.Forms.Label Label_CDN_Status_List;
        public System.Windows.Forms.Label Label_Game_Language;
        public System.Windows.Forms.ComboBox ComboBox_Game_Language;
        public System.Windows.Forms.Label Label_CDN_Source;
        public System.Windows.Forms.Label Label_Version;
        public System.Windows.Forms.Button Button_CDN_Sources;
        public System.Windows.Forms.Button Button_Save;
        public System.Windows.Forms.CheckBox CheckBox_LZMA_Downloader;
        public System.Windows.Forms.Label Label_Download_Method;
        private System.Windows.Forms.ToolTip ToolTip_Hover;
        private System.Windows.Forms.CheckBox CheckBox_Alt_WebCalls;
        public System.Windows.Forms.Label Label_WebClient_Settings;
        private System.Windows.Forms.NumericUpDown NumericUpDown_WebClient_Timeout;
        private System.Windows.Forms.Label Label_WebClient_Timeout;
    }
}