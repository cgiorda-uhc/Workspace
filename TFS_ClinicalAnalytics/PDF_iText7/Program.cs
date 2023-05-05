using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;

using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iTextSharp.text;

namespace PDF_iText7
{
    class Program
    {
        static void Main(string[] args)
        {
            string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strSQL = "";

            DataTable dt = new DataTable();

            float[] pointColumnWidths;


            string strPDFPath = @"C:\Users\cgiorda\Documents\itextpdf_text.pdf";
            if (File.Exists(strPDFPath))
                File.Delete(strPDFPath);

            PDF_Helper.colorTableHeaderBackground = ColorConstants.BLUE;
            PDF_Helper.colorTableHeaderForeground = ColorConstants.WHITE;
            PDF_Helper.initializePDF(strPDFPath);


            PDF_Helper.addImage(@"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - UCS Companion Application\Template\uhc_logo.jpg");

            strSQL = "SELECT Distinct t1.Taxid, t1.GROUP_NAME, t1.Reporting_Period, t1.BUNDLE_TYPE, CONVERT(VARCHAR(10), t1.REPORT_DATE, 101) as REPORT_DATE FROM dbo.BP_DATA t1";
            dt = DBConnection32.getMSSQLDataTable(strConnectionString, strSQL);
            pointColumnWidths = new float[] { 1F, 6F, 1F, 2F, 1F };
            PDF_Helper.addTable(dt, pointColumnWidths);


            PDF_Helper.addBlankLine();


            strSQL = "SELECT t1.MEASURE_DESC, t1.Taxid,  CONVERT(VARCHAR(10), t1.MEASURE_BEGIN_DATE, 101) as MEASURE_BEGIN_DATE,  CONVERT(VARCHAR(10), t1.MEASURE_END_DATE, 101) as MEASURE_END_DATE, CONVERT(VARCHAR(6),ROUND(t1.NATIONAL_RATE * 100,1)) + '%'  as BASELINE_CONTRACT_RATE  FROM dbo.BP_DATA t1";
            dt = DBConnection32.getMSSQLDataTable(strConnectionString, strSQL);
            pointColumnWidths = new float[] { 7F, 1F, 1F, 1F, 1F };
            PDF_Helper.addTable(dt, pointColumnWidths);

            PDF_Helper.addBlankLine();

            strSQL = "IF OBJECT_ID('tempdb..#BP_DATA_TMP') IS NOT NULL DROP TABLE #BP_DATA_TMP IF OBJECT_ID('tempdb..#BP_DATA_TOTALS_TMP') IS NOT NULL DROP TABLE #BP_DATA_TOTALS_TMP SELECT t1.MEASURE_DESC, t1.TAXID, t1.GROUP_RATE, t1.GROUP_NUMERATOR, t1.GROUP_DENOMINATOR, t1.RESULT AS QUAL_RESULT, t1.Measure_Volume, t1.National_Rate, t1.RESULT INTO #BP_DATA_TMP FROM dbo.BP_DATA t1 WHERE  ('-9999' = '-9999' OR t1.BUNDLE_TYPE = '-9999' ) AND  t1.REPORT_TYPE = 'Contract' AND REPORT_DATE ='6/11/2020'  AND t1.Taxid in (581661116, 431696710, 475021191, 222159534, 222159534) AND Convert(DateTime, Convert(VarChar, t1.MEASURE_BEGIN_DATE, 101)) >= '10/1/2019' AND Convert(DateTime, Convert(VarChar, t1.MEASURE_END_DATE, 101)) <= '12/31/2019'; WITH t AS ( SELECT MEASURE_DESC, sum(GROUP_DENOMINATOR) as total_events, sum(GROUP_NUMERATOR) as total_yes, SUM(GROUP_NUMERATOR)/SUM(GROUP_DENOMINATOR) as bnchmrk_obsrvd FROM #BP_DATA_TMP GROUP BY MEASURE_DESC ) SELECT t.MEASURE_DESC, t.total_events, t.total_yes, t.bnchmrk_obsrvd, total_events - total_yes as total_no, t.total_events * avg(bnchmrk_obsrvd) as expected_yes, t.total_events - (t.total_events * avg(bnchmrk_obsrvd)) as expected_no, CASE WHEN (t.total_events * avg(bnchmrk_obsrvd)) = 0 OR (t.total_events - (t.total_events * avg(bnchmrk_obsrvd))) = 0 THEN -9999 ELSE ((SQUARE((total_yes - (t.total_events * avg(bnchmrk_obsrvd))))/(t.total_events * avg(bnchmrk_obsrvd))) + (SQUARE(((total_events - total_yes ) - (t.total_events - (t.total_events * avg(bnchmrk_obsrvd)))))/(t.total_events - (t.total_events * avg(bnchmrk_obsrvd))))) END as chisquare_avg, avg(bnchmrk_obsrvd) as bnchmrk_expctd INTO #BP_DATA_TOTALS_TMP FROM t GROUP BY t.MEASURE_DESC,t.total_events, t.total_yes, t.bnchmrk_obsrvd SELECT tmp.MEASURE_DESC, tmp.TAXID,CONVERT(VARCHAR(6), ROUND(tmp.GROUP_RATE * 100,1)) + '%' as GROUP_RATE, tmp.GROUP_NUMERATOR, tmp.GROUP_DENOMINATOR, tmp.RESULT, tmp.Measure_Volume FROM ( SELECT t1.MEASURE_DESC as MEASURE_NM_SORT, t1.MEASURE_DESC, t1.TAXID, t1.GROUP_RATE, t1.GROUP_NUMERATOR, t1.GROUP_DENOMINATOR, t1.RESULT As RESULT, t1.Measure_Volume as Measure_Volume, 1 as ROW_NUM FROM #BP_DATA_TMP t1  UNION ALL select distinct t1.MEASURE_DESC as MEASURE_NM_SORT, 'TOTAL' as MEASURE_DESC, NULL as TAXID, t2.bnchmrk_obsrvd as GROUP_RATE, t2.total_yes as GROUP_NUMERATOR, t2.total_events as GROUP_DENOMINATOR, case when (t2.bnchmrk_obsrvd > t2.bnchmrk_expctd) and (t2.chisquare_avg > 3.841) then 'NOT MET' ELSE 'MET' END as RESULT, CASE WHEN t2.total_events >= 20 THEN 'SUFFICIENT' ELSE 'INSUFFICIENT' END as MEASURE_VOLUME, 2 as ROW_NUM FROM #BP_DATA_TMP t1 INNER JOIN #BP_DATA_TOTALS_TMP t2 ON t1.MEASURE_DESC = t2.MEASURE_DESC) as tmp ORDER BY tmp.MEASURE_NM_SORT, tmp.ROW_NUM";
            dt = DBConnection32.getMSSQLDataTable(strConnectionString, strSQL);
            pointColumnWidths = new float[] { 6F, 1F, 1F, 1F, 1F, 1F, 1F };
            PDF_Helper.addTable(dt, pointColumnWidths);


            PDF_Helper.finalizerPDF();


            return;















            //// By default column width is calculated automatically for the best fit.
            //// useAllAvailableWidth() method makes table use the whole page's width while placing the content.
            //Table table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
            //table.AddCell("Taxid");
            //table.AddCell("GROUP_NAME");
            //table.AddCell("Reporting_Period");
            //table.AddCell("BUNDLE_TYPE");
            //table.AddCell("REPORT_DATE");
            ////table.SetHeaderRows(1);


            //table.SetSkipFirstHeader(true);
            //List<List<string>> dataset = GetData();
            //foreach (List<string> record in dataset)
            //{
            //    foreach (string field in record)
            //    {
            //        table.AddCell(new Cell().Add(new Paragraph(field)));
            //    }
            //}

            //doc.Add(table);

            //doc.Close();






            //String logoPath = "C:\\iText\\logos\\itext_logo_recent.png";

            //pdfDocument.AddNewPage();
            //document.Add(new AreaBreak(AreaBreakType.LAST_PAGE));

            //int logoNumber = pdfDocument.GetNumberOfPages();

            //String description = "iText logo version: " + logoNumber;
            //document.Add(new Paragraph(description));
            //document.Add(new Image(ImageDataFactory.Create(logoPath)));

            //PdfFileSpec fileSpec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, logoPath, "logo_v" + logoNumber + ".png", PdfName.ApplicationOctetStream);
            //pdfDocument.AddAssociatedFile(description, fileSpec);
            //pdfDocument.GetPage(logoNumber).AddAssociatedFile(fileSpec);

            //document.Close();












            //iText.Kernel.Pdf.PdfDocument pdfDoc = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfWriter(@"C:\Users\cgiorda\Documents\itextpdf_text.pdf"));
            //Document doc = new Document(pdfDoc);
            //// table with 2 columns:
            //PdfPTable table = new PdfPTable(5);
            //// header row:
            //table.AddCell("Taxid");
            //table.AddCell("GROUP_NAME");
            //table.AddCell("Reporting_Period");
            //table.AddCell("BUNDLE_TYPE");
            //table.AddCell("REPORT_DATE");
            //table.HeaderRows = 1;
            //table.SkipFirstHeader = true;


            //doc.Add(table);
            // many data rows:
            //for (int i = 1; i


            strSQL = "SELECT t1.MEASURE_DESC, t1.Taxid, t1.MEASURE_BEGIN_DATE, t1.MEASURE_END_DATE, t1.NATIONAL_RATE as BASELINE_CONTRACT_RATE  FROM dbo.BP_DATA t1  WHERE ('-9999' = '-9999' OR t1.BUNDLE_TYPE = '-9999' ) AND  t1.REPORT_TYPE = 'Contract' AND REPORT_DATE ='6/11/2020'  AND t1.Taxid in (581661116, 431696710, 475021191, 222159534, 222159534) AND Convert(DateTime, Convert(VarChar, t1.MEASURE_BEGIN_DATE, 101)) >= '10/1/2019' AND Convert(DateTime, Convert(VarChar, t1.MEASURE_END_DATE, 101)) <= '12/31/2019' ";
            dt = DBConnection32.getMSSQLDataTable(strConnectionString, strSQL);

            

            strSQL = "IF OBJECT_ID('tempdb..#BP_DATA_TMP') IS NOT NULL DROP TABLE #BP_DATA_TMP IF OBJECT_ID('tempdb..#BP_DATA_TOTALS_TMP') IS NOT NULL DROP TABLE #BP_DATA_TOTALS_TMP SELECT t1.MEASURE_DESC, t1.TAXID, t1.GROUP_RATE, t1.GROUP_NUMERATOR, t1.GROUP_DENOMINATOR, t1.RESULT AS QUAL_RESULT, t1.Measure_Volume, t1.National_Rate, t1.RESULT INTO #BP_DATA_TMP FROM dbo.BP_DATA t1 WHERE  ('-9999' = '-9999' OR t1.BUNDLE_TYPE = '-9999' ) AND  t1.REPORT_TYPE = 'Contract' AND REPORT_DATE ='6/11/2020'  AND t1.Taxid in (581661116, 431696710, 475021191, 222159534, 222159534) AND Convert(DateTime, Convert(VarChar, t1.MEASURE_BEGIN_DATE, 101)) >= '10/1/2019' AND Convert(DateTime, Convert(VarChar, t1.MEASURE_END_DATE, 101)) <= '12/31/2019'; WITH t AS ( SELECT MEASURE_DESC, sum(GROUP_DENOMINATOR) as total_events, sum(GROUP_NUMERATOR) as total_yes, SUM(GROUP_NUMERATOR)/SUM(GROUP_DENOMINATOR) as bnchmrk_obsrvd FROM #BP_DATA_TMP GROUP BY MEASURE_DESC ) SELECT t.MEASURE_DESC, t.total_events, t.total_yes, t.bnchmrk_obsrvd, total_events - total_yes as total_no, t.total_events * avg(bnchmrk_obsrvd) as expected_yes, t.total_events - (t.total_events * avg(bnchmrk_obsrvd)) as expected_no, CASE WHEN (t.total_events * avg(bnchmrk_obsrvd)) = 0 OR (t.total_events - (t.total_events * avg(bnchmrk_obsrvd))) = 0 THEN -9999 ELSE ((SQUARE((total_yes - (t.total_events * avg(bnchmrk_obsrvd))))/(t.total_events * avg(bnchmrk_obsrvd))) + (SQUARE(((total_events - total_yes ) - (t.total_events - (t.total_events * avg(bnchmrk_obsrvd)))))/(t.total_events - (t.total_events * avg(bnchmrk_obsrvd))))) END as chisquare_avg, avg(bnchmrk_obsrvd) as bnchmrk_expctd INTO #BP_DATA_TOTALS_TMP FROM t GROUP BY t.MEASURE_DESC,t.total_events, t.total_yes, t.bnchmrk_obsrvd SELECT tmp.MEASURE_DESC, tmp.TAXID, tmp.GROUP_RATE, tmp.GROUP_NUMERATOR, tmp.GROUP_DENOMINATOR, tmp.RESULT, tmp.Measure_Volume FROM ( SELECT t1.MEASURE_DESC as MEASURE_NM_SORT, t1.MEASURE_DESC, t1.TAXID, t1.GROUP_RATE, t1.GROUP_NUMERATOR, t1.GROUP_DENOMINATOR, t1.RESULT As RESULT, t1.Measure_Volume as Measure_Volume, 1 as ROW_NUM FROM #BP_DATA_TMP t1  UNION ALL select distinct t1.MEASURE_DESC as MEASURE_NM_SORT, 'TOTAL' as MEASURE_DESC, NULL as TAXID, t2.bnchmrk_obsrvd as GROUP_RATE, t2.total_yes as GROUP_NUMERATOR, t2.total_events as GROUP_DENOMINATOR, case when (t2.bnchmrk_obsrvd > t2.bnchmrk_expctd) and (t2.chisquare_avg > 3.841) then 'NOT MET' ELSE 'MET' END as RESULT, CASE WHEN t2.total_events >= 20 THEN 'SUFFICIENT' ELSE 'INSUFFICIENT' END as MEASURE_VOLUME, 2 as ROW_NUM FROM #BP_DATA_TMP t1 INNER JOIN #BP_DATA_TOTALS_TMP t2 ON t1.MEASURE_DESC = t2.MEASURE_DESC) as tmp ORDER BY tmp.MEASURE_NM_SORT, tmp.ROW_NUM";
            dt = DBConnection32.getMSSQLDataTable(strConnectionString, strSQL);


        }
    }
}
