using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using Xceed.Wpf.Toolkit;

namespace UCS_Project_Manager
{


    public class InverseAndBooleansToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.LongLength > 0)
            {

                //USED LAST BIND TO OVERRIDE OTHER SO CAN CHANGE WHEN REPORT IS GENERATED
                if ((bool)values[values.Length - 1] == true)
                    return false;

                var cnt = 0;
                foreach (var value in values)
                {

                    //FINAL ITEM OVERRIDES ALL DISABLEING WHEN RUNNING
                    if (cnt == values.Length - 1)
                        continue;

                    if (value + "" == "" || value + "" == "0" )//NULL OR ZERO DISABLE!!!
                    {
                        return false;
                    }

                    cnt++;
                }
            }
            else
                return false;

            
            return true;
        }



        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class RadioButtonCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            //CSG ADDED 5192021
            if (value == null)
                return false;

            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            //CSG ADDED 5192021
            if (value == null)
                return false;

            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }



    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            return !val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            return !val;
        }
    }



    public class AutoCompleteBoxMultiSelect : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //What should I write here !!!
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }


    public class TextBoxHelpers : DependencyObject
    {

        public static bool GetIsNumeric(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNumericProperty);
        }

        public static void SetIsNumeric(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNumericProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsNumeric.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNumericProperty =
     DependencyProperty.RegisterAttached("IsNumeric", typeof(bool), typeof(TextBoxHelpers), new PropertyMetadata(false, new PropertyChangedCallback((s, e) =>
     {
         TextBox targetTextbox = s as TextBox;
         if (targetTextbox != null)
         {
             if ((bool)e.OldValue && !((bool)e.NewValue))
             {
                 targetTextbox.PreviewTextInput -= targetTextbox_PreviewTextInput;

             }
             if ((bool)e.NewValue)
             {
                 targetTextbox.PreviewTextInput += targetTextbox_PreviewTextInput;
                 targetTextbox.PreviewKeyDown += targetTextbox_PreviewKeyDown;
             }
         }
     })));

        static void targetTextbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        static void targetTextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Char newChar = e.Text.ToString()[0];
            e.Handled = !Char.IsNumber(newChar);
        }
    }




    public class SyncedColumnWidthsBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            this.AssociatedObject.LoadingRow += this.SyncColumnWidths;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.LoadingRow -= this.SyncColumnWidths;
        }

        private void SyncColumnWidths(object sender, DataGridRowEventArgs e)
        {
            var dataGrid = this.AssociatedObject;

            foreach (DataGridColumn c in dataGrid.Columns)
                c.Width = 0;

            e.Row.UpdateLayout();

            foreach (DataGridColumn c in dataGrid.Columns)
                c.Width = DataGridLength.Auto;
        }
    }



    public class EpisodeCountComparisonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
                              object parameter, CultureInfo culture)
        {
            bool bFlagItem = false;
            if (values.Length == 5)
            {

                int outputValue = int.MinValue;
                if (Int32.TryParse(values[0].ToString(), out outputValue))
                {





                    //outputValue
                    string strPremiumSpecialty = values[1].ToString();
                    int intGroupValue = (int)values[2];
                    string[] strArrPSGroup = values[3].ToString().Split(',');
                    int intSingleValue = (int)values[4];


                    /*
                                   <sys:Int32 x:Key="GroupValue">1000</sys:Int32>
                <sys:String x:Key="PSGroup">FAMED,INTMD,PEDS</sys:String>
                <sys:Int32 x:Key="SingleValue">500</sys:Int32>*/

                    if (strArrPSGroup.Contains(strPremiumSpecialty) && outputValue < intGroupValue)
                        bFlagItem = true;
                    else if (outputValue < intSingleValue)
                        bFlagItem = true;
                    else
                        bFlagItem = false;


                    //itemsExistInRange = minValue <= outputValue
                    // && outputValue <= maxValue;
                }
            }
            return bFlagItem;
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
                                    object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class SearchFilterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
                              object parameter, CultureInfo culture)
        {
           return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
                                    object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class SelectAllCheckComboBox : CheckComboBox
    {
        protected override void UpdateText()
        {
            //Do not display the “Select All” in the TextBox.
            var selectedItemsList = this.SelectedItems.Cast<object>().Select(x => this.GetItemDisplayValue(x)).Where(x => !x.Equals("--All--"));

            var newValue = String.Join(this.Delimiter, selectedItemsList);
            if (String.IsNullOrEmpty(this.Text) || !this.Text.Equals(newValue))
            {
                this.SetCurrentValue(CheckComboBox.TextProperty, newValue);
            }
        }
    }

    public class SelectAllCheckComboBoxTilde : CheckComboBox
    {
        protected override void UpdateText()
        {
            //Do not display the “Select All” in the TextBox.
            var selectedItemsList = this.SelectedItems.Cast<object>().Select(x => this.GetItemDisplayValue(x)).Where(x => !x.Equals("--All--"));

            var newValue = String.Join("~", selectedItemsList);
            if (String.IsNullOrEmpty(this.Text) || !this.Text.Equals(newValue))
            {
                this.SetCurrentValue(CheckComboBox.TextProperty, newValue);
            }
        }
    }

}
