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
using VCPortal_WPF.UserControls.Shared;
using VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
using VCPortal_WPF.Components;

namespace VCPortal_WPF.UserControls.Projects.MHP;
/// <summary>
/// Interaction logic for MHPEIControl.xaml
/// </summary>
public partial class MHPControl : UserControl
{

    private ProcCodeTrendsViewModel _viewModel => (ProcCodeTrendsViewModel)DataContext;

    public MHPControl()
    {
        InitializeComponent();

        modalContentControl.Content = new StatusControl();
        //DataContext = new MainViewModel("MHP", config, excelFunctions, logger).CurrentViewModel;
    }

    private void btnSwitchReport_Click(object sender, RoutedEventArgs e)
    {
        var content = (sender as Button).Content.ToString();
    }



    private void btnAddCustSeg_Click(object sender, RoutedEventArgs e)
    {
        var value = txtCUST_SEGFilter.SearchText;
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        lstSelectedCustSeg.Items.Add(value);
        txtCUST_SEGFilter.SearchText = "";

    }


    private void btnRemoveSelected_Click(object sender, RoutedEventArgs e)
    {
        var item = lstSelectedCustSeg.SelectedItem;
        if(item == null)
        {
            return;
        }

        lstSelectedCustSeg.Items.Remove(item);

    }

    private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
    {
        lstSelectedCustSeg.Items.Clear();
    }


    bool blJustProcessed;
    private void cbxStateFilter_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
    {
        if (blJustProcessed)
        {
            blJustProcessed = false;
            return;
        }
        var _checkComboBox = (SelectAllCheckComboBox)sender;
        var value = e.Item.ToString();
        checkForAll(_checkComboBox, value, e.IsSelected);
    }


    private void cbxStateFilter_Loaded(object sender, RoutedEventArgs e)
    {
        blJustProcessed = false;
    }

    private void cbxLegalEntityFilter_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
    {
        if (blJustProcessed)
        {
            blJustProcessed = false;
            return;
        }
        var _checkComboBox = (SelectAllCheckComboBoxTilde)sender;
        var value = e.Item.ToString();
        checkForAll(_checkComboBox, value, e.IsSelected);
    }

    private void cbxLegalEntityFilter_Loaded(object sender, RoutedEventArgs e)
    {
        blJustProcessed = false;
    }


    private void CbxFINC_ARNG_CDFilter_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
    {
        if (blJustProcessed)
        {
            blJustProcessed = false;
            return;
        }
        var _checkComboBox = (SelectAllCheckComboBox)sender;
        var value = e.Item.ToString();
        checkForAll(_checkComboBox, value, e.IsSelected);
    }


    private void CbxFINC_ARNG_CDFilter_Loaded(object sender, RoutedEventArgs e)
    {
        blJustProcessed = false;
    }

    private void CbxMKT_SEG_RLLP_DESCFilter_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
    {
        if (blJustProcessed)
        {
            blJustProcessed = false;
            return;
        }
        var _checkComboBox = (SelectAllCheckComboBox)sender;
        var value = e.Item.ToString();
        checkForAll(_checkComboBox, value, e.IsSelected);
    }

    private void CbxMKT_SEG_RLLP_DESCFilter_Loaded(object sender, RoutedEventArgs e)
    {
        blJustProcessed = false;
    }


    private void CbxMKT_TYP_DESCFilter_Loaded(object sender, RoutedEventArgs e)
    {
        blJustProcessed = false;
    }
    private void CbxMKT_TYP_DESCFilter_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
    {
        if (blJustProcessed)
        {
            blJustProcessed = false;
            return;
        }
        var _checkComboBox = (SelectAllCheckComboBox)sender;
        var value = e.Item.ToString();
        checkForAll(_checkComboBox, value, e.IsSelected);
    }

    private void CbxCS_TADM_PRDCT_MAP_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
    {
        if (blJustProcessed)
        {
            blJustProcessed = false;
            return;
        }
        var _checkComboBox = (SelectAllCheckComboBox)sender;
        var value = e.Item.ToString();
        checkForAll(_checkComboBox, value, e.IsSelected);
    }

    private void CbxCS_TADM_PRDCT_MAP_Loaded(object sender, RoutedEventArgs e)
    {
        blJustProcessed = false;
    }


    private void CbxGroupNumbers_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
    {
        if (blJustProcessed)
        {
            blJustProcessed = false;
            return;
        }
        var _checkComboBox = (SelectAllCheckComboBox)sender;
        var value = e.Item.ToString();
        checkForAll(_checkComboBox, value, e.IsSelected);
    }

    private void CbxGroupNumbers_Loaded(object sender, RoutedEventArgs e)
    {
        blJustProcessed = false;
    }

    private void CbxProductCode_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
    {
        if (blJustProcessed)
        {
            blJustProcessed = false;
            return;
        }
        var _checkComboBox = (SelectAllCheckComboBox)sender;
        var value = e.Item.ToString();
        checkForAll(_checkComboBox, value, e.IsSelected);

    }

    private void CbxProductCode_Loaded(object sender, RoutedEventArgs e)
    {
        blJustProcessed = false;
    }


    private void CbxStateFilter_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
    {
        if (blJustProcessed)
        {
            blJustProcessed = false;
            return;
        }
        var _checkComboBox = (SelectAllCheckComboBox)sender;
        var value = e.Item.ToString();
        checkForAll(_checkComboBox, value, e.IsSelected);
    }

    private void checkForAll(SelectAllCheckComboBox _checkComboBox, string strValue, bool isSelected)
    {
        if (strValue == "-9999" || strValue == "--All--")
        {
            // Select All
            if (isSelected)
            {

                blJustProcessed = true;
                _checkComboBox.SelectAll();

            }
            else
            {

                _checkComboBox.UnSelectAll();
            }
        }
    }

    private void checkForAll(SelectAllCheckComboBoxTilde _checkComboBox, string strValue, bool isSelected)
    {
        if (strValue == "-9999" || strValue == "--All--")
        {
            // Select All
            if (isSelected)
            {

                blJustProcessed = true;
                _checkComboBox.SelectAll();

            }
            else
            {

                _checkComboBox.UnSelectAll();
            }
        }
    }


}
