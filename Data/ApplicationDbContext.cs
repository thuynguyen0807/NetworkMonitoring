using Microsoft.EntityFrameworkCore;
using NetworkMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }

        public DbSet<Devices> Devices { get; set; }

        public DbSet<OIDs> OIDs { get; set; }

        public DbSet<Result> Results { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
