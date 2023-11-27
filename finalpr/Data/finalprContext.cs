using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using finalpr.Models;

namespace finalpr.Data
{
    public class finalprContext : DbContext
    {
        public finalprContext (DbContextOptions<finalprContext> options)
            : base(options)
        {
        }

        public DbSet<finalpr.Models.items> items { get; set; } = default!;
        public DbSet<finalpr.Models.roles> roles { get; set; } 
        public DbSet<finalpr.Models.users>? users { get; set; }
        public DbSet<finalpr.Models.orders>? orders { get; set; }


    }
}
