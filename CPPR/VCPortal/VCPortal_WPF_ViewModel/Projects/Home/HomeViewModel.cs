using CommunityToolkit.Mvvm.ComponentModel;
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;
using VCPortal_Models.Models.ActiveDirectory;
using VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.Home;
public partial class HomeViewModel : ObservableObject
{

    private readonly IExcelFunctions _excelFunctions;
    private readonly IConfiguration? _config;
    private readonly Serilog.ILogger _logger;
    private readonly IRelationalDataAccess _db_sql;
    private readonly IChemotherapyPX_Repo _chemo_sql;
    private readonly IMHPUniverse_Repo _mhp_sql;
    private readonly IProcCodeTrends_Repo _pct_db;
    private readonly IEDCAdhoc_Repo _edc_db;
    private readonly IETGFactSymmetry_Repo _etg_db;

    public MessageViewModel UserMessageViewModel { get; }

    [ObservableProperty]
    private UserAccessModel currentUser;

    [ObservableProperty]
    private List<string> currentAccess;

    public HomeViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger, IRelationalDataAccess db_sql, IChemotherapyPX_Repo chemo_sql, IMHPUniverse_Repo mhp_sql, IProcCodeTrends_Repo pct_db, IEDCAdhoc_Repo edc_db, IETGFactSymmetry_Repo etg_db)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = config;
        _db_sql = db_sql;
        _chemo_sql = chemo_sql;
        _mhp_sql = mhp_sql;
        _pct_db = pct_db;
        _edc_db = edc_db;
        _etg_db = etg_db;

        UserMessageViewModel = new MessageViewModel();

        if (Authentication.CurrentUser == null)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "Authentication Failed. Please contact the system admin.";
            _logger.Error("Authenitcation Failed  for {CurrentUser}", Authentication.UserName);
            return;
        }

        CurrentUser = Authentication.CurrentUser;
        currentAccess = new List<string>();
        var groups = CurrentUser.Groups.ToList();

        if (groups.Contains("ms\\chemopx", StringComparer.OrdinalIgnoreCase))
        {
            currentAccess.Add("MS\\ChemoPX");
        }

        if (groups.Contains("ms\\etgsymm", StringComparer.OrdinalIgnoreCase))
        {
            currentAccess.Add("MS\\ETGSymm");
        }

        if (groups.Contains("ms\\mhp_universe", StringComparer.OrdinalIgnoreCase))
        {
            currentAccess.Add("MS\\MHP_Universe");
        }


        if (groups.Contains("ms\\mhp_universe", StringComparer.OrdinalIgnoreCase))
        {
            currentAccess.Add("MS\\EDC_Adhoc");
        }



        if (groups.Contains("ms\\pc_trends", StringComparer.OrdinalIgnoreCase))
        {
            currentAccess.Add("MS\\PC_Trends");
        }

    }
}
