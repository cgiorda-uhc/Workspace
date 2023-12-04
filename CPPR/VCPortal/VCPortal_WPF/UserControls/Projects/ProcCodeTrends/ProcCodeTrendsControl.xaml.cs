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
using VCPortal_WPF.Components;
using VCPortal_WPF.UserControls.Shared;

namespace VCPortal_WPF.UserControls.Projects.ProcCodeTrends;
/// <summary>
/// Interaction logic for ProcCodeTrendsControl1.xaml
/// </summary>
public partial class ProcCodeTrendsControl : UserControl
{
    public ProcCodeTrendsControl()
    {
        InitializeComponent();

        modalContentControl.Content = new StatusControl();
    }




    private void btnAddProcCd_Click(object sender, RoutedEventArgs e)
    {
        var value = txtProc_CodeFilter.SearchText;
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        lstSelectedProcCode.Items.Add(value);
        txtProc_CodeFilter.SearchText = "";

    }


    private void btnRemoveSelected_Click(object sender, RoutedEventArgs e)
    {
        var item = lstSelectedProcCode.SelectedItem;
        if (item == null)
        {
            return;
        }

        lstSelectedProcCode.Items.Remove(item);

    }

    private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
    {
        lstSelectedProcCode.Items.Clear();
    }




    bool blJustProcessed;
    private void cbx_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
    {
        //if (blJustProcessed)
        //{
        //    blJustProcessed = false;
        //    return;
        //}
        var _checkComboBox = (SelectAllCheckComboBox)sender;
        var value = e.Item.ToString();
        checkForAll(_checkComboBox, value, e.IsSelected);
    }

    //private void cbx_Loaded(object sender, RoutedEventArgs e)
    //{
    //    blJustProcessed = false;
    //}

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
                blJustProcessed = true;
                _checkComboBox.UnSelectAll();
            }
        }
    }


}
