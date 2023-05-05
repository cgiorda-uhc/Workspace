using System.ComponentModel.DataAnnotations;

namespace VCWebPortal.API.Models
{
    public class VCWebPortalTest
    {
        [Key]
        public Guid id { get; set; }
        public string CardholderName { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public int CVC { get; set; }
    }
}
