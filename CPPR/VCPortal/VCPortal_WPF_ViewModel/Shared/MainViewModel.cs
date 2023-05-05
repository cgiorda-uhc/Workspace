using CommunityToolkit.Mvvm.ComponentModel;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using System.ComponentModel;
using VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Projects.MHP;


namespace VCPortal_WPF_ViewModel.Shared;
using Collections = System.Collections.Generic;
public partial class MainViewModel : ObservableObject, ViewModelBase
{

    public ViewModelBase CurrentViewModel;


    //public MainViewModel(NavigationStore navigationStore, IConfiguration config, IExcelFunctions excelFunctions)
    public MainViewModel(string header, IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {
        CurrentViewModel?.Dispose();


        if (header == "ETG Fact Symmetry")
        {
            CurrentViewModel = new ETGFactSymmetryListingViewModel(config, excelFunctions, logger);
        }
        else if (header == "Chemotherapy PX")
        {
            CurrentViewModel = new ChemotherapyPXListingViewModel(config, excelFunctions, logger);
        }
        else if (header == "MHP")
        {
            CurrentViewModel = new MHPViewModel(config, excelFunctions);
        }
    }



    //public MainViewModel(Collections.IReadOnlyList<string> arguments, IConfiguration config, IExcelFunctions excelFunctions)
    //{
    //    Arguments = arguments;
    //    ETGFactSymmetryListingViewModel = new ETGFactSymmetryListingViewModel(config, excelFunctions);
    //    MHPViewModel = new MHPViewModel(config, excelFunctions);
    //}

    bool disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                //dispose managed resources
            }
        }
        //dispose unmanaged resources
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
