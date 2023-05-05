using System.Windows;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Windows.Interop;
using UCS_Project_Manager.Helpers;

namespace UCS_Project_Manager
{
    //http://codesigning.optum.com/Dashboard/NewRequestSelectApp
    /// <summary>
    /// Interaction logic for MainProjectHub.xaml
    /// </summary>
    public partial class UCSProjectManager : Window
    {
        public UCSProjectManager()
        {
     

            //GlobalState.IsDesignMode = (Process.GetCurrentProcess().ProcessName == "devenv" ? true : false);
            //GlobalState.IsDesignMode = true;
            InitializeComponent();

            // this.contentControl.Content = new ETG_Fact_Symmetry();
            // && Authentication.getUser() != "cgiorda"
            if (Authentication.isMemberOf("MHP_Universe") && Authentication.getUser() != "cgiorda")
            {
                this.contentControl.Content = new MHP_Yearly_Universes_Reporting();
            }
            else
            {
                this.contentControl.Content = new ETG_Fact_Symmetry();
            }



            //this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight);
            //this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);
            //txt.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            //txtCode.GetBindingExpression(TextBox.TextProperty).UpdateSource();


        }

        //NOT BEING USED YET BUT FIX SCREEN DRAG FOUNDATION
        public Screen GetCurrentScreen(Window window)
        {
            return Screen.FromHandle(new WindowInteropHelper(window).Handle);
        }
        //MAIN PAGE HEADER TAG
        private void Window_LocationChanged(object sender, System.EventArgs e)
        {
            var screen = GetCurrentScreen(this);
            this.Height = screen.Bounds.Height;
            this.Width = screen.Bounds.Width;

        }
    }
}
