using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;

namespace AddReminderToSpecialHandling
{
    class AddReminderToSpecialHandling
    {
        static void Main(string[] args)
        {
            string strPath = "C:\\Users\\cgiorda\\Desktop\\PR Spec Nov2017_Reminders";
            string strPath2 = "C:\\Users\\cgiorda\\Desktop\\PR Spec Nov2017_Reminders123";


            string strSQL = "select a.MPIN, b.taxid,folder_name from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and special_handling='Yes'";
            DataTable dt = DBConnection64.getMSSQLDataTable("data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;", strSQL);

            string strMPIN = null;
            string[] strCurrentPathArr;
            string strFileName = null;
            string strNewPath = null;

            foreach (string f in Directory.GetFiles(strPath + "_Tmp\\", "*.pdf", SearchOption.AllDirectories))
            {

                strCurrentPathArr =f.Split('\\');
                strFileName = strCurrentPathArr[strCurrentPathArr.Length - 1];
                strMPIN = strFileName.Split('_')[0];

                DataRow[] drTmp = dt.Select("MPIN = " + strMPIN);
                if (drTmp.Length > 0)
                {
                    strNewPath = strPath2 + "\\SpecialHandling\\" + drTmp[0]["folder_name"] + "\\" + drTmp[0]["taxid"] + "\\" ;

                }
                else
                {
                    strNewPath = strPath2 + "\\RegularMailing\\" ;
                }


                if (!Directory.Exists(strNewPath))
                {
                    Directory.CreateDirectory(strNewPath);
                }

                File.Copy(f, strNewPath + strFileName);



            }


        }
    }
}
