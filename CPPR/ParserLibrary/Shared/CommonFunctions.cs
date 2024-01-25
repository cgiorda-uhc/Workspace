using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

namespace ProjectManagerLibrary.Shared;

public class CommonFunctions
{


    public static string getCleanTableName(string name)
    {
        var arrTable = name.Split('.');
        var table = arrTable[arrTable.Length - 1];
        //CLEAN FILE NAME FOR USE AS TABLE NAME
        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
        {
            table = table.Replace(c, '_');
        }
        table = table.Substring(0, Math.Min(32, table.Length)).ToUpper();

        return table;

    }



    public static string getCreateTmpTableScript(string schema, string table, List<string> columns)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE '" + schema + "' AND name like '" + table + "') DROP TABLE " + schema + "." + table + ";");
        sb.Append("CREATE TABLE [" + schema + "].[" + table + "](");
        foreach (string c in columns)
        {
            sb.Append(" [" + c + "] [VARCHAR](MAX) NULL,");
        }
        return sb.ToString().TrimEnd(',') + ") ON [PRIMARY];";
    }

    public static string getCreateFinalTableScript(string schema, string table, IEnumerable<DataTypeModel> dataTypes)
    {
        StringBuilder sb = new StringBuilder();

        string colType, newType;
        int colLength;
        sb.Append("CREATE TABLE [" + schema + "].[" + table + "](");
        foreach (var d in dataTypes)
        {
            colType = d.ColumnType.Split('-')[1];
            colLength = (d.ColumnLength == 0 ? 1 : d.ColumnLength);
            


            if (colType == "CHAR" || colType == "VARCHAR")
            {
                newType = colType + "(" + colLength + ")";
            }
            else if (colType == "INT")
            {
                if (colLength < 5)
                {
                    newType = "SMALLINT";
                }
                else if (colLength < 10)
                {
                    newType = "INT";
                }
                else if (colLength < 16)
                {
                    newType = "BIGINT";
                }
                else
                {
                    newType = "VARCHAR(" + colLength + ")";
                }
            }
            else
            {
                newType = colType;
            }

            sb.Append(" [" + d.ColumnName.ToUpper() + "] " + newType + " NULL,");

        }
        return "IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE '" + schema + "' AND name like '" + table + "') DROP TABLE " + schema + "." + table + "; " + sb.ToString().TrimEnd(',') + ") ON [PRIMARY];";
    }


    public static string getSelectInsertScript(string schema, string table_src, string tmp_dest, List<string> columns)
    {
        StringBuilder sb = new StringBuilder();

        foreach (string c in columns)
        {
            sb.Append("[" + c + "],");
        }
   
        return "INSERT INTO [" + schema + "].[" + tmp_dest + "] (" + sb.ToString().TrimEnd(',') + ") SELECT " + sb.ToString().TrimEnd(',') + " FROM [" + schema + "].[" + table_src + "]; DROP TABLE [" + schema + "].[" + table_src + "];";

    }


    public static string getTableAnalysisScript(string schema, string table, List<string> columns)
    {
        StringBuilder sb = new StringBuilder();

        //POST PROCESSING TO DETERMIN PROPER DATA TYPES AND LENGTHS
        foreach (var col in columns)
        {
            sb.Append("SELECT ColumnName, MAX(ColumnType) as ColumnType, MAX(ColumnLength) as ColumnLength FROM (");
            sb.Append("SELECT DISTINCT '" + col + "' as ColumnName, ");
            sb.Append("CASE WHEN ISNUMERIC([" + col + "]) = 1 AND LEN([" + col + "]) = 1 AND [" + col + "] NOT LIKE '%[2-9]%' THEN '1-BIT' ELSE ");
            sb.Append("CASE WHEN ISNUMERIC([" + col + "]) = 1 AND CHARINDEX('.',[" + col + "]) > 0 THEN '3-FLOAT' ELSE ");
            sb.Append("CASE WHEN ISNUMERIC([" + col + "]) = 1 AND CHARINDEX('.',[" + col + "]) = 0 THEN '2-INT' ELSE ");
            sb.Append("CASE WHEN ISDATE([" + col + "]) = 1 THEN '4-DATE' ELSE ");
            sb.Append("CASE WHEN LEN([" + col + "]) = 1 AND [" + col + "] LIKE '%[a-z]%' THEN '5-CHAR' ");
            sb.Append("ELSE '6-VARCHAR' ");
            sb.Append("END END END END END AS ColumnType, ");
            sb.Append("MAX(LEN([" + col + "]))  AS ColumnLength ");
            sb.Append("From [" + schema + "].[" + table + "] ");
            sb.Append("WHERE [" + col + "]  IS NOT NULL GROUP BY [" + col + "] ");
            //sb.Append("GROUP BY [" + col + "] ");
            sb.Append(") tmp GROUP BY ColumnName ");
            sb.Append("UNION ALL ");

        }

        return sb.ToString().TrimEnd('U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' ');
    }



    public static void ExtractFromZipFile(string fileName, string workingPath, List<FileConfig> fileConfigs)
    {

        var filepath = workingPath + "\\" + fileName;
        using (ZipArchive archive = ZipFile.OpenRead(filepath))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                foreach (var cfg in fileConfigs)
                {

                    if (string.IsNullOrEmpty(cfg.ZippedMatch))
                    {
                        continue;
                    }

                    if (entry.FullName.ToLower().StartsWith(cfg.ZippedMatch.ToLower()))
                    {
                        var f = Path.Combine(workingPath, entry.FullName);
                        if (!File.Exists(f))
                        {
                            entry.ExtractToFile(f);
                        }
                    }
                }
            }
        }

        File.Delete(filepath);
    }

    public static void ExtractFromZipFile(string fileName, string workingPath, List<FileExcelConfig> fileConfigs)
    {

        var filepath = workingPath + "\\" + fileName;
        using (ZipArchive archive = ZipFile.OpenRead(filepath))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                foreach (var cfg in fileConfigs)
                {

                    if (string.IsNullOrEmpty(cfg.ZippedMatch))
                    {
                        continue;
                    }

                    if (entry.FullName.ToLower().StartsWith(cfg.ZippedMatch.ToLower()))
                    {
                        var f = Path.Combine(workingPath, entry.FullName);
                        if (!File.Exists(f))
                        {
                            entry.ExtractToFile(f);
                        }
                    }
                }
            }
        }

        File.Delete(filepath);
    }

}
