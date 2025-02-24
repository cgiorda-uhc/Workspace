﻿
using DataAccessLibrary.DataAccess;
using ConsoleLibraryTesting;
using Microsoft.Extensions.Configuration;
using ProjectManagerLibrary.Projects;
using ProjectManagerLibrary.Concrete;
using Serilog;
using SASConnectionLibrary;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;
using System.Diagnostics;
using VCPortal_Models.Models.PCCM;
using ProjectManagerLibrary.Models;


var adHoc = new AdHoc();


//AD HOC GLOBAL VARIABLES
adHoc.ConnectionStringMSSQL = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
adHoc.TableMHP = "stg.MHP_Yearly_Universes";
adHoc.ConnectionStringTD = "Data Source=UDWPROD;User ID=cgiorda;Password=BooWooDooFoo2023!!;Authentication Mechanism=LDAP;Session Mode=TERADATA;Session Character Set=ASCII;Persist Security Info=true;Connection Timeout=99999;";
adHoc.ConnectionStringVC = "Data Source=wn000103397;Initial Catalog=VCT_DB;Persist Security Info=True;User ID=vct_app_user;Password=BooWooDooFoo2023!!;connect timeout=300000;";
//adHoc.ConnectionStringVC = "data source=VCT_DB;server=localhost;Persist Security Info=True;database=VCT_DB;Integrated Security=SSPI;connect timeout=300000;";

adHoc.ConnectionStringUHPD = "data source=UHPD_Reporting;server=WP000052579;Persist Security Info=True;database=PD_Reporting;Integrated Security=SSPI;connect timeout=300000;";

//adHoc.ConnectionStringPD = "data source=UHPD_Analytics;server=DBSWP0662;Persis4t Security Info=True;database=UHPD_Analytics;Integrated Security=SSPI;connect timeout=300000;";
//adHoc.ConnectionStringPD = "data source=UHPD_Analytics;server=DBSWP200136;Persist Security Info=True;database=UHPD_Analytics;Integrated Security=SSPI;connect timeout=300000;";
adHoc.ConnectionStringPD = "data source=UHPD_Analytics;server=DBSWS200137;Persist Security Info=True;database=UHPD_Analytics;Integrated Security=SSPI;connect timeout=300000;";

//adHoc.ConnectionStringUHN = "data source=UHN_Reporting;server=WP000074441CLS;Persist Security Info=True;database=UHN_Reporting;Integrated Security=SSPI;connect timeout=300000;";
adHoc.ConnectionStringUHN = "data source=UHN_Reporting;server=WP000074680;Persist Security Info=True;database=SourceData;Integrated Security=SSPI;connect timeout=300000;";

adHoc.ConnectionStringSnowflakeODBC = @"DRIVER=SnowflakeDSIIDriver;SERVER=uhgdwaas.east-us-2.azure.snowflakecomputing.com;ROLE=AR_PRD_CHRIS_GIORDANO_UHC_ROLE;AUTHENTICATOR=SNOWFLAKE_JWT;UID=chris_giordano@uhc.com;PRIV_KEY_FILE=C:\Users\cgiorda\Documents\credentials\rsa_key.p8;PRIV_KEY_FILE_PWD=Sigmund2010!!; WAREHOUSE=OHBI_PRD_CONSUME_FREQ_WH;";

adHoc.ConnectionStringSnowflake = @"DRIVER=SnowflakeDSIIDriver;ACCOUNT=uhgdwaas.east-us-2.azure.snowflakecomputing.com;HOST=uhgdwaas.east-us-2.azure.snowflakecomputing.com;ROLE=AR_PRD_CHRIS_GIORDANO_UHC_ROLE;AUTHENTICATOR=SNOWFLAKE_JWT;USER=chris_giordano@uhc.com;private_key_file=C:\Users\cgiorda\Documents\credentials\rsa_key.p8;private_key_pwd=Sigmund2010!!; WAREHOUSE=UGP_PRD_END_USERS_WH;";


//adHoc.ConnectionStringSnowflake3 = @"DRIVER=SnowflakeDSIIDriver;ACCOUNT=uhgdwaas.east-us-2.azure.snowflakecomputing.com;ROLE=AR_PRD_CHRIS_GIORDANO_UHC_ROLE;AUTHENTICATOR=SNOWFLAKE_JWT;USER=chris_giordano@uhc.com;private_key_file=C:\Users\cgiorda\Documents\credentials\rsa_key.p8;private_key_pwd=Sigmund2010!!; WAREHOUSE=OHBI_PRD_CONSUME_FREQ_WH;";


//adHoc.ConnectionStringSnowflake4 = @"DRIVER=SnowflakeDSIIDriver;SERVER=uhgdwaas.east-us-2.azure.snowflakecomputing.com;ROLE=AR_PRD_CHRIS_GIORDANO_UHC_ROLE;AUTHENTICATOR=SNOWFLAKE_JWT;UID=chris_giordano@uhc.com;PRIV_KEY_FILE=C:\Users\cgiorda\Documents\credentials\rsa_key.p8;PRIV_KEY_FILE_PWD=Sigmund2010!!; WAREHOUSE=UGP_PRD_END_USERS_WH;";

//adHoc.ConnectionStringSnowflake = @"DRIVER=SnowflakeDSIIDriver;SERVER=uhgdwaas.east-us-2.azure.snowflakecomputing.com;ROLE=AR_PRD_CHRIS_GIORDANO_UHC_ROLE;AUTHENTICATOR=SNOWFLAKE_JWT;UID=chris_giordano@uhc.com;PRIV_KEY_FILE=C:\Users\cgiorda\Documents\credentials\rsa_key.p8;PRIV_KEY_FILE_PWD=Sigmund2010!!; WAREHOUSE=OHBI_PRD_CONSUME_FREQ_WH;";


adHoc.ConnectionStringNDAR = "data source=UHN_Reporting;server=WP000074680;Persist Security Info=True;database=ndar;Integrated Security=SSPI;connect timeout=300000;";

adHoc.ConnectionStringGalaxy = "Data Source=UDWPROD;User ID=cgiorda;Password=BooWooDooFoo2023!!;Database=GALAXY;Authentication Mechanism=LDAP;Session Mode=TERADATA;Session Character Set=ASCII;Persist Security Info=true;Connection Timeout=99999;";


adHoc.TableUGAP = "stg.MHP_Yearly_Universes_UGAP";
adHoc.Limit = 3000;

adHoc.TATReportTemplatePath = "\\\\nasv0048\\ucs_ca\\PHS_DATA_NEW\\Home Directory - Automation\\ExcelTemplates\\TAT_Reporting\\TAT_Template.xlsx";

adHoc.PEGReportTemplatePath = "\\\\nasv0048\\ucs_ca\\PHS_DATA_NEW\\Home Directory - Automation\\ExcelTemplates\\PEG Template\\341 PEG DQ&C Results - Template.xlsx";

adHoc.EBMReportTemplatePath = "\\\\nasv0048\\ucs_ca\\PHS_DATA_NEW\\Home Directory - Automation\\ExcelTemplates\\DQ&C Report Automation\\EBM Template\\342 EBM DQ&C Results - Template.xlsx";

adHoc.UGAPConfigPath = @"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - Automation\UGAP_CFG\";

adHoc.UGAPConfigOutputFile = @"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - Automation\UGAP_CFG\output\UGAP_Config_Automated.txt";

adHoc.ReportsTimelinessPath = @"\\NASGWFTP03\Care_Core_FTP_Files\Radiology";

adHoc.PPACA_TAT_EmailTemplatePath = @"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - Automation\EmailTemplates\PPACA_TAT.txt";

adHoc.ProjectsPath = @"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - Automation\Projects";


//INSTANTIATE DATABASE OBJECT
IRelationalDataAccess db_sqsl = new SqlDataAccess();


//INSTANTIATE CONFIG 
var builder = new ConfigurationBuilder()
                 .AddJsonFile($"appsettings.json", true, true);
var config = builder.Build();


//INSTANTIATE LOGGER
Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(config)
           .CreateLogger();


//INSTANTIATE SAS CONNECTION
var sas_cfg = config.GetSection("SASConnection").Get<SASConnection_Model>();
SASConnection.SASHost = sas_cfg.SASHost;
SASConnection.SASPort = sas_cfg.SASPort;
SASConnection.SASClassIdentifier = sas_cfg.SASClassIdentifier;
SASConnection.SASUserName = sas_cfg.SASUserName;
SASConnection.SASPassword = sas_cfg.SASPassword;
SASConnection.SASUserNameUnix = sas_cfg.SASUserNameUnix;
SASConnection.SASPasswordUnix = sas_cfg.SASPasswordUnix;
SASConnection.SASUserNameOracle = sas_cfg.SASUserNameOracle;
SASConnection.SASPasswordOracle = sas_cfg.SASPasswordOracle;


 
long result = -1;


Log.Logger.Information("Ad Hoc Processes Start");
//COPY PASTE ADHOC FUNCTIONS HERE:
//COPY PASTE ADHOC FUNCTIONS HERE:
//COPY PASTE ADHOC FUNCTIONS HERE:



List<string> files_loaded = new List<string>();
files_loaded.Add("Oxford June -Radiology Cardiology Universe 2024.xlsx");
files_loaded.Add("United PCP- Rad & Card_June_2024.xlsx");
files_loaded.Add("Oxford June-Gastro Universe 2024.xlsx");
files_loaded.Add("United PCP-Gastro_June_2024.xlsx");
files_loaded.Add("Americhoice June-Radiology Cardiology Universe 2024.xlsx");
//MHP UGAP CLEANUP
await adHoc.cleanupMemberDataAsync(files_loaded);
//MHP LOAD TO VCT_DB
await adHoc.transferMHPDataAsync(files_loaded, "June", "2024");
return;




//GET FILE 'Create_Date' FROM EVICORE TAT REPORTING Mary Ann Dimartino
await adHoc.getReportsTimelinessAsync();
//GENERATE FINAL TAT REPORTS
await adHoc.generateTATReportsAsync();
return;


//GENERATE DYNAMIC EMAIL FOR PPACA_TAT Mary Ann Dimartino
await adHoc.PPACA_TAT_Email();
return;



//CHECK FOR NEW TEAMMATE IN AD AND EMAIL Kristy IF NEED BE
var adr = new ADDirectReportAlertsLR(config, db_sqsl);
result = await adr.RefreshTable();
return;


//PEG ETL Angela RS
await adHoc.getPEGSourceDataAsync();
//PEG REPORT
await adHoc.generatePEGReportsAsync();
//EBM ETL Angela RS
await adHoc.getEBMSourceDataAsync();
//EBM REPORT
await adHoc.generateEBMReportsAsync();
return;



//HANDLES TI MAPPING FOR ETG Brandee Shemo
//PARSES 3 FILES, LOADS TO DB, OUPUPTS ALL FILES PLUS SOME
//TODO CHANGE DB FROM ILUCA TO VC!!!!!!
await adHoc.UGAPConfig();
return;




//SAS CONNECT TEST
//SAS CONNECT TEST
//SAS CONNECT TEST
SASConnection.create_SAS_instance();
SASConnection.runStoredProcess("SAS_test_20240503.sas", "/hpsasfin/int/winfiles7/Program/UEP/ICUE_ADT/EI_ER_report/SAS_file/SDR");
SASConnection.destroy_SAS_instance();
return;


//INNA SAS PROCESS
await adHoc.MedicalNecessity_ACIS_Parser();
return;



//EVICORE MONTHLY PROCESS START
//EVICORE MONTHLY PROCESS START
//EVICORE MONTHLY PROCESS START
//CHECK FOR NEW EVICORE FILES EACH MONTH


var vds = new DataSourceVerification(config);
result = await vds.CheckDataSources();
return;


//PROCESS UHC_Scorecard_*_*.xls* INTO stg.EviCore_Scorecard
var esc = new EvicoreScorecard(config, db_sqsl);
result = await esc.LoadEvicoreScorecardData();
return;


//PROCESS NICE_UHCWestEligibility_*_Medicare_Final_for_membership.xlsx INTO stg.EviCore_NICEDetails
var nice = new NICEUHCWestEligibility(config, db_sqsl);
result = await nice.LoadNICEUHCWestEligibilityData();
return;

//PROCESS AMERICHOICE_Allstates_Auths Per 1000 by Modality with Exclusions_*_*_*.xlsx INTO stg.EviCore_AmerichoiceAllstatesAuths
var aasa = new EviCoreAmerichoiceAllstatesAuth(config, db_sqsl);
result = await aasa.LoadEviCoreAmerichoiceAllstatesAuthData();
return;

//PROCESS United_Enterprise_Wide_*_TAT_UHC_Enterprise_*_*.xlsx INTO stg.EviCore_TAT
var ppca = new PPACATAT(config, db_sqsl);
result = await ppca.LoadTATData();
return;


//PROCESS YTD - Cisco - UHC Metrics *_*.xlsx INTO stg.EviCore_YTDMetrics
var ytdm = new EviCoreYTDMetrics(config, db_sqsl);
result = await ytdm.LoadEviCoreYTDMetricsData();
return;

//PROCESS MHP Files INTO stg.MHP_Yearly_Universes
var mhp = new MHPUniverse(config, db_sqsl);
result = await mhp.LoadMHPUniverseData();
return;


//PROCESS Site of Care Report_*_*.xlsx INTO stg.SiteOfCare_Data_v3
var soc = new SiteOfCare(config, db_sqsl);
result = await soc.LoadSiteOfCareData();
return;



//PROCESS United Gastro Site of Care Report *_*.xlsx INTO stg.SiteOfCare_Gastro
var socg = new SiteOfCareGastro(config, db_sqsl);
result = await socg.LoadSiteOfCareData();
return;


//PROCESS CRC_Pivot_Rawdata_*.xlsx INTO stg.EviCore_MR_MembershipDetails
var mrm = new EviCoreMRMembershipDetails(config, db_sqsl);
result = await mrm.LoadEviCoreMRMembershipDetails();
return;




//EVICORE MONTHLY PROCESS END
//EVICORE MONTHLY PROCESS END
//EVICORE MONTHLY PROCESS END



//PREMIUM DESIGNATION ETL Angela RS
await adHoc.getETGSymmSourceDataAsync(18);


return;



Log.Logger.Information("Ad Hoc Processes End");



////BELOW IS NOT IN USE
////BELOW IS NOT IN USE
////BELOW IS NOT IN USE
////BELOW IS NOT IN USE
////BELOW IS NOT IN USE
////BELOW IS NOT IN USE
////BELOW IS NOT IN USE



//MARY ANN NOT IN USE
//await adHoc.runSLAAutomation();






//IRelationalDataAccess db_odbc = new ODBCDataAccess();



////STEP 3 CREATE NEW TABLES AND COLUMNS VIA INNA SQL
////var sql = "update a set a.PSU_NEW='New' from stg.IR_PCCM_Final as a inner join (select MBR_ID,MBR_PGM_ID,min(RPT_MTH) as min_mo from stg.IR_PCCM_Final where PSU_NEW_ORIG='New' group by MBR_ID,MBR_PGM_ID) as b on a.MBR_ID=b.MBR_ID and a.MBR_PGM_ID=b.MBR_PGM_ID and a.RPT_MTH=min_mo where a.PSU_NEW_ORIG='New'; update a set a.PSU_NEW='Retained' from stg.IR_PCCM_Final as a where a.PSU_NEW is null;";

////await db_sqsl.Execute(connectionString: adHoc.ConnectionStringMSSQL, sql);




//var sql = "TRUNCATE TABLE [stg].[IR_PCCM_Final_unq_v3];INSERT INTO [stg].[IR_PCCM_Final_unq_v3] ([MBR_ID] ,[INDV_ID] ,[MBR_PGM_ID] ,[PGM_CATGY_TYP_DESC] ,[PGM_TYP_DESC] ,[CALC_PGM_TYP] ,[NOM_DEPT_TYP_DESC] ,[NOM_RSN_TYP_DESC] ,[MBR_PGM_STS_TYP_DESC] ,[MBR_PGM_STS_RSN_TYP_DESC] ,[CREAT_DT] ,[PRE_ENRL_DT] ,[OPS_ENROLLED_DT] ,[OPS_ENGAGED_DT] ,[END_DT] ,[OPS_IDENTIFIED] ,[OPS_QUALIFIED] ,[OPS_ATTEMPTED] ,[OPS_CONTACTED] ,[OPS_MBR_CONTACTED] ,[OPS_ENROLLED] ,[OPS_ENGAGED] ,[PSU_IND] ,[PSU_NEW_ORIG] ,[RPT_MTH_YR_DISPLAY] ,[RPT_MTH] ,[RPT_YR] ,[RPT_DAYS] ,[PSU_NEW] ,[RPT_DATE]) select t. [MBR_ID] ,[INDV_ID] ,[MBR_PGM_ID] ,[PGM_CATGY_TYP_DESC] ,[PGM_TYP_DESC] ,[CALC_PGM_TYP] ,[NOM_DEPT_TYP_DESC] ,[NOM_RSN_TYP_DESC] ,[MBR_PGM_STS_TYP_DESC] ,[MBR_PGM_STS_RSN_TYP_DESC] ,[CREAT_DT] ,[PRE_ENRL_DT] ,[OPS_ENROLLED_DT] ,[OPS_ENGAGED_DT] ,[END_DT] ,[OPS_IDENTIFIED] ,[OPS_QUALIFIED] ,[OPS_ATTEMPTED] ,[OPS_CONTACTED] ,[OPS_MBR_CONTACTED] ,[OPS_ENROLLED] ,[OPS_ENGAGED] ,[PSU_IND] ,[PSU_NEW_ORIG] ,[RPT_MTH_YR_DISPLAY] ,[RPT_MTH] ,[RPT_YR] ,[RPT_DAYS] ,[PSU_NEW], cast(cast(t.RPT_YR*10000 + t.RPT_MTH*100 + 1 as varchar(255)) as date) as RPT_DATE from (select a.*, ROW_NUMBER() OVER(Partition by MBR_ID,RPT_MTH_YR_DISPLAY ORDER BY RPT_DAYS desc,END_DT desc) row_num from stg.IR_PCCM_Final as a) as t where row_num=1 order by MBR_ID,CREAT_DT,RPT_MTH;update a set QUAL_categ='Newly Qualified (Not Qualified in Any of the Previous 12 Months)' from stg.IR_PCCM_Final_unq_v3 as a where a.PSU_NEW='New' and OPS_QUALIFIED=1;update a set ENRL_CATEG='Newly Enrolled' from stg.IR_PCCM_Final_unq_v3 as a inner join (select MBR_ID,min(RPT_DATE) as min_RPT_DATE from stg.IR_PCCM_Final_unq_v3 where OPS_ENROLLED=1 group by MBR_ID) as m on a.MBR_ID=m.MBR_ID and a.RPT_DATE=min_RPT_DATE where OPS_ENROLLED=1;";

//await db_sqsl.Execute(connectionString: adHoc.ConnectionStringMSSQL, sql);




//sql = "SELECT [MBR_ID] ,[INDV_ID] ,[MBR_PGM_ID] ,[PGM_CATGY_TYP_DESC] ,[PGM_TYP_DESC] ,[CALC_PGM_TYP] ,[NOM_DEPT_TYP_DESC] ,[NOM_RSN_TYP_DESC] ,[MBR_PGM_STS_TYP_DESC] ,[MBR_PGM_STS_RSN_TYP_DESC] ,[CREAT_DT] ,[PRE_ENRL_DT] ,[OPS_ENROLLED_DT] ,[OPS_ENGAGED_DT] ,[END_DT] ,[OPS_IDENTIFIED] ,[OPS_QUALIFIED] ,[OPS_ATTEMPTED] ,[OPS_CONTACTED] ,[OPS_MBR_CONTACTED] ,[OPS_ENROLLED] ,[OPS_ENGAGED] ,[PSU_IND] ,[PSU_NEW_ORIG] ,[RPT_MTH_YR_DISPLAY] ,[RPT_MTH] ,[RPT_YR] ,[RPT_DAYS] ,[PSU_NEW] ,[RPT_DATE], QUAL_CATEG, ENRL_CATEG FROM stg.IR_PCCM_Final_unq_v3 ORDER BY MBR_ID ASC,[RPT_DATE] ASC";
//var uniq = await db_sqsl.LoadData<PCCM_Model>(connectionString: adHoc.ConnectionStringMSSQL, sql);

//Int64? mem_id = null;
//bool? Last_OPS = null;


//foreach (var un in uniq)
//{
//    if (mem_id != un.MBR_ID)
//    {
//        mem_id = un.MBR_ID;
//        Last_OPS = null;
//    }

//    if (un.QUAL_CATEG != null)
//    {
//        continue;
//    }


//    if (un.OPS_QUALIFIED == true)
//    {
//        if (un.PSU_NEW == "Retained")
//        {
//            if (Last_OPS == true)
//            {
//                un.QUAL_CATEG = "Qualified Prior Month";
//            }
//            else
//            {
//                un.QUAL_CATEG = "Newly Qualified (Qualified at Least Once in the Previous 12 Months)";
//            }
//        }
//    }
//    else if (un.OPS_QUALIFIED == false)
//    {
//        if (Last_OPS == true)
//        {
//            un.QUAL_CATEG = "Qualified Prior Month but Not Current Month";
//        }
//    }

//    Last_OPS = un.OPS_QUALIFIED;

//}

//mem_id = null;
//Last_OPS = null;

//foreach (var un in uniq)
//{ 
//    if (mem_id != un.MBR_ID)
//    {
//        mem_id = un.MBR_ID;
//        Last_OPS = null;
//    }

//    if (un.ENRL_CATEG != null)
//    {
//        continue;
//    }


//    if (un.OPS_ENROLLED == true)
//    {
//        un.ENRL_CATEG = "Retained";

//        //if (Last_OPS == true)
//        //{
//        //    un.ENRL_CATEG = "Retained Since Prior Month";
//        //}
//        //else if (Last_OPS == false)
//        //{
//        //    un.ENRL_CATEG = "Another category, will be clarified";
//        //}
//        //else 
//        //{
//        //    un.ENRL_CATEG = null;
//        //}

