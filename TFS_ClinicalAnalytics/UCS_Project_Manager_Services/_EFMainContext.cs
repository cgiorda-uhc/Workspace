using System;
using System.Data.Entity;
using UCS_Project_Manager;


namespace UCS_Project_Manager_Services
{
    public class _EFMainContext : DbContext
    {



        //public DbSet<ProjectIntakeSample1_Model> projectIntakeSample1 { get; set; }
        //public DbSet<CPM_Intake_Model> CPM_Intake { get; set; }



        public DbSet<ETG_Dim_Master_Model> ETG_Dim_Master { get; set; }

        public DbSet<ETG_Dim_Premium_Spec_Master_Model> ETG_Dim_Premium_Spec_Master { get; set; }

        ////public DbSet<ETG_Fact_Symmetry_LOB_Model> ETG_Fact_Symmetry_LOB { get; set; }
        public DbSet<ETG_Dim_LOB_Model> ETG_Dim_LOB { get; set; }

        public DbSet<ETG_Fact_Symmetry_Model> ETG_Fact_Symmetry { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


            // CHANGE DBO TO CSG SCHEMA
            Database.SetInitializer<_EFMainContext>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("csg");


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
