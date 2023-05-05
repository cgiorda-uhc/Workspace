using System.Drawing;


namespace FileParsingLibrary.Models;

public class MSWordFormattedText
{
    public string Text { get; set; }
    public Color ForeColor { get; set; }
    public string FontType { get; set; }
    public float FontSize { get; set; }
    public bool Bold { get; set; }
}
