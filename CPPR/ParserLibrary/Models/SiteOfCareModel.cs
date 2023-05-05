using NPOI.SS.Formula.PTG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models;

public class SiteOfCareModel
{

    public string SUBCARRIER { get; set; }
    public string ENCOUNTERID { get; set; }
    public DateTime? ENCOUNTERDATEKEY { get; set; }
    public DateTime? ENCOUNTERFINALCLOSEDDATEKEY { get; set; }
    public Int16? PROCEDURESEQUENCENUM { get; set; }
    public string ENCOUNTERCLINICALSTATUSLEVEL1 { get; set; }
    public string PROCEDURECODE { get; set; }
    public string PROCEDURESTATUSDESC { get; set; }
    public Int16? REQUESTEDUNITS { get; set; }
    public Int16? APPROVEDUNITS { get; set; }
    public char? SOCENABLEDCPTYN { get; set; }
    public string SOCOVERTURNDESC { get; set; }
    public char? SOCMEMBERINSCOPE { get; set; }
    public string SOCCATEGORY { get; set; }
    public string SOCINITIALFACTYPE { get; set; }
    public string SOCFINALFACTYPE { get; set; }
    public string SOCDECISION { get; set; }
    public string SOCWATERFALLCAT { get; set; }
    public string SOCRESUBMITENCOUNTERID { get; set; }
    public string SOCATTESTATION { get; set; }
    public string SOCAPPROVALREASON { get; set; }
    public string REFERRINGPROVIDERTIN { get; set; }
    public string REFERRINGPROVIDERNPI { get; set; }
    public string REFERRINGPROVIDERFULLNAME { get; set; }
    public string REFERRINGPROVIDERZIPCODE { get; set; }
    public string REFERRINGPROVIDERSTATE { get; set; }
    public string PATIENTCARRIERMEMBERID { get; set; }
    public string INITIALREQUESTEDPROVIDERTIN { get; set; }
    public string INITIALREQUESTEDPROVIDERFULLNAME { get; set; }
    public string INITIALREQUESTEDPROVIDERSTATE { get; set; }
    public string INITIALREQUESTEDPROVIDERZIPCODE { get; set; }
    public string FINALREQUESTEDPROVIDERTIN { get; set; }
    public string FINALREQUESTEDPROVIDERFULLNAME { get; set; }
    public string FINALREQUESTEDPROVIDERSTATE { get; set; }
    public string FINALREQUESTEDPROVIDERZIPCODE { get; set; }
    public string SOCWORKABLE { get; set; }
    public string ENCOUNTERSTANDARDPRODUCTLEVEL1 { get; set; }

    public string report_type { get; set; }
    public int file_month { get; set; }
    public int file_year { get; set; }
    public DateTime file_date { get; set; }
    public string sheet_name { get; set; }
    public string file_name { get; set; }
    public string file_path { get; set; }



}
