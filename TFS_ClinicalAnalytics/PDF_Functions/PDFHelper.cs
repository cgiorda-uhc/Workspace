using System;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PDF_Functions
{
    public class PDFHelper
    {

        public static void mergePDF(string strSourcFilePath1, string strSourcFilePath2, string strDestinationFilePath, bool blDeleteOldCover = false)
        {
            using (PdfDocument one = PdfReader.Open(strSourcFilePath1, PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(strSourcFilePath2, PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf, false);
                CopyPages(two, outPdf, blDeleteOldCover );

                outPdf.Save(strDestinationFilePath);
            }

            
        }

        private static void CopyPages(PdfDocument from, PdfDocument to, bool blDeleteOldCover)
        {


            for (int i = 0; i < from.PageCount; i++)
            {
                if (blDeleteOldCover && i == 0)
                    continue;

                to.AddPage(from.Pages[i]);
            }
        }


    }
}
