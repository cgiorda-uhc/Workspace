using ClosedXML.Excel;
using System.Text;
using VCPortal_Models.Models.MHP;

namespace VCPortal_Shared.Excel
{
    public  class MHCUniverse_Excel
    {

        public string FinalStatus { get; set; }


        private StringBuilder _sbStatus = new StringBuilder();

        public MHCUniverse_Excel()
        {
        }

        public async Task<Stream> ExportEIToExcel(List<MHP_EI_Model> mhp_results, List<MPHUniverseDetails_Model> mhp_details, CancellationToken token)
        {

            //throw new Exception("Oh nooooooo!!!");

            //mhp_results.OrderBy(o => o.LegalEntity).OrderBy(o => o.ExcelRow).ToList();
            ////mhp_details.OrderBy(o => o.LegalEntity).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();
            //mhp_details.OrderBy(o => o.LEG_ENTY_NBR).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();

            string strFilePath = @"\\WP000003507\Home Directory - UCS Team Portal\Files\MHP_Reporting_Template.xlsx";

            int intNameCntTmp = 0;
            XLWorkbook wb = new XLWorkbook(strFilePath);
            IXLWorksheet wsSource = null;


            foreach (MHP_EI_Model mhp in mhp_results)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (mhp.ExcelRow == 4)
                {
                    _sbStatus.Append("-Creating sheet for " + mhp.LegalEntity + Environment.NewLine);
                    //Status = sbStatus.ToString();

                    wsSource = wb.Worksheet("template");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);
                    var newSheetName = mhp.LegalEntity.Split('-')[0].Trim();
                    wsSource.CopyTo(newSheetName);
                    wsSource = wb.Worksheet(newSheetName);
                    wsSource.Cell("A1").Value = mhp.State + " " + mhp.LegalEntity + " : " + mhp.StartDate + "-" + mhp.EndDate;
                    intNameCntTmp++;
                }