//    }
//    else if (un.OPS_ENROLLED == false)
//    {
//        if (Last_OPS == true)
//        {
//            un.ENRL_CATEG = "Enrolled Prior Month but not Current Month";
//        }
//        else
//        {
//            un.ENRL_CATEG = "Not Enrolled";
//        }

//        //else if (Last_OPS == false)
//        //{
//        //    un.ENRL_CATEG = "OPS_ENROLLED=0 for current month and OPS_ENROLLED=0 for Prior month  ";
//        //}
//        //else
//        //{
//        //    un.ENRL_CATEG = null; 
//        //}

//    }

//    Last_OPS = un.OPS_ENROLLED;

//}


//var columnss = typeof(PCCM_Model).GetProperties().Select(p => p.Name).ToArray();
//await db_sqsl.BulkSave<PCCM_Model>(connectionString: adHoc.ConnectionStringMSSQL, "stg.IR_PCCM_Final_unq_v3", uniq, columnss, truncate: true);

//return;



////var sql = "TRUNCATE TABLE [stg].[IR_PCCM_Final_Unq];INSERT INTO [stg].[IR_PCCM_Final_Unq] ([MBR_ID] ,[INDV_ID] ,[MBR_PGM_ID] ,[PGM_CATGY_TYP_DESC] ,[PGM_TYP_DESC] ,[CALC_PGM_TYP] ,[NOM_DEPT_TYP_DESC] ,[NOM_RSN_TYP_DESC] ,[MBR_PGM_STS_TYP_DESC] ,[MBR_PGM_STS_RSN_TYP_DESC] ,[CREAT_DT] ,[PRE_ENRL_DT] ,[OPS_ENROLLED_DT] ,[OPS_ENGAGED_DT] ,[END_DT] ,[OPS_IDENTIFIED] ,[OPS_QUALIFIED] ,[OPS_ATTEMPTED] ,[OPS_CONTACTED] ,[OPS_MBR_CONTACTED] ,[OPS_ENROLLED] ,[OPS_ENGAGED] ,[PSU_IND] ,[PSU_NEW_ORIG] ,[RPT_MTH_YR_DISPLAY] ,[RPT_MTH] ,[RPT_YR] ,[RPT_DAYS] ,[PSU_NEW] ,[RPT_DATE]) select t. [MBR_ID] ,[INDV_ID] ,[MBR_PGM_ID] ,[PGM_CATGY_TYP_DESC] ,[PGM_TYP_DESC] ,[CALC_PGM_TYP] ,[NOM_DEPT_TYP_DESC] ,[NOM_RSN_TYP_DESC] ,[MBR_PGM_STS_TYP_DESC] ,[MBR_PGM_STS_RSN_TYP_DESC] ,[CREAT_DT] ,[PRE_ENRL_DT] ,[OPS_ENROLLED_DT] ,[OPS_ENGAGED_DT] ,[END_DT] ,[OPS_IDENTIFIED] ,[OPS_QUALIFIED] ,[OPS_ATTEMPTED] ,[OPS_CONTACTED] ,[OPS_MBR_CONTACTED] ,[OPS_ENROLLED] ,[OPS_ENGAGED] ,[PSU_IND] ,[PSU_NEW_ORIG] ,[RPT_MTH_YR_DISPLAY] ,[RPT_MTH] ,[RPT_YR] ,[RPT_DAYS] ,[PSU_NEW], cast(cast(t.RPT_YR*10000 + t.RPT_MTH*100 + 1 as varchar(255)) as date) as RPT_DATE from (select a.*, ROW_NUMBER() OVER(Partition by MBR_ID,RPT_MTH_YR_DISPLAY ORDER BY RPT_DAYS desc,END_DT desc) row_num from stg.IR_PCCM_Final as a) as t where row_num=1 order by MBR_ID,CREAT_DT,RPT_MTH;update a set QUAL_categ='Newly Qualified (Not Qualified in Any of the Previous 12 Months)' from stg.IR_PCCM_Final_unq as a where a.PSU_NEW='New' and OPS_QUALIFIED=1;";

////await db_sqsl.Execute(connectionString: adHoc.ConnectionStringMSSQL, sql);


////sql = "SELECT [MBR_ID] ,[INDV_ID] ,[MBR_PGM_ID] ,[PGM_CATGY_TYP_DESC] ,[PGM_TYP_DESC] ,[CALC_PGM_TYP] ,[NOM_DEPT_TYP_DESC] ,[NOM_RSN_TYP_DESC] ,[MBR_PGM_STS_TYP_DESC] ,[MBR_PGM_STS_RSN_TYP_DESC] ,[CREAT_DT] ,[PRE_ENRL_DT] ,[OPS_ENROLLED_DT] ,[OPS_ENGAGED_DT] ,[END_DT] ,[OPS_IDENTIFIED] ,[OPS_QUALIFIED] ,[OPS_ATTEMPTED] ,[OPS_CONTACTED] ,[OPS_MBR_CONTACTED] ,[OPS_ENROLLED] ,[OPS_ENGAGED] ,[PSU_IND] ,[PSU_NEW_ORIG] ,[RPT_MTH_YR_DISPLAY] ,[RPT_MTH] ,[RPT_YR] ,[RPT_DAYS] ,[PSU_NEW] ,[RPT_DATE], QUAL_CATEG FROM stg.IR_PCCM_Final_unq ORDER BY MBR_ID ASC,[RPT_DATE] ASC";
////var uniq = await db_sqsl.LoadData<PCCM_Model>(connectionString: adHoc.ConnectionStringMSSQL, sql);

////Int64? mem_id = null;
////bool? Last_OPS_QUALIFIED = null;

////foreach (var un in uniq)
////{
////    if(mem_id != un.MBR_ID)
////    {
////        mem_id = un.MBR_ID;
////        Last_OPS_QUALIFIED = null;
////    }

////    if(un.QUAL_CATEG != null)
////    {
////        continue;
////    }


////    if(un.OPS_QUALIFIED == true )
////    {
////        if(un.PSU_NEW == "Retained")
////        {
////            if (Last_OPS_QUALIFIED == true)
////            {
////                un.QUAL_CATEG = "Qualified Prior Month";
////            }
////            else
////            {
////                un.QUAL_CATEG = "Newly Qualified (Qualified at Least Once in the Previous 12 Months)";
////            }
////        }
////    }
////    else if (un.OPS_QUALIFIED == false)
////    {
////        if (Last_OPS_QUALIFIED == true)
////        {
////            un.QUAL_CATEG = "Qualified Prior Month but Not Current Month";
////        }
////    }

////    Last_OPS_QUALIFIED = un.OPS_QUALIFIED;

////}
////var columnss = typeof(PCCM_Model).GetProperties().Select(p => p.Name).ToArray();
////await db_sqsl.BulkSave<PCCM_Model>(connectionString: adHoc.ConnectionStringMSSQL, "stg.IR_PCCM_Final_unq", uniq, columnss, truncate: true);

////return;


////STEP 1 GET SNOWFLAKE DATA
////var sql = "Select MBR_ID, INDV_ID, MBR_PGM_ID, PGM_CATGY_TYP_DESC, PGM_TYP_DESC, CALC_PGM_TYP, NOM_DEPT_TYP_DESC, NOM_RSN_TYP_DESC, MBR_PGM_STS_TYP_DESC, MBR_PGM_STS_RSN_TYP_DESC, CREAT_DT, PRE_ENRL_DT, OPS_ENROLLED_DT, OPS_ENGAGED_DT, END_DT,OPS_IDENTIFIED,OPS_QUALIFIED, OPS_ATTEMPTED,OPS_CONTACTED,OPS_MBR_CONTACTED,OPS_ENROLLED,OPS_ENGAGED,PSU_IND, PSU_NEW as PSU_NEW_ORIG from OHBI_PRD_CONSUME_DB.RPT_UHC.UHC_POPFUNNEL_RPT where PSU_IND=1";


////var t = await db_odbc.LoadData<PCCM_Model>(connectionString: adHoc.ConnectionStringSnowflake, sql);


////var columnss = typeof(PCCM_Model).GetProperties().Select(p => p.Name).ToArray();
////await db_sqsl.BulkSave<PCCM_Model>(connectionString: adHoc.ConnectionStringMSSQL, "stg.IR_PCCM", t, columnss, truncate: true);

////STEP 2 USE SNOWFLAKE ABOVE TO EXPAND TO FINAL
////sql = "SELECT * FROM stg.IR_PCCM ORDER BY MBR_ID,MBR_PGM_ID, CREAT_DT, END_DT;";
////List<PCCM_Model> pccm_final = new List<PCCM_Model>();
////var pccm = await db_sqsl.LoadData<PCCM_Model>(connectionString: adHoc.ConnectionStringMSSQL, sql);

////DateTime? create_dt = null;
////DateTime? end_dt = null;
////DateTime current_dt;
////int month_cnt = 1;
////int total_days = 0;

////foreach (var p in  pccm)
////{

////    create_dt = p.CREAT_DT;
////    end_dt = (p.END_DT == null ? DateTime.Now : p.END_DT);
////    current_dt = (DateTime)create_dt;

////    month_cnt = (((end_dt.Value.Year - create_dt.Value.Year) * 12) + end_dt.Value.Month - create_dt.Value.Month) + 1;

////    for (int i = 1; i <= month_cnt; i++)
////    {

////        var pcm = new PCCM_Model();
////        pcm.MBR_ID = p.MBR_ID;
////        pcm.INDV_ID =  p.INDV_ID ;
////        pcm.MBR_PGM_ID= p.MBR_PGM_ID ;
////        pcm.PGM_CATGY_TYP_DESC= p.PGM_CATGY_TYP_DESC;
////        pcm.PGM_TYP_DESC= p.PGM_TYP_DESC;
////        pcm.CALC_PGM_TYP= p.CALC_PGM_TYP;
////        pcm.NOM_DEPT_TYP_DESC= p.NOM_DEPT_TYP_DESC;
////        pcm.NOM_RSN_TYP_DESC= p.NOM_RSN_TYP_DESC;
////        pcm.MBR_PGM_STS_TYP_DESC= p.MBR_PGM_STS_TYP_DESC;
////        pcm.MBR_PGM_STS_RSN_TYP_DESC= p.MBR_PGM_STS_RSN_TYP_DESC;
////        pcm.CREAT_DT= p.CREAT_DT;
////        pcm.PRE_ENRL_DT= p.PRE_ENRL_DT;
////        pcm.OPS_ENROLLED_DT= p.OPS_ENROLLED_DT;
////        pcm.OPS_ENGAGED_DT= p.OPS_ENGAGED_DT;
////        pcm.END_DT= end_dt;
////        pcm.OPS_IDENTIFIED= p.OPS_IDENTIFIED;
////        pcm.OPS_QUALIFIED= p.OPS_QUALIFIED;
////        pcm.OPS_ATTEMPTED= p.OPS_ATTEMPTED;
////        pcm.OPS_CONTACTED= p.OPS_CONTACTED;
////        pcm.OPS_MBR_CONTACTED= p.OPS_MBR_CONTACTED;
////        pcm.OPS_ENROLLED= p.OPS_ENROLLED;
////        pcm.OPS_ENGAGED= p.OPS_ENGAGED;
////        pcm.PSU_IND= p.PSU_IND;
////        pcm.PSU_NEW_ORIG = p.PSU_NEW_ORIG;


////        pcm.RPT_MTH_YR_DISPLAY = current_dt.ToString("MMM") + " " + current_dt.ToString("yy");
////        pcm.RPT_MTH = current_dt.ToString("MM");
////        pcm.RPT_YR = current_dt.ToString("yyyy");



////        //ADD 1 to all day
////        if(p.CREAT_DT == p.END_DT)
////        {
////            total_days = 1;
////        }
////        else if (current_dt.Year == create_dt.Value.Year && current_dt.Month == create_dt.Value.Month)
////        {
////            if(current_dt.Year == end_dt.Value.Year && current_dt.Month == end_dt.Value.Month && current_dt.Day != end_dt.Value.Day)
////            {
////                total_days = (end_dt.Value.Day - current_dt.Day) + 1 ;
////            }
////            else
////            {
////                total_days = (DateTime.DaysInMonth(current_dt.Year, current_dt.Month) - current_dt.Day) + 1;
////            }

////        }
////        else if (current_dt.Year == end_dt.Value.Year && current_dt.Month == end_dt.Value.Month)
////        {
////            total_days = end_dt.Value.Day;

////        }
////        else
////        {
////            total_days = DateTime.DaysInMonth(current_dt.Year, current_dt.Month);
////        }

////        pcm.RPT_DAYS = total_days;

////        current_dt = current_dt.AddMonths(1);

////        pccm_final.Add(pcm);
////    }


////}



////columnss = typeof(PCCM_Model).GetProperties().Select(p => p.Name).ToArray();
////await db_sqsl.BulkSave<PCCM_Model>(connectionString: adHoc.ConnectionStringMSSQL, "stg.IR_PCCM_Final", pccm_final, columnss, truncate: true);

//////string filepath = "C:\\Users\\cgiorda\\Desktop\\Projects\\PCCM";

//////await adHoc.parseCSV(filepath, fileNamePrefix: "ir_", chrDelimiter : ',');


////return;


////
////await adHoc.getEDCSourceDataAsync();

////return;

//adHoc.PEGReportTemplatePath = "C:\\Users\\cgiorda\\Desktop\\Projects\\DQ&C Report Automation\\PEG Template\\341 PEG DQ&C Results - Template.xlsx";

////adHoc.EBMReportTemplatePath = "C:\\Users\\cgiorda\\Desktop\\Projects\\DQ&C Report Automation\\EBM Template\\342 EBM DQ&C Results - Template.xlsx";

////await adHoc.getPEGSourceDataAsync();
////await adHoc.generatePEGReportsAsync();
////await adHoc.generateEBMReportsAsync();


////await adHoc.getETGSymmSourceDataAsync(16);


////return;

////await adHoc.PPACA_TAT_Email();

////return;




//await adHoc.getETGSymmSourceDataAsync(15);



////await adHoc.generateEBMReportsAsync();
////await adHoc.generatePEGReportsAsync();







////List<string> files_loaded = new List<string>();
////files_loaded.Add("Oxford June -Gastro Universe 2023.xlsx");
////files_loaded.Add("United PCP- Gastro_June_2023.xlsx");
////files_loaded.Add("United PCP- Rad & Card_June_2023.xlsx");
////files_loaded.Add("Oxford June -Radiology Cardiology Universe 2023.xlsx");
////files_loaded.Add("Americhoice June -Radiology Cardiology Universe 2023.xlsx");


////await adHoc.cleanupMemberDataAsync(files_loaded);


////await adHoc.transferMHPDataAsync(files_loaded);

//return;

////List<string> files_loaded = new List<string>();
////files_loaded.Add("United PCP- Rad & Card_April_2023.xlsx");
////files_loaded.Add("Americhoice April -Radiology Cardiology Universe 2023.xlsx");
////files_loaded.Add("Oxford  April -Radiology Cardiology Universe 2023.xlsx");

//string connectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
////string connectionStringVC = "data source=VCT_DB;server=localhost;Persist Security Info=True;database=VCT_DB;Integrated Security=SSPI;connect timeout=300000;";
//string connectionStringVC = "Data Source=wn000103397;Initial Catalog=VCT_DB;Persist Security Info=True;User ID=vct_app_user;Password=BooWooDooFoo2023!!;connect timeout=300000;";
//string connectionStringUHN = "data source=UHN_Reporting;server=WP000074441CLS;Persist Security Info=True;database=UHN_Reporting;Integrated Security=SSPI;connect timeout=300000;";
//string connectionStringPD = "data source=UHPD_Analytics;server=DBSWP0662;Persist Security Info=True;database=UHPD_Analytics;Integrated Security=SSPI;connect timeout=300000;";
//string connectionStringTD = "Data Source=UDWPROD;User ID=cgiorda;Password=BooWooDooFoo2023!!;Authentication Mechanism=LDAP;Session Mode=TERADATA;Session Character Set=ASCII;Persist Security Info=true;Connection Timeout=99999;";

//string connectionStringUHPD = "data source=UHPD_Reporting;server=WP000052579;Persist Security Info=True;database=PD_Reporting;Integrated Security=SSPI;connect timeout=300000;";



//IRelationalDataAccess db_sql = new SqlDataAccess();
//IRelationalDataAccess db_td = new TeraDataAccess();
//string strSQL;
//string[] columns;





////await adHoc.getETGSymmSourceDataAsync();
//await adHoc.getEBMSourceDataAsync();
//await adHoc.getPEGSourceDataAsync();

////EBM DATA LOAD
////EBM DATA LOAD
////EBM DATA LOAD
////1 ebm.DQC_DATA_UHPD_SOURCE
//strSQL = "select cur.REPORT_CASE_ID, cur.REPORT_RULE_ID, cur.COND_NM, cur.RULE_DESC, cur.PREM_SPCL_CD, cur.CNFG_POP_SYS_ID, case when cur.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when cur.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when cur.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else 'UNKNOWN' end as LOB, Replace(Str(cur.UNET_MKT_NBR, 7), Space(1), '0') as MKT_NBR, cur.UNET_MKT_NBR, cur.MKT_DESC as UNET_MKT_DESC, cur.Cur_Version as Current_Version, cur.Cur_CMPLNT_CNT as Current_Market_Compliant, cur.Cur_OPRTNTY_CNT as Current_Market_Opportunity, cur.Cur_NAT_CMPLNC_CNT as Current_National_Compliant, cur.Cur_NAT_OPRTNTY_CNT as Current_National_Opportunity, prev.Prev_Version as Previous_Version, prev.Prev_CMPLNT_CNT as Previous_Market_Compliant, prev.Prev_OPRTNTY_CNT as Previous_Market_Opportunity, prev.Prev_NAT_CMPLNC_CNT as Previous_National_Compliant, prev.Prev_NAT_OPRTNTY_CNT as Previous_National_Opportunity, Concat(@@servername, ' - ', Db_Name()) as DTLocation, Cast(GetDate() as Date) as data_Extract_Dt from ( select a.REPORT_CASE_ID, a.REPORT_RULE_ID, a.PREM_SPCL_CD, Sum(a.CMPLNT_CNT) as Cur_CMPLNT_CNT, Sum(a.OPRTNTY_CNT) as Cur_OPRTNTY_CNT, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration) as Cur_Version, b.COND_NM, b.RULE_DESC, c.NAT_CMPLNC_CNT as Cur_NAT_CMPLNC_CNT, c.NAT_OPRTNTY_CNT as Cur_NAT_OPRTNTY_CNT, a.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC from PD_Reporting.DQC.DQC_342_EBM_QLTY_MPIN_MSR_SUMMARY a inner join PD_Reporting.DQC.DQC_342_EBM_RULE_DESCRIPTION b on a.REPORT_CASE_ID = b.REPORT_CASE_ID and a.REPORT_RULE_ID = b.REPORT_RULE_ID and a.Iteration = b.Iteration and a.PD_Version = b.PD_Version and a.Run = b.Run inner join PD_Reporting.DQC.DQC_342_EBM_QLTY_EXPT_MSR c on b.REPORT_CASE_ID = c.REPORT_CASE_ID and b.REPORT_RULE_ID = c.REPORT_RULE_ID and a.CNFG_POP_SYS_ID = c.CNFG_POP_SYS_ID and a.PREM_SPCL_CD = c.PREM_SPCL_CD and b.Iteration = c.Iteration and b.PD_Version = c.PD_Version and b.Run = c.Run inner join PD_Reporting.DQC.DQC_341_PROV_ROLLOUT_UNET_MKT d on a.MPIN = d.MPIN and c.Iteration = d.Iteration and c.PD_Version = d.PD_Version and c.Run = d.Run inner join PD_Reporting.DQC.DQC_341_UNET_MKT e on d.UNET_MKT_NBR = e.UNET_MKT_NBR inner join ( select b.* from ( select a.Iteration, a.Run, a.run_sequence, a.PREM_DESG_VER_NBR, Rank() over (Order by a.PREM_DESG_VER_NBR Desc, a.run_sequence Desc, a.Iteration Desc) as rank from ( select a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end as run_sequence, a.PREM_DESG_VER_NBR from PD_Reporting.DQC.DQC_342_EBM_QLTY_EXPT_MSR a group by a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end, a.PREM_DESG_VER_NBR ) a ) b where b.rank = 1 ) f on a.Iteration = f.Iteration and a.Run = f.Run and a.PREM_DESG_VER_NBR = f.PREM_DESG_VER_NBR group by a.REPORT_CASE_ID, a.REPORT_RULE_ID, a.PREM_SPCL_CD, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration), b.COND_NM, b.RULE_DESC, c.NAT_CMPLNC_CNT, c.NAT_OPRTNTY_CNT, a.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC ) cur left join ( select a.REPORT_CASE_ID, a.REPORT_RULE_ID, a.PREM_SPCL_CD, Sum(a.CMPLNT_CNT) as Prev_CMPLNT_CNT, Sum(a.OPRTNTY_CNT) as Prev_OPRTNTY_CNT, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration) as Prev_Version, b.COND_NM, b.RULE_DESC, c.NAT_CMPLNC_CNT as Prev_NAT_CMPLNC_CNT, c.NAT_OPRTNTY_CNT as Prev_NAT_OPRTNTY_CNT, a.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC from PD_Reporting.DQC.DQC_342_EBM_QLTY_MPIN_MSR_SUMMARY a inner join PD_Reporting.DQC.DQC_342_EBM_RULE_DESCRIPTION b on a.REPORT_CASE_ID = b.REPORT_CASE_ID and a.REPORT_RULE_ID = b.REPORT_RULE_ID and a.Iteration = b.Iteration and a.PD_Version = b.PD_Version and a.Run = b.Run inner join PD_Reporting.DQC.DQC_342_EBM_QLTY_EXPT_MSR c on b.REPORT_CASE_ID = c.REPORT_CASE_ID and b.REPORT_RULE_ID = c.REPORT_RULE_ID and a.CNFG_POP_SYS_ID = c.CNFG_POP_SYS_ID and a.PREM_SPCL_CD = c.PREM_SPCL_CD and b.Iteration = c.Iteration and b.PD_Version = c.PD_Version and b.Run = c.Run inner join PD_Reporting.DQC.DQC_341_PROV_ROLLOUT_UNET_MKT d on a.MPIN = d.MPIN and c.Iteration = d.Iteration and c.PD_Version = d.PD_Version and c.Run = d.Run inner join PD_Reporting.DQC.DQC_341_UNET_MKT e on d.UNET_MKT_NBR = e.UNET_MKT_NBR inner join ( select b.* from ( select a.Iteration, a.Run, a.run_sequence, a.PREM_DESG_VER_NBR, Rank() over (Order by a.PREM_DESG_VER_NBR Desc, a.run_sequence Desc, a.Iteration Desc) as rank from ( select a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end as run_sequence, a.PREM_DESG_VER_NBR from PD_Reporting.DQC.DQC_342_EBM_QLTY_EXPT_MSR a group by a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end, a.PREM_DESG_VER_NBR ) a ) b where b.rank = 2 ) f on a.Iteration = f.Iteration and a.Run = f.Run and a.PREM_DESG_VER_NBR = f.PREM_DESG_VER_NBR group by a.REPORT_CASE_ID, a.REPORT_RULE_ID, a.PREM_SPCL_CD, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration), b.COND_NM, b.RULE_DESC, c.NAT_CMPLNC_CNT, c.NAT_OPRTNTY_CNT, a.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC ) prev on cur.REPORT_CASE_ID = prev.REPORT_CASE_ID and cur.REPORT_RULE_ID = prev.REPORT_RULE_ID and cur.PREM_SPCL_CD = prev.PREM_SPCL_CD and cur.UNET_MKT_NBR = prev.UNET_MKT_NBR and cur.CNFG_POP_SYS_ID = prev.CNFG_POP_SYS_ID";

