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
using System.ComponentModel;

namespace UCS_Project_Manager
{
    /// <summary>
    /// Interaction logic for MHP_Yearly_Universes_Reporting.xaml
    /// </summary>
    public partial class MHP_Yearly_Universes_Reporting : UserControl
    {



        public MHP_Yearly_Universes_Reporting()
        {
            InitializeComponent();


        }


        #region CHANGE TITLE
        public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.RegisterAttached("WindowTitleProperty",
                typeof(string), typeof(UserControl),
                new FrameworkPropertyMetadata(null, WindowTitlePropertyChanged));

        public static string GetWindowTitle(DependencyObject element)
        {
            return (string)element.GetValue(WindowTitleProperty);
        }

        public static void SetWindowTitle(DependencyObject element, string value)
        {
            element.SetValue(WindowTitleProperty, value);
        }

        private static void WindowTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Application.Current.MainWindow.Title = e.NewValue + "";
        }


        #endregion

        bool blJustProcessed;



        private void CbxMKT_TYP_DESCFilter_Loaded(object sender, RoutedEventArgs e)
        {
            blJustProcessed = false;
            //var _checkComboBox = (SelectAllCheckComboBox)sender;
            //_checkComboBox.SelectAll();
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




        private void CbxMKT_SEG_RLLP_DESCFilter_Loaded(object sender, RoutedEventArgs e)
        {
            blJustProcessed = false;
            //var _checkComboBox = (SelectAllCheckComboBox)sender;
            //_checkComboBox.SelectAll();
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


        private void CbxLegalEntityFilter_Loaded(object sender, RoutedEventArgs e)
        {
            blJustProcessed = false;
            //var _checkComboBox = (SelectAllCheckComboBox)sender;
            //_checkComboBox.SelectAll();
        }


        private void CbxLegalEntityFilter_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
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


        private void CbxFINC_ARNG_CDFilter_Loaded(object sender, RoutedEventArgs e)
        {
            blJustProcessed = false;
            //var _checkComboBox = (SelectAllCheckComboBox)sender;
            //_checkComboBox.SelectAll();
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

        private void CbxStateFilter_Loaded(object sender, RoutedEventArgs e)
        {
            blJustProcessed = false;
            //var _checkComboBox = (SelectAllCheckComboBox)sender;
            //_checkComboBox.SelectAll();
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
            //var _checkComboBox = (SelectAllCheckComboBox)sender;
            //_checkComboBox.SelectAll();
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
            //var _checkComboBox = (SelectAllCheckComboBox)sender;
            //_checkComboBox.SelectAll();
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
            //var _checkComboBox = (SelectAllCheckComboBox)sender;
            //_checkComboBox.SelectAll();
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private Boolean AutoScroll = true;
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (ScrollViewer.VerticalOffset == ScrollViewer.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set auto-scroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and auto-scroll mode set
                // Autoscroll
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ExtentHeight);
            }
        }

        private void ReportType_Checked(object sender, RoutedEventArgs e)
        {
            cbxProductCode.UnSelectAll();
            cbxStateFilter.UnSelectAll();
            cbxLegalEntityFilter.UnSelectAll();
            cbxFINC_ARNG_CDFilter.UnSelectAll();
            cbxMKT_SEG_RLLP_DESCFilter.UnSelectAll();
            cbMKT_TYP_DESCFilter.UnSelectAll();
            txtCUST_SEGFilter.Text = "";
            cbxCS_TADM_PRDCT_MAP.UnSelectAll();
            cbxGroupNumbers.UnSelectAll();
        }

      
    }
}
