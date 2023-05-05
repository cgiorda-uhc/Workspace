using GhostscriptSharp;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Pdf.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class ORC_CSG
    {

        public static void test(string strPath, string outputFilePrefix, string outputFolder, bool overwriteExistingFiles)
        {
            string strNewImagePath = null;
            string strText = null;
            string strFileName = null;
            string strTextFilePath = null; 

            foreach (string f in Directory.GetFiles(strPath, "*.pdf", SearchOption.TopDirectoryOnly))
            {
                strFileName = System.IO.Path.GetFileNameWithoutExtension(f);

                strNewImagePath = outputFolder + "\\" + strFileName;


                if(!Directory.Exists(strNewImagePath))
                {
                    Directory.CreateDirectory(strNewImagePath);
                }


                ImageExtractor.ExtractImagesFromFile(f, outputFilePrefix, strNewImagePath, overwriteExistingFiles);


                strText = TextExtractor.GetTextFromAllPages(f, null, strNewImagePath, true);
                strTextFilePath = strNewImagePath + "\\" + strFileName + ".txt";
                if (!File.Exists(strTextFilePath))
                {
                    //File.Delete(path);

                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(strTextFilePath))
                    {
                        sw.Write(strText);
                    }

                }

            }
        }



        public static string getTextFromPDF(string strPath,  string outputFilePrefix, string outputFolder, bool overwriteExistingFiles)
        {

                return TextExtractor.GetTextFromAllPages(strPath, outputFilePrefix, outputFolder, overwriteExistingFiles);

        }



        //private static int ExtractImagesFromFile(string pdfFileName, string outputFilePrefix, string outputDirectory, bool overwriteExistingImages)
        //{
        //    return ImageExtractor.ExtractImagesFromFile(pdfFileName, outputFilePrefix, outputDirectory, overwriteExistingImages);
        //}





        //private readonly string MULTIPLE_FILE_LOCATION = "output%d.jpg";
        //private void ConvertPDFToBitmap(string PDF, int StartPageNum, int EndPageNum)
        //{
        //    GhostscriptWrapper.GeneratePageThumbs(@"C:\Users\User\Downloads\English_Medium_Extra_for_WEB-2.pdf",
        //                "Example.png", 1, 3, 130, 130);
        //}

    }


}