//var ebm = await db_sql.LoadData<DQC_DATA_EBM_UHPD_SOURCE_Model>(connectionString: connectionStringUHPD, strSQL);

//columns = typeof(DQC_DATA_EBM_UHPD_SOURCE_Model).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<DQC_DATA_EBM_UHPD_SOURCE_Model>(connectionString: connectionStringVC, "ebm.DQC_DATA_UHPD_SOURCE", ebm, columns, truncate: true);



////EBM DATA LOAD
////EBM DATA LOAD
////EBM DATA LOAD





////PEG DATA LOAD
////PEG DATA LOAD
////PEG DATA LOAD

////3 peg.PEG_ANCH_UHPD_SOURCE
//strSQL = "select b.PEG_ANCH_CATGY, b.PEG_ANCH_SBCATGY, b.PEG_ANCH_SBCATGY_DESC, a.PEG_ANCH_CATGY_ID, a.PEG_ANCH_CATGY_DESC, Concat(@@servername, ' - ', Db_Name()) as PACLocation from PD.CNFG_ANCH_SBCATGY b inner join PD.PEG_ANCHOR_CATEGORY a on b.PEG_ANCH_CATGY = a.PEG_ANCH_CATGY group by b.PEG_ANCH_CATGY, b.PEG_ANCH_SBCATGY, b.PEG_ANCH_SBCATGY_DESC, a.PEG_ANCH_CATGY_ID, a.PEG_ANCH_CATGY_DESC";
//var pa = await db_sql.LoadData<PEG_ANCH_Model>(connectionString: connectionStringPD, strSQL);
//columns = typeof(PEG_ANCH_Model).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<PEG_ANCH_Model>(connectionString: connectionStringVC, "peg.PEG_ANCH_UHPD_SOURCE", pa, columns, truncate: true);


////2 vct.Rate_Region
//strSQL = "select PD.RATE_REGION.MKT_NBR, PD.RATE_REGION.MKT_NM, PD.RATE_REGION.MAJ_MKT_NM, PD.RATE_REGION.RGN_NM, PD.RATE_REGION.MKT_RLLP_NM, Concat(@@servername, ' - ', Db_Name()) as RRLocation from PD.RATE_REGION";
//var rr = await db_sql.LoadData<Rate_Region_Model>(connectionString: connectionStringPD, strSQL);
//columns = typeof(Rate_Region_Model).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<Rate_Region_Model>(connectionString: connectionStringVC, "vct.Rate_Region", rr, columns, truncate: true);


////1 peg.DQC_DATA_UHPD_SOURCE
//strSQL = "select cur.PEG_ANCH_CATGY, cur.PEG_ANCH_SBCATGY, cur.PREM_SPCL_CD, cur.SVRTY_LVL_CD, cur.APR_DRG_RLLP_NBR, cur.QLTY_MSR_NM, cur.CNFG_POP_SYS_ID, case when cur.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when cur.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when cur.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else 'UNKNOWN' end as LOB, Replace(Str(cur.UNET_MKT_NBR, 7), Space(1), '0') as MKT_NBR, cur.UNET_MKT_NBR, cur.MKT_DESC as UNET_MKT_DESC, cur.Cur_Version as Current_Version, cur.Cur_CMPLNT_CNT as Current_Market_Compliant, cur.Cur_OPRTNTY_CNT as Current_Market_Opportunity, cur.Cur_NAT_CMPLNC_CNT as Current_National_Compliant, cur.Cur_NAT_OPRTNTY_CNT as Current_National_Opportunity, prev.Prev_Version as Previous_Version, prev.Prev_CMPLNT_CNT as Previous_Market_Compliant, prev.Prev_OPRTNTY_CNT as Previous_Market_Opportunity, prev.Prev_NAT_CMPLNC_CNT as Previous_National_Compliant, prev.Prev_NAT_OPRTNTY_CNT as Previous_National_Opportunity, Concat(@@servername, ' - ', Db_Name()) as DTLocation, Cast(GetDate() as Date) as data_Extract_Dt from ( select c.PEG_ANCH_SBCATGY, c.PEG_ANCH_CATGY, c.SVRTY_LVL_CD, c.PREM_SPCL_CD, Sum(c.CMPLNT_CNT) as Cur_CMPLNT_CNT, Sum(c.OPRTNTY_CNT) as Cur_OPRTNTY_CNT, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration) as Cur_Version, c.APR_DRG_RLLP_NBR, c.QLTY_MSR_NM, c.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC, f.NAT_CMPLNC_CNT as Cur_NAT_CMPLNC_CNT, f.NAT_OPRTNTY_CNT as Cur_NAT_OPRTNTY_CNT from ( select a.Iteration, a.Run, a.run_sequence, a.PREM_DESG_VER_NBR, Rank() over (Order by a.PREM_DESG_VER_NBR Desc, a.run_sequence Desc, a.Iteration Desc) as rank, a.PD_Version from ( select a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end as run_sequence, a.PREM_DESG_VER_NBR, a.PD_Version from PD_Reporting.DQC.DQC_341_PEG_QLTY_EXPT_MSR a group by a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end, a.PREM_DESG_VER_NBR, a.PD_Version ) a ) b inner join PD_Reporting.DQC.DQC_341_PEG_QLTY_MPIN_MSR_SUMMARY c on b.Iteration = c.Iteration and b.PD_Version = c.PD_Version and b.Run = c.Run inner join PD_Reporting.DQC.DQC_341_PROV_ROLLOUT_UNET_MKT d on c.MPIN = d.MPIN and c.Iteration = d.Iteration and c.PD_Version = d.PD_Version and c.Run = d.Run inner join PD_Reporting.DQC.DQC_341_UNET_MKT e on d.UNET_MKT_NBR = e.UNET_MKT_NBR inner join PD_Reporting.DQC.DQC_341_PEG_QLTY_EXPT_MSR f on c.PEG_ANCH_SBCATGY = f.PEG_ANCH_SBCATGY and c.PEG_ANCH_CATGY = f.PEG_ANCH_CATGY and c.SVRTY_LVL_CD = f.SVRTY_LVL_CD and c.QLTY_MSR_NM = f.QLTY_MSR_NM and c.CNFG_POP_SYS_ID = f.CNFG_POP_SYS_ID and c.PREM_SPCL_CD = f.PREM_SPCL_CD and d.Iteration = f.Iteration and d.PD_Version = f.PD_Version and d.Run = f.Run and c.APR_DRG_RLLP_NBR = f.APR_DRG_RLLP_NBR where b.rank = 1 group by c.PEG_ANCH_SBCATGY, c.PEG_ANCH_CATGY, c.SVRTY_LVL_CD, c.PREM_SPCL_CD, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration), c.APR_DRG_RLLP_NBR, c.QLTY_MSR_NM, c.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC, f.NAT_CMPLNC_CNT, f.NAT_OPRTNTY_CNT ) cur left join ( select c.PEG_ANCH_SBCATGY, c.PEG_ANCH_CATGY, c.SVRTY_LVL_CD, c.PREM_SPCL_CD, Sum(c.CMPLNT_CNT) as Prev_CMPLNT_CNT, Sum(c.OPRTNTY_CNT) as Prev_OPRTNTY_CNT, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration) as Prev_Version, c.APR_DRG_RLLP_NBR, c.QLTY_MSR_NM, c.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC, f.NAT_CMPLNC_CNT as Prev_NAT_CMPLNC_CNT, f.NAT_OPRTNTY_CNT as Prev_NAT_OPRTNTY_CNT from ( select a.Iteration, a.Run, a.run_sequence, a.PREM_DESG_VER_NBR, Rank() over (Order by a.PREM_DESG_VER_NBR Desc, a.run_sequence Desc, a.Iteration Desc) as rank, a.PD_Version from ( select a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end as run_sequence, a.PREM_DESG_VER_NBR, a.PD_Version from PD_Reporting.DQC.DQC_341_PEG_QLTY_EXPT_MSR a group by a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end, a.PREM_DESG_VER_NBR, a.PD_Version ) a ) b inner join PD_Reporting.DQC.DQC_341_PEG_QLTY_MPIN_MSR_SUMMARY c on b.Iteration = c.Iteration and b.PD_Version = c.PD_Version and b.Run = c.Run inner join PD_Reporting.DQC.DQC_341_PROV_ROLLOUT_UNET_MKT d on c.MPIN = d.MPIN and c.Iteration = d.Iteration and c.PD_Version = d.PD_Version and c.Run = d.Run inner join PD_Reporting.DQC.DQC_341_UNET_MKT e on d.UNET_MKT_NBR = e.UNET_MKT_NBR inner join PD_Reporting.DQC.DQC_341_PEG_QLTY_EXPT_MSR f on c.PEG_ANCH_SBCATGY = f.PEG_ANCH_SBCATGY and c.PEG_ANCH_CATGY = f.PEG_ANCH_CATGY and c.SVRTY_LVL_CD = f.SVRTY_LVL_CD and c.QLTY_MSR_NM = f.QLTY_MSR_NM and c.CNFG_POP_SYS_ID = f.CNFG_POP_SYS_ID and c.PREM_SPCL_CD = f.PREM_SPCL_CD and d.Iteration = f.Iteration and d.PD_Version = f.PD_Version and d.Run = f.Run and c.APR_DRG_RLLP_NBR = f.APR_DRG_RLLP_NBR where b.rank = 2 group by c.PEG_ANCH_SBCATGY, c.PEG_ANCH_CATGY, c.SVRTY_LVL_CD, c.PREM_SPCL_CD, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration), c.APR_DRG_RLLP_NBR, c.QLTY_MSR_NM, c.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC, f.NAT_CMPLNC_CNT, f.NAT_OPRTNTY_CNT ) prev on cur.PEG_ANCH_SBCATGY = prev.PEG_ANCH_SBCATGY and cur.PEG_ANCH_CATGY = prev.PEG_ANCH_CATGY and cur.SVRTY_LVL_CD = prev.SVRTY_LVL_CD and cur.PREM_SPCL_CD = prev.PREM_SPCL_CD and cur.APR_DRG_RLLP_NBR = prev.APR_DRG_RLLP_NBR and cur.QLTY_MSR_NM = prev.QLTY_MSR_NM and cur.CNFG_POP_SYS_ID = prev.CNFG_POP_SYS_ID and cur.UNET_MKT_NBR = prev.UNET_MKT_NBR";
//var dqc = await db_sql.LoadData<DQC_DATA_UHPD_SOURCE_Model>(connectionString: connectionStringUHPD, strSQL);
//columns = typeof(DQC_DATA_UHPD_SOURCE_Model).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<DQC_DATA_UHPD_SOURCE_Model>(connectionString: connectionStringVC, "peg.DQC_DATA_UHPD_SOURCE", dqc, columns, truncate: true);


////PEG DATA LOAD
////PEG DATA LOAD
////PEG DATA LOAD


////ETG DATA LOAD
////ETG DATA LOAD
////ETG DATA LOAD

////STEP 1 etg.NRX_Cost_UGAP_SOURCE
//strSQL = "select ETG_D.ETG_BAS_CLSS_NBR, ETG_D.TRT_CD, Count(Distinct ETG_D.INDV_SYS_ID) as MEMBER_COUNT, Count(Distinct ETG_D.EPSD_NBR) as EPSD_COUNT, Sum(ETG_D.TOT_ALLW_AMT) as ETGD_TOT_ALLW_AMT, Sum(ETG_D.RX_ALLW_AMT) as ETGD_RX_ALLW_AMT, case when Sum(ETG_D.TOT_ALLW_AMT) = 0 then 0 else NVL(Sum(ETG_D.RX_ALLW_AMT), 0) / Sum(ETG_D.TOT_ALLW_AMT) end as RX_RATE from ( select ED1.INDV_SYS_ID, ED1.EPSD_NBR, EN1.ETG_BAS_CLSS_NBR, EN1.ETG_TX_IND as TRT_CD, Sum(ED1.QLTY_INCNT_RDUC_AMT) as TOT_ALLW_AMT, Query1.RX_ALLW_AMT from CLODM001.ETG_DETAIL ED1 inner join CLODM001.ETG_NUMBER EN1 on ED1.ETG_SYS_ID = EN1.ETG_SYS_ID inner join CLODM001.DATE_FST_SRVC DFS1 on ED1.FST_SRVC_DT_SYS_ID = DFS1.FST_SRVC_DT_SYS_ID inner join ( select C.INDV_SYS_ID from ( select B.INDV_SYS_ID, Min(B.PHRM_BEN_FLG) as MIN_PHARMACY_FLG, Sum(B.NUM_DAY) as NUM_DAY from ( select a.INDV_SYS_ID, ( case when a.END_DT > '2022-12-31' then Cast('2022-12-31' as Date) else a.END_DT end - case when a.EFF_DT < '2022-01-01' then Cast('2022-01-01' as Date) else a.EFF_DT end) + 1 as NUM_DAY, a.PHRM_BEN_FLG from CLODM001.MEMBER_DETAIL_INPUT a where a.EFF_DT <= '2022-12-31' and a.END_DT >= '2022-01-01') as B group by B.INDV_SYS_ID ) C where C.MIN_PHARMACY_FLG = 'Y' and C.NUM_DAY >= 210 ) as MT on ED1.INDV_SYS_ID = MT.INDV_SYS_ID left join ( select ED2.INDV_SYS_ID, ED2.EPSD_NBR, Sum(ED2.QLTY_INCNT_RDUC_AMT) as RX_ALLW_AMT from CLODM001.ETG_DETAIL ED2 inner join CLODM001.DATE_FST_SRVC DFS2 on ED2.FST_SRVC_DT_SYS_ID = DFS2.FST_SRVC_DT_SYS_ID inner join CLODM001.HP_SERVICE_TYPE_CODE HSTC2 on ED2.HLTH_PLN_SRVC_TYP_CD_SYS_ID = HSTC2.HLTH_PLN_SRVC_TYP_CD_SYS_ID where DFS2.FST_SRVC_DT Between '2022-01-01'and '2022-12-31'  and ED2.QLTY_INCNT_RDUC_AMT > 0 and HSTC2.HLTH_PLN_SRVC_TYP_LVL_1_NM = 'PHARMACY' group by ED2.INDV_SYS_ID, ED2.EPSD_NBR ) Query1 on ED1.INDV_SYS_ID = Query1.INDV_SYS_ID and ED1.EPSD_NBR = Query1.EPSD_NBR where ED1.EPSD_NBR not in (0, -1) and DFS1.FST_SRVC_DT Between '2022-01-01' and '2022-12-31' and ED1.QLTY_INCNT_RDUC_AMT > 0 group by ED1.INDV_SYS_ID, ED1.EPSD_NBR, EN1.ETG_BAS_CLSS_NBR, EN1.ETG_TX_IND, Query1.RX_ALLW_AMT ) as ETG_D group by ETG_D.ETG_BAS_CLSS_NBR, ETG_D.TRT_CD";

//var nrxx = await db_td.LoadData<NRX_Cost_UGAPModel>(connectionString: connectionStringTD, strSQL);

//columns = typeof(NRX_Cost_UGAPModel).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<NRX_Cost_UGAPModel>(connectionString: connectionStringVC, "etg.NRX_Cost_UGAP_SOURCE", nrxx, columns, truncate: true);

////STEP 2 etg.ETG_Episodes_UGAP_SOURCE
////BROKEN APART DUE TO 200+ MILLION ROWS
//List<string> lst_lob = new List<string>();
//lst_lob.Add("COMMERCIAL");
//lst_lob.Add("MEDICARE");
//lst_lob.Add("MEDICAID");

//List<string> lst_yr = new List<string>();
//lst_yr.Add("2021");
//lst_yr.Add("2022");


//List<string> lst_qrt = new List<string>();
//lst_qrt.Add("01-01~03-31");
//lst_qrt.Add("04-01~06-30");
//lst_qrt.Add("07-01~09-30");
//lst_qrt.Add("10-01~12-31");

//int lob_id;

//bool blTruncate = true;


//foreach (var l in lst_lob)
//{
//    lob_id = (l == "COMMERCIAL" ? 1 : (l == "MEDICARE" ? 2 : 3));


//    Console.WriteLine("LOB:" + lob_id + " - " + l);

//    foreach (var y in lst_yr)
//    {


//        foreach (var q in lst_qrt)
//        {
//            var startdate = y + "-" + q.Split('~')[0];
//            var enddate  =  y + "-" + q.Split('~')[1];

//            Console.WriteLine("ETG Start Date: " + startdate);
//            Console.WriteLine("ETG End Date: " + enddate);


//            strSQL = "select es.EPSD_NBR, es.TOT_ALLW_AMT, en.SVRTY, en.ETG_BAS_CLSS_NBR, en.ETG_TX_IND, up.PROV_MPIN, es.TOT_NP_ALLW_AMT, " + lob_id + " as LOB_ID from CLODM001.ETG_SUMMARY es inner join CLODM001.ETG_NUMBER en on es.ETG_SYS_ID = en.ETG_SYS_ID inner join CLODM001.UNIQUE_PROVIDER up on es.RESP_UNIQ_PROV_SYS_ID = up.UNIQ_PROV_SYS_ID inner join CLODM001.INDIVIDUAL ind on es.INDV_SYS_ID = ind.INDV_SYS_ID inner join CLODM001.CLNOPS_CUSTOMER_SEGMENT ccs on ind.CLNOPS_CUST_SEG_SYS_ID = ccs.CLNOPS_CUST_SEG_SYS_ID inner join CLODM001.PRODUCT prod on ccs.PRDCT_SYS_ID = prod.PRDCT_SYS_ID inner join CLODM001.DATE_ETG_START DES on es.ETG_STRT_DT_SYS_ID = DES.ETG_STRT_DT_SYS_ID where es.EP_TYP_NBR in (0, 1, 2, 3) and es.TOT_ALLW_AMT >= 35 and ISNULL(en.SVRTY,'') <> '' and prod.PRDCT_LVL_1_NM = '" + l + "' and DES.ETG_STRT_DT >= '" + startdate + "' and DES.ETG_STRT_DT <= '" + enddate + "'";

//            Console.WriteLine("UGAP Pull Start Time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

//            var cnt = await db_td.ExecuteScalar(connectionString: connectionStringTD, "SELECT COUNT(*) FROM (" + strSQL + ") tmp;");

//            Console.WriteLine("Count: " + string.Format("{0:#,0}", cnt));

//            var ugap = await db_td.LoadData<ETG_Episodes_UGAP>(connectionString: connectionStringTD, strSQL);

//            columns = typeof(ETG_Episodes_UGAP).GetProperties().Select(p => p.Name).ToArray();
//            await db_sql.BulkSave<ETG_Episodes_UGAP>(connectionString: connectionStringVC, "etg.ETG_Episodes_UGAP_SOURCE", ugap, columns, truncate: blTruncate);
//            Console.WriteLine("UGAP Pull End Time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

//            blTruncate = false;
//            ugap = null;
//        }

//    }

//}


////STEP 3 etg.PrimarySpecWithCode_PDNDB_SOURCE
////1 NDB
//strSQL = "Select prov.MPIN, prov.ProvType, prov.PrimSpec NDB_SPCL_CD, spcl.SpecTypeCd, spcl.PrimaryInd, spcltyp.ShortDesc From dbo.PROVIDER As prov Left Join dbo.PROV_SPECIALTIES spcl On prov.MPIN = spcl.MPIN And spcl.PractInSpecInd = 'Y' Left Join dbo.SPECIALTY_TYPES spcltyp On spcl.SpecTypeCd = spcltyp.SpecTypeCd;";
//var ndb = await db_sql.LoadData<PrimarySpecUHNModel>(connectionString: connectionStringUHN, strSQL);
////2 PD
//strSQL = "select A.PREM_SPCL_CD, A.NDB_SPCL_TYP_CD from PD.CNFG_PREM_SPCL_MAP A where A.PREM_DESG_VER_NBR = 15;";
//var pd = await db_sql.LoadData<PremiumSpecPDModel>(connectionString: connectionStringPD, strSQL);
////3 JOIN NDB + PD INTO etg.PrimarySpecWithCode_PDNDB_SOURCE
//var pd_ndb = from n in ndb
//             join p in pd on n.NDB_SPCL_CD equals p.NDB_SPCL_TYP_CD into n_p_join
//             from np in n_p_join.DefaultIfEmpty()
//             select new PrimarySpecWithCodeModel
//             {
//                 MPIN = n.MPIN,
//                 ProvType = n.ProvType,
//                 NDB_SPCL_CD = n.NDB_SPCL_CD,
//                 SpecTypeCd = n.SpecTypeCd,
//                 PrimaryInd = n.PrimaryInd,
//                 ShortDesc = n.ShortDesc,
//                 PREM_SPCL_CD = ((n.NDB_SPCL_CD == "033" || n.NDB_SPCL_CD == "101" || n.NDB_SPCL_CD == "500") ? "CARDVS" : ((n.NDB_SPCL_CD == "007") ? "DERMA" : ((n.NDB_SPCL_CD == "038") ? "GERIA" : ((n.NDB_SPCL_CD == "093" || n.NDB_SPCL_CD == "504" || n.NDB_SPCL_CD == "059") ? "HEMAONC" : ((n.NDB_SPCL_CD == "479" || n.NDB_SPCL_CD == "095") ? "VASC" : ((n.NDB_SPCL_CD == "024" || n.NDB_SPCL_CD == "359" || n.NDB_SPCL_CD == "337" || n.NDB_SPCL_CD == "233") ? "PLASTIC" : (np == null ? null : np.PREM_SPCL_CD))))))),
//                 Secondary_Spec = (n.SpecTypeCd == "304" ? "CARDEP" : null)
//             };

