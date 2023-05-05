using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS_Project_Manager
{
    static class GlobalState
    {
        public static bool IsDesignMode = false;
        public static string strVersionPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\params.txt";
    }
}
