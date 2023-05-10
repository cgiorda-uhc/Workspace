using CommunityToolkit.Mvvm.Input;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Projects.MHP;

namespace VCPortal_WPF_ViewModel.Shared;
public class MainWindowViewModel : INotifyPropertyChanged
{
    public string MainWinVMString { get; set; } = "Hello from MainWindoViewModel";




    private readonly IExcelFunctions _excelFunctions;
    private readonly IConfiguration? _config;
    private readonly Serilog.ILogger _logger;

    public MainWindowViewModel(string header, IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = config;

        //if (header == "ETG Fact Symmetry")
        //{
        //    currentViewModel = new ETGFactSymmetryListingViewModel(config, excelFunctions, logger);
        //}
        //else if (header == "Chemotherapy PX")
        //{
        //    currentViewModel = new ChemotherapyPXListingViewModel(config, excelFunctions, logger);
        //}
        //else if (header == "MHP")
        //{
        //    currentViewModel = new MHPViewModel(config, excelFunctions);
        //}
    }

    public ObservableCollection<TypeAndDisplay> NavigationViewModelTypes { get; set; } = new ObservableCollection<TypeAndDisplay>
    (
        new List<TypeAndDisplay>
        {
             new TypeAndDisplay{ Name="Chemotherapy PX", VMType= typeof(ChemotherapyPXListingViewModel) },
             new TypeAndDisplay{ Name="ETG Fact Symmetry", VMType= typeof(ETGFactSymmetryListingViewModel) },
             new TypeAndDisplay{ Name="EBM Mapping" },
             new TypeAndDisplay{ Name="PEG Mapping" }
        }
    );


    public ObservableCollection<TypeAndDisplay> NavigationViewModelTypesReports { get; set; } = new ObservableCollection<TypeAndDisplay>
(
    new List<TypeAndDisplay>
    {
             new TypeAndDisplay{ Name="MHP" },
             new TypeAndDisplay{ Name="Compliance Reporting" }
    }
);


    private object currentViewModel;

    public object CurrentViewModel
    {
        get { return currentViewModel; }
        set { currentViewModel = value; RaisePropertyChanged(); }
    }
    private RelayCommand<Type> navigateCommand;
    public RelayCommand<Type> NavigateCommand
    {
        get
        {
            return navigateCommand
            ?? (navigateCommand = new RelayCommand<Type>(
            vmType =>
            {
                if (vmType != null)
                {        
                    CurrentViewModel = null;
                    CurrentViewModel = Activator.CreateInstance(vmType, _config, _excelFunctions, _logger);
                }
            }));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void RaisePropertyChanged([CallerMemberName] String propertyName = "")
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
