using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.MHP;

namespace FileParsingLibrary.MSExcel.Custom.MHP
{
    public static class MHPExcelExport
    {

        public static async Task<byte[]> ExportEIToExcel(List<MHP_EI_Model> mhp_results, List<MHPEIDetails_Model> mhp_details, List<MHPEIDetails_Model> mhp_details_all, Func<string> getterStatus, Action<string> setterStatus, CancellationToken token)
        {

            //throw new Exception("Oh nooooooo!!!");
            byte[] final = new byte[0];
            mhp_results.OrderBy(o => o.LegalEntity).OrderBy(o => o.ExcelRow).ToList();
            //mhp_details.OrderBy(o => o.LegalEntity).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();
            mhp_details.OrderBy(o => o.LEG_ENTY_NBR).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();

            string strFilePath = @"\\WP000003507\csg_share\VCPortal\Files\MHP_Reporting_Template.xlsx";

            int intNameCntTmp = 0;
            XLWorkbook wb = new XLWorkbook(strFilePath);
            IXLWorksheet wsSource = null;
            IXLRange range;
            int rowCnt = 0;
            StringBuilder sbStatus = new StringBuilder();
            sbStatus.Append(getterStatus());
            foreach (MHP_EI_Model mhp in mhp_results)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }


                if (mhp.ExcelRow == 4)
                {

  
                    sbStatus.Append("--Creating sheet for " + mhp.LegalEntity + Environment.NewLine);
                    setterStatus(sbStatus.ToString());

                    wsSource = wb.Worksheet("template");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);
                    var newSheetName = mhp.LegalEntity.Split('-')[0].Trim();
                    wsSource.CopyTo(newSheetName);
                    wsSource = wb.Worksheet(newSheetName);
                    wsSource.Cell("A1").Value = mhp.State + " " + mhp.LegalEntity + " : " + mhp.StartDate + "-" + mhp.EndDate;
                    wsSource.Cell("A1").Style.Font.Bold = true;
                    wsSource.Cell("A1").Style.Fill.BackgroundColor = XLColor.Yellow;
                    wsSource.Cell("A1").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    intNameCntTmp++;
                }

