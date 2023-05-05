using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace UCS_Project_Manager
{
    [Table("ETG_Dim_Premium_Spec_Master")]
    public class ETG_Dim_Premium_Spec_Master_Model : ModelBase
    {
        public ETG_Dim_Premium_Spec_Master_Model()
        {

        }


        private Int16 _intPremiumSpecialtyId;
        [Key]
        [Column("Premium_Specialty_id", TypeName = "SMALLINT")]
        public Int16 Premium_Specialty_id
        {
            get { return this._intPremiumSpecialtyId; }
            set { this._intPremiumSpecialtyId = value; }
        }


        private string _strPremiumSpecialty;
        [Column("Premium_Specialty", TypeName = "VARCHAR")]
        [StringLength(255)]
        public string Premium_Specialty
        {
            get { return this._strPremiumSpecialty; }
            set { this._strPremiumSpecialty = value; }
        }

        //TEST!!!!!
        //public virtual ETG_Fact_Symmetry_Model ETG_Fact_Symmetry_Model { get; set; }
    }
}
