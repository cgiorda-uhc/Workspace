using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Spreadsheet;
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
using VCPortal_WPF_ViewModel.Projects.Home;
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

        CurrentViewModel = Activator.CreateInstance(typeof(HomeViewModel), _config, _excelFunctions, _logger);

        populateNavigation();

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




    public ObservableCollection<TypeAndDisplay> NavigationViewModelTypesDM { get; set; }
    public ObservableCollection<TypeAndDisplay> NavigationViewModelTypesReports { get; set; }
    //public ObservableCollection<TypeAndDisplay> NavigationViewModelTypes { get; set; } = new ObservableCollection<TypeAndDisplay>
    //(

    //    new List<TypeAndDisplay>
    //    {
    //        new TypeAndDisplay{ Name="Chemotherapy PX", VMType= typeof(ChemotherapyPXListingViewModel) },
    //         new TypeAndDisplay{ Name="ETG Fact Symmetry", VMType= typeof(ETGFactSymmetryListingViewModel) },
    //         new TypeAndDisplay{ Name="EBM Mapping" },
    //         new TypeAndDisplay{ Name="PEG Mapping" }
    //    }



    //);

    private void populateNavigation()
    {
        if (Authentication.CurrentUser != null)
        {
            var groups = Authentication.CurrentUser.Groups.ToList();

            NavigationViewModelTypesDM = new ObservableCollection<TypeAndDisplay>();
            if (groups.Contains("ms\\chemopx", StringComparer.OrdinalIgnoreCase))
            {
                NavigationViewModelTypesDM.Add(new TypeAndDisplay { Name = "Chemotherapy PX", VMType = typeof(ChemotherapyPXListingViewModel), Question= "You have unsaved changes. Continue anyway?", CheckSaves=true });
            }

            if (groups.Contains("ms\\etgsymm", StringComparer.OrdinalIgnoreCase))
            {
                NavigationViewModelTypesDM.Add(new TypeAndDisplay { Name = "ETG Fact Symmetry", VMType = typeof(ETGFactSymmetryListingViewModel), Question = "You have unsaved changes. Continue anyway?", CheckSaves = true });
            }


            NavigationViewModelTypesDM.Add(new TypeAndDisplay { Name = "EBM Mapping" });
            NavigationViewModelTypesDM.Add(new TypeAndDisplay { Name = "PEG Mapping" });


            NavigationViewModelTypesReports = new ObservableCollection<TypeAndDisplay>();


            if (groups.Contains("ms\\mhp_universe", StringComparer.OrdinalIgnoreCase))
            {
                NavigationViewModelTypesReports.Add(new TypeAndDisplay { Name = "MHP", VMType = typeof(MHPViewModel) });
            }



            NavigationViewModelTypesReports.Add(new TypeAndDisplay { Name = "Compliance Reporting" });

        }

    }
    


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
