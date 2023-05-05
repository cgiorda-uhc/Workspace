using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete
{
    public class DataSource
    {
        public DataSource()
        {
        }

        private DateTime _fileDate;
        public DateTime LastUpdateDate   // property
        {
            get { return _fileDate; }
            set { _fileDate = value; }
        }

        private string _fileName;
        public string SourceName   // property
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private string _fileSearch;
        public string SearchString   // property
        {
            get { return _fileSearch; }
            set { _fileSearch = value; }
        }

        private string _filePath;
        public string SourceRoute   // property
        {
            get { return _filePath; }
            set { _filePath = value; }
        }


        private string _tableName;
        public string DestinationName   // property
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        private bool _hasDateFolders;
        public bool HasDateFolders   // property
        {
            get { return _hasDateFolders; }
            set { _hasDateFolders = value; }
        }

        private string _checkType;
        public string SourceType   // property
        {
            get { return _checkType; }
            set { _checkType = value; }
        }


    }
}
