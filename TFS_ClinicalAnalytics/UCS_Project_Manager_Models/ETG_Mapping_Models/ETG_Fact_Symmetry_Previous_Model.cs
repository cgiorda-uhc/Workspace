using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCS_Project_Manager
{
    [Table("ETG_Fact_Symmetry")]
    public class ETG_Fact_Symmetry_Previous_Model : ModelBase
    {
        public ETG_Fact_Symmetry_Previous_Model()
        {

        }

        private Int64 _intETGFactSymmetryid;
        [Key]
        [Column("ETG_Fact_Symmetry_id", TypeName = "BIGINT")]
        public Int64 ETG_Fact_Symmetry_id
        {
            get { return this._intETGFactSymmetryid; }
            set { this._intETGFactSymmetryid = value; }
        }


        private string _strETGBaseClass;
        [Column("ETG_Base_Class", TypeName = "VARCHAR")]
        [StringLength(15)]
        public string ETG_Base_Class
        {
            get { return this._strETGBaseClass; }
            set { this._strETGBaseClass = value; }
        }


        private Int16 _intPremiumSpecialtyId;
        [Column("Premium_Specialty_id", TypeName = "SMALLINT")]
        public Int16 Premium_Specialty_id
        {
            get { return this._intPremiumSpecialtyId; }
            set { this._intPremiumSpecialtyId = value; }
        }


        private string _strData_Period;
        [Column("Data_Period", TypeName = "VARCHAR")]
        [StringLength(15)]
        public string Data_Period
        {
            get { return this._strData_Period; }
            set { this._strData_Period = value; }
        }

        private DateTime _dtData_Date;
        [Column("Data_Date", TypeName = "Date")]
        public DateTime Data_Date
        {
            get { return this._dtData_Date; }
            set { this._dtData_Date = value; }
        }


        private bool? _blHasCommercial;
        [Column("has_Commercial", TypeName = "BIT")]
        public bool? has_Commercial
        {
            get { return this._blHasCommercial; }
            set { this._blHasCommercial = value; }
        }

        private bool? _blHasMedicare;
        [Column("has_Medicare", TypeName = "BIT")]
        public bool? has_Medicare
        {
            get { return this._blHasMedicare; }
            set { this._blHasMedicare = value; }
        }

        private bool? _blHasMedicaid;
        [Column("has_Medicaid", TypeName = "BIT")]
        public bool? has_Medicaid
        {
            get { return this._blHasMedicaid; }
            set { this._blHasMedicaid = value; }
        }

        private string _strLOBString;
        [NotMapped]
        public string LOBString
        {
            get
            {

                if (has_Commercial == null && has_Medicare == null && has_Medicaid == null)
                    _strLOBString = "Not Selected";
                else if (has_Commercial == true && has_Medicare == true && has_Medicaid == true)
                    _strLOBString = "All";
                else if (has_Commercial == true && has_Medicare == true)
                    _strLOBString = "Commercial + Medicare";
                else if (has_Commercial == true && has_Medicaid == true)
                    _strLOBString = "Commercial + Medicaid";
                else if (has_Medicare == true && has_Medicaid == true)
                    _strLOBString = "Medicare + Medicaid";
                else if (has_Commercial == true)
                    _strLOBString = "Commercial Only";
                else if (has_Medicare == true)
                    _strLOBString = "Medicare Only";
                else if (has_Medicaid == true)
                    _strLOBString = "Medicaid Only";
                else
                    _strLOBString = "Not Selected";

                return _strLOBString;
            }
            set
            {
                this._strLOBString = value;
            }
        }

    }

}
