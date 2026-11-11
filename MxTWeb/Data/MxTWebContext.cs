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
        public DbSet<Shipment> Shipment { get; set; } = default!;
        public DbSet<CyropreservedRequest> CyropreservedRequest { get; set; } = default!;
        public DbSet<ChipusageRequest> ChipUsageRequest { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Component>()
            //    .HasIndex(x => x.PartNumber)
            //    .IsUnique();

            // CyropreservedRequest -> Cyropreserved FK 관계 설정
            modelBuilder.Entity<CyropreservedRequest>()
                .HasOne(r => r.Cyropreserved)
                .WithMany()
                .HasForeignKey(r => r.CyropreservedId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── 조회 성능용 인덱스 ──────────────────────────────────────────
            // nvarchar(max) 컬럼에는 인덱스를 걸 수 없으므로 검색/정렬 키 컬럼은 길이를 지정한다.

            // 칩 사용 대장: 칩 종류별 목록(날짜 내림차순) / 대시보드 구분별 주간 집계
            modelBuilder.Entity<ChipUsage>(e =>
            {
                e.Property(x => x.Chip).HasMaxLength(50);
                e.Property(x => x.Division).HasMaxLength(20);
                e.Property(x => x.WaferInfo).HasMaxLength(100);
                e.Property(x => x.Lot).HasMaxLength(100);
                e.HasIndex(x => new { x.Chip, x.UseDate });
                e.HasIndex(x => new { x.Division, x.UseDate });
            });

            // 압력 테스트: Lot 별 조회(행 펼치기) / 테스트 날짜순 목록
            modelBuilder.Entity<ChipPressure>(e =>
            {
                e.Property(x => x.Lot).HasMaxLength(100);
                e.Property(x => x.WaferInfo).HasMaxLength(100);
                e.HasIndex(x => x.Lot);
                e.HasIndex(x => x.TestDate);
            });

            // 부품 입출고 이력: 대시보드 최근 10건(EntryDate) / 상태별 주간 합계
            modelBuilder.Entity<CompHistory>(e =>
            {
                e.Property(x => x.state).HasMaxLength(20);
                e.HasIndex(x => x.EntryDate);
                e.HasIndex(x => new { x.state, x.Date });
            });
        }
    }
}
