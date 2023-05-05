using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using WCDocumentGenerator;
using PDF_Functions;

namespace AddCoverLetterToExistingPDF
{
    class AddCoverLettersToExistingPDF
    {

        static string strExistingReportsPath_GLOBAL = null;
        static string strNewReportsPath_GLOBAL = null;
        static string strWordTemplate_GLOBAL = null;
        static string strExisitngFolderCopyStart_GLOBAL = null;
        static string strTempPath_GLOBAL = null;

        static void Main(string[] args)
        {

            strExistingReportsPath_GLOBAL = ConfigurationManager.AppSettings["ExistingReportsPath"];
            strWordTemplate_GLOBAL = ConfigurationManager.AppSettings["WordTemplate"];
            strNewReportsPath_GLOBAL = ConfigurationManager.AppSettings["NewReportsPath"];
            strExisitngFolderCopyStart_GLOBAL = ConfigurationManager.AppSettings["ExisitngFolderCopyStart"];
            strTempPath_GLOBAL = ConfigurationManager.AppSettings["PDFTemp"];



            MSWord.populateWordParameters(false, true, strTempPath_GLOBAL, strWordTemplate_GLOBAL);
            MSWord.openWordApp();

            foreach (string f in Directory.GetFiles(strExistingReportsPath_GLOBAL, "*.pdf", SearchOption.AllDirectories))
            {
                prepFile(f);
            }

            MSWord.closeWordApp();


        }


        static DataTable dt = null;
        static int intCnt = 1;
        private static void prepFile(string strFile)
        {


            getFileDetails(strFile);

            string strNewPath = strNewReportsPath_GLOBAL + "\\" + strCreatePath_GLOBAL;
            string strNewFullFilePath = strNewPath + "\\" + strCurrentFileName_GLOBAL.Replace(".pdf", "_reminder.pdf");
            if (File.Exists(strNewFullFilePath))
            {
                Console.WriteLine(intCnt + ": " + strCurrentMPIN_GLOBAL + " already has an updated profile skipping to next...");
                intCnt++;
                return;
            }



            if (strCurrentMPIN_GLOBAL.StartsWith("~$")) //TEMP FILE IGNORE!!!!
            {
                return;
            }


            if (dt == null)
            {
                string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
                string strSQL = "select a.MPIN, 'March 1, 2017' as [Date],FirstName + ' ' + LastName as [Physician Name],ProvDegree as [Prov_Degree], LongDesc as Specialty,b.Street as [Address 1],b.City,b.[State],b.zipcd as [ZIP Code],P_LastName, RCMO,RCMO_title as [RCMO title] from dbo.PBP_Outl_ph2 as a inner join dbo.PBP_outl_demogr_ph2 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph2 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph2 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2";
                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
            }


            DataTable dtTmp = null;
            DataRow[] drTmp = dt.Select("MPIN = " + strCurrentMPIN_GLOBAL);
            if(drTmp.Length > 0)
                dtTmp = dt.Select("MPIN = " + strCurrentMPIN_GLOBAL).CopyToDataTable();
            else
                return;
            //if (dtTmp.Rows.Count < 1)

            //dt = new DataTable();
            //dt.Clear();
            //dt.Columns.Add("FirstName");
            //dt.Columns.Add("LastName");
            //dt.Columns.Add("Date");
            //dt.Columns.Add("Test");
            //DataRow dr = dt.NewRow();
            //dr["FirstName"] = "Chris";
            //dr["LastName"] = "Giordano";
            //dr["Date"] = DateTime.Now.ToLongDateString();
            //dr["Test"] = intCnt.ToString();
            //dt.Rows.Add(dr);




            if (!Directory.Exists(strNewPath))
            {
                Directory.CreateDirectory(strNewPath);
            }

            MSWord.openWordDocument();


            MSWord.replaceManyPlaceholders(dtTmp);


            MSWord.convertWordToPDF("temp", null, null);

            MSWord.closeWordDocument();



            PDFHelper.mergePDF(strTempPath_GLOBAL + "temp.pdf", strFile, strNewFullFilePath, false);


            //Footer Change value in page 1 in footer and potentially last



            Console.WriteLine(intCnt + ": " + strCurrentMPIN_GLOBAL + " profile updated!");

            intCnt++;

        }

        static string strCurrentMPIN_GLOBAL = null;
        static string strCurrentFileName_GLOBAL = null;
        static string strCreatePath_GLOBAL = null;
        private static void getFileDetails(string strCurrentPath)
        {

            strCurrentMPIN_GLOBAL = null;
            strCurrentFileName_GLOBAL = null;
            strCreatePath_GLOBAL = null;

            string[] strCurrentPathArr = strCurrentPath.Split('\\');
            StringBuilder sbTmp = new StringBuilder();

            for (int i = 0; i < strCurrentPathArr.Length; i++)
            {

                if (i == strCurrentPathArr.Length - 1)
                {
                    strCurrentFileName_GLOBAL = strCurrentPathArr[i];
                }
                else if (strExisitngFolderCopyStart_GLOBAL == strCurrentPathArr[i] || sbTmp.Length > 0)
                {
                    sbTmp.Append(strCurrentPathArr[i] + "\\");
                }

            }
            strCurrentMPIN_GLOBAL = strCurrentFileName_GLOBAL.Split('_')[0];
            //strCreatePath_GLOBAL = sbTmp.ToString().Replace("\\word\\", "\\pdf");
            strCreatePath_GLOBAL = sbTmp.ToString().Replace("\\word\\", "").Replace("QA\\", "Reminders\\");

        }



    }
}
