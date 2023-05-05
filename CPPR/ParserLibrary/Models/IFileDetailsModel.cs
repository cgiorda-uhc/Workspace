namespace ProjectManagerLibrary.Models
{
    public interface IFileDetailsModel
    {
        public string report_type { get; set; }
        public int file_month { get; set; }
        public int file_year { get; set; }
        public DateTime file_date { get; set; }
        public string sheet_name { get; set; }
        public string file_name { get; set; }
        public string file_path { get; set; }

    }
}