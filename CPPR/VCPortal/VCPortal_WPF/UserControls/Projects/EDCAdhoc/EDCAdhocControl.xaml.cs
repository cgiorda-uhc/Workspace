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

namespace VCPortal_WPF.UserControls.Projects.EDCAdhoc;
/// <summary>
/// Interaction logic for EDCAdhoc.xaml
/// </summary>
public partial class EDCAdhocControl : UserControl
{
    public EDCAdhocControl()
    {
        InitializeComponent();

        modalContentControl.Content = new StatusControl();
    }


    private void btnSwitchReport_Click(object sender, RoutedEventArgs e)
    {
        var content = (sender as Button).Content.ToString();
    }




    private void btnAddAC_Click(object sender, RoutedEventArgs e)
    {
        var value = txtAC_Filter.SearchText;
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        lstACFilters.Items.Add(value);
        txtAC_Filter.SearchText = "";

    }


    private void btnRemoveSelectedAC_Click(object sender, RoutedEventArgs e)
    {
        var item = lstACFilters.SelectedItem;
        if (item == null)
        {
            return;
        }

        lstACFilters.Items.Remove(item);

    }

    private void btnRemoveAllAC_Click(object sender, RoutedEventArgs e)
    {
        lstACFilters.Items.Clear();
    }


    bool blJustProcessed;
    private void cbx_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
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

    private void cbx_Loaded(object sender, RoutedEventArgs e)
    {
        blJustProcessed = false;
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
