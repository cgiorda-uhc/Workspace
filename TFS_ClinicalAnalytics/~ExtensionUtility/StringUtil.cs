using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class StringUtil
    {
        public static string getSafeFileName(this string strFileName)
        {
            return string.Join("_", strFileName.Split(Path.GetInvalidFileNameChars()));

        }
    }



}
