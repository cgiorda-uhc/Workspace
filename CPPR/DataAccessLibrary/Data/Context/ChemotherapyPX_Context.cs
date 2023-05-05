using Microsoft.EntityFrameworkCore;
using VCPortal_Models.Models.ChemoPx;

namespace DataAccessLibrary.Data.Context;

public class ChemotherapyPX_Context : DbContext
    {
        public ChemotherapyPX_Context(DbContextOptions<ChemotherapyPX_Context> options) : base(options)
        {

        }

   

        //MAP MODEL TO DB 
        //MIGRATION WILL CREATE A TABLE NAMED Commands
        public DbSet<ChemotherapyPXModel> ChemotherapyPXContext { get; set; }

    }

