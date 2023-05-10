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
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
using FileParsingLibrary.MSExcel;
using Microsoft.Extensions.Configuration;
using VCPortal_WPF_ViewModel.Projects.MHP;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF.UserControls.Projects.MHP;
/// <summary>
/// Interaction logic for MHPEIControl.xaml
/// </summary>
public partial class MHPEIControl : UserControl
{
    public MHPEIControl(IConfiguration config, IExcelFunctions excelFunctions, Serilog.ILogger logger)
    {
        InitializeComponent();
        //DataContext = new MainViewModel("MHP", config, excelFunctions, logger).CurrentViewModel;
    }
}
