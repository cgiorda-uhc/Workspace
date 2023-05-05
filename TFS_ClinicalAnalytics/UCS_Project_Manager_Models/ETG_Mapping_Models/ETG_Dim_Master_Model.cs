using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCS_Project_Manager
{
    [Table("ETG_Dim_Master")]
    public class ETG_Dim_Master_Model : ModelBase
    {
        public ETG_Dim_Master_Model()
        {

        }

        private string _strETGBaseClass;
        [Key]
        [Column("ETG_Base_Class", TypeName = "VARCHAR")]
        [StringLength(15)]
        public string ETG_Base_Class
        {
            get { return this._strETGBaseClass; }
            set { this._strETGBaseClass = value; }
        }

        private string _strETGDescription;
        [Column("ETG_Description", TypeName = "VARCHAR")]
        [StringLength(255)]
        public string ETG_Description
        {
            get { return this._strETGDescription; }
            set { this._strETGDescription = value; }
        }

        private string _strETGDisplay;
        [Column("ETG_Display", TypeName = "VARCHAR")]
        [StringLength(255)]
        public string ETG_Display
        {
            get { return this._strETGDisplay; }
            set { this._strETGDisplay = value; }
        }


        //TEST!!!!!
        //public virtual ETG_Fact_Symmetry_Model ETG_Fact_Symmetry_Model { get; set; }
    }
}
