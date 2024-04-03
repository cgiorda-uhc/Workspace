using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VCPortal_WPF.UserControls.Projects.ProcCodeTrends
{
    /// <summary>
    /// Interaction logic for ProcCodeTrendsHelp.xaml
    /// </summary>
    public partial class ProcCodeTrendsHelpControl : UserControl
    {
        public ProcCodeTrendsHelpControl()
        {
            InitializeComponent();

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {

            string address = e.Uri.AbsoluteUri;
            System.Diagnostics.Process.Start(new ProcessStartInfo(address) { UseShellExecute = true });

            e.Handled = true;
        }
    }
}
