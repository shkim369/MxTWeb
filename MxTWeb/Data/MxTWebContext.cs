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

        public DbSet<MxTWeb.Models.Cyropreserved> Cyropreserved { get; set; } = default!;
        public DbSet<MxTWeb.Models.Reagent> Reagent { get; set; } = default!;
        public DbSet<MxTWeb.Models.Equipment> Equipment { get; set; } = default!;
        public DbSet<MxTWeb.Models.ChipUsage> ChipUsage { get; set; } = default!;
    }
}
