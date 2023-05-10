using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
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
using VCPortal_WPF.UserControls;
using VCPortal_WPF.UserControls.Projects.ChemotherapyPX;
using VCPortal_WPF.UserControls.Projects.ETGFactSymmetry;
using VCPortal_WPF.UserControls.Shared;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    //public MainWindow(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    public MainWindow(Serilog.ILogger logger)
    { 
        InitializeComponent();


        logger.Information("Starting MainWindow....");


        //DataContext = new MainViewModel("", config, excelFunctions, logger).CurrentViewModel;
        //this.menuControl.Content = new MenuItemsControl(config,excelFunctions, logger);

        //this.Title = "Value Creation Assistant Home";
        //this.contentControl.Content = new HomeControl();

        //if(Authentication.UserName == "cgiordaa" || Authentication.UserName == "sdonela")
        //{
        //    this.Title = "Chemotherapy PX";
        //    this.contentControl.Content = new ChemotherapyPXControl(config, excelFunctions, logger);
        //}
        //else
        //{
        //    this.Title = "ETG Fact Symmetry";
        //    this.contentControl.Content = new ETGFactSymmetryControl(config, excelFunctions, logger);
        //}
        
    }

    private void Exit_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
    {
        Window.GetWindow(this).Close();
    }
}
