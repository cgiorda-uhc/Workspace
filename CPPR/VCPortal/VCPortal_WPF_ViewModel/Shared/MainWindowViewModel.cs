using CommunityToolkit.Mvvm.Input;
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using DocumentFormat.OpenXml.Spreadsheet;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using NPOI.OpenXmlFormats.Shared;
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
using VCPortal_WPF_ViewModel.Projects.EDCAdhoc;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Projects.Home;
using VCPortal_WPF_ViewModel.Projects.MHP;
using VCPortal_WPF_ViewModel.Projects.ProcCodeTrends;

namespace VCPortal_WPF_ViewModel.Shared;
public class MainWindowViewModel : INotifyPropertyChanged
{
    public string MainWinVMString { get; set; } = "Hello from MainWindoViewModel";




    private readonly IExcelFunctions _excelFunctions;
    private readonly IConfiguration? _config;
    private readonly Serilog.ILogger _logger;
    //private readonly IRelationalDataAccess _db_sql;
    //private readonly IChemotherapyPX_Repo _chemo_sql;
    //private readonly IMHPUniverse_Repo _mhp_sql;
    //private readonly IProcCodeTrends_Repo _pct_db;
    //private readonly IEDCAdhoc_Repo _edc_db;
    //private readonly IETGFactSymmetry_Repo _etg_db;

    private readonly DBRepoModel _dBRepo;


    //public MainWindowViewModel(string header, IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger, IRelationalDataAccess db_sql, IChemotherapyPX_Repo chemo_sql, IMHPUniverse_Repo mhp_sql, IProcCodeTrends_Repo pct_db, IEDCAdhoc_Repo edc_db, IETGFactSymmetry_Repo etg_db)
    public MainWindowViewModel(string header, IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger, DBRepoModel dBRepo)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = config;
        //_db_sql = db_sql;
        //_chemo_sql = chemo_sql;
        //_mhp_sql = mhp_sql;
        //_pct_db = pct_db;
        //_edc_db = edc_db;
        //_etg_db = etg_db;
        _dBRepo = dBRepo;

        CurrentViewModel = Activator.CreateInstance(typeof(HomeViewModel), _config, _excelFunctions, _logger, dBRepo);
        //CurrentViewModel = Activator.CreateInstance(typeof(HomeViewModel), _config, _excelFunctions, _logger, db_sql, chemo_sql, mhp_sql, pct_db, edc_db, etg_db);
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
                NavigationViewModelTypesReports.Add(new TypeAndDisplay { Name = "MHP Reporting", VMType = typeof(MHPViewModel) });
            }


            if (groups.Contains("ms\\mhp_universe", StringComparer.OrdinalIgnoreCase))
            {
                NavigationViewModelTypesReports.Add(new TypeAndDisplay { Name = "EDCAdhoc Reporting", VMType = typeof(EDCAdhocViewModel) });
            }



            if (groups.Contains("ms\\pc_trends", StringComparer.OrdinalIgnoreCase))
            {
                NavigationViewModelTypesReports.Add(new TypeAndDisplay { Name = "ProcCode Trending", VMType = typeof(ProcCodeTrendsViewModel) });
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
                    //CurrentViewModel = Activator.CreateInstance(vmType, _config, _excelFunctions, _logger, _db_sql, _chemo_sql, _mhp_sql, _pct_db, _edc_db, _etg_db);
                    CurrentViewModel = Activator.CreateInstance(vmType, _config, _excelFunctions, _logger, _dBRepo);
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
