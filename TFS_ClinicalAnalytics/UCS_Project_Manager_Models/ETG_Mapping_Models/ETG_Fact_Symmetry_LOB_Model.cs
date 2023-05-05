using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCS_Project_Manager
{
    [Table("ETG_Fact_Symmetry_LOB")]
    public class ETG_Fact_Symmetry_LOB_Model : ModelBase
    {

        public ETG_Fact_Symmetry_LOB_Model()
        {

        }

        private Int64 _intSymmetryLOBId;
        [Key]
        [Column("Symmetry_LOB_id", TypeName = "BIGINT")]
        public Int64 Symmetry_LOB_id
        {
            get { return this._intSymmetryLOBId; }
            set { this._intSymmetryLOBId = value; }
        }



        //[ForeignKey("ETG_Fact_Symmetry_Model")]
        //public virtual ETG_Fact_Symmetry_Model ETG_Fact_Symmetry_Model { get; set; }
        [Column("ETG_Fact_Symmetry_id", TypeName = "BIGINT")]
        public Int64 ETG_Fact_Symmetry_id { get; set; }




        private Int16 _intLOBId;
        [Column("LOB_id", TypeName = "SMALLINT")]
        public Int16 LOB_id
        {
            get { return this._intLOBId; }
            set { this._intLOBId = value; }
        }
        //[ForeignKey(nameof(LOB_id))]
        //public virtual ETG_Dim_LOB_Model ETG_Dim_LOB { get; set; }



    }
}
