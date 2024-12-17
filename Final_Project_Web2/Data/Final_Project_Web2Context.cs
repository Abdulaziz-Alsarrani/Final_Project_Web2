using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Final_Project_Web2.Models;

namespace Final_Project_Web2.Data
{
    public class Final_Project_Web2Context : DbContext
    {
        public Final_Project_Web2Context (DbContextOptions<Final_Project_Web2Context> options)
            : base(options)
        {
        }

        public DbSet<Final_Project_Web2.Models.usersaccounts> usersaccounts { get; set; } = default!;
        public DbSet<Final_Project_Web2.Models.orders> orders { get; set; } = default!;
        public DbSet<Final_Project_Web2.Models.orderline> orderline { get; set; } = default!;
        public DbSet<Final_Project_Web2.Models.items> items { get; set; } = default!;
        public DbSet<Final_Project_Web2.report> report { get; set; } = default!;
    }
}
