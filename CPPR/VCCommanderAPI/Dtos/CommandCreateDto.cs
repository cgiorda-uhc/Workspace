using System.ComponentModel.DataAnnotations;

namespace VCCommandAPI.Dtos
{
    public class CommandCreateDto
    {
        //DTO MAPS TO Command.cs MODEL

        //AUTO PK NOT NEEDED FOR INSERTS/CREATE
        //public int Id { get; set; }


        //WE ADD ATTRIBUITES TO THE DTO TO VALIDATE USER CREATED OBJECTS
        //WITHOUT IT WOULD THROW UGLY NON SPECIFIC 500 ERROR
        //NOW WE GET CLEAN JSON MESSENGING 400 ERROR:
        /*
           {
            "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "title": "One or more validation errors occurred.",
            "status": 400,
            "traceId": "00-34b0e56164eeaba049e05ae03edcc0be-50a09bf8274cb1e9-00",
            "errors": {
                "Platform": [
                    "The Platform field is required."
                ]
            }
         */


        //REQUIRED FIELD IN Command.cs!!!
        [Required]
        [MaxLength(255)]
        public string HowTo { get; set; }

        //REQUIRED FIELD IN Command.cs!!!
        [Required]
        [MaxLength(255)]
        public string Line { get; set; }

        //REQUIRED FIELD IN Command.cs!!!
        [Required]
        [MaxLength(255)]
        public string Platform { get; set; }

        //AUTO NOT NEEDED FOR CREATE/CREATE
        //public DateTime InsertDate { get; set; }


    }


    //'PUT' (AKA UPDATE) LIMITATIONS
    //MUST SUPPLY ALL FIELDS EVEN IF ONLY ONE IS UPDATES
    //INEFFICEINT AND ERROR PRONE FOR LARGE OBJECTS
    //NOT USED MUCH NOW IN FAVOUR OF 'PATCH'


}
