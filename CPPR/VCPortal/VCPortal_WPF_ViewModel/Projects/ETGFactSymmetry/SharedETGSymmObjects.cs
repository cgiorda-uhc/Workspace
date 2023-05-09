using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Dtos.ETGFactSymmetry;

namespace VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
public static class SharedETGSymmObjects
{
    public static ObservableCollection<ETGFactSymmetry_Tracking_UpdateDto> ETGFactSymmetry_Tracking_List { get; set; }
}