//columns = typeof(PrimarySpecWithCodeModel).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<PrimarySpecWithCodeModel>(connectionString: connectionStringVC, "etg.PrimarySpecWithCode_PDNDB_SOURCE", pd_ndb, columns, truncate: true);


////UNUSED DELETE???
////strSQL = "SELECT prim.MPIN, CASE WHEN prim.[PREM_SPCL_CD] ='CARDCD' AND sec.[secondary_spec] = 'CARDEP' THEN 'CARDEP' ELSE CASE WHEN prim.[PREM_SPCL_CD] in ('NS', 'ORTHO') THEN 'NOS' ELSE [PREM_SPCL_CD] END END as [PREM_SPCL_CD] FROM (SELECT [PREM_SPCL_CD], [MPIN] FROM [vct].[PrimarySpecWithCode] GROUP BY [PREM_SPCL_CD], [MPIN] ) prim LEFT JOIN (SELECT [Secondary_Spec], [MPIN] FROM [vct].[PrimarySpecWithCode] GROUP BY [Secondary_Spec], [MPIN]) sec ON prim.MPIN = sec.MPIN";
////VC DB 


////STEP 4 etg.ETG_Cancer_Flag_PD_SOURCE
//strSQL = "select a.ETG_BASE_CLASS, a.CNCR_IND from PD.CNFG_CNCR_REL_ETG a inner join ( select Max(PD.CNFG_CNCR_REL_ETG.PREM_DESG_VER_NBR) as Max_PREM_DESG_VER_NBR from PD.CNFG_CNCR_REL_ETG ) b on a.PREM_DESG_VER_NBR = b.Max_PREM_DESG_VER_NBR";
//var can = await db_sql.LoadData<ETG_Cancer_Flag_PDModel>(connectionString: connectionStringPD, strSQL);
//columns = typeof(ETG_Cancer_Flag_PDModel).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave< ETG_Cancer_Flag_PDModel> (connectionString: connectionStringVC, "etg.ETG_Cancer_Flag_PD_SOURCE", can, columns, truncate: true);

////STEP 5 etg.PremiumNDBSpec_PD_SOURCE
//strSQL = "select n.NDB_SPCL_TYP_CD, n.SPCL_TYP_CD_DESC, c.PREM_SPCL_CD from pd.CLCT_SPCL_TYP_CD n left join ( select b.PREM_SPCL_CD, b.NDB_SPCL_TYP_CD from PD.CNFG_PREM_SPCL_MAP b inner join ( select Max(PD.CNFG_PREM_SPCL_MAP.PREM_DESG_VER_NBR) as Max_PREM_DESG_VER_NBR from PD.CNFG_PREM_SPCL_MAP ) a on b.PREM_DESG_VER_NBR = a.Max_PREM_DESG_VER_NBR ) c on n.NDB_SPCL_TYP_CD = c.NDB_SPCL_TYP_CD where n.NDB_SPCL_TYP_CD <> ' '";
//var pndb = await db_sql.LoadData<PremiumNDBSpecPDModel>(connectionString: connectionStringPD, strSQL);
//columns = typeof(PremiumNDBSpecPDModel).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<PremiumNDBSpecPDModel>(connectionString: connectionStringVC, "etg.PremiumNDBSpec_PD_SOURCE", pndb, columns, truncate: true);

////STEP 6 etg.ETG_Mapped_PD_SOURCE
//strSQL = "select LTRIM(RTRIM(a.PREM_SPCL_CD)) as PREM_SPCL_CD, a.TRT_CD, a.ETG_BASE_CLASS from pd.CNFG_ETG_SPCL a inner join ( select Max(PD.CNFG_ETG_SPCL.PREM_DESG_VER_NBR) as Max_PREM_DESG_VER_NBR from PD.CNFG_ETG_SPCL ) Query1 on a.PREM_DESG_VER_NBR = Query1.Max_PREM_DESG_VER_NBR";
//var map = await db_sql.LoadData<ETG_Mapped_PD>(connectionString: connectionStringPD, strSQL);
//columns = typeof(ETG_Mapped_PD).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<ETG_Mapped_PD>(connectionString: connectionStringVC, "etg.ETG_Mapped_PD_SOURCE", map, columns, truncate: true);






////UGAP TO VC DB FORM ETG UGAP CFG REPORT
//strSQL = "Select distinct ETG_BAS_CLSS_NBR, MPC_NBR from CLODM001.ETG_NUMBER";

//var u = await db_td.LoadData<MPCNBR_UGAPModel>(connectionString: connectionStringTD, strSQL);

//columns = typeof(MPCNBR_UGAPModel).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<MPCNBR_UGAPModel>(connectionString: connectionStringVC, "vct.ETG_MPCNBR_UGAP", u, columns, truncate: true);



//return;

////strSQL = "SELECT * FROM  [IL_UCA].[dbo].[cs_product_map];";
////var pm = await db_sql.LoadData<CS_Product_Map>(connectionString: connectionString, strSQL);
////columns = typeof(CS_Product_Map).GetProperties().Select(p => p.Name).ToArray();
////await db_sql.BulkSave<CS_Product_Map>(connectionString: connectionStringVC, "vct.cs_product_map", pm, columns, truncate: true);



//strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Group_State];";
//var gs = await db_sql.LoadData<MHP_Group_State_Model>(connectionString: connectionString, strSQL);
//columns = typeof(MHP_Group_State_Model).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<MHP_Group_State_Model>(connectionString: connectionStringVC, "mhp.MHP_Group_State", gs, columns, truncate:true);




//strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Universes_Filter_Cache];";
//var fs = await db_sql.LoadData<MHP_Reporting_Filters>(connectionString: connectionString, strSQL);
//columns = typeof(MHP_Reporting_Filters).GetProperties().Select(p => p.Name).ToArray();
//await db_sql.BulkSave<MHP_Reporting_Filters>(connectionString: connectionStringVC, "mhp.MHP_Universes_Filter_Cache", fs, columns, truncate: true);



//return;

//StringBuilder sb = new StringBuilder();

//foreach (string f in files_loaded)
//{
//    sb.Append("'" +  f + "',");
//}

//strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Yearly_Universes]  WHERE file_name in (" + sb.ToString().TrimEnd(',') + ");";


//var mhp = await db_sql.LoadData<MHPUniverseModel>(connectionString: connectionString, strSQL);


// columns = typeof(MHPUniverseModel).GetProperties().Select(p => p.Name).ToArray();


//await db_sql.BulkSave<MHPUniverseModel>(connectionString: connectionStringVC, "mhp.MHP_Yearly_Universes", mhp, columns);

//////////////////////////////////////////////////////////////////////////////



//strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] WHERE mhp_uni_id in (SELECT [mhp_uni_id] FROM [IL_UCA].[stg].[MHP_Yearly_Universes] WHERE file_name in (" + sb.ToString().TrimEnd(',') + "));";


//var mhp_ugap = await db_sql.LoadData<MHPMemberDetailsModel>(connectionString: connectionString, strSQL);


//columns = typeof(MHPMemberDetailsModel).GetProperties().Select(p => p.Name).ToArray();


//await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: connectionStringVC, "mhp.MHP_Yearly_Universes_UGAP", mhp_ugap, columns);



//return;





////adHoc.ConnectionStringMSSQL = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
////adHoc.TableMHP = "stg.MHP_Yearly_Universes";
////adHoc.ConnectionStringTD = "Data Source=UDWPROD;User ID=cgiorda;Password=BooWooDooFoo2023!!;Authentication Mechanism=LDAP;Session Mode=TERADATA;Session Character Set=ASCII;Persist Security Info=true;Connection Timeout=99999;";
////adHoc.TableUGAP = "stg.MHP_Yearly_Universes_UGAP";
////adHoc.Limit = 3000;

////List<string> files_loaded = new List<string>();
////files_loaded.Add("Americhoice March -Radiology Cardiology Universe 2023.xlsx");
////files_loaded.Add("Oxford March-Radiology Cardiology Universe 2023.xls");
////files_loaded.Add("United PCP- Rad & Card_March_2023.xlsx");
////await adHoc.cleanupMemberDataAsync(files_loaded);

////return;




////var closed_xml = new ClosedXMLFunctions();


////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////             new KeyValuePair<string, string>("State of Issue","State_of_Issue"),
////            new KeyValuePair<string, string>("State of Residence","State_of_Residence"),
////            new KeyValuePair<string, string>("Enrollee First Name","Enrollee_First_Name"),
////            new KeyValuePair<string, string>("Enrollee Last Name","Enrollee_Last_Name"),
////            new KeyValuePair<string, string>("Cardholder ID","Cardholder_ID"),
////            new KeyValuePair<string, string>("Funding Arrangement","Funding_Arrangement"),
////            new KeyValuePair<string, string>("Authorization","Authorization"),
////            new KeyValuePair<string, string>("Authorization Type","Authorization_Type"),
////            new KeyValuePair<string, string>("Date the request was received","Request_Date"),
////            new KeyValuePair<string, string>("Time the request was received","Request_Time"),
////            new KeyValuePair<string, string>("Request Decision","Request_Decision"),
////            new KeyValuePair<string, string>("Date of Decision","Decision_Date"),
////            new KeyValuePair<string, string>("Time of Decision","Decision_Time"),
////            new KeyValuePair<string, string>("Decision Reason","Decision_Reason"),
////            new KeyValuePair<string, string>("Was Extension Taken","Extension_Taken"),
////            new KeyValuePair<string, string>("Was Extension Taken?","Extension_Taken"),
////            new KeyValuePair<string, string>("Date of member notification of extension","Member_Notif_Extension_Date"),
////            new KeyValuePair<string, string>("Date additional information received","Additional_Info_Date"),
////            new KeyValuePair<string, string>("Date oral notification provided to enrollee","Oral_Notification_Enrollee_Date"),
////            new KeyValuePair<string, string>("Time oral notification provided to enrollee","Oral_Notification_Enrollee_Time"),
////            new KeyValuePair<string, string>("Date oral notification provided to provider","Oral_Notification_Provider_Date"),
////            new KeyValuePair<string, string>("Time oral notification provided to provider","Oral_Notification_Provider_Time"),
////            new KeyValuePair<string, string>("Date written notification sent to enrollee","Written_Notification_Enrollee_Date"),
////            new KeyValuePair<string, string>("Time written notification sent to enrollee","Written_Notification_Enrollee_Time"),
////            new KeyValuePair<string, string>("Date written notification sent to provider","Written_Notification_Provider_Date"),
////            new KeyValuePair<string, string>("Time written notification sent to provider","Written_Notification_Provider_Time"),
////            new KeyValuePair<string, string>("Primary Procedure Code(s) Requested","Primary_Procedure_Code_Req"),
////            new KeyValuePair<string, string>("Primary Procedure Code Requested","Primary_Procedure_Code_Req"),
////            new KeyValuePair<string, string>("Procedure Code Description","Procedure_Code_Description"),
////            new KeyValuePair<string, string>("Primary Diagnosis Code","Primary_Diagnosis_Code"),
////            new KeyValuePair<string, string>("Diagnosis Description","Diagnosis_Code_Description"),
////            new KeyValuePair<string, string>("Diagnosis Code Description","Diagnosis_Code_Description"),
////            new KeyValuePair<string, string>("Place of Service","Place_of_Service"),
////            new KeyValuePair<string, string>("Member Date of Birth","Member_Date_of_Birth"),
////            new KeyValuePair<string, string>("Was an urgent request made but processed as standard?","Urgent_Processed_Standard"),
////            new KeyValuePair<string, string>("Date of request for additional information","Request_Additional_Info_Date"),
////            new KeyValuePair<string, string>("Date additional information requested","Request_Additional_Info_Date"),
////            new KeyValuePair<string, string>("First Tier, Downstream, and Related Entity","FirstTier_Downstream_RelatedEntity"),
////            new KeyValuePair<string, string>("Par/Non-Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("PAR/NON PAR","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par Non Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par Non/ Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par-Non-Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par/Non-Par","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Inpatient/Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Inpatient Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Inpatient /Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Delegate Number","Delegate_Number"),
////            new KeyValuePair<string, string>("ProgramType","ProgramType"),
////            new KeyValuePair<string, string>("Program Type","ProgramType"),
////            new KeyValuePair<string, string>("Insurance Carrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("InsCarrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("Insurance_Carrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("Group Number","Group_Number"),
////            new KeyValuePair<string, string>("Method of Contact","Intake_Method"),
////            new KeyValuePair<string, string>("Intake Method","Intake_Method"),
////            new KeyValuePair<string, string>("MethodofContactDesc","Intake_Method")
////            };


////try
////{

////    string file_path = @"C:\Users\cgiorda\Desktop\Projects\MHPUniverse\";
////    string file_name = @"United PCP- Rad & Card_March_2023.xlsx";
////    string sheet_name = "Rad & Card IFP - U12";
////    var mhp = closed_xml.ImportExcel<MHPUniverseModel>(file_path + file_name, sheet_name, "A1:AR1", 2, nullCheck: "State of Issue");
////    foreach (var m in mhp)
////    {
////        //NOT IN SHEET
////        m.file_month = 3;
////        m.file_year = 2023;
////        m.file_date = new DateTime(2023, 3, 01);
////        m.sheet_name = sheet_name;//strType
////        m.file_name = file_name;
////        m.file_path = "\\\\NASGWFTP03\\Care_Core_FTP_Files\\Radiology";
////        m.classification = "IFP";

////    }

////    string cs = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
////    IRelationalDataAccess db_sql = new SqlDataAccess();
////    string[] columns = typeof(MHPUniverseModel).GetProperties().Select(p => p.Name).ToArray();

////    await db_sql.BulkSave<MHPUniverseModel>(connectionString: cs, "stg.MHP_Yearly_Universes", mhp, columns);

////}
////catch (Exception e)
////{
////    var s = e.Message;
////}


////return;






////CREATE INDEX indx_mhp_uni_id ON[stg].[MHP_Yearly_Universes_UGAP_2023_rep] (mhp_uni_id);
////CREATE INDEX indx_LEG_ENTY_NBR ON[stg].[MHP_Yearly_Universes_UGAP_2023_rep] (LEG_ENTY_NBR);
////CREATE INDEX indx_LEG_ENTY_FULL_NM ON[stg].[MHP_Yearly_Universes_UGAP_2023_rep] (LEG_ENTY_FULL_NM);
////CREATE INDEX indx_MKT_SEG_RLLP_DESC ON[stg].[MHP_Yearly_Universes_UGAP_2023_rep] (MKT_SEG_RLLP_DESC);
////CREATE INDEX indx_FINC_ARNG_DESC ON[stg].[MHP_Yearly_Universes_UGAP_2023_rep] (FINC_ARNG_DESC);
////CREATE INDEX indx_MKT_TYP_DESC ON[stg].[MHP_Yearly_Universes_UGAP_2023_rep] (MKT_TYP_DESC);
////CREATE INDEX indx_CS_CO_CD_ST ON [stg].[MHP_Yearly_Universes_UGAP_2023_rep] (CS_CO_CD_ST);
////CREATE INDEX indx_PRDCT_SYS_ID ON[stg].[MHP_Yearly_Universes_UGAP_2023_rep] (PRDCT_SYS_ID);
////CREATE INDEX indx_CS_PRDCT_CD_SYS_ID ON[stg].[MHP_Yearly_Universes_UGAP_2023_rep] (CS_PRDCT_CD_SYS_ID);
////CREATE INDEX indx_CS_CO_CD ON[stg].[MHP_Yearly_Universes_UGAP_2023_rep] (CS_CO_CD);
////CREATE INDEX indx_PRDCT_CD_DESC ON[stg].[MHP_Yearly_Universes_UGAP_2023_rep] (PRDCT_CD_DESC);
////CREATE INDEX indx_State_of_Issue ON[stg].[MHP_Yearly_Universes_2023_rep] (State_of_Issue);
////CREATE INDEX indx_Authorization ON[stg].[MHP_Yearly_Universes_2023_rep] ([Authorization]);
////CREATE INDEX indx_Request_Decision ON[stg].[MHP_Yearly_Universes_2023_rep] (Request_Decision);
////CREATE INDEX indx_Request_Date ON[stg].[MHP_Yearly_Universes_2023_rep] (Request_Date);
////CREATE INDEX indx_Authorization_Type ON[stg].[MHP_Yearly_Universes_2023_rep] (Authorization_Type);
////CREATE INDEX indx_Decision_Reason ON[stg].[MHP_Yearly_Universes_2023_rep] (Decision_Reason);
////CREATE INDEX indx_file_name ON[stg].[MHP_Yearly_Universes_2023_rep] (file_name);
////CREATE INDEX indx_sheet_name ON[stg].[MHP_Yearly_Universes_2023_rep] (sheet_name);
////char chrDelimiter = '|';
////List<string>? strLstColumnNames = null;
////StreamReader? csvreader = null;
////string _strTableName;
////string[] strLstFiles = Directory.GetFiles(@"C:\Users\cgiorda\Desktop\Projects\UGAP Configuration", "*.txt", SearchOption.TopDirectoryOnly);
////string? strInputLine = "";
////string[] csvArray;
////string strSQL;
////int intBulkSize = 10000;
////var connectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
////IRelationalDataAccess db_dest = new SqlDataAccess();
////System.Data.DataTable dtTransfer = new System.Data.DataTable();
////System.Data.DataRow? drCurrent = null;
////foreach (var strFile in  strLstFiles)
////{
////    var filename = "ugapcfg_" +Path.GetFileName(strFile).Replace(".txt", "");

////    var table = CommonFunctions.getCleanTableName(filename);
////    var tmp_table = table.Substring(0, Math.Min(28, table.Length)) + "_TMP";


////    csvreader = new StreamReader(strFile);
////    while ((strInputLine = csvreader.ReadLine()) != null)
////    {
////        csvArray = strInputLine.Split(new char[] { chrDelimiter });
////        //FIRST PASS ONLY GETS COLUMNS AND CREATES TABLE SQL
////        if (strLstColumnNames == null)
////        {
////            strLstColumnNames = new List<string>();
////            //GET AND CLEAN COLUMN NAMES FOR TABLE
////            foreach (string c in csvArray)
////            {
////                var colName = c.getSafeFileName();
////                strLstColumnNames.Add(colName.ToUpper());
////            }


////            //SQL FOR TMP TABLE TO STORE ALL VALUES A VARCHAR(MAX)
////            strSQL = CommonFunctions.getCreateTmpTableScript("stg", tmp_table, strLstColumnNames);
////            await db_dest.Execute(connectionString: connectionString, strSQL);

////            strSQL = "SELECT * FROM [stg].[" + tmp_table + "]; ";
////            //CREATE TMP TABLE AND COLLECT NEW DB TABLE FOR BULK TRANSFERS
////            dtTransfer = await db_dest.LoadDataTable(connectionString, strSQL);
////            dtTransfer.TableName = "stg." + tmp_table;

////            //GOT COLUMNS, CREATED TMP TABLE FOR FIRST PASS
////            continue;
////        }
////        //CLONE ROW FOR TRANSFER
////        drCurrent = dtTransfer.NewRow();
////        //POPULATE ALL COLUMNS FOR CURRENT ROW
////        for (int i = 0; i < strLstColumnNames.Count; i++)
////        {
////            drCurrent[strLstColumnNames[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : csvArray[i].TrimStart('\"').TrimEnd('\"'));

////        }
////        dtTransfer.Rows.Add(drCurrent);

////        if (dtTransfer.Rows.Count == intBulkSize) //intBulkSize = 10000 DEFAULT
////        {
////            await db_dest.BulkSave(connectionString: connectionString, dtTransfer);
////            dtTransfer.Rows.Clear();
////        }


////    }

////    //CATCH REST OF UPLOADS OUTSIDE CSV LOOP
////    if (dtTransfer.Rows.Count > 0)
////        await db_dest.BulkSave(connectionString: connectionString, dtTransfer);



////    strSQL = CommonFunctions.getTableAnalysisScript("stg", tmp_table, strLstColumnNames);
////    var dataTypes = (await db_dest.LoadData<DataTypeModel>(connectionString: connectionString, strSQL));

////    strSQL = CommonFunctions.getCreateFinalTableScript("stg", table, dataTypes);
////    await db_dest.Execute(connectionString: connectionString, strSQL);

////    strSQL = CommonFunctions.getSelectInsertScript("stg", tmp_table, table, strLstColumnNames);
////    await db_dest.Execute(connectionString: connectionString, strSQL);

////    strLstColumnNames = null;
////}











////var adHoc = new AdHoc();

////adHoc.ConnectionStringMSSQL = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
////adHoc.TableMHP = "stg.MHP_Yearly_Universes_2023_rep";
////adHoc.ConnectionStringTD = "Data Source=UDWPROD;User ID=cgiorda;Password=BooWooDooFoo2023!!;Authentication Mechanism=LDAP;Session Mode=TERADATA;Session Character Set=ASCII;Persist Security Info=true;Connection Timeout=99999;";
////adHoc.TableUGAP = "stg.MHP_Yearly_Universes_UGAP_2023_rep";
////adHoc.Limit = 3000;

