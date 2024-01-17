using DocumentFormat.OpenXml.Packaging;

using DocumentFormat.OpenXml;

using DocumentFormat.OpenXml.Wordprocessing;

namespace FileParsingLibrary.MSWord;

public class OpenXMLWordFunctions : IDisposable
{
    private MemoryStream _ms;
    private WordprocessingDocument _wordprocessingDocument;

    public OpenXMLWordFunctions(string file)
    {
        _ms = new MemoryStream();
        _wordprocessingDocument = WordprocessingDocument.Open(file, true);

    }
    public void AddParagraph(string sentence)
    {
        List<Run> runList = ListOfStringToRunList(new List<string> { sentence });
        AddParagraph(runList);
    }
    public void AddParagraph(List<string> sentences)
    {
        List<Run> runList = ListOfStringToRunList(sentences);
        AddParagraph(runList);
    }

    public void AddParagraph(List<Run> runList)
    {
        var para = new Paragraph();
        foreach (Run runItem in runList)
        {
            para.AppendChild(runItem);
        }

        Body body = _wordprocessingDocument.MainDocumentPart.Document.Body;
        body.AppendChild(para);
    }

    public void ReplaceText(string placeholder, string value)
    {
        var document = _wordprocessingDocument.MainDocumentPart.Document;
        foreach (var text in document.Descendants<Text>()) // <<< Here
        {
            if (text.Text.Contains(placeholder))
            {
                text.Text = text.Text.Replace(placeholder, value);
            }
        }
    }


    public void ReplaceBulletsXML(string placeholderouter, string placeholderinner, string xml)
    {
        var document = _wordprocessingDocument.MainDocumentPart.Document;

        var bookmark_name = (placeholderouter + "_" + placeholderinner).Replace("&", "").ToLower();

        //var bookmark = document.MainDocumentPart.RootElement.Descendants<BookmarkStart>().Where(x => x.Name == bookmark_name).FirstOrDefault();
        var bookmark = document.MainDocumentPart.RootElement.Descendants<BookmarkStart>().Where(x => x.Name == bookmark_name).FirstOrDefault();

        bookmark.InnerXml = xml;


    }


    public void ReplaceBulletsXMLOLD(string placeholderouter, string placeholderinner, string xml)
    {
        var document = _wordprocessingDocument.MainDocumentPart.Document;
        Body documentBody = document.MainDocumentPart.Document.Body;
        Table documentTable = documentBody.Descendants<Table>().Where(tbl => tbl.InnerText.Contains(placeholderouter)).FirstOrDefault();

        var f = documentTable.ChildElements.Where(tbl => tbl.InnerText.Contains(placeholderinner)).FirstOrDefault();         //foreach (var text in document.Descendants<Text>()) // <<< Here
        //{
        //    if (text.Text.Contains(placeholder))
        //    {
        //        var s = text.InnerXml;
        //    }
        //}
        f.InnerXml = xml ;


    }



