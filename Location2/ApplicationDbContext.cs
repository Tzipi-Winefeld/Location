using Location2.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Location2
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Settlement> Settlements { get; set; }
    }

}
