using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive_Connect
{
    class Hive_Connect
    {
        static void Main(string[] args)
        {
            DataTable t = null;
            string strConnectionString = string.Format("DSN={0};Uid={1};Pwd={2}", "SDR - Hive", "cgiorda", "BooWooDooFoo2023!!");
            t = DBConnection64.getODBCDataTable(strConnectionString, "SHOW TABLES;");




        ////Create a hive connection
        ////I've my cluster in https://www.hadooponazure.com
        //var hive = new SampleHiveConnection(
        //            "http://dbslp0503", //your connection string
        //            "10869",                       //port                    
        //            "cgiorda",                      //your username
        //            "BooWooDooFoo2023!!");                 //your password


        //    //Get the results
        //    //Make sure you goto the dashboard and turn on the ODBC port
        //    var res = from d in hive.DeviceInfoTable
        //              where d.ClientId < 100
        //              select d;


        //    var result = hive.ExecuteQuery(res.ToString());

        //    var list = result.Result.ReadToEnd();

            //result.Wait();



            //Console.WriteLine("The results are: {0}", result.Result.ReadToEnd());

            ////Dump it to the console if you like
            //var list = res.ToList();


        }
    }
    

}
