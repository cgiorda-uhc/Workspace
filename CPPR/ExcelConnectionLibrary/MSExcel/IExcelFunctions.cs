using FileParsingLibrary.Models;

namespace FileParsingLibrary.MSExcel
{
    public interface IExcelFunctions
    {
        List<KeyValuePair<string, string>> Mappings { get; set; }

        Task<byte[]> ExportToExcelAsync<T>(List<T> list, string worksheetTitle,  Func<string> getterStatus, Action<string> setterStatus);

        Task<byte[]> ExportToExcelAsync(List<ExcelExport> excelExports);


        Task<byte[]> ExportToExcelAsync(List<ExcelExport> excelExports, Func<string> getterStatus, Action<string> setterStatus);
        //Task<byte[]> ExportToExcelAsync<T>(List<ExcelExport<T>> excelExports);

        byte[] ExportToExcel<T>(List<T> list, string worksheetTitle, List<string[]> titles);

        object GetValueFromExcel(string fileName, string sheetName, string cell);
        List<T> ImportExcel<T>(string fileName, string sheetName, string columnHeaderRange, int startingRow, string nullCheck = null);
    }
}