using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VCCommandAPI.Dtos
{
    public class CommandReadDto 
    {
        //DTO MAPS TO Command.cs MODEL
        public int Id { get; set; }
        public string HowTo { get; set; }

        public string Line { get; set; }

        public string Platform { get; set; }

        //CLIENT DOESNT NEED THIS DETAIL
        //public DateTime InsertDate { get; set; }


    }
}