    public void ReplaceBullets(string placeholderouter, string placeholderinner, List<string> selectedItems)
    {
        var document = _wordprocessingDocument.MainDocumentPart.Document;


        //Body documentBody = document.MainDocumentPart.Document.Body;
        //Table documentTable = documentBody.Descendants<Table>().Where(tbl => tbl.InnerText.Contains(placeholderouter)).FirstOrDefault();

        //var section = documentTable.ChildElements.Where(tbl => tbl.InnerText.Contains(placeholderinner)).FirstOrDefault();
        //var final = section.LastChild.ChildElements.Where(tbl => tbl.InnerText.Contains("empty")).FirstOrDefault();

        var bookmark_name = (placeholderouter + "_" + placeholderinner).Replace("&", "").ToLower();

        //var bookmark = document.MainDocumentPart.RootElement.Descendants<BookmarkStart>().Where(x => x.Name == bookmark_name).FirstOrDefault();
        var bookmarkStart = document.MainDocumentPart.RootElement.Descendants<BookmarkStart>().Where(x => x.Name == bookmark_name).FirstOrDefault();
        var bookmarkEnds = document.MainDocumentPart.RootElement.Descendants<BookmarkEnd>();

        var s = bookmarkStart.Elements<Paragraph>();



        //Get the id of the bookmark start to find the bookmark end
        var id = bookmarkStart.Id.Value;
                    var bookmarkEnd = bookmarkEnds.Where(i => i.Id.Value == id).First();

                    if (selectedItems.Count > 0)
                    {
                        foreach (string item in selectedItems)
                        {
                            //Adding the bulleted list dynamically 
                            Paragraph para = new Paragraph
                                (new ParagraphProperties(
                                    new NumberingProperties(
                                       new NumberingLevelReference() { Val = 1 },
                                       new NumberingId() { Val = 2 })),
                                       new Run(
                                        new RunProperties(),
                                        new Text(item) { Space = SpaceProcessingModeValues.Preserve }));
                             var runElement = new Run(para);
                            bookmarkStart.InsertAfter(runElement, bookmarkEnd);
                        }
                    }







        //if (selectedItems.Count > 0)
        //{


        //    foreach (string item in selectedItems)
        //    {
        //        //Adding the bulleted list dynamically 
        //        Paragraph para = new Paragraph
        //            (new ParagraphProperties(
        //                new NumberingProperties(
        //                   new NumberingLevelReference() { Val = 1 },
        //                   new NumberingId() { Val = 2 })),
        //                   new Run(
        //                    new RunProperties(),
        //                    new Text(item) { Space = SpaceProcessingModeValues.Preserve }));
        //        bookmark.AppendChild(para);
        //    }

        //}


        //IDictionary<String, BookmarkStart> bookmarkMap = new Dictionary<String, BookmarkStart>();

        //foreach (BookmarkStart bookmarkStart in document.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
        //{
        //    bookmarkMap[bookmarkStart.Name] = bookmarkStart;
        //}

        //foreach (BookmarkStart bookmarkStart in bookmarkMap.Values)
        //{
        //    Run bookmarkText = bookmarkStart.NextSibling<Run>();
        //    if (bookmarkText != null)
        //    {
        //        bookmarkText.GetFirstChild<Text>().Text = "blah";
        //    }
        //}








        //if (selectedItems.Count > 0)
        //{


        //    foreach (string item in selectedItems)
        //    {
        //        //Adding the bulleted list dynamically 
        //        Paragraph para = new Paragraph
        //            (new ParagraphProperties(
        //                new NumberingProperties(
        //                   new NumberingLevelReference() { Val = 1 },
        //                   new NumberingId() { Val = 2 })),
        //                   new Run(
        //                    new RunProperties(),
        //                    new Text(item) { Space = SpaceProcessingModeValues.Preserve }));
        //        bookmark.AppendChild(para);
        //    }

        //}



        //Run bookmarkText = bookmark.NextSibling<Run>();





        //IDictionary<String, BookmarkStart> bookmarkMap = new Dictionary<String, BookmarkStart>();

        //foreach (BookmarkStart bookmarkStart in document.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
        //{
        //    bookmarkMap[bookmarkStart.Name] = bookmarkStart;
        //}

        //foreach (BookmarkStart bookmarkStart in bookmarkMap.Values)
        //{
        //    Run bookmarkText = bookmarkStart.NextSibling<Run>();
        //    if (bookmarkText != null)
        //    {
        //        bookmarkText.GetFirstChild<Text>().Text = "blah";
        //    }
        //}

        //var bookmark = bookmarkMap.Where(x => x.Key == bookmark_name).FirstOrDefault();



        //Body documentBody = document.MainDocumentPart.Document.Body;
        //foreach (Table documentTable in documentBody.Descendants<Table>().
        // Where(tbl => tbl.InnerText.Contains(placeholder)))
        //{
        //    if (selectedItems.Count > 0)
        //    {
        //        var secondRow = documentTable.Descendants<TableRow>().Where(tr => tr.InnerText.
        //                  Contains(placeholder2)).FirstOrDefault();
        //        //TableRow newRow = new TableRow();
        //        //TableCell newcell = new TableCell();
        //        TableRow newRow = new TableRow();
        //        TableCell newcell = new TableCell();



        //        foreach (string item in selectedItems)
        //        {
        //            //Adding the bulleted list dynamically 
        //            Paragraph para = new Paragraph
        //                (new ParagraphProperties(
        //                    new NumberingProperties(
        //                       new NumberingLevelReference() { Val = 1 },
        //                       new NumberingId() { Val = 1 })),
        //                       new Run(
        //                        new RunProperties(),
        //                        new Text(item) { Space = SpaceProcessingModeValues.Preserve }));
        //            newcell.Append(para);
        //        }
        //        newRow.Append(newcell);
        //        secondRow.InsertAfterSelf(newRow);
        //    }
        //}

   // }




    //public void ReplaceBulletsOLD(string placeholderouter, string placeholderinner, List<string> selectedItems)
    //{
    //    var document = _wordprocessingDocument.MainDocumentPart.Document;



    //    Body documentBody = document.MainDocumentPart.Document.Body;
    //    foreach (Table documentTable in documentBody.Descendants<Table>().
    //     Where(tbl => tbl.InnerText.Contains(placeholderouter)))
    //    {
    //        if (selectedItems.Count > 0)
    //        {
    //            var secondRow = documentTable.Descendants<TableRow>().Where(tr => tr.InnerText.
    //                      Contains(placeholderinner)).FirstOrDefault();
    //            //TableRow newRow = new TableRow();
    //            //TableCell newcell = new TableCell();
    //            TableRow newRow = new TableRow();
    //            TableCell newcell = new TableCell();



    //            foreach (string item in selectedItems)
    //            {
    //                //Adding the bulleted list dynamically 
    //                Paragraph para = new Paragraph
    //                    (new ParagraphProperties(
    //                        new NumberingProperties(
    //                           new NumberingLevelReference() { Val = 2 },
    //                           new NumberingId() { Val = 1 })),
    //                           new Run(
    //                            new RunProperties(),
    //                            new Text(item) { Space = SpaceProcessingModeValues.Preserve }));
    //                newcell.Append(para);
    //            }
    //            secondRow.Append(newcell);
    //            //secondRow.InsertAfterSelf(newRow);
    //        }
    //    }

    }


