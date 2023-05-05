namespace ProjectManagerLibrary.Models
{
    public interface ICSScorecardModel
    {
        double ApprovalsPer1000 { get; set; }
        int Approved { get; set; }
        int Auto_Approved { get; set; }
        int Compliant_Routine_Cases { get; set; }
        int Compliant_Urgent_Cases { get; set; }
        int Denied { get; set; }
        int Expired { get; set; }
        int Fax { get; set; }
        DateTime file_date { get; set; }
       int file_month { get; set; }
        string file_name { get; set; }
        string file_path { get; set; }
        int file_year { get; set; }
        string ignore_reason { get; set; }
        bool is_ignored { get; set; }
        string Modality { get; set; }
        int Others { get; set; }
        int Phone { get; set; }
        string report_type { get; set; }
        double RequestsPer1000 { get; set; }
        int Routine_Cases { get; set; }
        string sheet_name { get; set; }
        string State { get; set; }
        int Urgent_Cases { get; set; }
        int Web { get; set; }
        int Withdrawn { get; set; }
    }
}