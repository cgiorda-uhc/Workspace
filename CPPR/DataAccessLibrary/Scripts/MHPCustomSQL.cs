using DataAccessLibrary.Models;
using DataAccessLibrary.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace DataAccessLibrary.Scripts
{
    public static class MHPCustomSQL
    {


        public static string VolatileName { get; set; } = "MissingMembersTmp";

        public static string VolatileColumnsDeclare { get; set; } = "mhp_uni_id BIGINT, Cardholder_ID_CLN  VARCHAR(11), State_Of_Issue VARCHAR(5),BTH_DT DATE, REQ_DT DATE, MBR_FST_NM VARCHAR(25), MBR_LST_NM VARCHAR(25) ";

        public static string VolatileColumns { get; set; } = "mhp_uni_id, Cardholder_ID_CLN, State_Of_Issue, BTH_DT, REQ_DT, MBR_FST_NM, MBR_LST_NM ";


        public static string MSSQLMHPMember(string tableMHP, string tableUGAP, string files_csv,string filters = "")
        {
            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("SELECT mhp_uni_id, REPLACE(SUBSTRING([Cardholder_ID], PATINDEX('%[^0]%', [Cardholder_ID]+'.'), LEN([Cardholder_ID])),[State_of_Issue],'') AS [Cardholder_ID_CLN], State_Of_Issue, CONVERT(char(10), [Member_Date_of_Birth],126) as Member_Date_of_Birth, CONVERT(char(10), [Request_Date], 126) as Request_Date, [Enrollee_First_Name] ,[Enrollee_Last_Name] ,[sheet_name] FROM " + tableMHP + " WHERE [Cardholder_ID] IS NOT NULL AND [Request_Date] IS NOT NULL " + filters + " AND file_name in ("+files_csv+") AND mhp_uni_id not in (select mhp_uni_id from " + tableUGAP + ") ORDER BY mhp_uni_id DESC ");


            return sbSQL.ToString();
        }

        public static List<MHPParameterModel> MHPParameters_SF()
        {
            List<MHPParameterModel> pm = new List<MHPParameterModel>();

            //EI
            //EI
            //EI
            pm.Add(new MHPParameterModel() { MHPSQL = "AND (Classification = '" + LOS.EI + "' ) ", UGAPSQL = "inner join " + VolatileName + " as mm on ltrim(a.MBR_ALT_ID, '0') = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt)", LOS = LOS.EI, SearchMethod = "MBR_ALT_ID/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.EI + "'  AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.EI, SearchMethod = "FN/LN/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.EI + "' AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.EI, SearchMethod = "FN%3/LN%3/BD/RD" });
            //EI_OX
            //EI_OX
            //EI_OX
            pm.Add(new MHPParameterModel() { MHPSQL = "AND (Classification = '" + LOS.EI_OX + "' )  ", UGAPSQL = "inner join " + VolatileName + " as mm on ltrim(a.MBR_ID, '0') = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.EI_OX, SearchMethod = "MBR_ID/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.EI_OX + "' AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.EI_OX, SearchMethod = "FN/LN/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.EI_OX + "' AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.EI_OX, SearchMethod = "FN%3/LN%3/BD/RD" });
            //IFP
            //IFP
            //IFP
            pm.Add(new MHPParameterModel() { MHPSQL = "AND (Classification = '" + LOS.IFP + "' ) ", UGAPSQL = "inner join " + VolatileName + " as mm on SUBSTR(a.MBR_ID, 0,10) = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.IFP, SearchMethod = "MBR_ID/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.IFP + "'  AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.IFP, SearchMethod = "FN/LN/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.IFP + "' AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM)  AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.IFP, SearchMethod = "FN%3/LN%3/BD/RD" });
            //CS
            //CS
            //CS
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.CS + "' AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on ltrim(a.MBR_ID, '0') = mm.Cardholder_ID_CLN  AND k.CS_CO_CD_ST = mm.State_Of_Issue AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.CS, SearchMethod = "MBR_ID/CS_CO_CD_ST/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.CS + "' AND [Member_Date_of_Birth]  IS NOT NULL  ", UGAPSQL = "inner join " + VolatileName + " as mm on ltrim(a.SBSCR_MEDCD_RCIP_NBR, '0')  = mm.Cardholder_ID_CLN  AND k.CS_CO_CD_ST = mm.State_Of_Issue AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.CS, SearchMethod = "SBSCR_MEDCD_RCIP_NBR/CS_CO_CD_ST/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.CS + "'  AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL  ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) AND k.CS_CO_CD_ST = mm.State_Of_Issue ", LOS = LOS.CS, SearchMethod = "FN/LN/BD/RD/CS_CO_CD_ST" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.CS + "'  AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL  ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) AND k.CS_CO_CD_ST = mm.State_Of_Issue ", LOS = LOS.CS, SearchMethod = "FN%3/LN%3/BD/RD/CS_CO_CD_ST" });


            return pm;
        }



        public static List<MHPParameterModel> MHPParameters()
        {
            List<MHPParameterModel> pm = new List<MHPParameterModel>();

            //EI
            //EI
            //EI
            pm.Add(new MHPParameterModel() { MHPSQL = "AND (Classification = '" + LOS.EI + "' ) ", UGAPSQL = "inner join " + VolatileName + " as mm on trim(leading '0' from a.MBR_ALT_ID) = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt)", LOS = LOS.EI, SearchMethod = "MBR_ALT_ID/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.EI + "'  AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.EI, SearchMethod = "FN/LN/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.EI + "' AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.EI, SearchMethod = "FN%3/LN%3/BD/RD" });
            //EI_OX
            //EI_OX
            //EI_OX
            pm.Add(new MHPParameterModel() { MHPSQL = "AND (Classification = '" + LOS.EI_OX + "' )  ", UGAPSQL = "inner join " + VolatileName + " as mm on trim(leading '0' from a.MBR_ID) = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.EI_OX, SearchMethod = "MBR_ID/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.EI_OX + "' AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.EI_OX, SearchMethod = "FN/LN/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.EI_OX + "' AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.EI_OX, SearchMethod = "FN%3/LN%3/BD/RD" });
            //IFP
            //IFP
            //IFP
            pm.Add(new MHPParameterModel() { MHPSQL = "AND (Classification = '" + LOS.IFP + "' ) ", UGAPSQL = "inner join " + VolatileName + " as mm on SUBSTR(a.MBR_ID, 0,10) = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.IFP, SearchMethod = "MBR_ID/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.IFP + "'  AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.IFP, SearchMethod = "FN/LN/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.IFP + "' AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM)  AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.IFP, SearchMethod = "FN%3/LN%3/BD/RD" });
            //CS
            //CS
            //CS
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.CS + "' AND [Member_Date_of_Birth]  IS NOT NULL ", UGAPSQL = "inner join " + VolatileName + " as mm on trim(leading '0' from a.MBR_ID) = mm.Cardholder_ID_CLN  AND k.CS_CO_CD_ST = mm.State_Of_Issue AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.CS, SearchMethod = "MBR_ID/CS_CO_CD_ST/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.CS + "' AND [Member_Date_of_Birth]  IS NOT NULL  ", UGAPSQL = "inner join " + VolatileName + " as mm on trim(leading '0' from a.SBSCR_MEDCD_RCIP_NBR) = mm.Cardholder_ID_CLN  AND k.CS_CO_CD_ST = mm.State_Of_Issue AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", LOS = LOS.CS, SearchMethod = "SBSCR_MEDCD_RCIP_NBR/CS_CO_CD_ST/BD/RD" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.CS + "'  AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL  ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) AND k.CS_CO_CD_ST = mm.State_Of_Issue ", LOS = LOS.CS, SearchMethod = "FN/LN/BD/RD/CS_CO_CD_ST" });
            pm.Add(new MHPParameterModel() { MHPSQL = "AND Classification = '" + LOS.CS + "'  AND [Enrollee_First_Name] IS NOT NULL AND [Enrollee_Last_Name] IS NOT NULL  AND [Member_Date_of_Birth]  IS NOT NULL  ", UGAPSQL = "inner join " + VolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) AND k.CS_CO_CD_ST = mm.State_Of_Issue ", LOS = LOS.CS, SearchMethod = "FN%3/LN%3/BD/RD/CS_CO_CD_ST" });


            return pm;
        }

        public static string UGAPSQLMemberDataCS_SF(string strFilterJoin, bool blIsCS)
        {
            StringBuilder sbSQL = new StringBuilder();


            sbSQL.Append("create or replace table " + VolatileName + "( ");

            sbSQL.Append(VolatileColumnsDeclare);

            sbSQL.Append(")");

            sbSQL.Append("{$Inserts}");

            sbSQL.Append("SELECT ");
            sbSQL.Append("mm.mhp_uni_id,  ");
            sbSQL.Append("b.BEN_STRCT_1_CD as PLN_VAR_SUBDIV_CD,  ");
            sbSQL.Append("c.eff_dt as mnth_eff_dt,  ");
            sbSQL.Append("NULL as LEG_ENTY_NBR,  ");
            sbSQL.Append("NULL as LEG_ENTY_FULL_NM,  ");
            sbSQL.Append("NULL as HCE_LEG_ENTY_ROLLUP_DESC, ");
            sbSQL.Append("NULL as MKT_TYP_DESC,  ");
            sbSQL.Append("NULL as CUST_SEG_NBR,  ");
            sbSQL.Append("NULL as CUST_SEG_NM,  "); //ADD TO DB!!!!
            sbSQL.Append("i.PRDCT_CD,  ");
            sbSQL.Append("i.PRDCT_CD_DESC,  ");
            sbSQL.Append("NULL as MKT_SEG_DESC,  ");
            sbSQL.Append("NULL as MKT_SEG_RLLP_DESC,  ");
            sbSQL.Append("NULL as MKT_SEG_CD,  ");
            sbSQL.Append("NULL as FINC_ARNG_CD,  ");
            sbSQL.Append("NULL as FINC_ARNG_DESC,  ");
            sbSQL.Append("a.MBR_FST_NM, ");
            sbSQL.Append("a.MBR_LST_NM, ");
            sbSQL.Append("a.BTH_DT, ");
            sbSQL.Append("a.MBR_ALT_ID, ");
            sbSQL.Append("a.MBR_ID, ");
            sbSQL.Append("b.PRDCT_SYS_ID, ");
            sbSQL.Append("b.CS_PRDCT_CD_SYS_ID, ");
            sbSQL.Append("k.CS_CO_CD, ");
            sbSQL.Append("k.CS_CO_CD_ST, ");
            sbSQL.Append("a.SBSCR_MEDCD_RCIP_NBR ");
            sbSQL.Append("FROM uhcdm001.hp_member a  ");
            sbSQL.Append("join uhcdm001.cs_demographics b on a.MBR_SYS_ID = b.MBR_SYS_ID  ");
            sbSQL.Append("join uhcdm001.date_eff c on b.DT_SYS_ID = c.EFF_DT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SOURCE_SYSTEM_COMBO d on b.SRC_SYS_COMBO_SYS_ID = d.SRC_SYS_COMBO_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SUBSCRIBER_COUNTY h on b.SBSCR_CNTY_SYS_ID = h.SBSCR_CNTY_SYS_ID  ");
            sbSQL.Append("join uhcdm001.PRODUCT i on b.PRDCT_SYS_ID = i.PRDCT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.cs_company_code k on b.CS_CO_CD_SYS_ID = k.CS_CO_CD_SYS_ID ");

            sbSQL.Append(strFilterJoin);
            sbSQL.Append("WHERE k.CS_CO_CD " + (blIsCS ? "<>" : "=") + " 'UHGEX'; ");




            return sbSQL.ToString();
        }




        public static string UGAPSQLMemberDataCS(string strFilterJoin, bool blIsCS)
        {
            StringBuilder sbSQL = new StringBuilder();


            sbSQL.Append("CREATE MULTISET VOLATILE TABLE " + VolatileName + "( ");

            sbSQL.Append(VolatileColumnsDeclare);

            sbSQL.Append(") PRIMARY INDEX(" + VolatileColumns + ") ON COMMIT PRESERVE ROWS; ");

            sbSQL.Append("{$vti}");

            sbSQL.Append("{$Inserts}");

            sbSQL.Append("{$vtc}");

            sbSQL.Append("COLLECT STATS COLUMN(" + VolatileColumns + ") ON " + VolatileName + "; ");
            sbSQL.Append("{$vts}");

            sbSQL.Append("SELECT ");
            sbSQL.Append("mm.mhp_uni_id,  ");
            sbSQL.Append("b.BEN_STRCT_1_CD as PLN_VAR_SUBDIV_CD,  ");
            sbSQL.Append("c.eff_dt as mnth_eff_dt,  ");
            sbSQL.Append("NULL as LEG_ENTY_NBR,  ");
            sbSQL.Append("NULL as LEG_ENTY_FULL_NM,  ");
            sbSQL.Append("NULL as HCE_LEG_ENTY_ROLLUP_DESC, ");
            sbSQL.Append("NULL as MKT_TYP_DESC,  ");
            sbSQL.Append("NULL as CUST_SEG_NBR,  ");
            sbSQL.Append("NULL as CUST_SEG_NM,  "); //ADD TO DB!!!!
            sbSQL.Append("i.PRDCT_CD,  ");
            sbSQL.Append("i.PRDCT_CD_DESC,  ");
            sbSQL.Append("NULL as MKT_SEG_DESC,  ");
            sbSQL.Append("NULL as MKT_SEG_RLLP_DESC,  ");
            sbSQL.Append("NULL as MKT_SEG_CD,  ");
            sbSQL.Append("NULL as FINC_ARNG_CD,  ");
            sbSQL.Append("NULL as FINC_ARNG_DESC,  ");
            sbSQL.Append("a.MBR_FST_NM, ");
            sbSQL.Append("a.MBR_LST_NM, ");
            sbSQL.Append("a.BTH_DT, ");
            sbSQL.Append("a.MBR_ALT_ID, ");
            sbSQL.Append("a.MBR_ID, ");
            sbSQL.Append("b.PRDCT_SYS_ID, ");
            sbSQL.Append("b.CS_PRDCT_CD_SYS_ID, ");
            sbSQL.Append("k.CS_CO_CD, ");
            sbSQL.Append("k.CS_CO_CD_ST, ");
            sbSQL.Append("a.SBSCR_MEDCD_RCIP_NBR ");
            sbSQL.Append("FROM uhcdm001.hp_member a  ");
            sbSQL.Append("join uhcdm001.cs_demographics b on a.MBR_SYS_ID = b.MBR_SYS_ID  ");
            sbSQL.Append("join uhcdm001.date_eff c on b.DT_SYS_ID = c.EFF_DT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SOURCE_SYSTEM_COMBO d on b.SRC_SYS_COMBO_SYS_ID = d.SRC_SYS_COMBO_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SUBSCRIBER_COUNTY h on b.SBSCR_CNTY_SYS_ID = h.SBSCR_CNTY_SYS_ID  ");
            sbSQL.Append("join uhcdm001.PRODUCT i on b.PRDCT_SYS_ID = i.PRDCT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.cs_company_code k on b.CS_CO_CD_SYS_ID = k.CS_CO_CD_SYS_ID ");

            sbSQL.Append(strFilterJoin);
            sbSQL.Append("WHERE k.CS_CO_CD " + (blIsCS ? "<>" : "=") + " 'UHGEX'; ");
            sbSQL.Append("{$dvt}");
            sbSQL.Append("drop table " + VolatileName + ";  ");



            return sbSQL.ToString();
        }


        public static string UGAPSQLLMemberDataEI_SF(string strFilterJoin, bool blIsOX)
        {
            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("create or replace table " + VolatileName + "( ");

            sbSQL.Append(VolatileColumnsDeclare);

            sbSQL.Append(")");

            sbSQL.Append("{$Inserts}");

            sbSQL.Append("SELECT ");
            sbSQL.Append("mm.mhp_uni_id,  ");
            sbSQL.Append("b.BEN_STRCT_1_CD as PLN_VAR_SUBDIV_CD,  ");
            sbSQL.Append("c.eff_dt as mnth_eff_dt,  ");
            sbSQL.Append("e.LEG_ENTY_NBR,  ");
            sbSQL.Append("e.LEG_ENTY_FULL_NM,  ");
            sbSQL.Append("e.HCE_LEG_ENTY_ROLLUP_DESC,  ");
            sbSQL.Append("f.MKT_TYP_DESC,  ");
            sbSQL.Append("g.CUST_SEG_NBR,  ");
            sbSQL.Append("g.CUST_SEG_NM,  "); //ADD TO DB!!!!
            sbSQL.Append("i.PRDCT_CD,  ");
            sbSQL.Append("i.PRDCT_CD_DESC,  ");
            sbSQL.Append("j.MKT_SEG_DESC,  ");
            sbSQL.Append("j.MKT_SEG_RLLP_DESC,  ");
            sbSQL.Append("j.MKT_SEG_CD,  ");
            sbSQL.Append("k.FINC_ARNG_CD,  ");
            sbSQL.Append("k.FINC_ARNG_DESC,  ");
            sbSQL.Append("a.MBR_FST_NM, ");
            sbSQL.Append("a.MBR_LST_NM, ");
            sbSQL.Append("a.BTH_DT, ");
            sbSQL.Append("a.MBR_ALT_ID, ");
            sbSQL.Append("a.MBR_ID, ");
            sbSQL.Append("NULL as PRDCT_SYS_ID, ");
            sbSQL.Append("NULL as CS_PRDCT_CD_SYS_ID, ");
            sbSQL.Append("NULL as CS_CO_CD, ");
            sbSQL.Append("NULL as CS_CO_CD_ST, ");
            sbSQL.Append("a.SBSCR_MEDCD_RCIP_NBR ");
            sbSQL.Append("FROM uhcdm001.hp_member a  ");
            sbSQL.Append("join uhcdm001.hp_demographics b on a.MBR_SYS_ID = b.MBR_SYS_ID  ");
            sbSQL.Append("join uhcdm001.date_eff c on b.DT_SYS_ID = c.EFF_DT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SOURCE_SYSTEM_COMBO d on b.SRC_SYS_COMBO_SYS_ID = d.SRC_SYS_COMBO_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SUBSCRIBER_COUNTY h on b.SBSCR_CNTY_SYS_ID = h.SBSCR_CNTY_SYS_ID  ");
            sbSQL.Append("join uhcdm001.LEGAL_ENTITY e on b.LEG_ENTY_SYS_ID = e.LEG_ENTY_SYS_ID  ");
            sbSQL.Append("join uhcdm001.MARKET_TYPE_CODE f on b.MKT_TYP_CD_SYS_ID = f.MKT_TYP_CD_SYS_ID  ");
            sbSQL.Append("join uhcdm001.CUSTOMER_SEGMENT g on b.CUST_SEG_SYS_ID = g.CUST_SEG_SYS_ID  ");
            sbSQL.Append("join uhcdm001.PRODUCT i on b.PRDCT_SYS_ID = i.PRDCT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.GROUP_INDICATOR j on b.GRP_IND_SYS_ID = j.GRP_IND_SYS_ID  ");
            sbSQL.Append("join uhcdm001.company_code k on b.CO_CD_SYS_ID = k.CO_CD_SYS_ID  ");
            sbSQL.Append(strFilterJoin);
            sbSQL.Append("WHERE e.HCE_LEG_ENTY_ROLLUP_DESC  " + (blIsOX ? "=" : "<>") + " 'OXFORD'; ");



            return sbSQL.ToString();
        }


        public static string UGAPSQLLMemberDataEI(string strFilterJoin, bool blIsOX)
        {
            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("CREATE MULTISET VOLATILE TABLE " + VolatileName + "( ");

            sbSQL.Append(VolatileColumnsDeclare);

            sbSQL.Append(") PRIMARY INDEX(" + VolatileColumns + ") ON COMMIT PRESERVE ROWS; ");

            sbSQL.Append("{$vti}");

            sbSQL.Append("{$Inserts}");

            sbSQL.Append("{$vtc}");


            sbSQL.Append("COLLECT STATS COLUMN(" + VolatileColumns + ") ON " + VolatileName + "; ");
            sbSQL.Append("{$vts}");

            sbSQL.Append("SELECT ");
            sbSQL.Append("mm.mhp_uni_id,  ");
            sbSQL.Append("b.BEN_STRCT_1_CD as PLN_VAR_SUBDIV_CD,  ");
            sbSQL.Append("c.eff_dt as mnth_eff_dt,  ");
            sbSQL.Append("e.LEG_ENTY_NBR,  ");
            sbSQL.Append("e.LEG_ENTY_FULL_NM,  ");
            sbSQL.Append("e.HCE_LEG_ENTY_ROLLUP_DESC,  ");
            sbSQL.Append("f.MKT_TYP_DESC,  ");
            sbSQL.Append("g.CUST_SEG_NBR,  ");
            sbSQL.Append("g.CUST_SEG_NM,  "); //ADD TO DB!!!!
            sbSQL.Append("i.PRDCT_CD,  ");
            sbSQL.Append("i.PRDCT_CD_DESC,  ");
            sbSQL.Append("j.MKT_SEG_DESC,  ");
            sbSQL.Append("j.MKT_SEG_RLLP_DESC,  ");
            sbSQL.Append("j.MKT_SEG_CD,  ");
            sbSQL.Append("k.FINC_ARNG_CD,  ");
            sbSQL.Append("k.FINC_ARNG_DESC,  ");
            sbSQL.Append("a.MBR_FST_NM, ");
            sbSQL.Append("a.MBR_LST_NM, ");
            sbSQL.Append("a.BTH_DT, ");
            sbSQL.Append("a.MBR_ALT_ID, ");
            sbSQL.Append("a.MBR_ID, ");
            sbSQL.Append("NULL as PRDCT_SYS_ID, ");
            sbSQL.Append("NULL as CS_PRDCT_CD_SYS_ID, ");
            sbSQL.Append("NULL as CS_CO_CD, ");
            sbSQL.Append("NULL as CS_CO_CD_ST, ");
            sbSQL.Append("a.SBSCR_MEDCD_RCIP_NBR ");
            sbSQL.Append("FROM uhcdm001.hp_member a  ");
            sbSQL.Append("join uhcdm001.hp_demographics b on a.MBR_SYS_ID = b.MBR_SYS_ID  ");
            sbSQL.Append("join uhcdm001.date_eff c on b.DT_SYS_ID = c.EFF_DT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SOURCE_SYSTEM_COMBO d on b.SRC_SYS_COMBO_SYS_ID = d.SRC_SYS_COMBO_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SUBSCRIBER_COUNTY h on b.SBSCR_CNTY_SYS_ID = h.SBSCR_CNTY_SYS_ID  ");
            sbSQL.Append("join uhcdm001.LEGAL_ENTITY e on b.LEG_ENTY_SYS_ID = e.LEG_ENTY_SYS_ID  ");
            sbSQL.Append("join uhcdm001.MARKET_TYPE_CODE f on b.MKT_TYP_CD_SYS_ID = f.MKT_TYP_CD_SYS_ID  ");
            sbSQL.Append("join uhcdm001.CUSTOMER_SEGMENT g on b.CUST_SEG_SYS_ID = g.CUST_SEG_SYS_ID  ");
            sbSQL.Append("join uhcdm001.PRODUCT i on b.PRDCT_SYS_ID = i.PRDCT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.GROUP_INDICATOR j on b.GRP_IND_SYS_ID = j.GRP_IND_SYS_ID  ");
            sbSQL.Append("join uhcdm001.company_code k on b.CO_CD_SYS_ID = k.CO_CD_SYS_ID  ");
            sbSQL.Append(strFilterJoin);
            sbSQL.Append("WHERE e.HCE_LEG_ENTY_ROLLUP_DESC  " + (blIsOX ? "=" : "<>") + " 'OXFORD'; ");
            sbSQL.Append("{$dvt}");
            sbSQL.Append("drop table " + VolatileName + ";  ");



            return sbSQL.ToString();
        }


        public static string UGAPVolatileInsert(MHPMemberSearchModel mhp, MHPParameterModel mhp_param)
        {
            StringBuilder sbSQL = new StringBuilder();

            var fnw = "";
            if(mhp.Enrollee_First_Name != null)
                fnw = (mhp_param.SearchMethod != "FN%3/LN%3/BD/RD" ? mhp.Enrollee_First_Name.Replace("'", "''") : mhp.Enrollee_First_Name.Substring(0, Math.Min(3, mhp.Enrollee_First_Name.Length)).Replace("'", "''") + "%");


            var lnw = "";
            if (mhp.Enrollee_Last_Name != null)
                lnw = (mhp_param.SearchMethod != "FN%3/LN%3/BD/RD" ? mhp.Enrollee_Last_Name.Replace("'", "''") : mhp.Enrollee_Last_Name.Substring(0, Math.Min(3, mhp.Enrollee_Last_Name.Length)).Replace("'", "''") + "%");

            var st = mhp.State_Of_Issue;
            var id =mhp.mhp_uni_id.ToString();
            var cidc = (mhp_param.LOS != LOS.IFP ? mhp.Cardholder_ID_CLN : mhp.Cardholder_ID_CLN.Substring(0, (mhp.Cardholder_ID_CLN.Length < 9 ? mhp.Cardholder_ID_CLN.Length : 9)));

            var bd = "";
            if (mhp.Member_Date_of_Birth != null)
                bd = mhp.Member_Date_of_Birth.ToString();

            var rd = mhp.Request_Date.ToString();

            sbSQL.Append("INSERT INTO " + VolatileName + " (" + VolatileColumns + ") VALUES(" + id + ",'" + cidc + "','" + st + "', '" + bd + "', '" + rd + "', '" + fnw + "', '" + lnw + "'); ");


            return sbSQL.ToString();
        } 
     }
}
