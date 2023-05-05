using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UCS_Project_Manager
{
    /// <summary>
    /// Interaction logic for SymmetryVersionSelect.xaml
    /// </summary>
    public partial class SymmetryVersionSelect : Window
    {
        public SymmetryVersionSelect()
        {
            InitializeComponent();

            if(File.Exists(GlobalState.strVersionPath))
            {
                File.Delete(GlobalState.strVersionPath);
            }
        }

        private void BtnVersion_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = Application.Current.MainWindow;
            mainWindow.Close();


            //UCS_Project_Manager_ViewModels.GlobalState.SymmetryVersion
            //GlobalState.SymmetryVersion = Symmetry_VerionFilter.SelectedValue.ToString();
            //ETG_Fact_Symmetry ef = new ETG_Fact_Symmetry();
            //ef.Symmetry_VerionCurrent.Content = Symmetry_VerionFilter.SelectedValue.ToString();



            using (StreamWriter newTask = new StreamWriter(GlobalState.strVersionPath, false))
            {
                newTask.Write(Symmetry_VerionFilter.SelectedValue.ToString());
            }






            UCSProjectManager efs = new UCSProjectManager();
            //System.Windows.Controls.Grid c = ((System.Windows.Controls.Grid)efs.Content);
            //var etg = (UCS_Project_Manager.ETG_Fact_Symmetry)c.Children[0];
           
            //((ETG_Fact_Symmetry_ViewModel)etg.DataContext).strFilteredSymmetry_Version = Symmetry_VerionFilter.SelectedValue.ToString();

            efs.Show();



            this.Close();

        }


    }
}
