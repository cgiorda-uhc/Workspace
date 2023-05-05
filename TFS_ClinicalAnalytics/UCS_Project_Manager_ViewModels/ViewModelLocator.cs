using GalaSoft.MvvmLight;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Collections.Generic;
using UCS_Project_Manager_Services;
using System.Linq;

namespace UCS_Project_Manager
{


    public static class ViewModelLocator
    {
       
        public static bool GetAutoHookedUpViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoHookedUpViewModelProperty);
        }

        public static void SetAutoHookedUpViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoHookedUpViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoHookedUpViewModel. 

        //This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoHookedUpViewModelProperty =
           DependencyProperty.RegisterAttached("AutoHookedUpViewModel",
           typeof(bool), typeof(ViewModelLocator), new
           PropertyMetadata(false, AutoHookedUpViewModelChanged));

        private static void AutoHookedUpViewModelChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            //if (DesignerProperties.GetIsInDesignMode(d)) return; //NEVER CALLED ?????
            DesignerProperties.GetIsInDesignMode(d); //KEEPS NULL ERROR OFF XAML ????
  
            Type viewModelType = Type.GetType(d.GetType().FullName + "_ViewModel");

            object objRepository = null;
            Type[] aTypes = Assembly.Load("UCS_Project_Manager_Services").GetExportedTypes();
            foreach(Type t in aTypes)
            {
                //if(t.Name == "ProjectIntakeSample1_Repository")
                if (t.Name == d.GetType().FullName.Replace("UCS_Project_Manager.", "") + "_Repository")
                {
                    objRepository = Activator.CreateInstance(t);
                    break;
                }
            }

            var viewModel =Activator.CreateInstance(viewModelType, objRepository);
            ((FrameworkElement)d).DataContext = viewModel;

        }
    }
    
}
