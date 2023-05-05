using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using System.Linq;
using System.Data;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Windows.Input;

namespace UCS_Project_Manager
{
    /// <summary>
    /// Interaction logic for ETG_Mapping.xaml
    /// </summary>
    public partial class ETG_Fact_Symmetry : UserControl
    {
        //private PagingCollectionView _cview;
        //private PagingCollectionView _cview;
        //private int _intMaxRowCount;
        //private readonly ETG_Mapping_ViewModel _viewModel = new ETG_Mapping_ViewModel();
        public ETG_Fact_Symmetry()
        {
            InitializeComponent();
            this.Loaded += ETG_Fact_Symmetry_Loaded;
            this.dgETGDataGrid.Sorted += OnSorted;
            //this.dgETGDataGrid.Sorting+= OnSorting;


            ((ETG_Fact_Symmetry_ViewModel)this.DataContext).UserName = ActiveDirectoryHelper.strCurrentUser;


            //var t = this.DataContext;


            this.dgETGDataGrid.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * .82);
            //this.dgETGDataGrid.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);


        }

        Window windowGLOBAL;
        void ETG_Fact_Symmetry_Loaded(object sender, RoutedEventArgs e)
        {
            windowGLOBAL = Window.GetWindow(this);
            windowGLOBAL.Closing += window_Closing;
        }

        

        void window_Closing(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            if (File.Exists(GlobalState.strVersionPath))
            {
                File.Delete(GlobalState.strVersionPath);
            }

            //do something before the window is closed...
            if (btnSubmit.IsEnabled == true)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Please Submit changes before exiting. Are your sure you want to exit?", "Exit Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }




