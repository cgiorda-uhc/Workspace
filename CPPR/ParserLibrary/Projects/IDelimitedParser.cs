using Microsoft.Extensions.Configuration;
using SharedFunctionsLibrary;
using System.Data;

namespace ProjectManagerLibrary.Projects
{
    public interface IDelimitedParser
    {
        public string TableName { get; set; }

        int getRowCount(string strConnectionString, string strSchemaName);

        List<string?>? getColumnNames(string strConnectionString, string strSchemaName);

        int getColumnCount(string strConnectionString, string strSchemaName);

        long parseDelimitedFiles(string strPathOrFile, char chrDelimiter, string strConnectionString, string strSchemaName, int intBulkSize, int intDelay);
    }
}