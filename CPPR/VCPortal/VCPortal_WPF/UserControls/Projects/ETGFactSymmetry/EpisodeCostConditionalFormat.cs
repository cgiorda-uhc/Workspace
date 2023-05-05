using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls.GridView;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;

namespace VCPortal_WPF.UserControls.Projects.ETGFactSymmetry;

public class EpisodeCountConditionalFormat : StyleSelector
{

    //https://docs.telerik.com/devtools/wpf/controls/radgridview/style-selectors/cell-style-selector
    public override Style SelectStyle(object item, DependencyObject container)
    {

        if (item is ETGFactSymmetryViewModel)
        {
            ETGFactSymmetryViewModel etg = item as ETGFactSymmetryViewModel;
            var cell = (GridViewCell)container;
            bool is_group = false;
            if(etg.Premium_Specialty == "FAMED" || etg.Premium_Specialty == "INTMD" || etg.Premium_Specialty == "PEDS")
            {
                is_group = true;
            }


           if (cell.Column.Name == "PC_Spec_Episode_Count")
            {
                if ((is_group && etg.PC_Spec_Episode_Count < 1000) || etg.PC_Spec_Episode_Count < 500)
                {
                    return FlaggedStyle;
                }
            }
            else if (cell.Column.Name == "EC_Episode_Count")
            {
                if ((is_group && etg.EC_Episode_Count < 1000) || etg.EC_Episode_Count < 500)
                {
                    return FlaggedStyle;
                }
            }
            else if (cell.Column.Name == "EC_Spec_Episode_Count")
            {
                if ((is_group && etg.EC_Spec_Episode_Count < 1000) || etg.EC_Spec_Episode_Count < 500)
                {
                    return FlaggedStyle;
                }
            }

        }
        return DefaultStyle;
    }
    public Style FlaggedStyle { get; set; }
    public Style DefaultStyle { get; set; }


}

//public class EpisodeCostConditionalFormat : IMultiValueConverter
//{
//    public object Convert(object[] values, Type targetType,
//                          object parameter, CultureInfo culture)
//    {
//        bool bFlagItem = false;
//        if (values.Length == 5)
//        {

//            int outputValue = int.MinValue;
//            if (Int32.TryParse(values[0].ToString(), out outputValue))
//            {

//                //outputValue
//                string strPremiumSpecialty = values[1].ToString();
//                int intGroupValue = (int)values[2];
//                string[] strArrPSGroup = values[3].ToString().Split(',');
//                int intSingleValue = (int)values[4];

//                if (strArrPSGroup.Contains(strPremiumSpecialty) && outputValue < intGroupValue)
//                    bFlagItem = true;
//                else if (outputValue < intSingleValue)
//                    bFlagItem = true;
//                else
//                    bFlagItem = false;


//                //itemsExistInRange = minValue <= outputValue
//                // && outputValue <= maxValue;
//            }
//        }
//        return bFlagItem;
//    }

//    public object[] ConvertBack(object value, Type[] targetTypes,
//                                object parameter, CultureInfo culture)
//    {
//        throw new NotImplementedException();
//    }
//}