////List<string> files_loaded = new List<string>();
////files_loaded.Add("United PCP-Rad & Card_June_2022.xlsx");
////files_loaded.Add("United PCP-Rad & Card_July_2022.xlsx");
////files_loaded.Add("United PCP- Rad & Card_May_2022.xlsx");
////files_loaded.Add("Oxford February-Radiology Cardiology Universe 2023.xls");
////files_loaded.Add("Americhoice February- Radiology Cardiology Universe 2023.xlsx");
////files_loaded.Add("Americhoice January- Radiology Cardiology Universe 2023.xlsx");
////files_loaded.Add("United PCP- Rad & Card_February_2023.xlsx");
////files_loaded.Add("Oxford January -Radiology Cardiology Universe 2023.xlsx");
////files_loaded.Add("United PCP- Rad & Card_January_2023.xlsx");
////await adHoc.cleanupMemberDataAsync(files_loaded);

////return;


////var closed_xml = new ClosedXMLFunctions();


////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////             new KeyValuePair<string, string>("State of Issue","State_of_Issue"),
////            new KeyValuePair<string, string>("State of Residence","State_of_Residence"),
////            new KeyValuePair<string, string>("Enrollee First Name","Enrollee_First_Name"),
////            new KeyValuePair<string, string>("Enrollee Last Name","Enrollee_Last_Name"),
////            new KeyValuePair<string, string>("Cardholder ID","Cardholder_ID"),
////            new KeyValuePair<string, string>("Funding Arrangement","Funding_Arrangement"),
////            new KeyValuePair<string, string>("Authorization","Authorization"),
////            new KeyValuePair<string, string>("Authorization Type","Authorization_Type"),
////            new KeyValuePair<string, string>("Date the request was received","Request_Date"),
////            new KeyValuePair<string, string>("Time the request was received","Request_Time"),
////            new KeyValuePair<string, string>("Request Decision","Request_Decision"),
////            new KeyValuePair<string, string>("Date of Decision","Decision_Date"),
////            new KeyValuePair<string, string>("Time of Decision","Decision_Time"),
////            new KeyValuePair<string, string>("Decision Reason","Decision_Reason"),
////            new KeyValuePair<string, string>("Was Extension Taken","Extension_Taken"),
////            new KeyValuePair<string, string>("Was Extension Taken?","Extension_Taken"),
////            new KeyValuePair<string, string>("Date of member notification of extension","Member_Notif_Extension_Date"),
////            new KeyValuePair<string, string>("Date additional information received","Additional_Info_Date"),
////            new KeyValuePair<string, string>("Date oral notification provided to enrollee","Oral_Notification_Enrollee_Date"),
////            new KeyValuePair<string, string>("Time oral notification provided to enrollee","Oral_Notification_Enrollee_Time"),
////            new KeyValuePair<string, string>("Date oral notification provided to provider","Oral_Notification_Provider_Date"),
////            new KeyValuePair<string, string>("Time oral notification provided to provider","Oral_Notification_Provider_Time"),
////            new KeyValuePair<string, string>("Date written notification sent to enrollee","Written_Notification_Enrollee_Date"),
////            new KeyValuePair<string, string>("Time written notification sent to enrollee","Written_Notification_Enrollee_Time"),
////            new KeyValuePair<string, string>("Date written notification sent to provider","Written_Notification_Provider_Date"),
////            new KeyValuePair<string, string>("Time written notification sent to provider","Written_Notification_Provider_Time"),
////            new KeyValuePair<string, string>("Primary Procedure Code(s) Requested","Primary_Procedure_Code_Req"),
////            new KeyValuePair<string, string>("Primary Procedure Code Requested","Primary_Procedure_Code_Req"),
////            new KeyValuePair<string, string>("Procedure Code Description","Procedure_Code_Description"),
////            new KeyValuePair<string, string>("Primary Diagnosis Code","Primary_Diagnosis_Code"),
////            new KeyValuePair<string, string>("Diagnosis Description","Diagnosis_Code_Description"),
////            new KeyValuePair<string, string>("Diagnosis Code Description","Diagnosis_Code_Description"),
////            new KeyValuePair<string, string>("Place of Service","Place_of_Service"),
////            new KeyValuePair<string, string>("Member Date of Birth","Member_Date_of_Birth"),
////            new KeyValuePair<string, string>("Was an urgent request made but processed as standard?","Urgent_Processed_Standard"),
////            new KeyValuePair<string, string>("Date of request for additional information","Request_Additional_Info_Date"),
////            new KeyValuePair<string, string>("Date additional information requested","Request_Additional_Info_Date"),
////            new KeyValuePair<string, string>("First Tier, Downstream, and Related Entity","FirstTier_Downstream_RelatedEntity"),
////            new KeyValuePair<string, string>("Par/Non-Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("PAR/NON PAR","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par Non Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par Non/ Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par-Non-Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par/Non-Par","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Inpatient/Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Inpatient Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Inpatient /Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Delegate Number","Delegate_Number"),
////            new KeyValuePair<string, string>("ProgramType","ProgramType"),
////            new KeyValuePair<string, string>("Program Type","ProgramType"),
////            new KeyValuePair<string, string>("Insurance Carrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("InsCarrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("Insurance_Carrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("Group Number","Group_Number"),
////            new KeyValuePair<string, string>("Method of Contact","Intake_Method"),
////            new KeyValuePair<string, string>("Intake Method","Intake_Method"),
////            new KeyValuePair<string, string>("MethodofContactDesc","Intake_Method")
////            };

////string file_path = @"C:\Users\cgiorda\Desktop\Projects\MHPUniverse\";
////string file_name = @"United PCP- Rad & Card_May_2022.xlsx";
////string sheet_name = "Rad & Card - U12";
////var mhp = closed_xml.ImportExcel<MHPUniverseModel>(file_path + file_name, sheet_name, "A1:AN1", 2, nullCheck: "State of Issue");
////foreach (var m in mhp)
////{
////    //NOT IN SHEET
////    m.file_month = 5;
////    m.file_year = 2022;
////    m.file_date = new DateTime(2022, 5, 01);
////    m.sheet_name = sheet_name;//strType
////    m.file_name = file_name;
////    m.file_path = "\\\\NASGWFTP03\\Care_Core_FTP_Files\\Radiology";
////    m.classification = "IFP";

////}

////string connectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
////IRelationalDataAccess db_sql = new SqlDataAccess();
////string[] columns = typeof(MHPUniverseModel).GetProperties().Select(p => p.Name).ToArray();

////await db_sql.BulkSave<MHPUniverseModel>(connectionString: connectionString, "stg.MHP_Yearly_Universes_2023_rep", mhp, columns);


////return;




















////string connectionString = "Data Source=localhost;Initial Catalog=PremiumDesignation_DB;Persist Security Info=True;Integrated Security=SSPI;connect timeout=300000;";
////IRelationalDataAccess db_sql = new SqlDataAccess();



////string strSQL = "SELECT * FROM [dbo].[VW_ETG_Symmetry_Main_Interface] ORDER BY Premium_Specialty, ETG_Description;";


////var results = await db_sql.LoadData<ETGFactSymmetry_ReadDto>(connectionString: connectionString, strSQL);








////var closed_xml = new ClosedXMLFunctions();

////string[] cols = typeof(ETGFactSymmetry_ReadDto).GetProperties().Select(p => p.Name).ToArray();
////List<string[]> columns = new List<string[]> { cols };




////var g = await closed_xml.ExportToExcelAsync<ETGFactSymmetry_ReadDto>(results.ToList(),"Test", columns);
////var file = "C:\\Users\\cgiorda\\Desktop\\Projects\\ETGFactSymmetry\\text.xlsx";

////if (System.IO.File.Exists(file))
////    System.IO.File.Delete(file);

////System.IO.File.WriteAllBytes(file, g);


////return;




////AdHoc ah = new AdHoc();
////await ah.runSLAAutomation();
////return;


////string file_path = @"C:\Users\cgiorda\Desktop\Projects\SiteOfCare\";
////string file_name = @"Site of Care Report_2023_02.xlsx";
////string sheet_name = "Case Detail";



//////var soc = closed_xml.ImportExcel<SiteOfCareModel>(file_path + file_name, sheet_name, "A1:AK1", 2, nullCheck: "subcarrier");






////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////             new KeyValuePair<string, string>("Call Center Statistics","Call_Center_Statistics")
////            };

////file_path = @"C:\Users\cgiorda\Desktop\Projects\CiscoYTDMetrics\";
//// file_name = @"YTD - Cisco - UHC Metrics 2023_02.xlsx";

////var month_name = new DateTime(2023, 2, 1).ToString("MMMM");

////var sheet_names = OpenXMLFunctions.GetSheetNames(file_path + file_name);

//////string connectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
//////IRelationalDataAccess db_sql = new SqlDataAccess();


////var sheet_ref = await db_sql.LoadData<string>(connectionString: connectionString, "SELECT distinct sheet_name from stg.EviCore_YTDMetrics");




////var data_models = new List<EviCoreYTDMetricsModel>();

////var mappings = new List<KeyValuePair<string, string>>
////            {
////                new KeyValuePair<string, string>("Total Calls","Total_Calls"),
////                new KeyValuePair<string, string>("ACD Calls","Total_Calls"),
////                new KeyValuePair<string, string>("Average Answer Speed","Avg_Speed_Answer"),
////                new KeyValuePair<string, string>("Total Calls Abandoned","Abandoned_Calls"),
////                new KeyValuePair<string, string>("Aban Calls","Abandoned_Calls"),
////                new KeyValuePair<string, string>("% Of Calls Abandoned","Abandoned_Percent"),
////                 new KeyValuePair<string, string>("% Abn Calls","Abandoned_Percent"),
////                new KeyValuePair<string, string>("Average Answer Speed","Avg_Speed_Answer"),
////                new KeyValuePair<string, string>("Avg ACD Time","Avg_Speed_Answer"),
////                new KeyValuePair<string, string>("Average Talk Time","Average_Talk_Time"),
////                new KeyValuePair<string, string>("% In Service Level","ASA_in_SL_Perent"),
////                new KeyValuePair<string, string>("% Ans Calls","ASA_in_SL_Perent")
////            };

////EviCoreYTDMetricsModel data_model = null;





////foreach (string sheetName in sheet_names)
////{

////    //if (!@sheet_ref.Contains(sheetName.Trim()))
////    //    continue;

////    var chk = sheet_ref.Where(fi => fi.ToLower().Trim().Contains(sheetName.Trim().ToLower()));
////    if (!chk.Any())
////    {
////        continue;
////    }




////    var lob = closed_xml.GetValueFromExcel(file_path + file_name, sheetName, "A2");
////    var export = closed_xml.ImportExcel<YTDCiscoExportModel>(file_path + file_name, sheetName, "A4:O4", 5, nullCheck: "Call Center Statistics");

////    foreach(var e in export)
////    {

////        if(e.Call_Center_Statistics.ToLower().Trim().StartsWith("intake"))
////        {
////            if(data_model != null)
////            {
////                data_model.Summary_of_Lob = lob.ToString();
////                data_model.file_month = 2;
////                data_model.file_year = 2023;
////                data_model.file_date = new DateTime(2023, 2, 01);
////                data_model.sheet_name = sheetName;//strType
////                data_model.file_name = file_name;
////                data_model.file_path = file_path;
////                data_model.report_type = "Cisco UHC Metrics";
////                data_models.Add(data_model);
////            }

////            data_model = new EviCoreYTDMetricsModel();
////            data_model.Call_Taker = "Intake";
////        }
////        else if (e.Call_Center_Statistics.ToLower().Trim().StartsWith("medical"))
////        {
////            if (data_model != null)
////            {
////                data_model.Summary_of_Lob = lob.ToString();
////                data_model.file_month = 2;
////                data_model.file_year = 2023;
////                data_model.file_date = new DateTime(2023, 2, 01);
////                data_model.sheet_name = sheetName;//strType
////                data_model.file_name = file_name;
////                data_model.file_path = file_path;
////                data_model.report_type = "Cisco UHC Metrics";
////                data_models.Add(data_model);
////            }

////            data_model = new EviCoreYTDMetricsModel();
////            data_model.Call_Taker = "MD";
////        }
////        else if (e.Call_Center_Statistics.ToLower().Trim().StartsWith("nurse"))
////        {
////            break;
////        }

////        var mapping = mappings.Where(m => m.Key.ToLower().Trim() == e.Call_Center_Statistics.ToLower().Trim());
////        if (mapping.Count() > 0)
////        {

////            var val = e.GetType().GetProperty(month_name).GetValue(e, null);
////            PropertyInfo propertyInfo = data_model.GetType().GetProperty(mapping.FirstOrDefault().Value);
////            object value;
////            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
////            {
////                if (string.IsNullOrEmpty(val + ""))
////                    value = null;
////                else
////                    value = Convert.ChangeType(val, propertyInfo.PropertyType.GetGenericArguments()[0]);
////            }
////            else
////            {
////                value = Convert.ChangeType(val, propertyInfo.PropertyType);
////            }
////            //propertyInfo.SetValue(f, Convert.ChangeType(value, propertyInfo.PropertyType), null);
////            propertyInfo.SetValue(data_model, value, null);
////        }


////    }
////    if (data_model != null)
////    {
////        data_model.Summary_of_Lob = lob.ToString();
////        data_model.file_month = 2;
////        data_model.file_year = 2023;
////        data_model.file_date = new DateTime(2023, 2, 01);
////        data_model.sheet_name = sheetName;//strType
////        data_model.file_name = file_name;
////        data_model.file_path = file_path;
////        data_model.report_type = "Cisco UHC Metrics";

////        data_models.Add(data_model);
////    }
////    data_model = null;



////}


////string[] columns = typeof(EviCoreYTDMetricsModel).GetProperties().Select(p => p.Name).ToArray();
////await db_sql.BulkSave<EviCoreYTDMetricsModel>(connectionString: connectionString, "stg.EviCore_YTDMetrics_2023", data_models, columns);






////const string fileToCreate = "C:\\Users\\cgiorda\\Desktop\\Projects\\Monthly SLA Review Call\\Monthly SLA Review Call_template.docx";
////List<MSWordFormattedText> lst = new List<MSWordFormattedText>();
////lst.Add(new MSWordFormattedText() { Text = "test1", Bold = false, FontSize =12, FontType = "Times New Roman", ForeColor= System.Drawing.Color.Black});
////lst.Add(new MSWordFormattedText() { Text = "test2", Bold = false, FontSize = 12, FontType = "Times New Roman", ForeColor = System.Drawing.Color.Red });
////lst.Add(new MSWordFormattedText() { Text = "test3", Bold = false, FontSize = 12, FontType = "Times New Roman", ForeColor = System.Drawing.Color.Black });
////var writer = new InteropWordFunctions(fileToCreate);



////outer = "M&R";

////inner = "Cardiology";
////bookmark_name = (outer + "_" + inner).Replace("&", "").ToLower();
////writer.addBulletedList(bookmark_name, lst, 2);

////inner = "Radiology";
////bookmark_name = (outer + "_" + inner).Replace("&", "").ToLower();
////writer.addBulletedList(bookmark_name, lst, 2);


////outer = "C&S";

////inner = "Cardiology";
////bookmark_name = (outer + "_" + inner).Replace("&", "").ToLower();
////writer.addBulletedList(bookmark_name, lst, 2);

////inner = "Radiology";
////bookmark_name = (outer + "_" + inner).Replace("&", "").ToLower();
////writer.addBulletedList(bookmark_name, lst, 2);



////outer = "E&I";

////inner = "Cardiology";
////bookmark_name = (outer + "_" + inner).Replace("&", "").ToLower();
////writer.addBulletedList(bookmark_name, lst, 2);

////inner = "Radiology";
////bookmark_name = (outer + "_" + inner).Replace("&", "").ToLower();
////writer.addBulletedList(bookmark_name, lst, 2);

////writer.Save();

////writer.DisposeWordInstance();



////var file = "C:\\Users\\cgiorda\\Documents\\GitHub\\Workspace\\CPPR\\ConsoleLibraryTesting\\OuterSectionTemplate.xml";
////var xml = System.IO.File.ReadAllText(file);
////file = "C:\\Users\\cgiorda\\Documents\\GitHub\\Workspace\\CPPR\\ConsoleLibraryTesting\\BulletTemplate.xml";
////var xmlbullet = System.IO.File.ReadAllText(file);




////xml = xml.Replace("{$section}", section);
////xml = xml.Replace("{$radiology}", xmlbullet + xmlbullet);
////xml = xml.Replace("{$cardiology}", xmlbullet + xmlbullet);
////xml = xml.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\"", "");



////////if (System.IO.File.Exists(fileToCreate))
//////    System.IO.File.Delete(fileToCreate);

////var writer = new OpenXMLWordFunctions(fileToCreate);

//////writer.ReplaceBulletsXML(section,"Radiology", xml);
////writer.ReplaceBullets(section, "Radiology", lst);
////List<string> fruitList = new List<string>() { "Apple", "Banana", "Carrot" };
////writer.AddBulletList(fruitList);
////writer.AddParagraph("This is a spacing paragraph 1.");

////List<string> animalList = new List<string>() { "Dog", "Cat", "Bear" };
////writer.AddBulletList(animalList);
////writer.AddParagraph("This is a spacing paragraph 2.");

////List<string> stuffList = new List<string>() { "Ball", "Wallet", "Phone" };
////writer.AddBulletList(stuffList);
////writer.AddParagraph("Done.");








////var adHoc = new AdHoc();

////adHoc.ConnectionStringMSSQL = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
////adHoc.TableMHP = "stg.MHP_Yearly_Universes";
////adHoc.ConnectionStringTD = "Data Source=UDWPROD;User ID=cgiorda;Password=BooWooDooFoo2023!!;Authentication Mechanism=LDAP;Session Mode=TERADATA;Session Character Set=ASCII;Persist Security Info=true;Connection Timeout=99999;";
////adHoc.TableUGAP = "stg.MHP_Yearly_Universes_UGAP";
////adHoc.Limit = 3000;

////List<string> files_loaded = new List<string>();
////files_loaded.Add("United PCP- Rad & Card_February_2023.xlsx");
////files_loaded.Add("United PCP- Rad & Card_January_2023.xlsx");
////files_loaded.Add("Oxford February-Radiology Cardiology Universe 2023.xls");
////files_loaded.Add("Oxford January -Radiology Cardiology Universe 2023.xlsx");
////files_loaded.Add("Americhoice February- Radiology Cardiology Universe 2023.xlsx");
////files_loaded.Add("Americhoice January- Radiology Cardiology Universe 2023.xlsx");
////await adHoc.cleanupMemberDataAsync(files_loaded);

//return;

////var closed_xml = new ClosedXMLFunctions();


////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////             new KeyValuePair<string, string>("State of Issue","State_of_Issue"),
////            new KeyValuePair<string, string>("State of Residence","State_of_Residence"),
////            new KeyValuePair<string, string>("Enrollee First Name","Enrollee_First_Name"),
////            new KeyValuePair<string, string>("Enrollee Last Name","Enrollee_Last_Name"),
////            new KeyValuePair<string, string>("Cardholder ID","Cardholder_ID"),
////            new KeyValuePair<string, string>("Funding Arrangement","Funding_Arrangement"),
////            new KeyValuePair<string, string>("Authorization","Authorization"),
////            new KeyValuePair<string, string>("Authorization Type","Authorization_Type"),
////            new KeyValuePair<string, string>("Date the request was received","Request_Date"),
////            new KeyValuePair<string, string>("Time the request was received","Request_Time"),
////            new KeyValuePair<string, string>("Request Decision","Request_Decision"),
////            new KeyValuePair<string, string>("Date of Decision","Decision_Date"),
////            new KeyValuePair<string, string>("Time of Decision","Decision_Time"),
////            new KeyValuePair<string, string>("Decision Reason","Decision_Reason"),
////            new KeyValuePair<string, string>("Was Extension Taken","Extension_Taken"),
////            new KeyValuePair<string, string>("Was Extension Taken?","Extension_Taken"),
////            new KeyValuePair<string, string>("Date of member notification of extension","Member_Notif_Extension_Date"),
////            new KeyValuePair<string, string>("Date additional information received","Additional_Info_Date"),
////            new KeyValuePair<string, string>("Date oral notification provided to enrollee","Oral_Notification_Enrollee_Date"),
////            new KeyValuePair<string, string>("Time oral notification provided to enrollee","Oral_Notification_Enrollee_Time"),
////            new KeyValuePair<string, string>("Date oral notification provided to provider","Oral_Notification_Provider_Date"),
////            new KeyValuePair<string, string>("Time oral notification provided to provider","Oral_Notification_Provider_Time"),
////            new KeyValuePair<string, string>("Date written notification sent to enrollee","Written_Notification_Enrollee_Date"),
////            new KeyValuePair<string, string>("Time written notification sent to enrollee","Written_Notification_Enrollee_Time"),
////            new KeyValuePair<string, string>("Date written notification sent to provider","Written_Notification_Provider_Date"),
////            new KeyValuePair<string, string>("Time written notification sent to provider","Written_Notification_Provider_Time"),
////            new KeyValuePair<string, string>("Primary Procedure Code(s) Requested","Primary_Procedure_Code_Req"),
////            new KeyValuePair<string, string>("Primary Procedure Code Requested","Primary_Procedure_Code_Req"),
////            new KeyValuePair<string, string>("Procedure Code Description","Procedure_Code_Description"),
////            new KeyValuePair<string, string>("Primary Diagnosis Code","Primary_Diagnosis_Code"),
////            new KeyValuePair<string, string>("Diagnosis Description","Diagnosis_Code_Description"),
////            new KeyValuePair<string, string>("Diagnosis Code Description","Diagnosis_Code_Description"),
////            new KeyValuePair<string, string>("Place of Service","Place_of_Service"),
////            new KeyValuePair<string, string>("Member Date of Birth","Member_Date_of_Birth"),
////            new KeyValuePair<string, string>("Was an urgent request made but processed as standard?","Urgent_Processed_Standard"),
////            new KeyValuePair<string, string>("Date of request for additional information","Request_Additional_Info_Date"),
////            new KeyValuePair<string, string>("Date additional information requested","Request_Additional_Info_Date"),
////            new KeyValuePair<string, string>("First Tier, Downstream, and Related Entity","FirstTier_Downstream_RelatedEntity"),
////            new KeyValuePair<string, string>("Par/Non-Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("PAR/NON PAR","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par Non Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par Non/ Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par-Non-Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par/Non-Par","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Inpatient/Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Inpatient Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Inpatient /Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Delegate Number","Delegate_Number"),
////            new KeyValuePair<string, string>("ProgramType","ProgramType"),
////            new KeyValuePair<string, string>("Program Type","ProgramType"),
////            new KeyValuePair<string, string>("Insurance Carrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("InsCarrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("Insurance_Carrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("Group Number","Group_Number"),
////            new KeyValuePair<string, string>("Intake Method","Intake_Method"),
////            new KeyValuePair<string, string>("MethodofContactDesc","Intake_Method")
////            };

