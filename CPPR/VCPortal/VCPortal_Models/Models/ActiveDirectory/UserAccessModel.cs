using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ActiveDirectory;
public class UserAccessModel
{
    public String FirstName { get; set; }
    public String MiddleName { get; set; }

    public String LastName { get; set; }

    public String FullName { get; set; }

    public String LoginName { get; set; }


    public String EmailAddress { get; set; }

    public List<String> Groups { get; set; }

}
