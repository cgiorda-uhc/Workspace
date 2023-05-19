using MathNet.Numerics;
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
using static Org.BouncyCastle.Math.EC.ECCurve;
using VCPortal_WPF_ViewModel.Shared;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.UriParser;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using System.Data;
using VCPortal_WPF.UserControls.Shared;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
using VCPortal_WPF_ViewModel.Projects.MHP;
using NPOI.SS.Formula.Functions;
using Telerik.Windows.Documents.Fixed.Model.Annotations;

namespace VCPortal_WPF.UserControls.Projects.ChemotherapyPX;
/// <summary>
/// Interaction logic for ChemotherapyPXControl.xaml
/// </summary>
public partial class ChemotherapyPXControl : UserControl
{
    private ChemotherapyPXListingViewModel _viewModel => (ChemotherapyPXListingViewModel)DataContext;
    public ChemotherapyPXControl()
    {
        //logger.Information("Initializing ChemotherapyPXControl for  for {CurrentUser}...", Authentication.UserName);
        //DataContext = new MainViewModel("Chemotherapy PX", config, excelFunctions, logger).CurrentViewModel;

        InitializeComponent();

        modalContentControl.Content = new StatusControl();
        //modalContentControl.DataContext = _viewModel;
        //modalContentControl.DataContext = MainWindowViewModel;
    }



    //public ChemotherapyPXControl(IConfiguration config, IExcelFunctions excelFunctions,  Serilog.ILogger logger)
    //{
    //    logger.Information("Initializing ChemotherapyPXControl for  for {CurrentUser}...", Authentication.UserName);
    //    DataContext = new MainViewModel("Chemotherapy PX", config, excelFunctions, logger).CurrentViewModel;

    //    InitializeComponent();


    //    modalContentControl.Content = new StatusControl();
    //    modalContentControl.DataContext = this.DataContext;
    //}

    //NOT IDEAL FOR MVVM BUT TIME IS LIMITED

    private void notifyVM()
    {
        //if (_viewModel == null)
        //{
        //    _viewModel = (ChemotherapyPXListingViewModel)DataContext;
        //}
        if (_viewModel.EditEndCallCommand.CanExecute(null))
            _viewModel.EditEndCallCommand.Execute(null);
    }


    private void runValidation()
    {
        var rows = this.ChemotherapyPXGridView.ChildrenOfType<GridViewRow>();

        foreach (var row in rows)
        {
            if (row is GridViewNewRow)
                continue;


            string code = "";
            var objType = row.Cells[0].Content.GetType();

            if (objType == typeof(RadAutoCompleteBox))
            {
                code = ((RadAutoCompleteBox)((GridViewEditorPresenter)row.Cells[0].Content).Content).SearchText;
            }
            else if (objType == typeof(TextBox))
            {

                code = ((TextBox)((GridViewEditorPresenter)row.Cells[0].Content).Content).Text;
            }
            else if (objType == typeof(TextBlock))
            {

                code = ((TextBlock)(row.Cells[0].Content)).Text;
            }
            else
            {
                return;
            }


            if (_viewModel.OC_ChemotherapyPXViewModel.Where(x => x.CODE == (code == "" ? null : code)).FirstOrDefault().ValidationResults != null)
            {
                row.Cells[0].ToolTip = "Test";
            }

            //foreach (var cell in row.Cells)
            //{
            //    e.Cell.ParentRow.Cells[0].ToolTip = "Test";
            //}
        }
    }

    

    private void ChemotherapyPXGridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
    {

        notifyVM();
        runValidation();



        //foreach (var item in ChemotherapyPXGridView.Items)
        //{
        //    string itemValue = item["CODE"].Text; // unique name of the column

        //    if (_viewModel.OC_ChemotherapyPXViewModel.Where(x => x.CODE == itemValue).FirstOrDefault().ValidationResults != null)
        //    {
        //        e.Cell.ParentRow.Cells[0].ToolTip = "Test";
        //    }


        //}

        //var val =dc.

        //if(dc.)





        //var objType = ((GridViewEditorPresenter)e.Cell.Content).Content.GetType();
        //object newvalue;

        //if (objType == typeof(RadButton))
        //{
        //    btnSave.IsEnabled = true;
        //    return;
        //}

        //if (objType == typeof(RadAutoCompleteBox))
        //{
        //    newvalue = ((RadAutoCompleteBox)((GridViewEditorPresenter)e.Cell.Content).Content).SearchText;
        //}
        //else if(objType == typeof(TextBox))
        //{
        //    newvalue = ((TextBox)((GridViewEditorPresenter)e.Cell.Content).Content).Text;
        //}
        //else if (objType == typeof(DatePicker))
        //{
        //    newvalue = ((DatePicker)((GridViewEditorPresenter)e.Cell.Content).Content).Text;//bug
        //}
        //else if (objType == typeof(RadComboBox))
        //{
        //    newvalue = ((RadComboBox)((GridViewEditorPresenter)e.Cell.Content).Content).Text;
        //}
        //else
        //{
        //    newvalue = ((TextBox)((GridViewEditorPresenter)e.Cell.Content).Content).Text;
        //}

        //if (_value + "" != newvalue + "")
        //{
        //    btnSave.IsEnabled = true;
        //}

    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        //btnSave.IsEnabled = false;


    }

    private object _value;
    private void ChemotherapyPXGridView_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
    {

        //_value = ((TextBlock)e.Cell.Content).Text;
    }

    private void btnExporData_Click(object sender, RoutedEventArgs e)
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
    }

}
