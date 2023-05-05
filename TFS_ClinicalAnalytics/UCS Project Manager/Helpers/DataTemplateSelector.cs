using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace UCS_Project_Manager
{
    public class AutoCompleteDataTemplateSelector : DataTemplateSelector
    {


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {


            if (item is string)
                return ((FrameworkElement)container).FindResource("WaitTemplate") as DataTemplate;
            else
                return ((FrameworkElement)container).FindResource("TheItemTemplate") as DataTemplate;


        }

    }
}

