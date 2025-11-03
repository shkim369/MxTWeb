using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MxTWeb.Models;

namespace MxTWeb.Data
{
    public class MxTWebContext : DbContext
    {
        public MxTWebContext (DbContextOptions<MxTWebContext> options)
            : base(options)
        {
        }

        public DbSet<Cyropreserved> Cyropreserved { get; set; } = default!;
        public DbSet<Reagent> Reagent { get; set; } = default!;
        public DbSet<Equipment> Equipment { get; set; } = default!;
        public DbSet<ChipUsage> ChipUsage { get; set; } = default!;
        public DbSet<ChipPressure> ChipPressure { get; set; } = default!;
    }
}
