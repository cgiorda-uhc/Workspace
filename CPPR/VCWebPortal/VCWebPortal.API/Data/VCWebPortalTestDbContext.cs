using Microsoft.EntityFrameworkCore;
using VCWebPortal.API.Models;

namespace VCWebPortal.API.Data
{
    public class VCWebPortalTestDbContext : DbContext
    {
        public VCWebPortalTestDbContext(DbContextOptions options) : base(options)
        {

        }

        //DBSet
        public DbSet<VCWebPortalTest> VCWebPortalTests { get; set; }

    }
}
