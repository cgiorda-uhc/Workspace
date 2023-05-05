using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VCCommandAPI.Models
{
    public class Command
    {
        [Key]
        [Column(TypeName = "INT")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "VARCHAR")]
        public string HowTo { get; set; }
        [Required]
        [MaxLength(255)]
        [Column(TypeName = "VARCHAR")]
        public string Line { get; set; }
        [Required]
        [MaxLength(255)]
        [Column(TypeName = "VARCHAR")]
        public string Platform { get; set; }

        [Required]
        //CUSTOM WITHIN Migration.cs
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "Date")]
        public DateTime InsertDate { get; set; }

    }
}
