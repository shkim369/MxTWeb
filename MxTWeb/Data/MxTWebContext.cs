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
        public DbSet<Component> Component { get; set; } = default!;
        public DbSet<CompHistory> CompHistory { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Component의 PartNumber를 Unique로 설정
            modelBuilder.Entity<Component>()
                .HasIndex(c => c.PartNumber)
                .IsUnique();

            // CompHistory와 Component의 관계 설정
            modelBuilder.Entity<CompHistory>()
                .HasOne(h => h.Component)
                .WithMany()
                .HasForeignKey(h => h.PartNumber)
                .HasPrincipalKey(c => c.PartNumber)
                .OnDelete(DeleteBehavior.Restrict); // 또는 Cascade
        }
    }
}
