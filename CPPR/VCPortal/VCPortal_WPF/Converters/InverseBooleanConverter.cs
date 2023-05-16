using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VCPortal_WPF.Converters;
[ValueConversion(typeof(bool), typeof(bool))]
public class InverseBooleanConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {

        if (value is bool val)
        {
            return !(bool?)val;
        }
        else
        {
            //if (targetType != typeof(bool))
            throw new InvalidOperationException("The target must be a boolean");

        }



    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    #endregion
}
