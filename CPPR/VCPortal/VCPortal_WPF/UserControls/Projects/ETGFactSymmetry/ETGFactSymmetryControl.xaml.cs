using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using VCPortal_WPF.Shared;
using VCPortal_WPF.UserControls.Shared;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF.UserControls.Projects.ETGFactSymmetry;
/// <summary>
/// Interaction logic for ETGFactSymmetry.xaml
/// </summary>
public partial class ETGFactSymmetryControl : UserControl
{

    private ETGFactSymmetryListingViewModel _viewModel => (ETGFactSymmetryListingViewModel)DataContext;



    public ETGFactSymmetryControl()
    {

        InitializeComponent();

        modalContentControl.Content = new StatusControl();
        //modalContentControl.DataContext = this.DataContext;
        //modalContentControl.Content = new StatusControl();
    }




    ////public ETGFactSymmetryControl(IConfiguration config, IExcelFunctions excelFunctions)
    //public ETGFactSymmetryControl(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    //{
    //    logger.Information("Initializing ETGFactSymmetryControl for  for {CurrentUser}...", Authentication.UserName);


    //    DataContext = new MainViewModel("ETG Fact Symmetry", config, excelFunctions, logger).CurrentViewModel;
    //    InitializeComponent();

    //    modalContentControl.Content = new StatusControl();
    //    modalContentControl.DataContext = this.DataContext;
    //    //modalContentControl.Content = new StatusControl();
    //}

    private void btnExporConfig_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.Cursor = Cursors.Wait;
            modalContentControl.Content = new StatusControl();

        }
        finally
        {
            this.Cursor = Cursors.Arrow;
        }
        //modalContentControl.Content = new TextBlock()
        //{
        //    Text = "Test Status",
        //    Width =  Double.NaN,
        //    Height = Double.NaN
        //};
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        //btnSave.IsEnabled = false;
    }

    private object _value;
    private void ETGGridView_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
    {
        //_value = ((TextBlock)e.Cell.Content).Text;
    }

    private void ETGGridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
    {
        //var content = ((GridViewEditorPresenter)e.Cell.Content).Content;
        //var type = content.GetType();
        //string newvalue = "";
        //if(type.Name == "RadComboBox") 
        //{
        //    newvalue = ((RadComboBox)content).Text;
        //}
        //else
        //{
        //    newvalue = ((TextBox)content).Text;
        //}

        //if (_value + "" != newvalue + "")
        //{
        //    btnSave.IsEnabled = true;
        //}

    }

    private void ETGGridView_Filtered(object sender, GridViewFilteredEventArgs e)
    {
        _viewModel.ETGFactSymmetryFilterItems.Clear();
        foreach (var i in ETGGridView.Items)
        {
            _viewModel.ETGFactSymmetryFilterItems.Add((ETGFactSymmetryViewModel)i as ETGFactSymmetryViewModel);
        }
    }

    private void btnClearFilters_Click(object sender, RoutedEventArgs e)
    {
        ETGGridView.FilterDescriptors.Clear();
    }



    //private void UserControl_Loaded(object sender, RoutedEventArgs e)
    //{
    //    Versions.SelectedIndex = 0;
    //}

    //private void uc_etgfact_Unloaded(object sender, RoutedEventArgs e)
    //{
    //    ETGGridView.ItemsSource = null;
    //    ETGGridView.Columns.Clear();
    //    ETGGridView.Items.Clear();
    //    ETGGridView.Items.Refresh();
    //}
}
