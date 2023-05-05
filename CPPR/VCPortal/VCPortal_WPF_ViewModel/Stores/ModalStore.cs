using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Stores;
public class ModalStore
{

    public event Action CurrentModelChanged;

    private ViewModelBase _currentViewModel;

    public ViewModelBase CurrentViewModel 
    { 
        get => _currentViewModel;
        set
        {
            _currentViewModel?.Dispose();
            _currentViewModel = value;
            OnCurrentModelChanged();
        }
    }

    private void OnCurrentModelChanged()
    {
        CurrentModelChanged?.Invoke();
    }
}