                wsSource.Cell("B" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_in_ip + "") ? null : mhp.cnt_in_ip + "");
                wsSource.Cell("D" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_on_ip + "") ? null : mhp.cnt_on_ip + "");
                wsSource.Cell("F" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_in_op + "") ? null : mhp.cnt_in_op + "");
                wsSource.Cell("H" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_on_op + "") ? null : mhp.cnt_on_op + "");

            }

            wb.Worksheet("template").Delete();


            int rowCnt = 2;
            string lastEntity = null;
            IXLRange range;
            foreach (MPHUniverseDetails_Model mhp in mhp_details)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (lastEntity != mhp.LEG_ENTY_NBR)
                {
                    _sbStatus.Append("-Creating details sheet for " + mhp.LEG_ENTY_NBR + " - " + mhp.LEG_ENTY_FULL_NM + Environment.NewLine);
                   // Status = sbStatus.ToString();


                    //NOT FIRST PASS SO RESIZE LAST NEW SHEET
                    if (lastEntity != null)
                    {

                        range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MPHUniverseDetails_Model).GetProperties().Length).Address);
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                        range.Style.Font.Bold = true;
                        range.Style.Fill.BackgroundColor = XLColor.Yellow;
                        //range.Style

                        wsSource.Columns().AdjustToContents(1, typeof(MPHUniverseDetails_Model).GetProperties().Length);   // Adjust column width
                        //wsSource.Rows().AdjustToContents(1, mhp_details.Count(n => n.LegalEntity == lastEntity));
                    }


                    //var newSheetName = mhp.LegalEntity.Split('-')[0].Trim();
                    var newSheetName = mhp.LEG_ENTY_NBR;
                    wsSource = wb.Worksheets.Add(newSheetName + "_Details");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);

                    wsSource.Cell("A1").Value = nameof(mhp.Authorization);
                    wsSource.Cell("B1").Value = nameof(mhp.Request_Decision);
                    wsSource.Cell("C1").Value = nameof(mhp.Authorization_Type);
                    wsSource.Cell("D1").Value = nameof(mhp.Par_NonPar_Site);
                    wsSource.Cell("E1").Value = nameof(mhp.Inpatient_Outpatient);
                    wsSource.Cell("F1").Value = nameof(mhp.Request_Date);
                    wsSource.Cell("G1").Value = nameof(mhp.State_of_Issue);
                    wsSource.Cell("H1").Value = nameof(mhp.FINC_ARNG_DESC);
                    wsSource.Cell("I1").Value = nameof(mhp.Decision_Reason);
                    wsSource.Cell("J1").Value = nameof(mhp.MKT_SEG_RLLP_DESC);
                    wsSource.Cell("K1").Value = nameof(mhp.MKT_TYP_DESC);
                    //wsSource.Cell("L1").Value = nameof(mhp.LEG_ENTY_FULL_NM);
                    wsSource.Cell("L1").Value = "LegalEntity";
                    wsSource.Cell("M1").Value = nameof(mhp.Enrollee_First_Name);
                    wsSource.Cell("N1").Value = nameof(mhp.Enrollee_Last_Name);
                    wsSource.Cell("O1").Value = nameof(mhp.Cardholder_ID);
                    wsSource.Cell("P1").Value = nameof(mhp.Member_Date_of_Birth);
                    wsSource.Cell("Q1").Value = nameof(mhp.Procedure_Code_Description);
                    wsSource.Cell("R1").Value = nameof(mhp.Primary_Diagnosis_Code);
                    //wsSource.Cell("S1").Value = nameof(mhp.Diagnosis_Code_Description);



                    lastEntity = mhp.LEG_ENTY_NBR;
                    rowCnt = 2;
                    intNameCntTmp++;
                }

                wsSource.Cell("A" + rowCnt).Value = mhp.Authorization;
                wsSource.Cell("B" + rowCnt).Value = mhp.Request_Decision;
                wsSource.Cell("C" + rowCnt).Value = mhp.Authorization_Type;
                wsSource.Cell("D" + rowCnt).Value = mhp.Par_NonPar_Site;
                wsSource.Cell("E" + rowCnt).Value = mhp.Inpatient_Outpatient;
                wsSource.Cell("F" + rowCnt).Value = mhp.Request_Date;
                wsSource.Cell("G" + rowCnt).Value = mhp.State_of_Issue;
                wsSource.Cell("H" + rowCnt).Value = mhp.FINC_ARNG_DESC;
                wsSource.Cell("I" + rowCnt).Value = mhp.Decision_Reason;
                wsSource.Cell("J" + rowCnt).Value = mhp.MKT_SEG_RLLP_DESC;
                wsSource.Cell("K" + rowCnt).Value = mhp.MKT_TYP_DESC;
                wsSource.Cell("L" + rowCnt).Value = mhp.LEG_ENTY_NBR + " - " + mhp.LEG_ENTY_FULL_NM;
                wsSource.Cell("M" + rowCnt).Value = mhp.Enrollee_First_Name;
                wsSource.Cell("N" + rowCnt).Value = mhp.Enrollee_Last_Name;
                wsSource.Cell("O" + rowCnt).Value = mhp.Cardholder_ID;
                wsSource.Cell("P" + rowCnt).Value = mhp.Member_Date_of_Birth;
                wsSource.Cell("Q" + rowCnt).Value = mhp.Procedure_Code_Description;
                wsSource.Cell("R" + rowCnt).Value = mhp.Primary_Diagnosis_Code;
                //wsSource.Cell("S" + rowCnt).Value = mhp.Diagnosis_Code_Description;



                rowCnt++;
            }
            //LAST SHEET RESIZE
            //wsSource.Columns().AdjustToContents();
            range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MPHUniverseDetails_Model).GetProperties().Length).Address);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            range.Style.Font.Bold = true;
            range.Style.Fill.BackgroundColor = XLColor.Yellow;


            wsSource.Columns().AdjustToContents(1, typeof(MPHUniverseDetails_Model).GetProperties().Length);   // Adjust column width
                                                                                                                         //wsSource.Rows().AdjustToContents(1, mhp_details.Count(n => n.LegalEntity == lastEntity));

            if (token.IsCancellationRequested)
            {
                //Status = "~~~Report Generation Cancelled~~~";
                token.ThrowIfCancellationRequested();
            }


            strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MHP_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";
            _sbStatus.Append("-Saving Excel here: " + strFilePath + Environment.NewLine);
            //Status = sbStatus.ToString();

            //CLEANUP
            //if (File.Exists(strFilePath))
            //File.Delete(strFilePath);
            //wb.SaveAs(strFilePath);

            Stream fs = new MemoryStream();
            wb.SaveAs(fs);
            fs.Position = 0;




            _sbStatus.Append("-Opening Excel" + Environment.NewLine);
            //Status = sbStatus.ToString();
            //DISPLAY
            //System.Diagnostics.Process.Start(strFilePath);


            FinalStatus = _sbStatus.ToString();



            return fs;
        }

    }
}
