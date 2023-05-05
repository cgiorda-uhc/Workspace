using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfaces.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Core.ViewModels
{
    [INotifyPropertyChanged]
    public partial class MHP_UM_Outcomes_ViewModel //: ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NotifyMe))]
        private List<MHP_Summary_Model> _mhp_summary; 
        //VS. THIS!!!!
        //private ObservableCollection<MHP_Summary_Model> _mhp_summary;
        //public ObservableCollection<MHP_Summary_Model> MHP_Summary
        //{
        //    get { return _mhp_summary; }
        //    set 
        //    {
        //       bool propChanged = SetProperty(ref _mhp_summary, value); 
        //        //if(propChanged)
        //        //{
        //        //    OnPropertyChanged(nameof(MHP_Details));
        //        //}
        //    }
        //    //set { _mhp_summary = value; }
        //}

        [ObservableProperty]
        private List<MHP_Details_Model> _mhp_details;
        //VS. THIS!!!!
        //private ObservableCollection<MHP_Details_Model> _mhp_details;
        //public ObservableCollection<MHP_Details_Model> MHP_Details
        //{
        //    get { return _mhp_details; }
        //    set
        //    {
        //        SetProperty(ref _mhp_details, value);
        //    }
        //    //set { _mhp_details = value; }
        //}

        public string NotifyMe  =>   "";


        [RelayCommand]
        private async Task Search(object item)
        {

          

        }






    }
}
