using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCS_Project_Manager
{
    [Table("ETG_Dim_LOB")]
    public class ETG_Dim_LOB_Model : ModelBase
    {
        public ETG_Dim_LOB_Model()
        {

        }

        private Int16 _intLOBId;
        [Key]
        [Column("LOB_id", TypeName = "SMALLINT")]
        public Int16 LOB_id
        {
            get { return this._intLOBId; }
            set { this._intLOBId = value; }
        }

        private string _strLOB;
        [Column("LOB", TypeName = "VARCHAR")]
        [StringLength(10)]
        public string LOB
        {
            get { return this._strLOB; }
            set { this._strLOB = value; }
        }

        //TEST!!!!!
        //[ForeignKey(nameof(LOB_id))]
        //public virtual ETG_Fact_Symmetry_LOB_Model ETG_Fact_Symmetry_LOB_Model { get; set; }
    }
}
