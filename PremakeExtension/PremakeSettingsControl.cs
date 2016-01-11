using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PremakeExtension
{
    public partial class PremakeSettingsControl : UserControl
    {
        internal PremakeSettings OptionsPage;

        public PremakeSettingsControl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            m_premakePathTextbox.Text = OptionsPage.PremakePath;
            m_forceGlobalSetting.Checked = OptionsPage.UseGlobalSetting;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            OptionsPage.PremakePath = m_premakePathTextbox.Text;
        }

        private void m_forceGlobalSetting_CheckedChanged(object sender, EventArgs e)
        {
            OptionsPage.UseGlobalSetting = m_forceGlobalSetting.Checked;
        }

        private void BrowseClick(object sender, EventArgs e)
        {
            // get the old path as a starting point.
            var oldDir = "c:\\";
            if (!string.IsNullOrEmpty(OptionsPage.PremakePath))
            {
                oldDir = Path.GetDirectoryName(OptionsPage.PremakePath) ?? "c:\\";
            }

            var dlg = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = false,
                InitialDirectory = oldDir,
                Filter = "exe files (*.exe)|*.exe",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_premakePathTextbox.Text = dlg.FileName;
                OptionsPage.PremakePath = dlg.FileName;
            }
        }
    }
}
