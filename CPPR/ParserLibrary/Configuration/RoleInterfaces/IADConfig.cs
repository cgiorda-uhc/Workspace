

namespace ProjectManagerLibrary.Configuration.RoleInterfaces
{
    public interface IADConfig 
    {
        public string LDAPDomain { get; set; }
        public string LDAPPath { get; set; }
        public string LDAPUser { get; set; }
        public string LDAPPW { get; set; }

    }
}
