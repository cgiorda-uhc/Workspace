using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using DataAccessLibrary.Scripts;
using DataAccessLibrary.Shared;
using FileParsingLibrary.Models;
using FileParsingLibrary.MSWord;
using ProjectManagerLibrary.Models;
using ProjectManagerLibrary.Shared;
using SharedFunctionsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleLibraryTesting
{
    public class AdHoc
    {
        public string ConnectionStringMSSQL { get; set; }
        public string TableMHP { get; set; }
        public string ConnectionStringTD { get; set; }
        public string TableUGAP { get; set; }
        public int Limit { get; set; }


        public async Task  runSLAAutomation()
        {
            var date = "03/01/2022";
            var last_thursday = AdHoc.GetLastChosenDayOfTheMonth(DateTime.ParseExact(date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture), DayOfWeek.Thursday);

            //DB
            string connectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
            IRelationalDataAccess db_sql = new SqlDataAccess();
            var results = await db_sql.LoadData<MonthlySLAReviewModel, dynamic>(connectionString: connectionString, storedProcedure: "dbo.sp_Monthly_SLA_Review", new { Date = date });
            //var results = await db_sql.LoadData<MonthlySLAReviewModel, dynamic>(connectionString: connectionString, storedProcedure: "dbo.sp_Monthly_SLA_Review", new {});


            //WORD
            var fontType = "Times New Roman";
            var fontSize = 12;
            var bold = false;
            string file = "C:\\Users\\cgiorda\\Desktop\\Projects\\Monthly SLA Review Call\\Monthly SLA Review Call_template.docx";
            string file_OUT = "C:\\Users\\cgiorda\\Desktop\\Projects\\Monthly SLA Review Call\\AutomatedMonthlySLASample_" + date.Replace("/", "_") + ".docx";
            var writer = new InteropWordFunctions(file);


            //PROCESS
            var bookmark_name = "";
            var currentModality = "";
            var text = "";
            var color = System.Drawing.Color.Black;
            List<MSWordFormattedText> lst = new List<MSWordFormattedText>();
            foreach (var row in results)
            {
                if (currentModality != row.Modality)
                {
                    //IF LIST IS POPULATED, PROCESS IT
                    if (lst.Count > 0)
                    {
                        writer.addBulletedList(bookmark_name, lst, 2);

                        lst = new List<MSWordFormattedText>();
                    }
                    currentModality = row.Modality;
                }


                bookmark_name = (row.LOB + "_" + row.Modality).Replace("&", "").ToLower();


                if (row.Penalty_SLA != 0)
                {
                    text = row.Miss.Replace("[SLA]", row.SLA.ToString()).Replace("[Percentage]", row.Percentage.ToString());
                    color = System.Drawing.Color.Red;
                }
                else
                {
                    text = row.Hit;
                    color = System.Drawing.Color.Black;
                }

                lst.Add(new MSWordFormattedText() { Text = text, Bold = false, FontType = fontType, FontSize = fontSize, ForeColor = color });
            }
            if (lst.Count > 0)
            {
                writer.addBulletedList(bookmark_name, lst, 2);
            }


            writer.FindAndReplaceInHeader("[Date]", last_thursday.ToString("MMMM") + " " + last_thursday.Day + ", " + last_thursday.Year);

            if (System.IO.File.Exists(file_OUT))
                System.IO.File.Delete(file_OUT);

            writer.Save(file_OUT);

            writer.DisposeWordInstance();

            return;
        }





        public static  DateTime GetLastChosenDayOfTheMonth(DateTime date, DayOfWeek dayOfWeek)
        {
            var lastDayOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            while (lastDayOfMonth.DayOfWeek != dayOfWeek)
                lastDayOfMonth = lastDayOfMonth.AddDays(-1);

            return lastDayOfMonth;
        }




        public async Task cleanupMemberDataAsync(List<string> files_loaded)
        {

            var files_csv = "'" + string.Join("','", files_loaded.Select(n => n.ToString()).ToArray()) + "'";


            //TWO DBS
            IRelationalDataAccess db_td = new TeraDataAccess();
            IRelationalDataAccess db_sql = new SqlDataAccess();

            //DRIVING LOOP
            var parameters = MHPCustomSQL.MHPParameters();

            string sql;
            StringBuilder sbSQL = new StringBuilder();

            int total;
            int total_counter;
            int limit_counter;
            var columns = typeof(MHPMemberDetailsModel).GetProperties().Select(p => p.Name).ToArray();
            foreach (var param in parameters)
            {
                sql = MHPCustomSQL.MSSQLMHPMember(TableMHP, TableUGAP, files_csv, param.MHPSQL);
                //FIND CURRENT MEMBERS
                var mhp_search = (await db_sql.LoadData<MHPMemberSearchModel>(connectionString: ConnectionStringMSSQL, sql));
                total = mhp_search.Count();
                Console.WriteLine(total + " records found");
                total_counter = 0;
                limit_counter = 0;

                foreach (var m in mhp_search)
                {
                    sbSQL.Append(MHPCustomSQL.UGAPVolatileInsert(m, param));
                    limit_counter++;
                    total_counter++;
                    if (limit_counter == Limit)
                    {
                        Console.WriteLine("Searching UGAP for " + total_counter + " out of " + total);
                        if (param.LOS == LOS.EI || param.LOS == LOS.EI_OX)
                            sql = MHPCustomSQL.UGAPSQLLMemberDataEI(param.UGAPSQL, param.LOS == LOS.EI_OX).Replace("{$Inserts}", sbSQL.ToString());
                        else
                            sql = MHPCustomSQL.UGAPSQLMemberDataCS(param.UGAPSQL, param.LOS == LOS.CS).Replace("{$Inserts}", sbSQL.ToString());

                        var ugap = await db_td.LoadData<MHPMemberDetailsModel>(connectionString: ConnectionStringTD, sql);
                        foreach (var u in ugap)
                        {
                            u.SearchMethod = param.SearchMethod;
                        }

                        Console.WriteLine("Loading " + ugap.Count() + " UGAP rows into MHP source.");
                        await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: ConnectionStringMSSQL, TableUGAP, ugap, columns);



                        sbSQL.Remove(0, sbSQL.Length);
                        limit_counter = 0;
                    }
                }
                //FINISHED BEFORE LIMIT SO PROCESS REMAINDER
                if (sbSQL.Length > 0)
                {
                    Console.WriteLine("Searching UGAP for " + total_counter + " out of " + total);

                    if (param.LOS == LOS.EI || param.LOS == LOS.EI_OX)
                        sql = MHPCustomSQL.UGAPSQLLMemberDataEI(param.UGAPSQL, param.LOS == LOS.EI_OX).Replace("{$Inserts}", sbSQL.ToString());
                    else
                        sql = MHPCustomSQL.UGAPSQLMemberDataCS(param.UGAPSQL, param.LOS == LOS.CS).Replace("{$Inserts}", sbSQL.ToString());

                    var ugap = await db_td.LoadData<MHPMemberDetailsModel>(connectionString: ConnectionStringTD, sql);
                    foreach (var u in ugap)
                    {
                        u.SearchMethod = param.SearchMethod;
                    }

                    Console.WriteLine("Loading " + ugap.Count() + " UGAP rows into MHP source.");
                    await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: ConnectionStringMSSQL, TableUGAP, ugap, columns);

                    sbSQL.Remove(0, sbSQL.Length);

                }

            }
        }



        public async Task UGAPConfig()
        {

            char chrDelimiter = '|';
            List<string>? strLstColumnNames = null;
            StreamReader? csvreader = null;
            string _strTableName;
            //string[] strLstFiles;
            string[] strLstFiles = Directory.GetFiles(@"C:\Users\cgiorda\Desktop\Projects\UGAP Configuration", "*.txt", SearchOption.TopDirectoryOnly);
            string? strInputLine = "";
            string[] csvArray;
            string strSQL;
            int intBulkSize = 10000;
            var connectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
            var tdConnectionString = "Data Source=UDWPROD;User ID=cgiorda;Password=BooWooDooFoo2023!!;Authentication Mechanism=LDAP;Session Mode=TERADATA;Session Character Set=ASCII;Persist Security Info=true;Connection Timeout=99999;";
            IRelationalDataAccess db_sql = new SqlDataAccess();
            IRelationalDataAccess db_td = new TeraDataAccess();
            System.Data.DataTable dtTransfer = new System.Data.DataTable();
            System.Data.DataRow? drCurrent = null;
            string filename;



            //1 GET FILES

            foreach (var strFile in strLstFiles)
            {
                filename = "ugapcfg_" + Path.GetFileName(strFile).Replace(".txt", "");

                var table = CommonFunctions.getCleanTableName(filename);
                var tmp_table = table.Substring(0, Math.Min(28, table.Length)) + "_TMP";


                csvreader = new StreamReader(strFile);
                while ((strInputLine = csvreader.ReadLine()) != null)
                {
                    csvArray = strInputLine.Split(new char[] { chrDelimiter });
                    //FIRST PASS ONLY GETS COLUMNS AND CREATES TABLE SQL
                    if (strLstColumnNames == null)
                    {
                        strLstColumnNames = new List<string>();
                        //GET AND CLEAN COLUMN NAMES FOR TABLE
                        foreach (string c in csvArray)
                        {
                            var colName = c.getSafeFileName();
                            strLstColumnNames.Add(colName.ToUpper());
                        }


                        //SQL FOR TMP TABLE TO STORE ALL VALUES A VARCHAR(MAX)
                        strSQL = CommonFunctions.getCreateTmpTableScript("stg", tmp_table, strLstColumnNames);
                        await db_sql.Execute(connectionString: connectionString, strSQL);

                        strSQL = "SELECT * FROM [stg].[" + tmp_table + "]; ";
                        //CREATE TMP TABLE AND COLLECT NEW DB TABLE FOR BULK TRANSFERS
                        dtTransfer = await db_sql.LoadDataTable(connectionString, strSQL);
                        dtTransfer.TableName = "stg." + tmp_table;

                        //GOT COLUMNS, CREATED TMP TABLE FOR FIRST PASS
                        continue;
                    }
                    //CLONE ROW FOR TRANSFER
                    drCurrent = dtTransfer.NewRow();
                    //POPULATE ALL COLUMNS FOR CURRENT ROW
                    for (int i = 0; i < strLstColumnNames.Count; i++)
                    {
                        drCurrent[strLstColumnNames[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : csvArray[i].TrimStart('\"').TrimEnd('\"'));

                    }
                    dtTransfer.Rows.Add(drCurrent);

                    if (dtTransfer.Rows.Count == intBulkSize) //intBulkSize = 10000 DEFAULT
                    {
                        await db_sql.BulkSave(connectionString: connectionString, dtTransfer);
                        dtTransfer.Rows.Clear();
                    }


                }

                //CATCH REST OF UPLOADS OUTSIDE CSV LOOP
                if (dtTransfer.Rows.Count > 0)
                    await db_sql.BulkSave(connectionString: connectionString, dtTransfer);



                strSQL = CommonFunctions.getTableAnalysisScript("stg", tmp_table, strLstColumnNames);
                var dataTypes = (await db_sql.LoadData<DataTypeModel>(connectionString: connectionString, strSQL));

                strSQL = CommonFunctions.getCreateFinalTableScript("stg", table, dataTypes);
                await db_sql.Execute(connectionString: connectionString, strSQL);

                strSQL = CommonFunctions.getSelectInsertScript("stg", tmp_table, table, strLstColumnNames);
                await db_sql.Execute(connectionString: connectionString, strSQL);

                strLstColumnNames = null;
            }

            //2 GENERTATE FINAL OUTPUT
            strSQL = "Select distinct ETG_BAS_CLSS_NBR, MPC_NBR from CLODM001.ETG_NUMBER";
            var mcp = await db_td.LoadData<UGAPMPCNBRModel>(connectionString: tdConnectionString, strSQL);



            strSQL = "SELECT [MPC_NBR] ,[ETG_BAS_CLSS_NBR] ,[ALWAYS] ,[ATTRIBUTED] ,[ERG_SPCL_CATGY_CD] ,[TRT_CD] ,[RX] ,[NRX] ,[RISK_Model] ,[LOW_MONTH] ,[HIGH_MONTH] FROM [IL_UCA].[dbo].[VW_UGAPCFG_FINAL]";

            var etg = await db_sql.LoadData<UGAPETGModel>(connectionString: connectionString, strSQL);


            foreach (var item in etg)
            {
                var m = mcp.Where(x => x.ETG_BAS_CLSS_NBR == item.ETG_BAS_CLSS_NBR).Select(x => x.MPC_NBR).FirstOrDefault();
                item.MPC_NBR = m;
            }


            List<UGAPETGModel> etg_final = etg.OrderBy(o => o.RISK_Model).ThenBy(o => o.MPC_NBR).ToList();
            StringBuilder sb = new StringBuilder();

            filename = "C:\\Users\\cgiorda\\Desktop\\Projects\\UGAP Configuration\\output\\UGAP_Config_Automated.txt";
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (var file = File.CreateText(filename))
            {
                string[] columns = typeof(UGAPETGModel).GetProperties().Select(p => p.Name).ToArray();
                foreach (var column in columns)
                {
                    sb.Append(column + "|");

                }
                file.WriteLine(sb.ToString().TrimEnd('|'));
                file.Flush();
                sb.Clear();

                foreach (var e in etg_final)
                {
                    sb.Append((e.MPC_NBR == null ? "" : e.MPC_NBR) + "|");
                    sb.Append((e.ETG_BAS_CLSS_NBR == null ? "" : e.ETG_BAS_CLSS_NBR) + "|");
                    sb.Append((e.ALWAYS == null ? "" : e.ALWAYS) + "|");
                    sb.Append((e.ATTRIBUTED == null ? "" : e.ATTRIBUTED) + "|");
                    sb.Append((e.ERG_SPCL_CATGY_CD == null ? "" : e.ERG_SPCL_CATGY_CD) + "|");
                    sb.Append((e.TRT_CD == null ? "" : e.TRT_CD) + "|");
                    sb.Append((e.RX == null ? "" : e.RX) + "|");
                    sb.Append((e.NRX == null ? "" : e.NRX) + "|");
                    sb.Append((e.RISK_Model == null ? "" : e.RISK_Model) + "|");
                    sb.Append((e.LOW_MONTH == null ? "" : e.LOW_MONTH) + "|");
                    sb.Append((e.HIGH_MONTH == null ? "" : e.HIGH_MONTH));
                    file.WriteLine(sb.ToString());
                    sb.Clear();
                }
                file.Flush();
            }


            return;
        }


    }


    

}
