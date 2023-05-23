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
using DocumentFormat.OpenXml.Drawing.Charts;

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



    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        //btnSave.IsEnabled = false;


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

    private void ChemotherapyPXGridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
    {

        //VALIDATE VIA VIEWMODEL
        notifyVM();

        //ADD RESULTS TO GRIDVIEW
        var row  = e.Row.DataContext as ChemotherapyPXViewModel;
        var chemo = _viewModel.OC_ChemotherapyPXViewModel.Where(x => x.CODE == row.CODE).FirstOrDefault();
        if (chemo.ValidationResults != null)
        {
            GridViewCellValidationResult validationResult;
            foreach (var v in chemo.ValidationResults)
            {
                validationResult = new GridViewCellValidationResult();
                validationResult.PropertyName = v.MemberNames.FirstOrDefault();
                validationResult.ErrorMessage = v.ErrorMessage;
                e.ValidationResults.Add(validationResult);
            }
            e.IsValid = false;
        }

    }
}
