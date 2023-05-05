
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_WPF_ViewModel.Shared;

//ONE VM TO RULE THEM ALL!!!
public interface ViewModelBase : IDisposable
{
    //public event PropertyChangedEventHandler PropertyChanged;

    //protected void OnPropertyChanged(string propertyName)
    //{
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));  
    //}

}
