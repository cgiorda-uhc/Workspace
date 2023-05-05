using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace UCS_Project_Manager_Models
{
    public class RelayCommand : ICommand
    {
        Action _TargetExecuteMethod;
        //CSG ADDED PARAMETER OPTIONS
        Action<object> _TargetExecuteMethodWithParameters;
        Func<bool> _TargetCanExecuteMethod;
        object[] _objParameterArr;

        public RelayCommand(Action executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }
        //CSG ADDED PARAMETER OPTIONS
        public RelayCommand(Action<object> executeMethodWithParameters)
        {
            _TargetExecuteMethodWithParameters = executeMethodWithParameters;
           // _objParameterArr = objParameterArr;
        }


        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {

            if (_TargetCanExecuteMethod != null)
            {
                return _TargetCanExecuteMethod();
            }

            if (_TargetExecuteMethod != null || _TargetExecuteMethodWithParameters != null)
            {
                return true;
            }

            return false;
        }
		
      // Beware - should use weak references if command instance lifetime  is longer than lifetime of UI objects that get hooked up to command
      // Prism commands solve this in their implementation 
        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod();
            }
            else if (_TargetExecuteMethodWithParameters != null)//CSG ADDED PARAMETER OPTIONS
            {
                _TargetExecuteMethodWithParameters?.Invoke(parameter);
            }
        }
    }

}
