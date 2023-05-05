namespace VCPortal_Models.Models.ActiveDirectory;

public interface IADConfig
{
    string LDAPDomain { get; set; }
    string LDAPPath { get; set; }
    string LDAPPW { get; set; }
    string LDAPUser { get; set; }
    string Name { get; set; }
    string SearchString { get; set; }
}