using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using System;
using iText.IO.Image;
using System.Data;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Layout;

namespace PDF_iText7
{
    class PDF_Helper
    {

        public static Color colorTableHeaderBackground = ColorConstants.BLUE;
        public static Color colorTableHeaderForeground = ColorConstants.WHITE;


        private static Document documentGlobal;
        private static PdfDocument pdfDocGlobal;
        private static string strPDFPathGlobal;

        public static void initializePDF(string strPDFPath)
        {
            strPDFPathGlobal = strPDFPath;
            PdfWriter writer = new PdfWriter(strPDFPathGlobal, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            //PdfDocument pdfDoc = new PdfDocument(new PdfWriter(OUTPUT_FILE, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocGlobal = new PdfDocument(writer);
            documentGlobal = new Document(pdfDocGlobal);
            documentGlobal.SetMargins(0, 0, 0, 0);
        }


        public static void addBlankLine()
        {
            documentGlobal.Add(new iText.Layout.Element.Paragraph("\n"));
        }


        public static void addImage(string strImagePath)
        {
            ImageData data = ImageDataFactory.Create(strImagePath);
            Image img = new Image(data);
            img.ScaleAbsolute(150f, 30f);
            documentGlobal.Add(img);
        }

        public static void addTable(DataTable dt, float[] pointColumnWidths)
        {
            Table tablePDF = null;
            Cell cellPDF = null;
            Int16 intCellCnt = 1;
            tablePDF = new Table(pointColumnWidths);
            tablePDF.SetMarginTop(0);
            tablePDF.SetMarginBottom(0);

            foreach (DataRow dr in dt.Rows)
            {
                if (tablePDF.GetNumberOfRows() == 0)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        cellPDF = new Cell(1, 1).Add(new iText.Layout.Element.Paragraph(column.ColumnName.ToString().ToUpper().Replace("_", " ").Trim()));
                        cellPDF.SetTextAlignment(TextAlignment.CENTER);
                        cellPDF.SetPadding(5);
                        cellPDF.SetBackgroundColor(colorTableHeaderBackground);
                        cellPDF.SetFontColor(colorTableHeaderForeground);
                        tablePDF.AddHeaderCell(cellPDF);
                        intCellCnt++;
                    }
                }


                foreach (DataColumn column in dt.Columns)
                {
                    var varValue = (!object.ReferenceEquals(dr[column.ColumnName.ToString()], DBNull.Value) ? dr[column.ColumnName.ToString()].ToString().Trim() : "");
                    cellPDF = new Cell(1, 1).Add(new iText.Layout.Element.Paragraph(varValue.ToString()));
                    cellPDF.SetPadding(5);
                    if (int.TryParse(varValue.ToString().Replace("%", "").Replace(".", ""), out _))
                        cellPDF.SetTextAlignment(TextAlignment.CENTER);
                    else
                        cellPDF.SetTextAlignment(TextAlignment.LEFT);


                    if (dr[0].ToString().ToLower().Trim().Equals("total"))
                        cellPDF.SetBold();

                    tablePDF.AddCell(cellPDF);
                }

            }
            // Fictitiously layout the element to find out, how much space it takes
            IRenderer tableRenderer = tablePDF.CreateRendererSubTree().SetParent(documentGlobal.GetRenderer());
            LayoutResult tableLayoutResult = tableRenderer.Layout(new LayoutContext(
                    new LayoutArea(0, new iText.Kernel.Geom.Rectangle(550 + 72, 1000))));

            pdfDocGlobal.SetDefaultPageSize(new iText.Kernel.Geom.PageSize(550 + 72,
                    tableLayoutResult.GetOccupiedArea().GetBBox().GetHeight() + 72));



            documentGlobal.Add(tablePDF);
        }




        public static void finalizerPDF(bool blShow = true)
        {
            documentGlobal.Close();
            if(blShow)
                System.Diagnostics.Process.Start(strPDFPathGlobal);
        }




    }
}
