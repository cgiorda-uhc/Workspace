using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace UCS_Project_Manager
{
    public class ModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Property changed
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName, Action<bool> message)
        {
            if (this.PropertyChanged != null)
            {
                // property changed
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                // send app message (mvvm light toolkit)
                if (message != null)
                    message(this.IsValid);
            }
        }


        //CHRIS ADDED
        protected void NotifyPropertyChanged(string propertyName, Action<string> message)
        {
            if (this.PropertyChanged != null)
            {
                // property changed
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                // send app message (mvvm light toolkit)
                if (message != null)
                    message(propertyName);
            }
        }


        //CHRIS ADDED
        protected void NotifyPropertyChanged(Action<ETG_Fact_Symmetry_Update_Tracker> message, ETG_Fact_Symmetry_Update_Tracker mess)
        {
            if (this.PropertyChanged != null)
            {
                // property changed
                this.PropertyChanged(this, new PropertyChangedEventArgs("message"));
                // send app message (mvvm light toolkit)
                if (message != null)
                    message(mess);
            }
        }
        #endregion



        #region Notify data error
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        // get errors by property
        public IEnumerable GetErrors(string propertyName)
        {
            if (this._errors.ContainsKey(propertyName))
                return this._errors[propertyName];
            return null;
        }

        // has errors
        public bool HasErrors
        {
            get { return (this._errors.Count > 0); }
        }

        // object is valid
        public bool IsValid
        {
            get { return !this.HasErrors; }

        }

        public void AddError(string propertyName, string error)
        {
            // Add error to list
            this._errors[propertyName] = new List<string>() { error };
            this.NotifyErrorsChanged(propertyName);
        }

        public void RemoveError(string propertyName)
        {
            // remove error
            if (this._errors.ContainsKey(propertyName))
                this._errors.Remove(propertyName);
            this.NotifyErrorsChanged(propertyName);
        }

        public void NotifyErrorsChanged(string propertyName)
        {
            // Notify
            if (this.ErrorsChanged != null)
                this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
        #endregion


    }
}
