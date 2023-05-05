using Org.BouncyCastle.Bcpg.Sig;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete
{
    public class SQLConfig : ISQLConfig
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public DBType DBType { get; set; }

        public List<string> SQL { get; set; }

        public SQLType SQLType { get; set; }

        public SQLAction SQLAction { get; set; }

        public string Schema { get; set; }

        public int Limit { get; set; }
    }
}
