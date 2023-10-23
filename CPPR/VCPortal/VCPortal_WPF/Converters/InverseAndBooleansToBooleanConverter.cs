using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VCPortal_WPF.Converters
{
    public class InverseAndBooleansToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.LongLength > 0)
            {

                ////USED LAST BIND TO OVERRIDE OTHER SO CAN CHANGE WHEN REPORT IS GENERATED
                //if ((bool)values[values.Length - 1] == true)
                //    return false;

                var cnt = 0;
                foreach (var value in values)
                {

                    //FINAL ITEM OVERRIDES ALL DISABLEING WHEN RUNNING
                    if (cnt == values.Length - 1)
                        continue;

                    if (value == null)
                        continue;

                    var t = value.GetType();
                    if (t== typeof(System.Windows.Controls.ItemCollection))
                    {
                        if(( value as System.Windows.Controls.ItemCollection).Count <= 0)
                        {
                            return false;
                        }
                        
                        
                    }
                    else if (value + "" == "" || value + "" == "0")//NULL OR ZERO DISABLE!!!
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
}