    public void Dispose()
    {
        CloseAndDisposeOfDocument();
        if (_ms != null)
        {
            _ms.Dispose();
            _ms = null;
        }
    }

    public MemoryStream SaveToStream()
    {
        _ms.Position = 0;
        return _ms;
    }

    public void SaveToFile(string fileName)
    {
        _wordprocessingDocument.MainDocumentPart.Document.Save();
        if (_wordprocessingDocument != null)
        {
            CloseAndDisposeOfDocument();
        }

    }

    private void CloseAndDisposeOfDocument()
    {
        if (_wordprocessingDocument != null)
        {
            //_wordprocessingDocument.Close();
            _wordprocessingDocument.Dispose();
            _wordprocessingDocument = null;
        }
    }

    private static List<Run> ListOfStringToRunList(List<string> sentences)
    {
        var runList = new List<Run>();
        foreach (string item in sentences)
        {
            var newRun = new Run();
            newRun.AppendChild(new Text(item));
            runList.Add(newRun);
        }

        return runList;
    }
}
//http://swatishrimali.blogspot.com/2014/06/adding-numbered-list-in-existing-table.html
//private string GenerateDocument(string sourceFile, string destinationFile,
//bool isRowDelete, List<string> selectedItems)
//{
//    System.IO.File.Copy(sourceFile, destinationFile, true);

//    using (var document = WordprocessingDocument.Open(destinationFile, true))
//    {
//        string documentText = null;
//        using (StreamReader sr = new StreamReader(document.MainDocumentPart.GetStream()))
//        {
//            documentText = sr.ReadToEnd();
//        }

//        Body documentBody = document.MainDocumentPart.Document.Body;
//        foreach (Table documentTable in documentBody.Descendants<Table>().
//         Where(tbl => tbl.InnerText.Contains("UniqueTableName")))
//        {
//            if (selectedItems.Count > 0)
//            {
//                var secondRow = documentTable.Descendants<TableRow>().Where(tr => tr.InnerText.
//                          Contains("TwoPlaceHolder")).FirstOrDefault();
//                TableRow newRow = new TableRow();
//                TableCell newcell = new TableCell();
//                foreach (string item in selectedItems)
//                {
//                    //Adding the bulleted list dynamically 
//                    Paragraph para = new Paragraph
//                        (new ParagraphProperties(
//                            new NumberingProperties(
//                               new NumberingLevelReference() { Val = 1 },
//                               new NumberingId() { Val = 2 })),
//                               new Run(
//                                new RunProperties(),
//                                new Text(item) { Space = SpaceProcessingModeValues.Preserve }));
//                    newcell.Append(para);
//                }
//                newRow.Append(newcell);
//                secondRow.InsertAfterSelf(newRow);
//            }
//            if (isRowDelete)
//            {
//                //removing the row
//                var rowForDelete = documentTable.Descendants<TableRow>().Where(tr => tr.InnerText.
//                     Contains("RowToRemove")).FirstOrDefault();
//                rowForDelete.Remove();
//            }
//        }
//        Regex regexTablename = new Regex("UniqueTableName");
//        documentText = regexTablename.Replace(documentText, HttpUtility.HtmlEncode("Edited Bullet List"));
//        using (StreamWriter sw = new StreamWriter(document.MainDocumentPart.GetStream(FileMode.Create)))
//        {
//            sw.Write(documentText);
//        }

//    }
//    return destinationFile;
//}