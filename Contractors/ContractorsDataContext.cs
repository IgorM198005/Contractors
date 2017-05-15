using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Contractors
{
    public class ContractorsDataContext : DbContext
    {
        public DbSet<Contractor> Contractors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Contractors.sl3");
        }
    }
}
