using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace PremakeExtension
{
    [Guid("C7552DEC-5430-4D95-B497-98D98B528A09")]
    public class PremakeSettings : DialogPage
    {
        public string PremakePath { get; set; } = string.Empty;
        public bool UseGlobalSetting { get; set; } = false;

        protected override IWin32Window Window
        {
            get
            {
                var page = new PremakeSettingsControl();
                page.OptionsPage = this;
                page.Initialize();
                return page;
            }
        }
    }
}