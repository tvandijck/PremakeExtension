namespace PremakeExtension
{
    partial class PremakeSettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_premakePathTextbox = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.m_forceGlobalSetting = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // m_premakePathTextbox
            // 
            this.m_premakePathTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_premakePathTextbox.Location = new System.Drawing.Point(3, 27);
            this.m_premakePathTextbox.Name = "m_premakePathTextbox";
            this.m_premakePathTextbox.Size = new System.Drawing.Size(374, 20);
            this.m_premakePathTextbox.TabIndex = 0;
            this.m_premakePathTextbox.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(383, 25);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(34, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.BrowseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Premake5 Executable Path:";
            // 
            // m_forceGlobalSetting
            // 
            this.m_forceGlobalSetting.AutoSize = true;
            this.m_forceGlobalSetting.Location = new System.Drawing.Point(3, 53);
            this.m_forceGlobalSetting.Name = "m_forceGlobalSetting";
            this.m_forceGlobalSetting.Size = new System.Drawing.Size(172, 17);
            this.m_forceGlobalSetting.TabIndex = 3;
            this.m_forceGlobalSetting.Text = "Force usage of the above path";
            this.m_forceGlobalSetting.UseVisualStyleBackColor = true;
            this.m_forceGlobalSetting.CheckedChanged += new System.EventHandler(this.m_forceGlobalSetting_CheckedChanged);
            // 
            // PremakeSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_forceGlobalSetting);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.m_premakePathTextbox);
            this.Name = "PremakeSettingsControl";
            this.Size = new System.Drawing.Size(420, 99);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox m_premakePathTextbox;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox m_forceGlobalSetting;
    }
}