        //4212022 PREVIOUS CHANGES CURRENT
        string previousMappingGLOBAL = null;
        private void ECPreviousMappingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            //CLICK COMBO :(
            if (previousMappingGLOBAL == null)
            {
                previousMappingGLOBAL = comboBox.SelectedItem.ToString();
            }
            else //SELECT FROM COMBO
            {
                //IF CHANGE FOUND
                if (previousMappingGLOBAL != comboBox.SelectedItem.ToString())
                {
                    //CHANGE CURRENT TO MATCH PREVIOUS
                    DataGridRow dataGridRow = SharedFunctions.FindParent<DataGridRow>(comboBox);

                    int rowIndex = dataGridRow.GetIndex();
                    var targetColumn = this.dgETGDataGrid.Columns.FirstOrDefault(c => c.Header.ToString().Trim().Equals("EC Current Mapping"));
                    var columnIndex = this.dgETGDataGrid.Columns.IndexOf(targetColumn);


                    DataGridCell dataGridCell = SharedFunctions.GetCell(this.dgETGDataGrid, rowIndex, columnIndex);
                    ComboBox currentMappingCbx = (ComboBox)dataGridCell.Content;
                    //MAKE CURRENT SAME AS NEW PREVIOUS
                    currentMappingCbx.SelectedValue = comboBox.SelectedItem.ToString();
                    //MAKE PREVIOUS PREVIOUS :( REFRESH??? UPDATE????
                    comboBox.SelectedValue = currentMappingCbx.SelectedValue;
           
                }
                previousMappingGLOBAL = null;
            }

        }







        #region GRID FILTER SECTION
        bool blJustProcessed;
        private void cbxPremiumSpecialtyFilter_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if(blJustProcessed)
            {
                blJustProcessed = false;
                return;
            }

            var _checkComboBox = (SelectAllCheckComboBox)sender;
            var value = ((KeyValuePair<string, string>)e.Item).Key;

            checkForAll(_checkComboBox, value, e.IsSelected);

        }
        
        private void cbxPremiumSpecialtyFilter_Loaded(object sender, RoutedEventArgs e)
        {
            blJustProcessed = true;
            var _checkComboBox = (SelectAllCheckComboBox)sender;
            _checkComboBox.SelectAll();
           
        }

        private void cbxETGBaseFilter_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (blJustProcessed)
            {
                blJustProcessed = false;
                return;
            }
            var _checkComboBox = (SelectAllCheckComboBox)sender;
            var value = ((KeyValuePair<string, string>)e.Item).Key;
            checkForAll(_checkComboBox, value, e.IsSelected);
        }

        private void cbxETGBaseFilter_Loaded(object sender, RoutedEventArgs e)
        {
            blJustProcessed = true;
            var _checkComboBox = (SelectAllCheckComboBox)sender;
            _checkComboBox.SelectAll();
         
        }

        private void CbxCurrentAttributionFilter_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
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

        private void CbxCurrentAttributionFilter_Loaded(object sender, RoutedEventArgs e)
        {
            blJustProcessed = true;
            var _checkComboBox = (SelectAllCheckComboBox)sender;
            _checkComboBox.SelectAll();
        }



        private void cbxMeasureStatusFilter_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
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

        private void cbxMeasureStatusFilter_Loaded(object sender, RoutedEventArgs e)
        {
            blJustProcessed = true;
            var _checkComboBox = (SelectAllCheckComboBox)sender;
            _checkComboBox.SelectAll();
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


        #endregion

        #region MENU EVENTS
        private void MenuItem_Click_ExportAll(object sender, RoutedEventArgs e)
        {
            //var t = this.DataContext;
            //var a = dgETGDataGrid.ItemsSource;

            //ExcelHelper.ExportDataSet(this.dgETGDataGrid.Columns.Select(cs => cs.Header).ToList(), "C:\\test");


        }

        private void MenuItem_Click_ExportFiltered(object sender, RoutedEventArgs e)
        {
            var t = this.DataContext;
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {

            windowGLOBAL.Close();

        }

        #endregion

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



        DataGridColumn _datagridColumnGLOBAL;
        object currentPos;
        private void OnSorted(object sender, ValueEventArgs<DataGridColumn> valueEventArgs)
        {
            // Persist Sort...
            _datagridColumnGLOBAL = (DataGridColumn)valueEventArgs.Value;
            currentPos = this.dgETGDataGrid.SelectedItem;
            //this.dgETGDataGrid.ScrollIntoView(1, column);
            this.dgETGDataGrid.ScrollIntoView(this.dgETGDataGrid.Items[0], _datagridColumnGLOBAL);
        }

        private void OnSorting(object sender, DataGridSortingEventArgs e)
        {

            //this.Dispatcher.BeginInvoke((Action)delegate ()
            //{
            //    //runs after sorting is done
            //    var t = (string.Format("sorting grid by '{0}' column in {1} order", e.Column.SortMemberPath, e.Column.SortDirection));

            //    _datagridColumnGLOBAL = e.Column;
            //    currentPos = this.dgETGDataGrid.SelectedItem;
            //    //this.dgETGDataGrid.ScrollIntoView(1, e.Column);

            //}, null);
        }

        private void DgETGDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //if (e.HorizontalChange != 0)
            //{
            //    if (_datagridColumnGLOBAL != null)
            //    {
            //        this.dgETGDataGrid.ScrollIntoView(_datagridColumnGLOBAL);
            //        _datagridColumnGLOBAL = null;

            //    }
            //}
        }



        private void DgETGDataGrid_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //var row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            //if (row == null)
            //{
            //    var cellInfo = this.dgETGDataGrid.SelectedCells[0];
            //    if (cellInfo.Column != null)
            //    {
            //        _datagridColumnGLOBAL = cellInfo.Column;
            //    }
            //    //this.dgETGDataGrid.ScrollIntoView(1, cellInfo.Column);  
            //}

        }

   
        private void MenuItem_Click_Filter(object sender, RoutedEventArgs e)
        {
            SymmetryVersionSelect sv = new SymmetryVersionSelect();
            sv.Symmetry_VerionCurrent.Content = "Current Symmetry Version = " + Symmetry_VerionCurrent.Text;
            sv.Symmetry_VerionFilter.ItemsSource = (string[])Symmetry_VerionFilter.ItemsSource;
            sv.ShowDialog();
        }

        //FOR PG DOWN BUTTON CRASH!!!!
        ScrollViewer _scrollViewer = null;
        private void DgETGDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = GetVisualChild<ScrollViewer>(this.dgETGDataGrid);
            }

            if (_scrollViewer == null) return;

            if (e.Key == Key.PageUp)
            {
                _scrollViewer.PageUp();
                e.Handled = true;
            }
            if (e.Key == Key.PageDown)
            {
                _scrollViewer.PageDown();
                e.Handled = true;
            }
        }

        static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

      



        //private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        //{
        //    string header = ((DataGridColumnHeader)sender).Content.ToString();
        //    var columnHeader = sender as DataGridColumnHeader;
        //    if (columnHeader != null)
        //    {
        //        //_datagridColumnGLOBAL = e.Column;
        //        currentPos = this.dgETGDataGrid.SelectedItem;
        //    }
        //}

    }

}
