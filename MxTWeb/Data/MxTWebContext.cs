using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using MxTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public DbSet<Component> Component { get; set; } = default!;
        public DbSet<CompHistory> CompHistory { get; set; } = default!;
        public DbSet<Bom> Bom { get; set; } = default!;
        public DbSet<BomDetail> BomDetail { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Component>()
            //    .HasIndex(x => x.PartNumber)
            //    .IsUnique();
        }
    }
}