                wsSource.Cell("B" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_in_ip + "") ? null : mhp.cnt_in_ip + "");
                wsSource.Cell("D" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_on_ip + "") ? null : mhp.cnt_on_ip + "");
                wsSource.Cell("F" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_in_op + "") ? null : mhp.cnt_in_op + "");
                wsSource.Cell("H" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_on_op + "") ? null : mhp.cnt_on_op + "");


            }



            wb.Worksheet("template").Delete();

            rowCnt = 2;
            string lastEntity = null;

            foreach (MHPEIDetails_Model mhp in mhp_details)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (lastEntity != mhp.LEG_ENTY_NBR)
                {
                    sbStatus.Append("--Creating details sheet for " + mhp.LEG_ENTY_NBR + " - " + mhp.LEG_ENTY_FULL_NM + Environment.NewLine);
                    setterStatus(sbStatus.ToString());

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
                    wsSource.Cell("L1").Value = "LegalEntity";
                    wsSource.Cell("M1").Value = nameof(mhp.Enrollee_First_Name);
                    wsSource.Cell("N1").Value = nameof(mhp.Enrollee_Last_Name);
                    wsSource.Cell("O1").Value = nameof(mhp.Cardholder_ID);
                    wsSource.Cell("P1").Value = nameof(mhp.Member_Date_of_Birth);
                    wsSource.Cell("Q1").Value = nameof(mhp.Procedure_Code_Description);
                    wsSource.Cell("R1").Value = "Primary_Procedure_Code";
                    wsSource.Cell("S1").Value = nameof(mhp.Primary_Diagnosis_Code);
                    wsSource.Cell("T1").Value = nameof(mhp.CUST_SEG_NBR);
                    wsSource.Cell("U1").Value = nameof(mhp.CUST_SEG_NM);
                    //wsSource.Cell("S1").Value = nameof(mhp.Diagnosis_Code_Description);



                    range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHPEIDetails_Model).GetProperties().Length).Address);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.BackgroundColor = XLColor.Yellow;
                    //range.Style
                    if (mhp.LEG_ENTY_NBR != lastEntity)
                        wsSource.Columns().AdjustToContents(1, 20);
                    //wsSource.Columns().AdjustToContents(1, typeof(MHPUniverseDetails_Model).GetProperties().Length);   // Adjust column width


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
                wsSource.Cell("Q" + rowCnt).SetValue(mhp.Procedure_Code_Description + "");
                wsSource.Cell("R" + rowCnt).SetValue(mhp.Primary_Procedure_Code_Req + "");
                wsSource.Cell("S" + rowCnt).SetValue(mhp.Primary_Diagnosis_Code + "");
                wsSource.Cell("T" + rowCnt).SetValue(mhp.CUST_SEG_NBR + "");
                wsSource.Cell("U" + rowCnt).SetValue(mhp.CUST_SEG_NM + "");
                //wsSource.Cell("S" + rowCnt).Value = mhp.Diagnosis_Code_Description;

                rowCnt++;
            }




            rowCnt = 2;


          
            if (token.IsCancellationRequested)
            {
                setterStatus("~~~Report Generation Cancelled~~~");
                token.ThrowIfCancellationRequested();
            }

            sbStatus.Append("--Creating details all sheet for EI " +  Environment.NewLine);
            setterStatus(sbStatus.ToString());

            //var newSheetName = mhp.LegalEntity.Split('-')[0].Trim();
            wsSource = wb.Worksheets.Add("EI_Details_All");
            // Copy the worksheet to a new sheet in this workbook
            //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);

            wsSource.Cell("A1").Value = "Authorization";
            wsSource.Cell("B1").Value = "Request_Decision";
            wsSource.Cell("C1").Value = "Authorization_Type";
            wsSource.Cell("D1").Value = "Par_NonPar_Site";
            wsSource.Cell("E1").Value = "Inpatient_Outpatient";
            wsSource.Cell("F1").Value = "Request_Date";
            wsSource.Cell("G1").Value = "State_of_Issue";
            wsSource.Cell("H1").Value = "FINC_ARNG_DESC";
            wsSource.Cell("I1").Value = "Decision_Reason";
            wsSource.Cell("J1").Value = "MKT_SEG_RLLP_DESC";
            wsSource.Cell("K1").Value = "MKT_TYP_DESC";
            wsSource.Cell("L1").Value = "LegalEntity";
            wsSource.Cell("M1").Value = "Enrollee_First_Name";
            wsSource.Cell("N1").Value = "Enrollee_Last_Name";
            wsSource.Cell("O1").Value = "Cardholder_ID";
            wsSource.Cell("P1").Value = "Member_Date_of_Birth";
            wsSource.Cell("Q1").Value = "Procedure_Code_Description";
            wsSource.Cell("R1").Value = "Primary_Procedure_Code";
            wsSource.Cell("S1").Value = "Primary_Diagnosis_Code";
            wsSource.Cell("T1").Value = "CUST_SEG_NBR";
            wsSource.Cell("U1").Value = "CUST_SEG_NM";
            //wsSource.Cell("S1").Value = nameof(mhp.Diagnosis_Code_Description);



            range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHPEIDetails_Model).GetProperties().Length).Address);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            range.Style.Font.Bold = true;
            range.Style.Fill.BackgroundColor = XLColor.Yellow;
        
            rowCnt = 2;
            intNameCntTmp++;
            foreach (MHPEIDetails_Model mhp in mhp_details_all)
            {

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
                wsSource.Cell("Q" + rowCnt).SetValue(mhp.Procedure_Code_Description + "");
                wsSource.Cell("R" + rowCnt).SetValue(mhp.Primary_Procedure_Code_Req + "");
                wsSource.Cell("S" + rowCnt).SetValue(mhp.Primary_Diagnosis_Code + "");
                wsSource.Cell("T" + rowCnt).SetValue(mhp.CUST_SEG_NBR + "");
                wsSource.Cell("U" + rowCnt).SetValue(mhp.CUST_SEG_NM + "");
                //wsSource.Cell("S" + rowCnt).Value = mhp.Diagnosis_Code_Description;

                rowCnt++;
            }


            wsSource.Columns().AdjustToContents(1, 20);



            if (token.IsCancellationRequested)
            {
                setterStatus("~~~Report Generation Cancelled~~~");
                token.ThrowIfCancellationRequested();
            }


         
            sbStatus.Append("--Preparing Excel file for saving" + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);

                final = ms.ToArray();
            }
 
            await Task.CompletedTask;
            return final;
        }



        public static async Task<byte[]> ExportCSToExcel(List<MHP_CS_Model> mhp_results, List<MHPCSDetails_Model> mhp_details, string products, Func<string> getterStatus, Action<string> setterStatus, CancellationToken token)
        {

            //throw new Exception("Oh nooooooo!!!");
            byte[] final = new byte[0];
            mhp_results.OrderBy(o => o.ExcelRow).ToList();
            //mhp_details.OrderBy(o => o.LegalEntity).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();
            mhp_details.OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();

            string strFilePath = @"\\WP000003507\Home Directory - UCS Team Portal\Files\MHPCS_Reporting_Template.xlsx";


            int intNameCntTmp = 0;
            XLWorkbook wb = new XLWorkbook(strFilePath);
            IXLWorksheet wsSource = null;
            StringBuilder sbStatus = new StringBuilder();
            sbStatus.Append(getterStatus());
            foreach (MHP_CS_Model mhp in mhp_results)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }


                if (mhp.ExcelRow == 4)
                {

                    sbStatus.Append("--Creating summary sheet for CS" + Environment.NewLine);
                    setterStatus(sbStatus.ToString());

                    wsSource = wb.Worksheet("template");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);
                    var newSheetName = "CS";
                    wsSource.CopyTo(newSheetName);
                    wsSource = wb.Worksheet(newSheetName);
                    wsSource.Cell("A1").Value = mhp.State + " (" + products + ") : " + mhp.StartDate + "-" + mhp.EndDate;
                    intNameCntTmp++;
                }

                wsSource.Cell("B" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_ip + "") ? null : mhp.cnt_ip + "");
                wsSource.Cell("E" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_op + "") ? null : mhp.cnt_op + "");

            }


            wb.Worksheet("template").Delete();

            bool blHead = true;
            int rowCnt = 2;
            IXLRange range;
            foreach (MHPCSDetails_Model mhp in mhp_details)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (blHead)
                {
                    sbStatus.Append("--Creating detail sheet for CS " + Environment.NewLine);
                    setterStatus(sbStatus.ToString());

                    range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHPCSDetails_Model).GetProperties().Length).Address);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.BackgroundColor = XLColor.Yellow;
                    //range.Style

                    wsSource.Columns().AdjustToContents(1, 20);   // Adjust column width
                                                                                                                                 //wsSource.Rows().AdjustToContents(1, mhp_details.Count(n => n.LegalEntity == lastEntity));



                    //var newSheetName = mhp.LegalEntity.Split('-')[0].Trim();
                    var newSheetName = "CS";
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
                    wsSource.Cell("H1").Value = nameof(mhp.Decision_Reason);
                    wsSource.Cell("I1").Value = nameof(mhp.CS_TADM_PRDCT_MAP);
                    wsSource.Cell("J1").Value = nameof(mhp.Enrollee_First_Name);
                    wsSource.Cell("K1").Value = nameof(mhp.Enrollee_Last_Name);
                    wsSource.Cell("L1").Value = nameof(mhp.Cardholder_ID);
                    wsSource.Cell("M1").Value = nameof(mhp.Member_Date_of_Birth);
                    wsSource.Cell("N1").Value = nameof(mhp.Procedure_Code_Description);
                    wsSource.Cell("O1").Value = nameof(mhp.Primary_Procedure_Code_Req);
                    wsSource.Cell("P1").Value = nameof(mhp.Primary_Diagnosis_Code);
                    wsSource.Cell("Q1").Value = nameof(mhp.Group_Number);
                    wsSource.Cell("R1").Value = nameof(mhp.PRDCT_CD_DESC);
                    //wsSource.Cell("S1").Value = nameof(mhp.Diagnosis_Code_Description);

                    rowCnt = 2;
                    intNameCntTmp++;
                    blHead = false;
                }

                wsSource.Cell("A" + rowCnt).Value = mhp.Authorization;
                wsSource.Cell("B" + rowCnt).Value = mhp.Request_Decision;
                wsSource.Cell("C" + rowCnt).Value = mhp.Authorization_Type;
                wsSource.Cell("D" + rowCnt).Value = mhp.Par_NonPar_Site;
                wsSource.Cell("E" + rowCnt).Value = mhp.Inpatient_Outpatient;
                wsSource.Cell("F" + rowCnt).Value = mhp.Request_Date;
                wsSource.Cell("G" + rowCnt).Value = mhp.State_of_Issue;
                wsSource.Cell("H" + rowCnt).Value = mhp.Decision_Reason;
                wsSource.Cell("I" + rowCnt).Value = mhp.CS_TADM_PRDCT_MAP;
                wsSource.Cell("J" + rowCnt).Value = mhp.Enrollee_First_Name;
                wsSource.Cell("K" + rowCnt).Value = mhp.Enrollee_Last_Name;
                wsSource.Cell("L" + rowCnt).Value = mhp.Cardholder_ID;
                wsSource.Cell("M" + rowCnt).Value = mhp.Member_Date_of_Birth;
                wsSource.Cell("N" + rowCnt).Value = mhp.Procedure_Code_Description;
                wsSource.Cell("O" + rowCnt).Value = mhp.Primary_Procedure_Code_Req;
                wsSource.Cell("P" + rowCnt).Value = mhp.Primary_Diagnosis_Code;
                wsSource.Cell("Q" + rowCnt).Value = mhp.Group_Number;
                wsSource.Cell("R" + rowCnt).SetValue(mhp.PRDCT_CD_DESC);
                //wsSource.Cell("S" + rowCnt).Value = mhp.Diagnosis_Code_Description;



                rowCnt++;
            }
            //LAST SHEET RESIZE
            //wsSource.Columns().AdjustToContents();
            range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHPCSDetails_Model).GetProperties().Length).Address);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            range.Style.Font.Bold = true;
            range.Style.Fill.BackgroundColor = XLColor.Yellow;

            //wsSource.Column(13).CellsUsed().SetDataType(XLDataType.Text);
            //wsSource.Column(14).CellsUsed().SetDataType(XLDataType.Text);
            //PRIMARY DAIG COL P = 15?
            //wsSource.Column(17).CellsUsed().SetDataType(XLDataType.Text);




            wsSource.Columns().AdjustToContents(1, typeof(MHPCSDetails_Model).GetProperties().Length);   // Adjust column width
                                                                                                                           //wsSource.Rows().AdjustToContents(1, mhp_details.Count(n => n.LegalEntity == lastEntity));

            if (token.IsCancellationRequested)
            {

                setterStatus("~~~Report Generation Cancelled~~~");
                token.ThrowIfCancellationRequested();
            }


            sbStatus.Append("--Preparing Excel file for saving" + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);

                final = ms.ToArray();
            }


            await Task.CompletedTask;
            return final;
        }
        public static async Task<byte[]> ExportIFPToExcel(List<MHP_IFP_Model> mhp_results, List<MHPIFPDetails_Model> mhp_details, Func<string> getterStatus, Action<string> setterStatus, CancellationToken token)
        {

            //throw new Exception("Oh nooooooo!!!");
            byte[] final = new byte[0];
            mhp_results.OrderBy(o => o.Product).OrderBy(o => o.ExcelRow).ToList();
            //mhp_details.OrderBy(o => o.LegalEntity).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();
            mhp_details.OrderBy(o => o.PRDCT_CD).OrderBy(o => o.Request_Date).OrderBy(o => o.Authorization).ToList();

            string strFilePath = @"\\WP000003507\Home Directory - UCS Team Portal\Files\MHP_Reporting_Template.xlsx";

            int intNameCntTmp = 0;
            XLWorkbook wb = new XLWorkbook(strFilePath);
            IXLWorksheet wsSource = null;
            IXLRange range;
            int rowCnt = 0;
            StringBuilder sbStatus = new StringBuilder();
            sbStatus.Append(getterStatus());
            foreach (MHP_IFP_Model mhp in mhp_results)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }


                if (mhp.ExcelRow == 4)
                {
                    sbStatus.Append("--Creating sheet for " + mhp.Product + Environment.NewLine);
                    setterStatus(sbStatus.ToString());

                    wsSource = wb.Worksheet("template");
                    // Copy the worksheet to a new sheet in this workbook
                    //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);
                    var newSheetName = mhp.Product;
                    wsSource.CopyTo(newSheetName);
                    wsSource = wb.Worksheet(newSheetName);
                    wsSource.Cell("A1").Value = mhp.State + " " + mhp.Product + " : " + mhp.StartDate + "-" + mhp.EndDate;
                    wsSource.Cell("A1").Style.Font.Bold = true;
                    wsSource.Cell("A1").Style.Fill.BackgroundColor = XLColor.Yellow;
                    wsSource.Cell("A1").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    intNameCntTmp++;
                }

                wsSource.Cell("B" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_in_ip + "") ? null : mhp.cnt_in_ip + "");
                wsSource.Cell("D" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_on_ip + "") ? null : mhp.cnt_on_ip + "");
                wsSource.Cell("F" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_in_op + "") ? null : mhp.cnt_in_op + "");
                wsSource.Cell("H" + mhp.ExcelRow).Value = (string.IsNullOrEmpty(mhp.cnt_on_op + "") ? null : mhp.cnt_on_op + "");


            }



            wb.Worksheet("template").Delete();

            rowCnt = 2;
            string lastProd = null;

            foreach (MHPIFPDetails_Model mhp in mhp_details)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (lastProd != mhp.PRDCT_CD)
                {
                    sbStatus.Append("--Creating details sheet for " + mhp.PRDCT_CD + " - " + mhp.PRDCT_CD_DESC + Environment.NewLine);
                    setterStatus(sbStatus.ToString());

                    //var newSheetName = mhp.LegalEntity.Split('-')[0].Trim();
                    var newSheetName = mhp.PRDCT_CD;
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
                    wsSource.Cell("H1").Value = nameof(mhp.Decision_Reason);
                    wsSource.Cell("I1").Value = "Products";
                    wsSource.Cell("J1").Value = nameof(mhp.Enrollee_First_Name);
                    wsSource.Cell("K1").Value = nameof(mhp.Enrollee_Last_Name);
                    wsSource.Cell("L1").Value = nameof(mhp.Cardholder_ID);
                    wsSource.Cell("M1").Value = nameof(mhp.Member_Date_of_Birth);
                    wsSource.Cell("N1").Value = nameof(mhp.Procedure_Code_Description);
                    wsSource.Cell("O1").Value = nameof(mhp.Primary_Procedure_Code_Req);
                    wsSource.Cell("P1").Value = nameof(mhp.Primary_Diagnosis_Code);

                    //wsSource.Cell("S1").Value = nameof(mhp.Diagnosis_Code_Description);



                    range = wsSource.Range(wsSource.Cell(1, 1).Address, wsSource.Cell(1, typeof(MHPIFPDetails_Model).GetProperties().Length).Address);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.BackgroundColor = XLColor.Yellow;
                    //range.Style
                    if (mhp.PRDCT_CD != lastProd)
                        wsSource.Columns().AdjustToContents(1,20);   // Adjust column width


                    lastProd = mhp.PRDCT_CD;
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
                wsSource.Cell("H" + rowCnt).Value = mhp.Decision_Reason;
                wsSource.Cell("I" + rowCnt).Value = mhp.PRDCT_CD + " - " + mhp.PRDCT_CD_DESC;
                wsSource.Cell("J" + rowCnt).Value = mhp.Enrollee_First_Name;
                wsSource.Cell("K" + rowCnt).Value = mhp.Enrollee_Last_Name;
                wsSource.Cell("L" + rowCnt).Value = mhp.Cardholder_ID;
                wsSource.Cell("M" + rowCnt).Value = mhp.Member_Date_of_Birth;
                wsSource.Cell("N" + rowCnt).Value = mhp.Procedure_Code_Description;
                wsSource.Cell("O" + rowCnt).Value = mhp.Primary_Procedure_Code_Req;
                wsSource.Cell("P" + rowCnt).SetValue(mhp.Primary_Diagnosis_Code);
                //wsSource.Cell("S" + rowCnt).Value = mhp.Diagnosis_Code_Description;

                rowCnt++;
            }


            if (token.IsCancellationRequested)
            {
                setterStatus("~~~Report Generation Cancelled~~~");
                token.ThrowIfCancellationRequested();
            }


            sbStatus.Append("--Preparing Excel file for saving" + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);

                final = ms.ToArray();
            }

            await Task.CompletedTask;
            return final;
        }

    }
}