////string file_path = @"C:\Users\cgiorda\Desktop\Projects\MHPUniverse\";
////string file_name = @"United PCP- Rad & Card_January_2023.xlsx";
////string sheet_name = "Rad & Card IFP - U12";
////var mhp = closed_xml.ImportExcel<MHPUniverseModel>(file_path + file_name, sheet_name, "A1:AN1", 2, nullCheck: "State of Issue");
////foreach (var m in mhp)
////{
////    //NOT IN SHEET
////    m.file_month = 1;
////    m.file_year = 2023;
////    m.file_date = new DateTime(2023, 1, 01);
////    m.sheet_name = sheet_name;//strType
////    m.file_name = file_name;
////    m.file_path = "\\\\NASGWFTP03\\Care_Core_FTP_Files\\Radiology";
////    m.classification = "IFP";

////}

////string connectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
////IRelationalDataAccess db_sql = new SqlDataAccess();
////string[] columns = typeof(MHPUniverseModel).GetProperties().Select(p => p.Name).ToArray();

////await db_sql.BulkSave<MHPUniverseModel>(connectionString: connectionString, "stg.MHP_Yearly_Universes", mhp, columns);



////IRelationalDataAccess db_td = new TeraDataAccess();
////IRelationalDataAccess db_sql = new SqlDataAccess();


////List<MHPParameterModel> param = MHPCustomSQL.MHPParameters();

////string connectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
////string tdConnectionString = "Data Source=UDWPROD;User ID=cgiorda;Password=BooWooDooFoo2023!!;Authentication Mechanism=LDAP;Session Mode=TERADATA;Session Character Set=ASCII;Persist Security Info=true;Connection Timeout=99999;";
////string tableMHP = "MHP_Yearly_Universes_2023";
////string tableUGAP = "MHP_Yearly_Universes_UGAP_Empty";
////string sql ;
////StringBuilder sbSQL = new StringBuilder();
////int limit = 10;
////int total;
////int total_counter;
////int limit_counter;
////var columns = typeof(MHPMemberDetailsModel).GetProperties().Select(p => p.Name).ToArray();
////foreach (var p in param)
////{
////    sql =  MHPCustomSQL.MSSQLMHPMember(tableMHP, tableUGAP, p.MHPSQL);
////    var mhp_search = (await db_sql.LoadData<MHPMemberSearchModel>(connectionString: connectionString, sql));
////    total = mhp_search.Count();
////    total_counter = 0;
////    limit_counter = 0;

////    foreach (var m in mhp_search)
////    {
////        sbSQL.Append(MHPCustomSQL.UGAPVolatileInsert(m, p));
////        limit_counter++;
////        total_counter++;
////        if (limit_counter == limit)
////        {

////            if (p.LOS == LOS.EI || p.LOS == LOS.EI_OX)
////                sql = MHPCustomSQL.UGAPSQLLMemberDataEI(p.UGAPSQL, p.LOS == LOS.EI_OX).Replace("{$Inserts}", sbSQL.ToString());
////            else
////                sql = MHPCustomSQL.UGAPSQLMemberDataCS(p.UGAPSQL, p.LOS == LOS.CS).Replace("{$Inserts}", sbSQL.ToString());

////            var ugap = await db_td.LoadData<MHPMemberDetailsModel>(connectionString: tdConnectionString, sql);
////            foreach(var u in ugap)
////            {
////                u.SearchMethod = p.SearchMethod;
////            }


////            await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: connectionString, "stg." + tableUGAP, ugap, columns);



////            sbSQL.Remove(0, sbSQL.Length);
////            limit_counter = 0;
////        }
////    }
////    //FINISHED BEFORE LIMIT SO PROCESS
////    if(sbSQL.Length > 0)
////    {
////        if (p.LOS == LOS.EI || p.LOS == LOS.EI_OX)
////            sql = MHPCustomSQL.UGAPSQLLMemberDataEI(p.UGAPSQL, p.LOS == LOS.EI_OX).Replace("{$Inserts}", sbSQL.ToString());
////        else
////            sql = MHPCustomSQL.UGAPSQLMemberDataCS(p.UGAPSQL, p.LOS == LOS.CS).Replace("{$Inserts}", sbSQL.ToString());

////        var ugap = await db_td.LoadData<MHPMemberDetailsModel>(connectionString: tdConnectionString, sql);
////        foreach (var u in ugap)
////        {
////            u.SearchMethod = p.SearchMethod;
////        }


////        await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: connectionString, "stg." + tableUGAP, ugap, columns);

////        sbSQL.Remove(0, sbSQL.Length);

////    }

////}



////sql = "CREATE MULTISET VOLATILE TABLE MissingMembersTmp( mhp_uni_id BIGINT, Cardholder_ID_CLN  VARCHAR(11), State_Of_Issue VARCHAR(5),BTH_DT DATE, REQ_DT DATE, MBR_FST_NM VARCHAR(25), MBR_LST_NM VARCHAR(25) ) PRIMARY INDEX(mhp_uni_id, Cardholder_ID_CLN, State_Of_Issue, BTH_DT, REQ_DT, MBR_FST_NM, MBR_LST_NM ) ON COMMIT PRESERVE ROWS; {$vti}INSERT INTO MissingMembersTmp (mhp_uni_id, Cardholder_ID_CLN, State_Of_Issue, BTH_DT, REQ_DT, MBR_FST_NM, MBR_LST_NM ) VALUES(7027627,'3432183739','WI', '1981-02-15', '2022-12-29', 'KAREN', 'SARMIENTO'); {$vtc}COLLECT STATS COLUMN(mhp_uni_id, Cardholder_ID_CLN, State_Of_Issue, BTH_DT, REQ_DT, MBR_FST_NM, MBR_LST_NM ) ON MissingMembersTmp; {$vts}SELECT mm.mhp_uni_id,  b.BEN_STRCT_1_CD as PLN_VAR_SUBDIV_CD,  c.eff_dt as mnth_eff_dt,  NULL as LEG_ENTY_NBR,  NULL as LEG_ENTY_FULL_NM,  NULL as HCE_LEG_ENTY_ROLLUP_DESC, NULL as MKT_TYP_DESC,  NULL as CUST_SEG_NBR,  NULL as CUST_SEG_NM,  i.PRDCT_CD,  i.PRDCT_CD_DESC,  NULL as MKT_SEG_DESC,  NULL as MKT_SEG_RLLP_DESC,  NULL as MKT_SEG_CD,  NULL as FINC_ARNG_CD,  NULL as FINC_ARNG_DESC,  a.MBR_FST_NM, a.MBR_LST_NM, a.BTH_DT, a.MBR_ALT_ID, a.MBR_ID, b.PRDCT_SYS_ID, b.CS_PRDCT_CD_SYS_ID, k.CS_CO_CD, k.CS_CO_CD_ST, a.SBSCR_MEDCD_RCIP_NBR FROM uhcdm001.hp_member a  join uhcdm001.cs_demographics b on a.MBR_SYS_ID = b.MBR_SYS_ID  join uhcdm001.date_eff c on b.DT_SYS_ID = c.EFF_DT_SYS_ID  join uhcdm001.SOURCE_SYSTEM_COMBO d on b.SRC_SYS_COMBO_SYS_ID = d.SRC_SYS_COMBO_SYS_ID  join uhcdm001.SUBSCRIBER_COUNTY h on b.SBSCR_CNTY_SYS_ID = h.SBSCR_CNTY_SYS_ID  join uhcdm001.PRODUCT i on b.PRDCT_SYS_ID = i.PRDCT_SYS_ID  join uhcdm001.cs_company_code k on b.CS_CO_CD_SYS_ID = k.CS_CO_CD_SYS_ID inner join MissingMembersTmp as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) AND k.CS_CO_CD_ST = mm.State_Of_Issue WHERE k.CS_CO_CD <> 'UHGEX'; {$dvt}drop table MissingMembersTmp; ";


////var  dataTypes = await db_td.LoadData<MHPMemberDetailsModel>(connectionString: "Data Source=UDWPROD;User ID=cgiorda;Password=BooWooDooFoo2023!!;Authentication Mechanism=LDAP;Session Mode=TERADATA;Session Character Set=ASCII;Persist Security Info=true;Connection Timeout=99999;", sql);





////    var closed_xml = new ClosedXMLFunctions();


////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////            new KeyValuePair<string, string>("State of Issue","State_of_Issue"),
////            new KeyValuePair<string, string>("State of Residence","State_of_Residence"),
////            new KeyValuePair<string, string>("Enrollee First Name","Enrollee_First_Name"),
////            new KeyValuePair<string, string>("Enrollee Last Name","Enrollee_Last_Name"),
////            new KeyValuePair<string, string>("Cardholder ID","Cardholder_ID"),
////            new KeyValuePair<string, string>("Funding Arrangement","Funding_Arrangement"),
////            new KeyValuePair<string, string>("Authorization","Authorization"),
////            new KeyValuePair<string, string>("Authorization Type","Authorization_Type"),
////            new KeyValuePair<string, string>("Date the request was received","Request_Date"),
////            new KeyValuePair<string, string>("Time the request was received","Request_Time"),
////            new KeyValuePair<string, string>("Request Decision","Request_Decision"),
////            new KeyValuePair<string, string>("Date of Decision","Decision_Date"),
////            new KeyValuePair<string, string>("Time of Decision","Decision_Time"),
////            new KeyValuePair<string, string>("Decision Reason","Decision_Reason"),
////            new KeyValuePair<string, string>("Was Extension Taken","Extension_Taken"),
////            new KeyValuePair<string, string>("Was Extension Taken?","Extension_Taken"),
////            new KeyValuePair<string, string>("Date of member notification of extension","Member_Notif_Extension_Date"),
////            new KeyValuePair<string, string>("Date additional information received","Additional_Info_Date"),
////            new KeyValuePair<string, string>("Date oral notification provided to enrollee","Oral_Notification_Enrollee_Date"),
////            new KeyValuePair<string, string>("Time oral notification provided to enrollee","Oral_Notification_Enrollee_Time"),
////            new KeyValuePair<string, string>("Date oral notification provided to provider","Oral_Notification_Provider_Date"),
////            new KeyValuePair<string, string>("Time oral notification provided to provider","Oral_Notification_Provider_Time"),
////            new KeyValuePair<string, string>("Date written notification sent to enrollee","Written_Notification_Enrollee_Date"),
////            new KeyValuePair<string, string>("Time written notification sent to enrollee","Written_Notification_Enrollee_Time"),
////            new KeyValuePair<string, string>("Date written notification sent to provider","Written_Notification_Provider_Date"),
////            new KeyValuePair<string, string>("Time written notification sent to provider","Written_Notification_Provider_Time"),
////            new KeyValuePair<string, string>("Primary Procedure Code(s) Requested","Primary_Procedure_Code_Req"),
////            new KeyValuePair<string, string>("Primary Procedure Code Requested","Primary_Procedure_Code_Req"),
////            new KeyValuePair<string, string>("Procedure Code Description","Procedure_Code_Description"),
////            new KeyValuePair<string, string>("Primary Diagnosis Code","Primary_Diagnosis_Code"),
////            new KeyValuePair<string, string>("Diagnosis Description","Diagnosis_Code_Description"),
////            new KeyValuePair<string, string>("Diagnosis Code Description","Diagnosis_Code_Description"),
////            new KeyValuePair<string, string>("Place of Service","Place_of_Service"),
////            new KeyValuePair<string, string>("Member Date of Birth","Member_Date_of_Birth"),
////            new KeyValuePair<string, string>("Was an urgent request made but processed as standard?","Urgent_Processed_Standard"),
////            new KeyValuePair<string, string>("Date of request for additional information","Request_Additional_Info_Date"),
////            new KeyValuePair<string, string>("Date additional information requested","Request_Additional_Info_Date"),
////            new KeyValuePair<string, string>("First Tier, Downstream, and Related Entity","FirstTier_Downstream_RelatedEntity"),
////            new KeyValuePair<string, string>("Par/Non-Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("PAR/NON PAR","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par Non Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par Non/ Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par-Non-Par Site","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Par/Non-Par","Par_NonPar_Site"),
////            new KeyValuePair<string, string>("Inpatient/Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Inpatient Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Inpatient /Outpatient","Inpatient_Outpatient"),
////            new KeyValuePair<string, string>("Delegate Number","Delegate_Number"),
////            new KeyValuePair<string, string>("ProgramType","ProgramType"),
////            new KeyValuePair<string, string>("Program Type","ProgramType"),
////            new KeyValuePair<string, string>("Insurance Carrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("InsCarrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("Insurance_Carrier","Insurance_Carrier"),
////            new KeyValuePair<string, string>("Group Number","Group_Number"),
////            new KeyValuePair<string, string>("Intake Method","Intake_Method"),
////            new KeyValuePair<string, string>("MethodofContactDesc","Intake_Method")
////            };

////string file_path = @"C:\Users\cgiorda\Desktop\Projects\MHPUniverse\";
////string file_name = @"Oxford January -Radiology Cardiology Universe 2023.xlsx";
////string sheet_name = "Oxford Monthly Reports";
////var eslos = closed_xml.ImportExcel<MHPUniverseModel>(file_path + file_name, sheet_name, "A1:AP1", 2);

////var d = "";




////IRelationalDataAccess db_src = new ODBCDataAccess();
////IDataReader dr = null;
////try
////{
////    var table = "EXT_CMS_2728_REPORT_TESTING";
////    //CLEAN FILE NAME FOR USE AS TABLE NAME
////    foreach (char c in System.IO.Path.GetInvalidFileNameChars())
////    {
////        table = table.Replace(c, '_');
////    }
////    table = table.Substring(0, Math.Min(32, table.Length));




////    dr = (await db_src.LoadData(connectionString: "DRIVER=SnowflakeDSIIDriver;SERVER=uhgdwaas.east-us-2.azure.snowflakecomputing.com;ROLE=AR_PRD_CHRIS_GIORDANO_UHC_ROLE;AUTHENTICATOR=SNOWFLAKE_JWT;UID=chris_giordano@uhc.com;PRIV_KEY_FILE=C:\\Users\\cgiorda\\Documents\\credentials\\rsa_key.p8;PRIV_KEY_FILE_PWD=Sigmund2010!!; WAREHOUSE=OHBI_PRD_CONSUME_FREQ_WH;", "select * from OHBI_PRD_CONSUME_DB.UHC_CLINHEALTHPRGM.EXT_CMS_2728_REPORT"));


////    ////DYNAMIC TMP TABLE USES [varchar](MAX) FOR CATCH ALL
////    List<string> columns = new List<string>();
////    for (int col = 0; col < dr.FieldCount; col++)
////    {
////        columns.Add(dr.GetName(col).ToString());
////    }

////    StringBuilder sbSQL = new StringBuilder();
////    sbSQL.Append("IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE 'stg' AND name like 'EXT_CMS_2728_REPORT_TESTING_TMP') DROP TABLE stg.EXT_CMS_2728_REPORT_TESTING_TMP;");
////    sbSQL.Append("CREATE TABLE [stg].[EXT_CMS_2728_REPORT_TESTING_TMP](");
////    foreach (string c in columns)
////    {
////        sbSQL.Append(" [" + c + "] [varchar](MAX) NULL,");
////    }
////    //CREATE NEW TMP TABLE
////    IRelationalDataAccess db_dest = new SqlDataAccess();

////    await db_dest.Execute("data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", sbSQL.ToString().TrimEnd(',') + ") ON [PRIMARY];");
////    sbSQL.Remove(0, sbSQL.Length);


////    await db_dest.BulkSave("data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", "stg.EXT_CMS_2728_REPORT_TESTING_TMP", dr);



////    //POST PROCESSING TO DETERMIN PROPER DATA TYPES AND LENGTHS
////    foreach (var col in columns)
////    {
////        sbSQL.Append("SELECT ColumnName, MAX(ColumnType) as ColumnType, MAX(ColumnLength) as ColumnLength FROM (");
////        sbSQL.Append("SELECT DISTINCT '" + col + "' as ColumnName, ");
////        sbSQL.Append("CASE WHEN ISNUMERIC([" + col + "]) = 1 AND LEN([" + col + "]) = 1 AND [" + col + "] NOT LIKE '%[2-9]%' THEN '1-BIT' ELSE ");
////        sbSQL.Append("CASE WHEN ISNUMERIC([" + col + "]) = 1 AND CHARINDEX('.',[" + col + "]) > 0 THEN '3-FLOAT' ELSE ");
////        sbSQL.Append("CASE WHEN ISNUMERIC([" + col + "]) = 1 AND CHARINDEX('.',[" + col + "]) = 0 THEN '2-INT' ELSE ");
////        sbSQL.Append("CASE WHEN ISDATE([" + col + "]) = 1 THEN '4-DATE' ELSE ");
////        sbSQL.Append("CASE WHEN LEN([" + col + "]) = 1 AND [" + col + "] LIKE '%[a-z]%' THEN '5-CHAR' ");
////        sbSQL.Append("ELSE '6-VARCHAR' ");
////        sbSQL.Append("END END END END END AS ColumnType, ");
////        sbSQL.Append("MAX(LEN([" + col + "]))  AS ColumnLength ");
////        sbSQL.Append("From [stg].[EXT_CMS_2728_REPORT_TESTING_TMP] ");
////        sbSQL.Append("WHERE [" + col + "]  IS NOT NULL GROUP BY [" + col + "] ");
////        sbSQL.Append(") tmp GROUP BY ColumnName ");
////        sbSQL.Append("UNION ALL ");

////    }

////    var  dataTypes = (await db_dest.LoadData<DataTypeModel>(connectionString: "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", sbSQL.ToString().TrimEnd('U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' ')));
////    sbSQL.Remove(0, sbSQL.Length);


////    //CREATE FINAL TABLE USING LENGTHS AND TYPES DETERMINED ABOVE
////    sbSQL.Append("CREATE TABLE [stg].[EXT_CMS_2728_REPORT_TESTING](");
////    string colType, newType;
////    int colLength;
////    foreach (var d in dataTypes)
////    {
////        colType = d.ColumnType.Split('-')[1];
////        colLength = d.ColumnLength;

////        if (colType == "CHAR" || colType == "VARCHAR")
////        {
////            newType = colType + "(" + colLength + ")";
////        }
////        else if (colType == "INT")
////        {
////            if (colLength < 5)
////            {
////                newType = "SMALLINT";
////            }
////            else if (colLength < 10)
////            {
////                newType = "INT";
////            }
////            else if (colLength < 16)
////            {
////                newType = "BIGINT";
////            }
////            else
////            {
////                newType = "VARCHAR(" + colLength + ")";
////            }
////        }
////        else
////        {
////            newType = colType;
////        }

////        sbSQL.Append(" [" + d.ColumnName + "] " + newType + " NULL,");

////    }
////    //DROP TABLE IF ALREAY EXISTS 
////    await db_dest.Execute(connectionString: "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", "IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE 'stg' AND name like 'EXT_CMS_2728_REPORT_TESTING') DROP TABLE stg.EXT_CMS_2728_REPORT_TESTING; " + sbSQL.ToString().TrimEnd(',') + ") ON [PRIMARY]; ");
////    sbSQL.Remove(0, sbSQL.Length);




////    foreach (string c in columns)
////    {
////        sbSQL.Append("[" + c + "],");
////    }
////    await db_dest.Execute(connectionString: "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", "INSERT INTO [stg].[EXT_CMS_2728_REPORT_TESTING] (" + sbSQL.ToString().TrimEnd(',') + ") SELECT " + sbSQL.ToString().TrimEnd(',') + " FROM [stg].[EXT_CMS_2728_REPORT_TESTING_TMP]; DROP TABLE  [stg].[EXT_CMS_2728_REPORT_TESTING_TMP];");
////    sbSQL.Remove(0, sbSQL.Length);



////}
////finally
////{

////    if(dr != null)
////    {
////        if(!dr.IsClosed)
////        {
////            dr.Close();
////        }
////    }
////    dr.Dispose();
////    dr = null;

////}


//////await db.BulkSave<dynamic>("data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", "stg.EXT_CMS_2728_REPORT_TESTING", rows.ToList(), columns.ToArray());




////return;







//////ITEMS TO IGNORE
////string[] ignore = { "Expedited Authorizations/Notifications",
////                    "%TAT < State Specific TAT",
////                    "SLA for TAT",
////                    "Standard Authorizations/Notifications",
////                    "% TAT < 48 hours",
////                    };


