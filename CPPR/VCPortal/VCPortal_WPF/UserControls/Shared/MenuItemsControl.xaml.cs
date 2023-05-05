
using Telerik.Windows.Controls;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using VCPortal_WPF.UserControls.Projects.ETGFactSymmetry;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using VCPortal_WPF.UserControls.Projects.MHP;
using VCPortal_WPF.UserControls.Projects.ChemotherapyPX;

namespace VCPortal_WPF.UserControls.Shared;
/// <summary>
/// Interaction logic for MenuItems.xaml
/// </summary>
public partial class MenuItemsControl : UserControl
{
    private readonly IConfiguration _config;
    private readonly IExcelFunctions _excel;
    private readonly Serilog.ILogger _logger;

    public MenuItemsControl(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    //public MenuItemsControl()
    {

        InitializeComponent();

        _config = config;
        _excel = excelFunctions;
        _logger = logger;

        //Add to a sub item
        var existingMenuItem = (RadMenuItem)this.MainMenu.Items[0];

        var newMenuItem = new RadMenuItem();
        newMenuItem.Header = "ETG Fact Symmetry";
        newMenuItem.Click += NewMenuItem_Click;
        existingMenuItem.Items.Add(newMenuItem);

        newMenuItem = new RadMenuItem();
        newMenuItem.Header = "Chemotherapy PX";
        newMenuItem.Click += NewMenuItem_Click;
        existingMenuItem.Items.Add(newMenuItem);

        newMenuItem = new RadMenuItem();
        newMenuItem.Header = "EBM Mapping";
        newMenuItem.Click += NewMenuItem_Click;
        existingMenuItem.Items.Add(newMenuItem);

        newMenuItem = new RadMenuItem();
        newMenuItem.Header = "PEG Mapping";
        newMenuItem.Click += NewMenuItem_Click;
        existingMenuItem.Items.Add(newMenuItem);


        existingMenuItem = (RadMenuItem)this.MainMenu.Items[2];

        newMenuItem = new RadMenuItem();
        newMenuItem.Header = "MHP";
        newMenuItem.Click += NewMenuItem_Click;
        existingMenuItem.Items.Add(newMenuItem);

        newMenuItem = new RadMenuItem();
        newMenuItem.Header = "Compliance Reporting";
        newMenuItem.Click += NewMenuItem_Click;
        existingMenuItem.Items.Add(newMenuItem);

    }

    private void NewMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
    {
        RadMenuItem menuItem = (RadMenuItem)sender;
        Window parentWindow = Window.GetWindow(this);
        parentWindow.Title = menuItem.Header.ToString();
        var contentControl = parentWindow.ChildrenOfType<ContentControl>().Where(x => x.Name == "contentControl").FirstOrDefault();

        if (menuItem.Header == "ETG Fact Symmetry")
        {
            contentControl!.Content = new ETGFactSymmetryControl(_config,_excel, _logger);
        }
        else if (menuItem.Header == "Chemotherapy PX")
        {
            contentControl!.Content = new ChemotherapyPXControl(_config, _excel, _logger);
        }
        else if(menuItem.Header == "MHP")
        {
            contentControl!.Content = new MHPEIControl(_config, _excel, _logger);
        }
        else
        {
            contentControl!.Content = new HomeControl();
        }
    }

    private void Home_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
    {
        Window parentWindow = Window.GetWindow(this);
        parentWindow.Title = "Value Creation Assistant Home";
        var contentControl = parentWindow.ChildrenOfType<ContentControl>().Where(x =>  x.Name == "contentControl").FirstOrDefault();
        contentControl!.Content = new HomeControl();
    }

    private void Exit_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
    {
        Window.GetWindow(this).Close();
    }
}
