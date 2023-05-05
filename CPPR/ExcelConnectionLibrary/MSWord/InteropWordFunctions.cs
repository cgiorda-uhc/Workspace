

using FileParsingLibrary.Models;
using Microsoft.Office.Interop.Word;
using System.Linq;
using Range = Microsoft.Office.Interop.Word.Range;

namespace FileParsingLibrary.MSWord;

public class InteropWordFunctions
{
    private Application _app;
    private Document _doc;
    private object _missing = System.Reflection.Missing.Value;

    public InteropWordFunctions()
    {
        
        _app = new Application();
        _doc = _app.Documents.Add(ref _missing, ref _missing, ref _missing, ref _missing);

    }
    public InteropWordFunctions(string file)
    {
        _app = new Application();
        _doc = _app.Documents.Open(file);

    }


    public void FindAndReplace( object findText, object replaceWithText)
    {
        //options
        object matchCase = false;
        object matchWholeWord = true;
        object matchWildCards = false;
        object matchSoundsLike = false;
        object matchAllWordForms = false;
        object forward = true;
        object format = false;
        object matchKashida = false;
        object matchDiacritics = false;
        object matchAlefHamza = false;
        object matchControl = false;
        object read_only = false;
        object visible = true;
        object replace = 2;
        object wrap = 1;
        //execute find and replace
        _app.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
            ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
            ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);

    }


    public void FindAndReplaceInHeader(object findText, object replaceWithText)
    {
        //options
        object matchCase = false;
        object matchWholeWord = true;
        object matchWildCards = false;
        object matchSoundsLike = false;
        object matchAllWordForms = false;
        object forward = true;
        object format = false;
        object matchKashida = false;
        object matchDiacritics = false;
        object matchAlefHamza = false;
        object matchControl = false;
        object read_only = false;
        object visible = true;
        object replace = 2;
        object wrap = 1;

        Selection selection;
        foreach (Section section in _doc.Sections)
        {
            Microsoft.Office.Interop.Word.HeadersFooters headers = section.Headers;
            foreach (Microsoft.Office.Interop.Word.HeaderFooter header in headers)
            {
                header.Range.Select();
                selection = _doc.Application.Selection;

                //selection.Find.Text = findText.ToString();
                //selection.Find.Replacement.Text = replaceWithText.ToString();
                //selection.Find.Wrap = WdFindWrap.wdFindContinue;

                selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
            }
        }
    }


    public void addBulletedList(string bookmark, List<MSWordFormattedText> bullets, int level = 1)
    {
        

        Range range = _doc.Bookmarks.get_Item(bookmark).Range;
        ListGallery listGallery = _doc.Application.ListGalleries[WdListGalleryType.wdBulletGallery];
        Paragraph paragraph = range.Paragraphs.Add();
        ListFormat listFormat = paragraph.Range.ListFormat;
        //range.ListFormat.ListOutdent();
        

        for (int i = 0; i < bullets.Count;i++)
        {
            //TRANLASTE System.Drawing.Color TO Microsoft.Office.Interop.Word.WdColor
            var wordColor = (Microsoft.Office.Interop.Word.WdColor)(bullets[i].ForeColor.R + 0x100 * bullets[i].ForeColor.G + 0x10000 * bullets[i].ForeColor.B);


            range.Text = bullets[i].Text;

            //range.Font.Bold = (bullets[i].Bold == true ? 1 :0);
            //range.Font.Name = bullets[i].FontType;
            //range.Font.Size = bullets[i].FontSize;
            range.Font.Color = wordColor;
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            ApplyListTemplate(listGallery, listFormat, level);

            if(i != bullets.Count -1) //USED TO AVOID FINAL BLANK BULLET??????? REVISIT!!!
            {
                range.InsertParagraphAfter();
                range.ListFormat.ListOutdent();
                range.ListFormat.ListOutdent();
                range.Collapse(WdCollapseDirection.wdCollapseEnd);
            }







            //paragraph.Range.Text = bullet;
            //ApplyListTemplate(listGallery, listFormat, level);
            //paragraph.Range.InsertParagraphAfter();
            ////paragraph = paragraph.Range.Paragraphs.Add();

        }


    }
    private void ApplyListTemplate(ListGallery listGallery, ListFormat listFormat, int level = 1)
    {
        listFormat.ApplyListTemplateWithLevel(
            listGallery.ListTemplates[level],
        ContinuePreviousList: true,
            ApplyTo: WdListApplyTo.wdListApplyToSelection,
            DefaultListBehavior: WdDefaultListBehavior.wdWord10ListBehavior,
            ApplyLevel: level);
    }

    public void Save(string file)
    {
        _app.ActiveDocument.SaveAs2(file);
        DisposeWordInstance();
    }

    public void Save()
    {
        _app.ActiveDocument.Save();
        DisposeWordInstance();
    }
    public void DisposeWordInstance()
    {
        if (_doc != null)
            _doc.Close(ref _missing, ref _missing, ref _missing);
        if (_app != null)
            _app.Quit();
        if (_doc != null)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(_doc);
        if (_app != null)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(_app);
        _doc = null;
        _app = null;
        GC.Collect(); // force final cleanup!
    }
}