////var Mappings = new List<KeyValuePair<string, string>>
////            {
////                new KeyValuePair<string, string>("Total Requests","Total_Requests"),
////                new KeyValuePair<string, string>("% Call","Per_Call"),
////                new KeyValuePair<string, string>("% Website","Per_Website"),
////                new KeyValuePair<string, string>("% Fax","Per_Fax"),
////                new KeyValuePair<string, string>("Approved","Approved"),
////                new KeyValuePair<string, string>("Denied","Denied"),
////                new KeyValuePair<string, string>("Withdrawn","Withdrawn"),
////                new KeyValuePair<string, string>("Admin Expired","Admin_Expired"),
////                new KeyValuePair<string, string>("Expired","Expired"),
////                new KeyValuePair<string, string>("Pending","Pending"),
////                new KeyValuePair<string, string>("Non-Cert (D + W + E) exc. Admin ex)","Non_Cert"),
////                new KeyValuePair<string, string>("Requests/1000","Requests_per_thou"),
////                new KeyValuePair<string, string>("Approvals/1000","Approval_per_thou"),
////                new KeyValuePair<string, string>("3DI","MOD_3DI"),
////                new KeyValuePair<string, string>("BONE DENSITY","MOD_BONE_DENSITY"),
////                new KeyValuePair<string, string>("CT SCAN","MOD_CT_SCAN"),
////                new KeyValuePair<string, string>("MRA","MOD_MRA"),
////                new KeyValuePair<string, string>("MRI","MOD_MRI"),
////                new KeyValuePair<string, string>("NOT COVERED PROCEDURE","MOD_NOT_COVERED_PROCEDURE"),
////                new KeyValuePair<string, string>("NUCLEAR CARDIOLOGY","MOD_NUCLEAR_CARDIOLOGY"),
////                new KeyValuePair<string, string>("NUCLEAR MEDICINE","MOD_NUCLEAR_MEDICINE"),
////                new KeyValuePair<string, string>("PET SCAN","MOD_PET_SCAN"),
////                new KeyValuePair<string, string>("ULTRASOUND","MOD_ULTRASOUND"),
////                new KeyValuePair<string, string>("UNLISTED PROCEDURE","MOD_UNLISTED_PROCEDURE"),
////                new KeyValuePair<string, string>("CARDIAC CATHETERIZATION","MOD_CARDIAC_CATHETERIZATION"),
////                new KeyValuePair<string, string>("CARDIAC CT/CCTA","MOD_CARDIAC_CT_CCTA"),
////                new KeyValuePair<string, string>("CARDIAC IMPLANTABLE DEVICES","MOD_CARDIAC_IMPLANTABLE_DEVICES"),
////                new KeyValuePair<string, string>("CARDIAC MRI","MOD_CARDIAC_MRI"),
////                new KeyValuePair<string, string>("CARDIAC PET","MOD_CARDIAC_PET"),
////                new KeyValuePair<string, string>("ECHO STRESS","MOD_ECHO_STRESS"),
////                new KeyValuePair<string, string>("ECHO STRESS-ADDON","MOD_ECHO_STRESS_ADDON"),
////                new KeyValuePair<string, string>("ECHOCARDIOGRAPHY","MOD_ECHOCARDIOGRAPHY"),
////                new KeyValuePair<string, string>("ECHOCARDIOGRAPHY-ADDON","MOD_ECHOCARDIOGRAPHY_ADDON"),
////                new KeyValuePair<string, string>("NUCLEAR STRESS","MOD_NUCLEAR_STRESS"),
////                new KeyValuePair<string, string>("CCCM Misc Cath Codes","MOD_CCCM_Misc_Cath_Codes")

////            };


////List<EvicoreScorecardModel> ecsFinalList = new List<EvicoreScorecardModel>();
////System.Reflection.PropertyInfo propModel;
////System.Reflection.PropertyInfo propSheetModel;

//////var properties = typeof(EvicoreScorecardSheetLOSModel).GetProperties();
////foreach(var m  in closed_xml.Mappings)
////{
////    if(m.Value != "Header")
////    {
////        ecsFinalList.Add(new EvicoreScorecardModel() { Header = m.Key, Summary_of_Lob= lob.ToString(), report_type= "UHC Scorecard", file_month = 1, file_year = 2023, file_date = new DateTime(2023, 1, 1), sheet_name= sheet_name, file_name = file_name, file_path = file_path });
////    }
////}



////foreach (var e in eslos.Where(x => !string.IsNullOrEmpty(x.EINotif)))
////{
////    if (e.Header.Trim().EqualsAnyOf(ignore))
////    {
////        continue;
////    }


////    //ex Map '% Fax' = 'Per_Fax'
////    var mapping = Mappings.SingleOrDefault(m => m.Key.ToLower().Trim() == e.Header.ToLower().Trim());
////    var colMapped = mapping.Value;



////    foreach (var f in ecsFinalList.Where(x => x.sheet_name == sheet_name))
////    {
////        //ex MAP 'E&I - Notif.' to 'EINotif'
////        mapping = closed_xml.Mappings.SingleOrDefault(m => m.Key.ToLower().Trim() == f.Header.ToLower().Trim());
////        var colSheetMapped = mapping.Value;

////        //DYNAMICALLY SET VALUES TO PROPERTIES BASED ON MAPPED STRINGS
////        propModel = typeof(EvicoreScorecardModel).GetProperty(colMapped); //ex colMapped = 'Per_Fax'
////        propSheetModel = typeof(EvicoreScorecardSheetLOSModel).GetProperty(colSheetMapped); //ex f.Header = 'EINotif'
////        var value = SharedFunctions.ConvertToType(propSheetModel.GetValue(e), propModel.PropertyType);
////        propModel.SetValue(f, value);

////    }


////}


////sheet_name = "CARDIOLOGY";
////lob = closed_xml.GetValueFromExcel(file_path + file_name, sheet_name, "C3");
////eslos = closed_xml.ImportExcel<EvicoreScorecardSheetLOSModel>(file_path + file_name, sheet_name, "C10:K10", 16);




//////var properties = typeof(EvicoreScorecardSheetLOSModel).GetProperties();
////foreach (var m in closed_xml.Mappings)
////{
////    if (m.Value != "Header")
////    {
////        ecsFinalList.Add(new EvicoreScorecardModel() { Header = m.Key, Summary_of_Lob = lob.ToString(), report_type = "UHC Scorecard", file_month = 1, file_year = 2023, file_date = new DateTime(2023, 1, 1), sheet_name = sheet_name, file_name = file_name, file_path = file_path });
////    }
////}




////foreach (var e in eslos.Where(x => !string.IsNullOrEmpty(x.EINotif)))
////{
////    if (e.Header.Trim().EqualsAnyOf(ignore))
////    {
////        continue;
////    }


////    var head = e.Header;

////    //ex Map '% Fax' = 'Per_Fax'
////    var mapping = Mappings.SingleOrDefault(m => m.Key.ToLower().Trim() == e.Header.ToLower().Trim());
////    var colMapped = mapping.Value;



////    foreach (var f in ecsFinalList.Where(x => x.sheet_name == sheet_name))
////    {
////        head = f.Header;


////        //ex MAP 'E&I - Notif.' to 'EINotif'
////        mapping = closed_xml.Mappings.SingleOrDefault(m => m.Key.ToLower().Trim() == f.Header.ToLower().Trim());
////        var colSheetMapped = mapping.Value;

////        //DYNAMICALLY SET VALUES TO PROPERTIES BASED ON MAPPED STRINGS
////        propModel = typeof(EvicoreScorecardModel).GetProperty(colMapped); //ex colMapped = 'Per_Fax'
////        propSheetModel = typeof(EvicoreScorecardSheetLOSModel).GetProperty(colSheetMapped); //ex f.Header = 'EINotif'
////        var value = SharedFunctions.ConvertToType(propSheetModel.GetValue(e), propModel.PropertyType);
////        propModel.SetValue(f, value);

////    }


////}





////sheet_name = "C&S RADIOLOGY";
////lob = closed_xml.GetValueFromExcel(file_path + file_name, sheet_name, "D3");
////var esstate = closed_xml.ImportExcel<EvicoreScorecardSheetStateModel>(file_path + file_name, sheet_name, "D7:W7", 11);



//////var properties = typeof(EvicoreScorecardSheetLOSModel).GetProperties();
////foreach (var p in typeof(EvicoreScorecardSheetStateModel).GetProperties())
////{
////    if (p.Name != "Header")
////    {
////        propSheetModel = typeof(EvicoreScorecardSheetStateModel).GetProperty(p.Name); //ex f.Header = 'EINotif'
////        var value = propSheetModel.GetValue(esstate[0]);
////        if(value != null)
////        {
////            ecsFinalList.Add(new EvicoreScorecardModel() { Header = p.Name, Summary_of_Lob = lob.ToString(), report_type = "UHC Scorecard", file_month = 1, file_year = 2023, file_date = new DateTime(2023, 1, 1), sheet_name = sheet_name, file_name = file_name, file_path = file_path });
////        }    


////    }
////}


////foreach (var e in esstate.Where(x => !string.IsNullOrEmpty(x.AZ)))
////{
////    if (e.Header.Trim().EqualsAnyOf(ignore))
////    {
////        continue;
////    }


////    var head = e.Header;

////    //ex Map '% Fax' = 'Per_Fax'
////    var mapping = Mappings.SingleOrDefault(m => m.Key.ToLower().Trim() == e.Header.ToLower().Trim());
////    var colMapped = mapping.Value;



////    foreach (var f in ecsFinalList.Where(x => x.sheet_name == sheet_name))
////    {

////        //DYNAMICALLY SET VALUES TO PROPERTIES BASED ON MAPPED STRINGS
////        propModel = typeof(EvicoreScorecardModel).GetProperty(colMapped); //ex colMapped = 'Per_Fax'
////        propSheetModel = typeof(EvicoreScorecardSheetStateModel).GetProperty(f.Header); //ex f.Header = 'EINotif'
////        var value = propSheetModel.GetValue(e);
////        if (value != null)
////        {
////            propModel.SetValue(f, SharedFunctions.ConvertToType(value, propModel.PropertyType));
////        }


////    }


////}




////sheet_name = "C&S CARDIOLOGY";
////lob = closed_xml.GetValueFromExcel(file_path + file_name, sheet_name, "D3");
////esstate = closed_xml.ImportExcel<EvicoreScorecardSheetStateModel>(file_path + file_name, sheet_name, "D7:S7", 11);



//////var properties = typeof(EvicoreScorecardSheetLOSModel).GetProperties();
////foreach (var p in typeof(EvicoreScorecardSheetStateModel).GetProperties())
////{
////    if (p.Name != "Header")
////    {
////        propSheetModel = typeof(EvicoreScorecardSheetStateModel).GetProperty(p.Name); //ex f.Header = 'EINotif'
////        var value = propSheetModel.GetValue(esstate[0]);
////        if (value != null)
////        {
////            ecsFinalList.Add(new EvicoreScorecardModel() { Header = p.Name, Summary_of_Lob = lob.ToString(), report_type = "UHC Scorecard", file_month = 1, file_year = 2023, file_date = new DateTime(2023, 1, 1), sheet_name = sheet_name, file_name = file_name, file_path = file_path });
////        }


////    }
////}


////foreach (var e in esstate.Where(x => !string.IsNullOrEmpty(x.AZ)))
////{
////    if (e.Header.Trim().EqualsAnyOf(ignore))
////    {
////        continue;
////    }


////    var head = e.Header;

////    //ex Map '% Fax' = 'Per_Fax'
////    var mapping = Mappings.SingleOrDefault(m => m.Key.ToLower().Trim() == e.Header.ToLower().Trim());
////    var colMapped = mapping.Value;



////    foreach (var f in ecsFinalList.Where(x => x.sheet_name == sheet_name))
////    {

////        //DYNAMICALLY SET VALUES TO PROPERTIES BASED ON MAPPED STRINGS
////        propModel = typeof(EvicoreScorecardModel).GetProperty(colMapped); //ex colMapped = 'Per_Fax'
////        propSheetModel = typeof(EvicoreScorecardSheetStateModel).GetProperty(f.Header); //ex f.Header = 'EINotif'
////        var value = propSheetModel.GetValue(e);
////        if (value != null)
////        {
////            propModel.SetValue(f, SharedFunctions.ConvertToType(value, propModel.PropertyType));
////        }


////    }


////}

//////ActiveDirectory ad = new ActiveDirectory("", "ms.ds.uhc.com", @"ms\peisaid", "BooWooDooFoo2023!!");

//////string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower().TrimStart('m', 's', '\\');

//////var l1 = ad.GetUserByUserName("username ");

////return;



////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////                new KeyValuePair<string, string>("State","State"),
////                new KeyValuePair<string, string>("Modality","Modality"),
////                new KeyValuePair<string, string>("Requests   / 1000","RequestsPer1000"),
////                new KeyValuePair<string, string>("Approved       / 1000","ApprovalsPer1000"),
////                new KeyValuePair<string, string>("Approved (A)","Approved"),
////                new KeyValuePair<string, string>("Auto Approved","Auto_Approved"),
////                new KeyValuePair<string, string>("Denied (D)","Denied"),
////                new KeyValuePair<string, string>("Withdrawn (W)","Withdrawn"),
////                new KeyValuePair<string, string>("Expired (Y)","Expired"),
////                new KeyValuePair<string, string>("Pending","Others")
////            };


////var cs = closed_xml.ImportExcel<CSScorecardModel>(@"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard\AMERICHOICE_Allstates_Auths Per 1000 by Modality with Exclusions_CARD_2022_12.xlsx", "ALL LOBs", "B5:AK5", 6, nullCheck: "Modality");





////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////                new KeyValuePair<string, string>("Carrier State","Carrier_State"),
////                new KeyValuePair<string, string>("Line of Business","Line_of_Business"),
////                new KeyValuePair<string, string>("Modality","Modality"),
////                new KeyValuePair<string, string>("Total Authorizations/Notifications","Total_Authorizations_Notifications"),
////                new KeyValuePair<string, string>("<= 2 BUS Days","LessEqual_2_BUS_Days"),
////                new KeyValuePair<string, string>("% <= 2 BUS Days","PerLessEqual_2_BUS_Days"),
////                new KeyValuePair<string, string>("< State TAT Requirements","Less_State_TAT_Requirements"),
////                new KeyValuePair<string, string>("% < State TAT Requirements","PerLess_State_TAT_Requirements"),
////                new KeyValuePair<string, string>("Average Business Days","Average_Business_Days"),
////                new KeyValuePair<string, string>("Average BUS Days Receipt Clinical","Average_BUS_Days_Receipt_Clinical"),
////                new KeyValuePair<string, string>("Avg CAL Days Case Creation","Avg_CAL_Days_Case_Creation"),
////                new KeyValuePair<string, string>("Average BUS Days Case Creation","Average_BUS_Days_Case_Creation"),
////                new KeyValuePair<string, string>("Avg Business Days Denial Letter Sent","Avg_Business_Days_Denial_Letter_Sent")
////            };


////var tat = closed_xml.ImportExcel<PPACATATModel>(@"C:\Users\cgiorda\Desktop\Projects\PPACA_TAT\United_Enterprise_Wide_Routine_TAT_UHC_Enterprise_2022_12.xlsx", "Sheet3", "C3:O3", 4);




////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////                new KeyValuePair<string, string>("Encounter ID","EncounterID"),
////                new KeyValuePair<string, string>("Case Init","CaseInit"),
////                new KeyValuePair<string, string>("Modality","Modality"),
////                new KeyValuePair<string, string>("Site Zip Code","SiteZipCode")
////            };


////var cs2 = closed_xml.ImportExcel<AllStatesCSScorecardModel>(@"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard\AMERICHOICE_Allstates_Auth_Details_CARD_2022_12.xlsx", "Allstates CARD", "B4:AM4", 5, nullCheck: "EncounterID");




////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////                new KeyValuePair<string, string>("CONTRACT NUMBER","Contract_Number"),
////                new KeyValuePair<string, string>("PBP","PBP"),
////                new KeyValuePair<string, string>("COMPANY STATE","Company_State"),
////                new KeyValuePair<string, string>("Total","Member_Count")
////            };


////var nice = closed_xml.ImportExcel<NICEUHCWestEligibilityModel>(@"C:\Users\cgiorda\Desktop\Projects\NICEUHCWestEligibility\NICE_UHCWestEligibility_202212_Medicare_Final_for_membership.xlsx", "by hplan_pbp", "A3:D3", 4);
//////var evi = closed_xml.ImportExcel<EviCoreMRMembershipDetailsModel>(@"C:\Users\cgiorda\Desktop\Projects\NICEUHCWestEligibility\NICE_UHCWestEligibility_202212_Medicare_Final_for_membership.xlsx", "by hplan_pbp", "A3:D3", 3, add: 0);



////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////                new KeyValuePair<string, string>("IncurredDt","IncurredDt"),
////                new KeyValuePair<string, string>("Program","Program"),
////                new KeyValuePair<string, string>("MemberCount","Member_Count"),
////            };


//// var evi = closed_xml.ImportExcel<EviCoreMRMembershipDetailsModel>(@"C:\Users\cgiorda\Desktop\Projects\EviCoreMRMembershipDetails\CRC_Pivot_Rawdata_202212.xlsx", "CRC_Pivot_Rawdata_202212", "A1:P1", 2,nullCheck: "IncurredDt");


////Console.WriteLine("start");








////return;











////try
////{
////    var list = Directory.GetFiles(@"C:\Users\cgiorda\Desktop\FTPMock", "AMERICHOICE_Allstates_Auths Per 1000 by Modality with Exclusions_CARD_*_*.xlsx", SearchOption.TopDirectoryOnly);
////}
////catch(Exception ex)
////{
////    var s = ex.Message;

////}

////Console.WriteLine("end");
////Console.ReadLine();



//////var closed_xml = new ClosedXMLFunctions();
////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////                new KeyValuePair<string, string>("State","State"),
////                new KeyValuePair<string, string>("Modality","Modality"),
////                new KeyValuePair<string, string>("Requests   / 1000","RequestsPer1000"),
////                new KeyValuePair<string, string>("Approved       / 1000","ApprovalsPer1000"),
////                new KeyValuePair<string, string>("Approved (A)","Approved"),
////                new KeyValuePair<string, string>("Auto Approved","Auto_Approved"),
////                new KeyValuePair<string, string>("Denied (D)","Denied"),
////                new KeyValuePair<string, string>("Withdrawn (W)","Withdrawn"),
////                new KeyValuePair<string, string>("Expired (Y)","Expired"),
////                new KeyValuePair<string, string>("Pending","Others")
////            };
////string colrange = "B5:AK5";
////int startingRow = 6;


////var sum_card = closed_xml.ImportExcel<CSScorecardModel>(@"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard\AMERICHOICE_Allstates_Auths Per 1000 by Modality with Exclusions_CARD_2022_12.xlsx", "ALL LOBs", colrange, startingRow, "Modality");

////string strLastState = null;
////foreach (var c in sum_card)
////{

////    //STATES REPEAT ARE BLANK IN SPREADSHEET
////    if (!string.IsNullOrEmpty(c.State))
////    {
////        strLastState = c.State;
////    }
////    else
////    {
////        c.State = strLastState;
////    }

////    c.report_type = "CS Scorecards";
////    c.file_month = 12;
////    c.file_year = 2022;
////    c.file_date = new DateTime(c.file_year, c.file_month, 1);
////    c.sheet_name = "CARD";
////    c.file_name  = "AMERICHOICE_Allstates_Auth_Details_CARD_2022_12.xlsx";
////    c.file_path = @"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard\";
////}

////colrange = "B5:AC5";

////var sum_rad = closed_xml.ImportExcel<CSScorecardModel>(@"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard\AMERICHOICE_Allstates_Auths Per 1000 by Modality with Exclusions_RAD_2022_12.xlsx", "ALL LOBs", colrange, startingRow, "Modality");

////strLastState = null;
////foreach (var c in sum_rad)
////{

////    //STATES REPEAT ARE BLANK IN SPREADSHEET
////    if (!string.IsNullOrEmpty(c.State))
////    {
////        strLastState = c.State;
////    }
////    else
////    {
////        c.State = strLastState;
////    }

////    c.report_type = "CS Scorecards";
////    c.file_month = 12;
////    c.file_year = 2022;
////    c.file_date = new DateTime(c.file_year, c.file_month, 1);
////    c.sheet_name = "RAD";
////    c.file_name = "Americhoice_Allstates_Auth_Details_RAD_2022_12.xlsx";
////    c.file_path = @"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard\";

////}

////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////                new KeyValuePair<string, string>("Encounter ID","EncounterID"),
////                new KeyValuePair<string, string>("Case Init","CaseInit"),
////                new KeyValuePair<string, string>("Modality","Modality"),
////                new KeyValuePair<string, string>("Site Zip Code","SiteZipCode")
////            };

////var summary_all = sum_card.Concat(sum_rad).ToList();


//////XLSToXLSXConverter.Convert(@"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard\AMERICHOICE_Allstates_Auth_Details_CARD_2022_12.xls");
//////XLSToXLSXConverter.Convert(@"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard\Americhoice_Allstates_Auth_Details_RAD_2022_12.xls");


////IRelationalDataAccess db = new SqlDataAccess();
////var zip_state = (await db.LoadData<ZipStateModel>(connectionString: "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", "SELECT [zip],[state] FROM [IL_UCA].[stg].[zip_state]"));


////colrange = "B4:AM4";
////startingRow = 5;
////var details_card = closed_xml.ImportExcel<AllStatesCSScorecardModel>(@"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard\AMERICHOICE_Allstates_Auth_Details_CARD_2022_12.xlsx", "Allstates CARD", colrange, startingRow, "EncounterID");



////for (int i = details_card.Count - 1; i >= 0; i--)
////{
////    var zs = zip_state.Where(x => x.zip == details_card[i].SiteZipCode).FirstOrDefault();
////    if (zs != null)
////    {
////        details_card[i].Site_State = zs.state;   // or set it to some other value
////        details_card[i].file_type = "CARD";
////    }
////    else
////    {
////        details_card.RemoveAt(i);
////    }
////}


////colrange = "B4:AM4";
////startingRow = 5;
////var details_rad = closed_xml.ImportExcel<AllStatesCSScorecardModel>(@"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard\Americhoice_Allstates_Auth_Details_RAD_2022_12.xlsx", "Allstates RAD", colrange, startingRow, "EncounterID");

////for (int i = details_rad.Count - 1; i >= 0; i--)
////{
////    var zs = zip_state.Where(x => x.zip == details_rad[i].SiteZipCode).FirstOrDefault();
////    if (zs != null)
////    {
////        details_rad[i].Site_State = zs.state;   // or set it to some other value
////        details_rad[i].file_type = "RAD";
////    }
////    else
////    {
////        details_rad.RemoveAt(i);
////    }
////}


////var details_all = details_card.Concat(details_rad).ToList();




