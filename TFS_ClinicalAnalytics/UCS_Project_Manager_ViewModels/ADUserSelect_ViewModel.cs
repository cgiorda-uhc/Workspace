using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Web;
using System.Xml;
using System.Net;
using System.Diagnostics;

namespace UCS_Project_Manager
{
    public class ADUserSelect_ViewModel : INotifyPropertyChanged
    {

        private List<string> _WaitMessage = new List<string>() { "Please Wait..." };
        public IEnumerable WaitMessage { get { return _WaitMessage; } }


        private ActiveDirectoryHelper _adh;
        private ADUserDetail _currentADUser;
        public ADUserSelect_ViewModel()
        {
            if (_adh == null)
                _adh = new ActiveDirectoryHelper();

            if (_currentADUser == null)
                _currentADUser = _adh.GetUserByLoginName(ActiveDirectoryHelper.strCurrentUser);

            CurrentSelectedADUser = _currentADUser;
          

        }

        //AUTOCOMPLETE START
        //AUTOCOMPLETE START
        //AUTOCOMPLETE START
        private string _QueryText;
        public string QueryText
        {
            get { return _QueryText; }
            set
            {
                if (_QueryText != value)
                {
                    _QueryText = value;
                    OnPropertyChanged("QueryText");
                    _ADCollection = null;
                    OnPropertyChanged("ADCollection");
                    //Debug.Print("QueryText: " + value);
                }
            }
        }

        public IEnumerable _ADCollection = null;
        public IEnumerable ADCollection
        {
            get
            {
                QueryAD(QueryText);
                return _ADCollection;
            }
        }
       
        private void QueryAD(string strQuery = "")
        {
            if (!string.IsNullOrEmpty(strQuery))
                strQuery = System.Text.RegularExpressions.Regex.Replace(strQuery, "[^a-zA-Z ]", String.Empty);


            if (!string.IsNullOrEmpty(strQuery))
            {

                //Debug.Print("Query: " + strQuery);
                //string sanitized = HttpUtility.HtmlEncode(strQuery);
                //string url = @"http://google.com/complete/search?output=toolbar&q=" + sanitized;
                //WebRequest httpWebRequest = HttpWebRequest.Create(url);
                //var webResponse = httpWebRequest.GetResponse();
                //XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Load(webResponse.GetResponseStream());
                //var result = xmlDoc.SelectNodes("//CompleteSuggestion");
                //_ADCollection = result;

                var result = _adh.GetUsersByName(strQuery);
                _ADCollection = result;

            }
            else
            {
               _ADCollection = null;
            }
        }

        //LABEL SELECTED START
        //LABEL SELECTED START
        //LABEL SELECTED START
        private ADUserDetail _currentSelectedADUser;
        public ADUserDetail CurrentSelectedADUser
        {
            get { return _currentSelectedADUser; }
            set { _currentSelectedADUser = value; }
        }

        //EVENTS
        //EVENTS
        //EVENTS
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string s)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(s));
            }
        }

    }
}
