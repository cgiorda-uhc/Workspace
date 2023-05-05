using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileParsingLibrary.WMSWord;

public class DocXWordFunctions
{
    //https://stackoverflow.com/questions/51031008/c-insert-and-indent-bullet-points-at-bookmark-in-word-document-using-office-in
    //        using System.Collections.Specialized;
    //...
    //...

    //DocX doc = DocX.Create("bullet-text.docx");

    //    var firstItem = bulletList[0];
    //    var firstItemLevel = firstItem.ToList().Count(c => c == '\\');
    //    // Using full Namespace to avoid ambiguous reference error.
    //    Xceed.Words.NET.List list = doc.AddList(firstItem.Replace("\\", ""), firstItemLevel, ListItemType.Numbered);

    //for (var i = 1; i<count; i++)
    //{
    //    var currentItem = bulletList[i];
    //    var item = currentItem.Replace(@"\", "");
    //    int listLevel = currentItem.ToList().Count(c => c == '\\')

    //    doc.AddListItem(list, item, listLevel, ListItemType.Numbered);

    //}

    //doc.InsertList(list);

    //doc.Save();

    //// Collapse the range to the end, as to not overwrite it. Unsure if you need this
    //range.Collapse(WdCollapseDirection.wdCollapseEnd);

    //// Insert into the selected range
    //range.InsertFile(Environment.CurrentDirectory + "\\bullet-text.docx");


}
