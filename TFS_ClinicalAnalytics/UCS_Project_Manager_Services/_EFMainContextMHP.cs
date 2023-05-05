using System;
using System.Data.Entity;
using UCS_Project_Manager;


namespace UCS_Project_Manager_Services
{
    public class _EFMainContextMHP : DbContext 
    {
        


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


            // CHANGE DBO TO CSG SCHEMA
            Database.SetInitializer<_EFMainContextMHP>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("stg");


            //modelBuilder.Entity<ETG_Fact_Symmetry_Model>()
            //    .HasMany<ETG_Fact_Symmetry_LOB_Model>(g => g.ETG_Fact_Symmetry_LOBs)
            //    .WithRequired(s => s.ETG_Fact_Symmetry_Model)
            //    .HasForeignKey<Int64>(s => s.ETG_Fact_Symmetry_id);


            //modelBuilder.Entity<ETG_Dim_Premium_Spec_Master_Model>()
            //.HasRequired(p => p.ETG_Fact_Symmetry_Model)
            //.WithOptional(p => p.ETG_Dim_Premium_Spec_Master_Model);

            //modelBuilder.Entity<ETG_Dim_Master_Model>()
            //.HasRequired(p => p.ETG_Fact_Symmetry_Model)
            //.WithOptional(p => p.ETG_Dim_Master_Model);


            //modelBuilder.Entity<ETG_Dim_LOB_Model>()
            //.HasRequired(p => p.ETG_Fact_Symmetry_LOB_Model)
            //.WithOptional(p => p.ETG_Dim_LOB);


            //.HasForeignKey<Int64>(s => s.ETG_Fact_Symmetry_id);
            // modelBuilder.Entity<PersonPhoto>()
            //.HasRequired(p => p.PhotoOf)
            //.WithOptional(p => p.Photo);


            //        modelBuilder.Entity<Grade>()
            //.HasMany<Student>(g => g.Students)
            //.WithRequired(s => s.CurrentGrade)
            //.HasForeignKey<int>(s => s.CurrentGradeId);
        }



    }
}
