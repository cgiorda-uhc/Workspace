using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ActiveDirectory;
public class UserAccessConfig
{
    public List<string> AllowedUsers { get; set; }

    public string EncryptionKey { get; set; }

    public string IdentityURL { get; set; }

    public string DateFormat { get; set; }
}
