using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aviad.WPF.Controls;

namespace UCS_Project_Manager
{
    /// <summary>
    /// Interaction logic for ADUserSelect.xaml
    /// </summary>
    public partial class ADUserSelect : UserControl
    {

        public ADUserSelect()
        {
            InitializeComponent();
        }



        //private ADUserDetail _adSelectedUser;
        //public ADUserDetail adSelectedUser
        //{
        //    get { return _adSelectedUser; }
        //    //set { SetValue(dpADUser, value); }
        //}


        private void AutoCompleteTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AutoCompleteTextBox ac = (AutoCompleteTextBox)sender;
            if (ac.currentSelection.DataContext != null)
            {
                this.DPCurrentADItem.DataContext = (ADUserDetail)ac.currentSelection.DataContext;

                //_adSelectedUser = (ADUserDetail)ac.currentSelection.DataContext;
                //this.DPCurrentADItem.DataContext = _adSelectedUser;
            }
        }
    }
}
