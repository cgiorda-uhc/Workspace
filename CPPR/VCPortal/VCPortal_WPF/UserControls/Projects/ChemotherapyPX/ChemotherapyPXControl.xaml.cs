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
using DocumentFormat.OpenXml.Spreadsheet;

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
        List<System.ComponentModel.DataAnnotations.ValidationResult> validationResults = null;
        string code = null;

        foreach (var item in this.ChemotherapyPXGridView.Items)
        {
            foreach (var column in this.ChemotherapyPXGridView.Columns.OfType<GridViewBoundColumnBase>())
            {

                if (column.Header.ToString().ToLower().Equals("id"))
                {
                    continue;
                }


                if (column.Header.ToString().ToLower().Equals("proc code"))
                {
                    code = column.GetValueForItem(item).ToString();

                    code = (code == "" ? null : code);
                    var chemo = _viewModel.OC_ChemotherapyPXViewModel.Where(x => x.CODE == code).FirstOrDefault();

                    validationResults = chemo.ValidationResults;
                }

                //var code = column.GetValueForItem(item).ToString();

                //code = (code == "" ? null : code);
                //var chemo = _viewModel.OC_ChemotherapyPXViewModel.Where(x => x.CODE == code).FirstOrDefault();

                //var validation = chemo.ValidationResults;
                var sb = new StringBuilder();
                if (validationResults != null)
                {
                    foreach (var v in validationResults)
                    {
                        sb.AppendLine(v.ErrorMessage);
                    }
                }

                //var rows = this.ChemotherapyPXGridView.ChildrenOfType<GridViewRow>();

                //foreach (var row in rows)
                //{
                //    if (row is GridViewNewRow)
                //        continue;

                //    var content = row.Cells[0].Content;
                //    var objType = content.GetType();

                //    string code2 = null;
                //    if (objType == typeof(GridViewEditorPresenter))
                //    {
                //        code2 = ((RadAutoCompleteBox)((GridViewEditorPresenter)content).Content).SearchText;
                //    }
                //    else if (objType == typeof(TextBlock))
                //    {

                //        code2 = ((TextBlock)(content)).Text;
                //    }

                //    if (code2 != code)
                //    {
                //        continue;
                //    }


                //    if (validationResults != null)
                //    {
                //        foreach (var cell in row.Cells)
                //        {
                //            cell.ToolTip = sb.ToString();
                //        }
                //        row.BorderBrush = Brushes.Red;
                //        row.BorderThickness = new Thickness(2);
                //    }
                //    else
                //    {
                //        foreach (var cell in row.Cells)
                //        {
                //            cell.ToolTip = null;
                //        }
                //        row.BorderBrush = Brushes.Black;
                //        row.BorderThickness = new Thickness(0);
                //    }



                }




            }
      




       // var rows = this.ChemotherapyPXGridView.ChildrenOfType<GridViewRow>();

        //foreach (var row in rows)
        //{
        //    if (row is GridViewNewRow)
        //        continue;

        //    //string code = row("Code".Text;

        //    string code = "";
        //    var content = row.Cells[0].Content;
        //    var objType = content.GetType();

        //    if (objType == typeof(GridViewEditorPresenter))
        //    {
        //        code = ((RadAutoCompleteBox)((GridViewEditorPresenter)content).Content).SearchText;
        //    }
        //    else if (objType == typeof(TextBlock))
        //    {

        //        code = ((TextBlock)(content)).Text;
        //    }
        //    else
        //    {
        //        return;
        //    }

        //    code = (code == "" ? null : code);
        //    var chemo = _viewModel.OC_ChemotherapyPXViewModel.Where(x => x.CODE == code).FirstOrDefault();
        //    if (chemo == null)
        //    {
        //        var g = row.Cells[0].Value;


        //        return;
        //    }



            //    var validation = chemo.ValidationResults;
            //    if (validation != null)
            //    {
            //        var sb = new StringBuilder();
            //        foreach(var v in validation)
            //        {
            //            sb.AppendLine(v.ErrorMessage);
            //        }

            //        foreach (var cell in row.Cells)
            //        {
            //            cell.ToolTip = sb.ToString();
            //        }
            //        row.BorderBrush = Brushes.Red;
            //        row.BorderThickness = new Thickness(2);
            //    }
            //    else
            //    {
            //        foreach (var cell in row.Cells)
            //        {
            //            cell.ToolTip = null;
            //        }
            //        row.BorderBrush = Brushes.Black;
            //        row.BorderThickness = new Thickness(0);
            //    }

            //    //foreach (var cell in row.Cells)
            //    //{
            //    //    e.Cell.ParentRow.Cells[0].ToolTip = "Test";
            //    //}
            //}
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

    private void ChemotherapyPXGridView_Sorted(object sender, GridViewSortedEventArgs e)
    {
        runValidation();
    }

    private void ChemotherapyPXGridView_Filtered(object sender, GridViewFilteredEventArgs e)
    {
        runValidation();
    }
}
