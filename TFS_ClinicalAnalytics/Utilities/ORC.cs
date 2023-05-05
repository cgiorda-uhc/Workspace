using IronOcr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class ORC
    {
        public string PDFtoText(string strPDFPath)
        {

            DateTime startTime;
            DateTime endTime;
            TimeSpan span;

            startTime = DateTime.Now;

            ////var Ocr = new IronOcr.AutoOcr();
            var Ocr = new IronOcr.AdvancedOcr()
            {
                CleanBackgroundNoise = true,
                EnhanceContrast = true,
                EnhanceResolution = true,
                Language = IronOcr.Languages.English.OcrLanguagePack,
                Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
                ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                DetectWhiteTextOnDarkBackgrounds = true,
                InputImageType = AdvancedOcr.InputTypes.AutoDetect,
                RotateAndStraighten = true,
                ReadBarCodes = true,
                ColorDepth = 4
            };


            var Results = Ocr.ReadPdf(strPDFPath);

            endTime = DateTime.Now;
            span = endTime.Subtract(startTime);
            Console.WriteLine("Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));


           return Results.Text.ToLower();
      
        }
    }
}
