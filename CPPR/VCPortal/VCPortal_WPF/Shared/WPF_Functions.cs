using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace VCPortal_WPF.Shared;
public class WPF_Functions
{
    public static Parent FindParent<Parent>(DependencyObject child)
            where Parent : DependencyObject
    {
        DependencyObject parentObject = child;

        //We are not dealing with Visual, so either we need to fnd parent or
        //get Visual to get parent from Parent Heirarchy.
        while (!((parentObject is System.Windows.Media.Visual)
                || (parentObject is System.Windows.Media.Media3D.Visual3D)))
        {
            if (parentObject is Parent || parentObject == null)
            {
                return parentObject as Parent;
            }
            else
            {
                parentObject = (parentObject as FrameworkContentElement).Parent;
            }
        }

        //We have not found parent yet , and we have now visual to work with.
        parentObject = VisualTreeHelper.GetParent(parentObject);

        //check if the parent matches the type we're looking for
        if (parentObject is Parent || parentObject == null)
        {
            return parentObject as Parent;
        }
        else
        {
            //use recursion to proceed with next level
            return FindParent<Parent>(parentObject);
        }
    }


    internal static double GetColumnXPosition(Telerik.Windows.Controls.GridViewColumn column, RadGridView grid)
    {
        double result = 0.0;

        if (grid == null)
            return result;

        for (int i = 0; i < grid.Columns.Count; i++)
        {
            Telerik.Windows.Controls.GridViewColumn dgc = grid.Columns[i];
            if (dgc.Equals(column))
                break;

            result += dgc.ActualWidth;
        }
        return result;
    }
    internal static RadGridView GetRowsDataGrid(GridViewRow row)
    {
        DependencyObject result = VisualTreeHelper.GetParent(row);
        while (result != null && !(result is RadGridView))
        {
            result = VisualTreeHelper.GetParent(result);
        }
        return result as RadGridView;
    }

}



