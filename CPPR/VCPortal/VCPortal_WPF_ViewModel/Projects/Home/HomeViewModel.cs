using CommunityToolkit.Mvvm.ComponentModel;
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

    public MessageViewModel UserMessageViewModel { get; }

    [ObservableProperty]
    private UserAccessModel currentUser;

    [ObservableProperty]
    private List<string> currentAccess;

    public HomeViewModel(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {
        _logger = logger;
        _excelFunctions = excelFunctions;
        _config = config;

        UserMessageViewModel = new MessageViewModel();

        if (Authentication.CurrentUser == null)
        {
            UserMessageViewModel.IsError = true;
            UserMessageViewModel.Message = "Authentication Failed. Please contact the system admin.";
            _logger.Error("Authenitcation Failed  for {CurrentUser}", Authentication.UserName);
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



    }
}
