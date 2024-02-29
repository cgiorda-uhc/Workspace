using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Http.Metadata;
using NPOI.SS.Formula.Functions;
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
using Telerik.Windows.Core;
using Telerik.Windows.Data;
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

        if (lstSelectedProcCode.SelectedItems == null)
            return;
        
        var items = lstSelectedProcCode.SelectedItems; //CANT CAST, MANUAL TRANSFER

        List<string> list = new List<string>();
        foreach (var item in items)
        {
            list.Add(item.ToString());
        }

        foreach (var l in list) 
        {
           
            lstSelectedProcCode.Items.Remove(l);
        }
   
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

    private void btnPasteProcCd_Click(object sender, RoutedEventArgs e)
    {
        List<string> missing_px = new List<string>();
        
        
        var pasted_items = txtProcCodes.Text.Trim().Replace("\r\n", ",").Replace("|",",").Split(',');
        var proc_codes = txtProc_CodeFilter.ItemsSource;

        var sb = new StringBuilder();
        foreach ( var proc_code in proc_codes )
        {
            var value = proc_code.ToString().ToLower().Trim();
            foreach (var item in pasted_items)
            {

                if( value.StartsWith(item.ToLower().Trim() + " - "))
                {

                    lstSelectedProcCode.Items.Add(proc_code);
                    break;
                }
            }
        }
        txtProcCodes.Text = "";



        List<string> list = new List<string>();
        foreach (object item in proc_codes)
            list.Add(item.ToString());

        foreach (var item in pasted_items)
        {
            var r = list.Where(r => r.StartsWith(item.ToUpper().Trim() + " - ")).FirstOrDefault();

            if(r == null )
            {
                sb.AppendLine(item);
            }

        }



        if (sb.Length > 0 )
        {
            MessageBox.Show("Proc Codes not found in DB:" + Environment.NewLine + sb.ToString());
        }


        ////var diff = pasted_items.Except(list);

        ////var diff = list.Where(x => !pasted_items.Contains(x)).ToList();

       
        //foreach (var p in pasted_items)
        //{

        //    if(!list.Contains(p))
        //    {
        //        sb.AppendLine(p);
        //    }

        //}



        //    if (diff != null)
        //{
            
            
        //    foreach(var s in diff)
        //    {
                
        //    }
        //    MessageBox.Show("Proc Codes not found in DB:" + Environment.NewLine + sb.ToString());

        //}
        

    }
}
