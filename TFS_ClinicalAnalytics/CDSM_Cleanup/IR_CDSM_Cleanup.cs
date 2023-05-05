using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSM_Cleanup
{
    class IR_CDSM_Cleanup
    {
        static void Main(string[] args)
        {
            CDSM_Cleanup();
        }

       static  void CDSM_Cleanup()
        {
            //SETUP PARAMETERS BEFORE USING IR_DataScrubber
            IR_DataScrubber.lstStrDegree = new List<string>();
            IR_DataScrubber.lstStrDegree.Add("MD");
            IR_DataScrubber.lstStrDegree.Add("DO");
            IR_DataScrubber.lstStrDegree.Add("DNP");
            IR_DataScrubber.lstStrDegree.Add("FNP");
            IR_DataScrubber.lstStrDegree.Add("NP");
            IR_DataScrubber.lstStrDegree.Add("PA");
            IR_DataScrubber.lstStrDegree.Add("RN");

            IR_DataScrubber.lstStrSuffix = new List<string>();
            IR_DataScrubber.lstStrSuffix.Add("SR");
            IR_DataScrubber.lstStrSuffix.Add("JR");
            //IR_DataScrubber.lstStrSuffix.Add("I"); //TOO CLOSE TO MIDDLENAME
            IR_DataScrubber.lstStrSuffix.Add("II");
            IR_DataScrubber.lstStrSuffix.Add("III");
            IR_DataScrubber.lstStrSuffix.Add("IV");
            //IR_DataScrubber.lstStrSuffix.Add("V"); //TOO CLOSE TO MIDDLENAME
            IR_DataScrubber.lstStrSuffix.Add("VI");



            StringBuilder sbBatchUpdatesContainer = new StringBuilder();


            //PLACE APP.CONFIG FILE DATA INTO VARIABLES START
            string strILUCA_ConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strUHN_ConnectionString = ConfigurationManager.AppSettings["UHN_Database"];



            DataTable dtSearch = null;
            DataTable dtMain = DBConnection64.getMSSQLDataTable(strILUCA_ConnectionString, "SELECT [Line_number] ,[eCW_Practice_Name3] ,[TaxID] ,[Health_System_Assoc_PG] ,[TaxIdOwnerName] ,[MPIN] FROM [IL_UCA].[dbo].[CDSM_dec06] WHERE MPIN is null");

            string strPractice = "";
            string strFirstName = "";
            string strLastName = "";
            string strTin = "";
            string strMPIN = "";
            string strLineNumber = "";
            string[] strNameArr = null;
            foreach (DataRow dr in dtMain.Rows)
            {


                if (dr["eCW_Practice_Name3"] != DBNull.Value)
                    strPractice = dr["eCW_Practice_Name3"].ToString();
                else
                    continue;

                if (dr["TaxID"] != DBNull.Value)
                    strTin = dr["TaxID"].ToString();
                else
                    continue;



                strLineNumber = dr["Line_number"].ToString();


               // strNameArr = strPractice.Replace(",", "").Replace(".", "").Replace(" MD ", "").Replace(" MD", "").Replace(" PC ", "").Replace(" PC", "").Replace("Advocate - ", "").Replace("SHS - ", "").Replace("CGHN - ", "").Replace("Lake Hospital - ", "").Replace("Advocate - ", "").Split('-');

                strNameArr = strPractice.Replace(",", "").Replace(".", "").Replace(" MD ", "").Replace(" MD", "").Replace(" PC ", "").Replace(" PC", "").Split('-');



                foreach (string s in strNameArr)
                {

                   dtSearch =  DBConnection64.getMSSQLDataTable(strUHN_ConnectionString, "select p.*,taxid from provider as p inner join[dbo].[PROV_TIN_PAY_AFFIL] as t on p.mpin=t.mpin where lastname ='"+ s + "' and taxid = " + strTin);

                    if (dtSearch.Rows.Count == 1)
                        break;

                }


                if (dtSearch.Rows.Count != 1)
                {

                    foreach (string s in strNameArr)
                    {

                        dtSearch = DBConnection64.getMSSQLDataTable(strUHN_ConnectionString, "select p.*,taxid from provider as p inner join[dbo].[PROV_TIN_PAY_AFFIL] as t on p.mpin=t.mpin where lastname ='Hormes' and firstname = 'Joseph' and taxid = " + strTin);

                    }

                }

                if (dtSearch.Rows.Count != 1)
                {

                    foreach (string s in strNameArr)
                    {

                        dtSearch = DBConnection64.getMSSQLDataTable(strUHN_ConnectionString, "select p.*,taxid from provider as p inner join[dbo].[PROV_TIN_PAY_AFFIL] as t on p.mpin=t.mpin where lastname ='Hormes' and firstname = 'Joseph' and taxid = " + strTin);

                    }
                }


                if (dtSearch.Rows.Count == 1)
                {
                    strMPIN = dr["MPIN"].ToString();
                    DBConnection64.ExecuteMSSQL(strILUCA_ConnectionString, "UPDATE CDSM_dec06 SET [MPIN] = " + strMPIN + ", update_date = getDate(), automated_match = 1 WHERE Line_number = " + strLineNumber);
                }




                //select p.*,taxid from provider as p inner join[dbo].[PROV_TIN_PAY_AFFIL] as t on p.mpin=t.mpin where lastname ='Hormes' and firstname = 'Joseph' and taxid = 273818647


            }


            


            //            select p.mpin,lastName,FirstName,taxid
            //from provider as p
            //inner join[dbo].[PROV_TIN_PAY_AFFIL] as t on p.mpin=t.mpin
            //where lastname ='Provida Family Medicine'
            //and taxid = 364292273




//            select p.mpin,lastName,FirstName,taxid
//from provider as p
//inner join[dbo].[PROV_TIN_PAY_AFFIL] as t on p.mpin=t.mpin
//where lastname ='Riley' and firstname = 'William'
//and taxid = 843078964--204923281

            



        }

    }
}
