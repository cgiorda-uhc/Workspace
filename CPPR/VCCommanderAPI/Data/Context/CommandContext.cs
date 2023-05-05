using Microsoft.EntityFrameworkCore;
using VCCommandAPI.Models;

namespace VCCommandAPI.Data.Context
{
    public class CommandContext : DbContext
    {
        public CommandContext(DbContextOptions<CommandContext> options) : base(options)
        {

        }

        //Package Manager MIGRATION COMMAND:
        //add-migration InitialMigration
        //Package Manager APPLY TO DB COMMAND:
        //Update-Database
        //Package Manager APPLY SPECIFIC MIGRATION:
        //update-database -migration InitialMigration2

        //MAP MODEL TO DB 
        //MIGRATION WILL CREATE A TABLE NAMED Commands
        public DbSet<Command> Commands { get; set; }

    }
}
