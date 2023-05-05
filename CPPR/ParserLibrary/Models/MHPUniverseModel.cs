

namespace ProjectManagerLibrary.Models;

public class MHPUniverseModel
{
    public string State_of_Issue { get; set; }
    public string State_of_Residence{ get; set; }
    public string Enrollee_First_Name{ get; set; }
    public string Enrollee_Last_Name{ get; set; }
    public string Cardholder_ID{ get; set; }
    public string Funding_Arrangement{ get; set; }
    public string Authorization{ get; set; }
    public string Authorization_Type{ get; set; }
    public DateTime Request_Date { get; set; }
    public TimeSpan Request_Time { get; set; }
    public string Request_Decision{ get; set; }
    public DateTime Decision_Date { get; set; }
    public TimeSpan Decision_Time { get; set; }
    public string Decision_Reason{ get; set; }
    public bool Extension_Taken{ get; set; }
    public DateTime Member_Notif_Extension_Date{ get; set; }
    public DateTime Additional_Info_Date { get; set; }
    public DateTime Oral_Notification_Enrollee_Date { get; set; }
    public TimeSpan Oral_Notification_Enrollee_Time { get; set; }
    public DateTime Oral_Notification_Provider_Date { get; set; }
    public TimeSpan Oral_Notification_Provider_Time { get; set; }
    public DateTime Written_Notification_Enrollee_Date { get; set; }
    public TimeSpan Written_Notification_Enrollee_Time { get; set; }
    public DateTime Written_Notification_Provider_Date { get; set; }
    public TimeSpan Written_Notification_Provider_Time { get; set; }
    public string Primary_Procedure_Code_Req{ get; set; }
    public string Procedure_Code_Description{ get; set; }
    public string Primary_Diagnosis_Code{ get; set; }
    public string Diagnosis_Code_Description{ get; set; }
    public Int16 Place_of_Service{ get; set; }
    public DateTime Member_Date_of_Birth { get; set; }
    public bool Urgent_Processed_Standard{ get; set; }
    public DateTime Request_Additional_Info_Date { get; set; }
    public string FirstTier_Downstream_RelatedEntity{ get; set; }
    public string Par_NonPar_Site{ get; set; }
    public string Inpatient_Outpatient{ get; set; }
    public int Delegate_Number{ get; set; }
    public string ProgramType{ get; set; }
    public string Insurance_Carrier{ get; set; }
    public string Group_Number{ get; set; }
    public string Intake_Method{ get; set; }


    public int file_month { get; set; }
    public int file_year { get; set; }
    public DateTime file_date { get; set; }
    public string sheet_name { get; set; }
    public string file_name { get; set; }
    public string file_path { get; set; }


    public string classification { get; set; }
}
