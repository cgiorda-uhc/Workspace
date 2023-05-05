using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_WPF_ViewModel.Shared;
public partial class MessageViewModel : ObservableObject, ViewModelBase
{


    [ObservableProperty]
    private string message;


    [ObservableProperty]
    private bool hasMessage;



    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
