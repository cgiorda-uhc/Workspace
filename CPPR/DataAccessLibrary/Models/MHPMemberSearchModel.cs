using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models;

public class MHPMemberSearchModel
{
    public Int64 mhp_uni_id { get; set; }

    public string Cardholder_ID_CLN { get; set; }

    public string State_Of_Issue { get; set; }

    public string Member_Date_of_Birth { get; set; }

    public string Request_Date { get; set; }

    public string Enrollee_First_Name { get; set; }

    public string Enrollee_Last_Name { get; set; }

    public string sheet_name { get; set; }

}
