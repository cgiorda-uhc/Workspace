using System;
using System.Collections.Generic;
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

namespace UCS_Project_Manager
{
    /// <summary>
    /// Interaction logic for CPM_Intake.xaml
    /// </summary>
    public partial class CPM_Intake : UserControl
    {
        public CPM_Intake()
        {
            InitializeComponent();
        }

        //private ADUserDetail _adSelectedUser;
        //public ADUserDetail adSelectedUser
        //{
        //    get { return  _adSelectedUser; }
        //    set
        //    {
               
        //    }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ADUserDetail _adSelectedUser = (ADUserDetail)this.adUserSelect.DPCurrentADItem.DataContext;
            this.FirstName.Text = _adSelectedUser.FirstName;
            this.LastName.Text = _adSelectedUser.LastName;
            this.Email.Text = _adSelectedUser.EmailAddress;
            this.Username.Text = _adSelectedUser.LoginName;
        }
    }
}