////var states = summary_all.Select(x => x.State).Distinct().ToList();


////var rad_card = new List<string>();
////rad_card.Add("RAD");
////rad_card.Add("CARD");



////var final = new List<CSScorecardModel>();

////foreach(var rc in rad_card)
////{
////    foreach (var state in states)
////    {
////        var modalities = summary_all.Where(x=> x.sheet_name == rc).Select(x => x.Modality).Distinct().ToList();
////        modalities.Insert(0, "ALL");


////        foreach (var modality in modalities)
////        {
////            var d = new CSScorecardModel();

////            d.State = state;
////            d.Modality = modality;


////            if (modality == "ALL")
////            {

////                d.Phone = details_all.Where(x => x.CaseInit == "Phone" && x.Site_State == state && x.file_type == rc).Count();
////                d.Web = details_all.Where(x => x.CaseInit == "Web" && x.Site_State == state && x.file_type == rc).Count();
////                d.Fax = details_all.Where(x => x.CaseInit == "Fax" && x.Site_State == state && x.file_type == rc).Count();
////                d.RequestsPer1000 = summary_all.Where(x => x.State == state &&  x.sheet_name == rc).Select(x => x.RequestsPer1000).Sum();
////                d.ApprovalsPer1000 = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.ApprovalsPer1000).Sum();

////                d.Approved = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Approved).Sum();
////                d.Auto_Approved = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Auto_Approved).Sum();

////                d.Denied = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Denied).Sum();
////                d.Withdrawn = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Withdrawn).Sum();
////                d.Expired = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Expired).Sum();
////                d.Others = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.Others).Sum();

////                d.report_type = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.report_type).FirstOrDefault();
////                d.file_month = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_month).FirstOrDefault();
////                d.file_year = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_year).FirstOrDefault();
////                d.file_date = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_date).FirstOrDefault();
////                d.sheet_name = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.sheet_name).FirstOrDefault();
////                d.file_name = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_name).FirstOrDefault();
////                d.file_path = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.report_type).FirstOrDefault();

////            }
////            else
////            {
////                var tmp = summary_all.Where(x => x.State == state && x.Modality == modality && x.sheet_name == rc).ToList();
////                if (tmp.Count() > 1)
////                {
////                    foreach (var t in tmp)
////                    {
////                        d.Approved = t.Approved;
////                        d.Auto_Approved = t.Auto_Approved;
////                        d.Denied = t.Denied;
////                        d.Withdrawn = t.Withdrawn;
////                        d.Expired = t.Expired;
////                        d.Others = t.Others;
////                        d.is_ignored = true;
////                        d.ignore_reason = "Duplicate Row";

////                        d.report_type = t.report_type;
////                        d.file_month = t.file_month;
////                        d.file_year = t.file_year;
////                        d.file_date = t.file_date;
////                        d.sheet_name = t.sheet_name;
////                        d.file_name = t.file_name;
////                        d.file_path = t.file_path;

////                    }
////                }
////                else if (tmp.Count == 1)
////                {
////                    d.Approved = tmp[0].Approved;
////                    d.Auto_Approved = tmp[0].Auto_Approved;
////                    d.Denied = tmp[0].Denied;
////                    d.Withdrawn = tmp[0].Withdrawn;
////                    d.Expired = tmp[0].Expired;
////                    d.Others = tmp[0].Others;
////                    d.is_ignored = false;
////                    d.ignore_reason = null;


////                    d.report_type = tmp[0].report_type;
////                    d.file_month = tmp[0].file_month;
////                    d.file_year = tmp[0].file_year;
////                    d.file_date = tmp[0].file_date;
////                    d.sheet_name = tmp[0].sheet_name;
////                    d.file_name = tmp[0].file_name;
////                    d.file_path = tmp[0].file_path;

////                }
////                else
////                {
////                    d.Approved = 0;
////                    d.Auto_Approved = 0;
////                    d.Denied = 0;
////                    d.Withdrawn = 0;
////                    d.Expired = 0;
////                    d.Others = 0;
////                    d.is_ignored = false;
////                    d.ignore_reason = null;

////                    d.report_type = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.report_type).FirstOrDefault();
////                    d.file_month = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_month).FirstOrDefault();
////                    d.file_year = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_year).FirstOrDefault();
////                    d.file_date = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_date).FirstOrDefault();
////                    d.sheet_name = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.sheet_name).FirstOrDefault();
////                    d.file_name = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_name).FirstOrDefault();
////                    d.file_path = summary_all.Where(x => x.State == state && x.sheet_name == rc).Select(x => x.file_path).FirstOrDefault();

////                }
////            }




////            final.Add(d);
////        }

////    }
////}


////string[] columns = typeof(CSScorecardModel).GetProperties().Select(p => p.Name).ToArray();

////await db.BulkSave<CSScorecardModel>("data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", "stg.EviCore_CS_Scorecard_2023", final, columns);






////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////            {
////                new KeyValuePair<string, string>("Carrier State","Carrier_State"),
////                new KeyValuePair<string, string>("Line of Business","Line_of_Business"),
////                new KeyValuePair<string, string>("Modality","Modality"),
////                new KeyValuePair<string, string>("Total Authorizations/Notifications","Total_Authorizations_Notifications"),
////                new KeyValuePair<string, string>("<= 2 BUS Days","LessEqual_2_BUS_Days"),
////                new KeyValuePair<string, string>("% <= 2 BUS Days","PerLessEqual_2_BUS_Days"),
////                new KeyValuePair<string, string>("< State TAT Requirements","Less_State_TAT_Requirements"),
////                new KeyValuePair<string, string>("% < State TAT Requirements","PerLess_State_TAT_Requirements"),
////                new KeyValuePair<string, string>("Average Business Days","Average_Business_Days"),
////                new KeyValuePair<string, string>("Average BUS Days Receipt Clinical","Average_BUS_Days_Receipt_Clinical"),
////                new KeyValuePair<string, string>("Avg CAL Days Case Creation","Avg_CAL_Days_Case_Creation"),
////                new KeyValuePair<string, string>("Average BUS Days Case Creation","Average_BUS_Days_Case_Creation"),
////                new KeyValuePair<string, string>("Avg Business Days Denial Letter Sent","Avg_Business_Days_Denial_Letter_Sent")
////            };



////var t = closed_xml.ImportExcel<PPACATATModel>(@"C:\Users\cgiorda\Desktop\Projects\PPACA_TAT\United_Enterprise_Wide_Routine_TAT_UHC_Enterprise_2022_12.xlsx", "Sheet3", "C3:O3", 4);


////var s = "";


////IRelationalDataAccess db = new SqlDataAccess();

////var projectName = "PPACA_TAT";

////var fd = (await db.LoadData<FileDateModel>(connectionString: "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", "select top 1 file_month, [file_year],file_date FROM [IL_UCA].[stg].[EviCore_TAT_2023] where file_date = (select max(file_date) from[IL_UCA].[stg].[EviCore_TAT_2023])")).FirstOrDefault();


////List <FileConfig> fileList = new List<FileConfig> ();
////fileList.Add(new FileConfig { ZippedFile = "Over_All_*_*.zip", FileName = "United_Enterprise_Wide_Routine_TAT_UHC_Enterprise_*_*.xlsx", FilePath = "\\\\NASGWFTP03\\Care_Core_FTP_Files\\Radiology", FileFormat = FileFormat.Excel, Destination = "stg.EviCore_TAT_2023", ZippedMatch = "United_Enterprise_Wide_Routine_" });

////fileList.Add(new FileConfig { FileName = "United_Enterprise_Wide_Urgent_TAT_UHC_Enterprise_*_*.xlsx", FilePath = "\\\\NASGWFTP03\\Care_Core_FTP_Files\\Radiology", FileFormat = FileFormat.Excel, Destination = "stg.EviCore_TAT_2023", ZippedMatch = "United_Enterprise_Wide_Urgent_" });

////List<string> filesFound = new List<string> ();
////int month,year;
////foreach (var file in fileList)
////{
////    var list = Directory.GetFiles(file.FilePath, file.FileName,SearchOption.TopDirectoryOnly);
////    foreach(var f in list)
////    {
////        //IF DATE THEN ADD
////        var fileName = Path.GetFileName(f).Replace(".xlsx", "");
////        var fileParsed = fileName.Split('_');
////        month = int.TryParse(fileParsed[8], out month) ? month : 0;
////        year = int.TryParse(fileParsed[7], out year) ? year : 0;

////        if((fd.file_month < month && fd.file_year == year) || fd.file_year < year)
////        {
////            filesFound.Add(f);
////        }

////    }

////    if (file.ZippedFile!= null)
////    {
////        list = Directory.GetFiles(file.FilePath, file.ZippedFile, SearchOption.TopDirectoryOnly);
////        foreach (var f in list)
////        {
////            var fileName = Path.GetFileName(f).Replace(".zip", "");
////            var fileParsed = fileName.Split('_');

////            var format = (fileParsed[2].Length == 3 ? "MMM" : "MMMM"); //Jan vs January
////            month = DateTime.ParseExact(fileParsed[2].Trim(), format, CultureInfo.CurrentCulture).Month;
////            year = int.TryParse(fileParsed[3], out year) ? year : 0;

////            if ((fd.file_month < month && fd.file_year == year) || fd.file_year < year)
////            {
////                filesFound.Add(f);
////            }
////        }
////    }

////}



////foreach (var file in filesFound)
////{
////    var fileName = Path.GetFileName(file);
////    var current = fileStaging + "\\" + projectName + "\\" + fileName;
////    File.Copy(file, current);

////    if (current.ToLower().EndsWith(".zip"))
////    {
////        using (ZipArchive archive = ZipFile.OpenRead(current))
////        {
////            foreach (ZipArchiveEntry entry in archive.Entries)
////            {
////                foreach(var f in fileList)
////                {

////                    if(entry.FullName.ToLower().StartsWith(f.ZippedMatch.ToLower()))
////                    {
////                        var ff = Path.Combine(fileStaging + "\\" + projectName + "\\", entry.FullName);

////                        entry.ExtractToFile(ff);
////                    }

////                }
////            }
////        }

////        File.Delete(current);
////    }

////}


////var closed_xml = new ClosedXMLFunctions();
////closed_xml.Mappings = new List<KeyValuePair<string, string>>
////{
////    new KeyValuePair<string, string>("Carrier State","Carrier_State"),
////    new KeyValuePair<string, string>("Line of Business","Line_of_Business"),
////    new KeyValuePair<string, string>("Modality","Modality"),
////    new KeyValuePair<string, string>("Total Authorizations/Notifications","Total_Authorizations_Notifications"),
////    new KeyValuePair<string, string>("<= 2 BUS Days","LessEqual_2_BUS_Days"),
////    new KeyValuePair<string, string>("% <= 2 BUS Days","PerLessEqual_2_BUS_Days"),
////    new KeyValuePair<string, string>("< State TAT Requirements","Less_State_TAT_Requirements"),
////    new KeyValuePair<string, string>("% < State TAT Requirements","PerLess_State_TAT_Requirements"),
////    new KeyValuePair<string, string>("Average Business Days","Average_Business_Days"),
////    new KeyValuePair<string, string>("Average BUS Days Receipt Clinical","Average_BUS_Days_Receipt_Clinical"),
////    new KeyValuePair<string, string>("Avg CAL Days Case Creation","Avg_CAL_Days_Case_Creation"),
////    new KeyValuePair<string, string>("Average BUS Days Case Creation","Average_BUS_Days_Case_Creation"),
////    new KeyValuePair<string, string>("Avg Business Days Denial Letter Sent","Avg_Business_Days_Denial_Letter_Sent")
////};



////var workingFiles = Directory.GetFiles(fileStaging + "\\" + projectName + "\\", "*.xlsx", SearchOption.TopDirectoryOnly);
////foreach (var file in workingFiles)
////{
////    var sheet_names = OpenXMLFunctions.GetSheetNames(file);


////    foreach (var sheet in sheet_names)
////    {
////        if (sheet.ToLower().Equals("document map") || sheet.ToLower().Equals("sheet2"))
////        {
////            continue;
////        }
////        var lob = closed_xml.GetValueFromExcel(file, sheet, "F1");
////        var ppaca = closed_xml.ImportExcel<PPACATATModel>(file, sheet, "C3:O3", 4);
////        string strLastState = null;
////        foreach (var p in ppaca)
////        {


////            p.Summary_of_Lob = lob.ToString();

////            //STATES REPEAT ARE BLANK IN SPREADSHEET
////            if (!string.IsNullOrEmpty(p.Carrier_State))
////            {
////                strLastState = p.Carrier_State;
////            }
////            else
////            {
////                p.Carrier_State = strLastState;
////            }


////            p.report_type = "Routine TAT";
////            p.file_month = "12";
////            p.file_year = "2022";
////            p.file_date = new DateTime(2022, 12, 01);
////            p.sheet_name = sheet;
////            p.file_name = "United_Enterprise_Wide_Routine_TAT_UHC_Enterprise_2022_12.xlsx";
////            p.file_path = @"\\nasv1005\fin360\phi2\acad\Program\Radiology\eviCore Monthly Reporting Package\2022\202212\";

////        }

////        ppaca.RemoveAll(o => string.IsNullOrEmpty(o.Modality));

////        string[] columns = typeof(PPACATATModel).GetProperties().Select(p => p.Name).ToArray();
////        await db.BulkSave<PPACATATModel>("data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", "stg.EviCore_TAT_2023", ppaca, columns);



////    }
////    var fileName = Path.GetFileName(file);
////    File.Move(file, fileStaging + "\\" + projectName + "\\Archive\\" + fileName);

////}   


////var csv_importer = new DelimitedFunctions();
////csv_importer.Mappings = new List<KeyValuePair<string, string>>
////{
////    new KeyValuePair<string, string>("",""),
////    new KeyValuePair<string, string>("",""),
////    new KeyValuePair<string, string>("",""),
////    new KeyValuePair<string, string>("",""),
////    new KeyValuePair<string, string>("",""),
////};






////ActiveDirectory ad = new ActiveDirectory("", "ms.ds.uhc.com", @"ms\peisaid", "BooWooDooFoo2023!!");
////PBIMembership pm = new PBIMembership();



////var l1 = pm.getAllADMembers("", "ms.ds.uhc.com", @"ms\peisaid", "BooWooDooFoo2023!!", "AZU_ORBIT_POWERBI_UHC_VC_CLIN_PROG_PERF_*");

////IConfigurationRoot configuration = new ConfigurationBuilder()
////    .SetBasePath(Directory.GetCurrentDirectory())
////            .AddJsonFile("appsettings.json")
////            .Build();

////IRelationalDataAccess d = new SqlDataAccess(configuration);

////var l2 = await d.LoadData<PBIMembershipModel>("SELECT [userid],[email],[global_group],[department] FROM [IL_UCA].[stg].[pbi_membership_test] ");

////StringBuilder sb = new StringBuilder();
////sb.AppendLine("New Members Added:" + Environment.NewLine);
////foreach (var item in l1)
////{
////   if(!l2.Any( l => l.userid == item.userid && l.email == item.email && l.department == item.department && l.global_group == item.global_group))
////    {
////        sb.AppendLine(item.userid + ", " + item.email + ", " + item.department + ", " + item.global_group);
////    }
////}

////sb.AppendLine(Environment.NewLine);

////sb.AppendLine("Members Removed:" + Environment.NewLine);
////foreach (var item in l2)
////{
////    if (!l1.Any(l => l.userid == item.userid && l.email == item.email && l.department == item.department && l.global_group == item.global_group))
////    {
////        sb.AppendLine(item.userid + ", " + item.email + ", " + item.department + ", " + item.global_group);
////    }
////}


////await d.Execute("Truncate Table stg.pbi_membership_test");



////string[] columns = typeof(PBIMembershipModel).GetProperties().Select(p => p.Name).ToArray();

//// await d.BulkSave<PBIMembershipModel>(configuration.GetConnectionString("Default"), "stg.pbi_membership_test", l1, columns);



////var firstNotSecond = l1.Except(l2).ToList();
////var secondNotFirst = l2.Except(l1).ToList();

////StringBuilder sb = new StringBuilder()
////sb.AppendLine("New Members Added:");
////foreach (var item in firstNotSecond)
////{
////    sb.AppendLine(item.userid + ", " + item.email + ", " + item.department + ", " + item.global_group);
////}


////sb.AppendLine("Members Removed:");
////foreach (var item in firstNotSecond)
////{
////    sb.AppendLine(item.userid + ", " + item.email + ", " + item.department + ", " + item.global_group);
////}

////var s = sb.ToString();
////var l = s;






////SELECT

////--ep 27: calculate CV3 and pct_in_spcl flags
////CASE WHEN ec2.spcl_CV >= 3 THEN 'N' ELSE  CASE WHEN ec2.spcl_CV < 3  THEN 'Y' ELSE NULL END END as CV3,
////ec4.spcl_Epsd_Cnt/ec5.spcl_tot_Epsd_cnt as pct_in_spcl,

////--step 29 : calculate volume, Pct_of_EPSD and recode ETG_TX_IND
////CASE WHEN ec2.[PREM_SPCL_CD] in ('FAMED', 'INTMD', 'PEDS') and ec4.spcl_Epsd_Cnt > 1000 then 'Y' ELSE
////CASE WHEN ec2.[PREM_SPCL_CD] in ('FAMED', 'INTMD', 'PEDS') and ec4.spcl_Epsd_Cnt <= 1000 then 'N' ELSE
////CASE WHEN ec2.[PREM_SPCL_CD] not in ('FAMED', 'INTMD', 'PEDS') and ec4.spcl_Epsd_Cnt > 500 then 'Y' ELSE
////CASE WHEN ec2.[PREM_SPCL_CD] not in ('FAMED', 'INTMD', 'PEDS') and ec4.spcl_Epsd_Cnt <= 500 then 'N' ELSE
////NULL END END END END as Volume,

////ec4.spcl_Epsd_Cnt/ec1.Epsd_Cnt as Pct_of_EPSD,

////CASE WHEN ISNULL(ec1.[ETG_TX_IND], '') = '' THEN '0' else ec1.[ETG_TX_IND] END as ETG_TX_IND




////FROM
////(
////    --Step 18: Calculate CV for  Commercial LOB

////    SELECT

////    SUM(CASE WHEN ec.[PD_CV_TOT] = 0 THEN NULL ELSE ec.[PD_CV_TOT] END) / SUM(ec.[PD_Epsd_Cnt]) as CV,
////    SUM([Epsd_Cnt]) as Epsd_Cnt,
////	SUM([Tot_Cost]) as Tot_Cost,
////		  [ETG_BAS_CLSS_NBR],
////		  [ETG_TX_IND]
////FROM[VCT_DB].[etgsymm].[VW_ETG_EPISODE_COST] ec
////GROUP BY [ETG_BAS_CLSS_NBR]
////		  ,[ETG_TX_IND]
////) ec1
////LEFT JOIN 
////(
////	--Step 20: Calculate CV for  Commercial LOB and premium specialty
////	SELECT 
////	SUM(CASE WHEN ec.[PD_CV_TOT] = 0 THEN NULL ELSE ec.[PD_CV_TOT] END ) / SUM(ec.[PD_Epsd_Cnt]) as spcl_CV,
////			[ETG_BAS_CLSS_NBR],
////			[ETG_TX_IND],
////			[PREM_SPCL_CD]
////FROM[VCT_DB].[etgsymm].[VW_ETG_EPISODE_COST] ec
////GROUP BY [ETG_BAS_CLSS_NBR],[ETG_TX_IND], [PREM_SPCL_CD]
////)ec2
////ON ec1.[ETG_BAS_CLSS_NBR] = ec2.[ETG_BAS_CLSS_NBR] AND ec1.[ETG_TX_IND] = ec2.[ETG_TX_IND]
////LEFT JOIN
////(
////	  --step 21:  Summarize NP cost and episodes by premium specialty
////	SELECT 
////	SUM(NP_Tot_Cost) as NP_Tot_Cost,
////	SUM(NP_Epsd_Cnt) as NP_Epsd_Cnt,
////		  [ETG_BAS_CLSS_NBR],
////		  [ETG_TX_IND],
////		  [PREM_SPCL_CD]
////FROM[VCT_DB].[etgsymm].[VW_ETG_EPISODE_COST] ec
////GROUP BY [ETG_BAS_CLSS_NBR],[ETG_TX_IND], [PREM_SPCL_CD]

////)ec3
////ON ec1.[ETG_BAS_CLSS_NBR] = ec3.[ETG_BAS_CLSS_NBR] AND ec1.[ETG_TX_IND] = ec3.[ETG_TX_IND]  AND ec2.[PREM_SPCL_CD] = ec3.[PREM_SPCL_CD]
////LEFT JOIN
////(
////	--Step 22: Summarize cost and episodes for Commercial LOB only(data from step 12) by premium specialty
////	SELECT 
////	SUM([Tot_Cost]) as spcl_Tot_Cost,
////	SUM([Average_Cost]) as spcl_Average_Cost,
////	SUM([Epsd_Cnt]) as spcl_Epsd_Cnt,
////			[ETG_BAS_CLSS_NBR],
////			[ETG_TX_IND],
////			[PREM_SPCL_CD]
////FROM[VCT_DB].[etgsymm].[VW_ETG_EPISODE_COST] ec
////GROUP BY [ETG_BAS_CLSS_NBR],[ETG_TX_IND], [PREM_SPCL_CD]
////)
////ec4 ON ec1.[ETG_BAS_CLSS_NBR] = ec4.[ETG_BAS_CLSS_NBR] AND ec1.[ETG_TX_IND] = ec4.[ETG_TX_IND]  AND ec3.[PREM_SPCL_CD] = ec4.[PREM_SPCL_CD]
////LEFT JOIN
////(
////	--Step 24 : specialty total episode count
////	SELECT 
////	SUM([Epsd_Cnt]) as spcl_tot_Epsd_cnt,
////			[PREM_SPCL_CD]
////FROM[VCT_DB].[etgsymm].[VW_ETG_EPISODE_COST] ec
////GROUP BY [PREM_SPCL_CD]
////)
////ec5 ON ec4.[PREM_SPCL_CD] = ec5.[PREM_SPCL_CD]




