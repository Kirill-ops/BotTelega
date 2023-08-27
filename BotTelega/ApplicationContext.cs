using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BotTelega
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ModelReservedIPv6> ReservedIPv6 => Set<ModelReservedIPv6>();
        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source= DatabaseReservedIPv6.db");
        }
    }
}
