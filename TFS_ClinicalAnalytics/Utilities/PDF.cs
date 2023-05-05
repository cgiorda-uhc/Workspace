using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Tesseract;

namespace Utilities
{


    public class PDF
    {

        public static string GetTextFromAllPages(String pdfPath, string outputFilePrefix, string outputFolder, bool overwriteExistingFiles)
        {

            //CSG 'ORC' ???  via iText.PDF
            PdfReader reader = new PdfReader(pdfPath);
            StringWriter output = new StringWriter();
            for (int i = 1; i <= reader.NumberOfPages; i++)
                output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy()));


            string strText = output.ToString();



            //CSG ORC TESSERECT AND iText.PDF
            if (strText.Length < 1000)
            {
                var strFileName = System.IO.Path.GetFileNameWithoutExtension(pdfPath);

                var strNewImagePath = outputFolder + "\\" + strFileName;

                if (Directory.Exists(strNewImagePath))
                    Directory.Delete(strNewImagePath, true);

                Directory.CreateDirectory(strNewImagePath);
                //EXTRACT IMAGES VIA iText.PDF
                ImageExtractor.ExtractImagesFromFile(pdfPath, outputFilePrefix, strNewImagePath, overwriteExistingFiles);

                //READ IMAGES VIA TESSERECT
                var sb = new StringBuilder();
                foreach (string f in Directory.GetFiles(strNewImagePath, "*.png", SearchOption.TopDirectoryOnly))
                {


                    using (TesseractEngine engine = new TesseractEngine(@".\tessdata", "eng", EngineMode.Default))
                    {
                        using (Pix img = Pix.LoadFromFile(f))
                        {
                            // Preprocessing here if needed

                            using (Page page = engine.Process(img))
                            {
                                //GetPageOrientation here
                                using (var pageIter = page.GetIterator())
                                {
                                    // Need to call Begin to init iterator point to first block.
                                    pageIter.Begin();

                                    var pageProps = pageIter.GetProperties();
                                    // TODO: Rotate image based on pageProps.Orientation

                                    // TODO: Deskew image or perform any other post processing
                                }


                                sb.Append(page.GetText().Replace("\n", "\r\n"));


                            }
                        }
                    }

                }



                strText = sb.ToString();


            }

            if(1==2)
            {
                //IRON ORC SOULTION
                //IRON ORC SOULTION
                //IRON ORC SOULTION
                //DateTime startTime;
                //DateTime endTime;
                //TimeSpan span;
                //startTime = DateTime.Now;
                //////var Ocr = new IronOcr.AutoOcr();
                //var Ocr = new IronOcr.AdvancedOcr()
                //{
                //    CleanBackgroundNoise = true,
                //    EnhanceContrast = true,
                //    EnhanceResolution = true,
                //    Language = IronOcr.Languages.English.OcrLanguagePack,
                //    Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
                //    ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                //    DetectWhiteTextOnDarkBackgrounds = true,
                //    InputImageType = AdvancedOcr.InputTypes.AutoDetect,
                //    RotateAndStraighten = true,
                //    ReadBarCodes = true,
                //    ColorDepth = 4
                //};


                //var Results = Ocr.ReadPdf(strPDFPath);

                //endTime = DateTime.Now;
                //span = endTime.Subtract(startTime);
                //Console.WriteLine("Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));


                //strText = Results.Text.ToLower();
            }
            

            return strText.ToLower();


        }
    }






    /// <summary>
    /// Helper lass to dump all images from a PDF into separate files
    /// </summary>
    internal class ImageExtractor : IRenderListener
    {
        int _currentPage = 1;
        int _imageCount = 0;
        readonly string _outputFilePrefix;
        readonly string _outputFolder;
        readonly bool _overwriteExistingFiles;

        private ImageExtractor(string outputFilePrefix, string outputFolder, bool overwriteExistingFiles)
        {
            _outputFilePrefix = outputFilePrefix;
            _outputFolder = outputFolder;
            _overwriteExistingFiles = overwriteExistingFiles;
        }

        /// <summary>
        /// Extract all images from a PDF file
        /// </summary>
        /// <param name="pdfPath">Full path and file name of PDF file</param>
        /// <param name="outputFilePrefix">Basic name of exported files. If null then uses same name as PDF file.</param>
        /// <param name="outputFolder">Where to save images. If null or empty then uses same folder as PDF file.</param>
        /// <param name="overwriteExistingFiles">True to overwrite existing image files, false to skip past them</param>
        /// <returns>Count of number of images extracted.</returns>
        public static int ExtractImagesFromFile(string pdfPath, string outputFilePrefix, string outputFolder, bool overwriteExistingFiles)
        {
            // Handle setting of any default values
            outputFilePrefix = outputFilePrefix ?? System.IO.Path.GetFileNameWithoutExtension(pdfPath);
            outputFolder = String.IsNullOrEmpty(outputFolder) ? System.IO.Path.GetDirectoryName(pdfPath) : outputFolder;

            var instance = new ImageExtractor(outputFilePrefix, outputFolder, overwriteExistingFiles);

            using (var pdfReader = new PdfReader(pdfPath))
            {
                if (pdfReader.IsEncrypted())
                    throw new ApplicationException(pdfPath + " is encrypted.");

                var pdfParser = new PdfReaderContentParser(pdfReader);

                while (instance._currentPage <= pdfReader.NumberOfPages)
                {
                    //CHRIS ADDED IN RESPONSE TO 'The color depth 1 is not supported'
                    try
                    {
                        pdfParser.ProcessContent(instance._currentPage, instance);
                    }
                    catch
                    {

                    }


                    instance._currentPage++;
                }
            }

            return instance._imageCount;
        }

        #region Implementation of IRenderListener

        public void BeginTextBlock() { }
        public void EndTextBlock() { }
        public void RenderText(TextRenderInfo renderInfo) { }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            var imageObject = renderInfo.GetImage();

            var imageFileName = String.Format("{0}_{1}_{2}.{3}", _outputFilePrefix, _currentPage, _imageCount, imageObject.GetFileType());
            var imagePath = System.IO.Path.Combine(_outputFolder, imageFileName);

            if (_overwriteExistingFiles || !File.Exists(imagePath))
            {
                var imageRawBytes = imageObject.GetImageAsBytes();

                File.WriteAllBytes(imagePath, imageRawBytes);

            }

            // Subtle: Always increment even if file is not written. This ensures consistency should only some
            //   of a PDF file's images actually exist.
            _imageCount++;
        }

        #endregion // Implementation of IRenderListener


    }

}