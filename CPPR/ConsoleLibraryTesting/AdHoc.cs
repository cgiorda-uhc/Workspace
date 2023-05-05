using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using DataAccessLibrary.Scripts;
using DataAccessLibrary.Shared;
using FileParsingLibrary.Models;
using FileParsingLibrary.MSWord;
using ProjectManagerLibrary.Models;
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
    }


    

}
