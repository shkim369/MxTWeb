using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MxTWeb.Data;
using MxTWeb.Enums;
using MxTWeb.Models;

namespace MxTWeb.Services
{
    /// <summary>홈 대시보드에 표시되는 요약 데이터 (읽기 전용 스냅샷)</summary>
    public sealed record DashboardSnapshot(
        IReadOnlyList<Cyropreserved> Cyropreserved,
        IReadOnlyList<Reagent> Reagents,
        IReadOnlyList<Equipment> Equipment,
        IReadOnlyList<ChipUsage> ChipUsages,
        IReadOnlyList<CompHistory> CompHistories,
        IReadOnlyList<Shipment> Shipments,
        int WeeklyShipmentCount,
        int WeeklyReceiptCount,
        int WeeklyChipProductionCount,
        int WeeklyChipUsageCount,
        int ShortageCount,
        int CyropreservedRequestCount,
        int ChipRequestCount);

    /// <summary>
    /// 대시보드 데이터를 짧게(30초) 메모리 캐시한다.
    /// 접속자 수와 무관하게 30초당 DB 쿼리 1세트만 실행되도록 하기 위함.
    /// 요약 화면이라 최대 30초 지연은 허용하고, 정확해야 하는 목록 페이지는 캐시하지 않는다.
    /// </summary>
    public sealed class DashboardService
    {
        private const string CacheKey = "dashboard:snapshot";
        private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(30);

        private readonly IDbContextFactory<MxTWebContext> _factory;
        private readonly IMemoryCache _cache;
        // 캐시 만료 순간 여러 사용자가 동시에 DB 를 치는 것(cache stampede)을 막기 위한 잠금
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        public DashboardService(IDbContextFactory<MxTWebContext> factory, IMemoryCache cache)
        {
            _factory = factory;
            _cache = cache;
        }

        public async Task<DashboardSnapshot> GetAsync(CancellationToken ct = default)
        {
            if (_cache.TryGetValue(CacheKey, out DashboardSnapshot? cached) && cached is not null)
                return cached;

            await _refreshLock.WaitAsync(ct);
            try
            {
                // 잠금을 기다리는 동안 다른 요청이 이미 채웠을 수 있음
                if (_cache.TryGetValue(CacheKey, out cached) && cached is not null)
                    return cached;

                var snapshot = await LoadAsync(ct);
                _cache.Set(CacheKey, snapshot, Ttl);
                return snapshot;
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        /// <summary>데이터 변경 직후 대시보드에 바로 반영하고 싶을 때 호출</summary>
        public void Invalidate() => _cache.Remove(CacheKey);

        private async Task<DashboardSnapshot> LoadAsync(CancellationToken ct)
        {
            // DbContext 는 스레드 안전하지 않으므로 쿼리는 순차 실행. 조회 전용이라 AsNoTracking.
            await using var db = await _factory.CreateDbContextAsync(ct);
            var weekAgo = DateTime.Now.AddDays(-7);

            var cyropreserved = await db.Cyropreserved.AsNoTracking().OrderByDescending(x => x.ReceiptDate).Take(10).ToListAsync(ct);
            var reagents = await db.Reagent.AsNoTracking().OrderByDescending(x => x.ReceiptDate).Take(10).ToListAsync(ct);
            var equipment = await db.Equipment.AsNoTracking().OrderByDescending(x => x.PurchaseDate).Take(10).ToListAsync(ct);
            var chipUsages = await db.ChipUsage.AsNoTracking().OrderByDescending(x => x.UseDate).Take(10).ToListAsync(ct);
            var compHistories = await db.CompHistory.AsNoTracking().Include(x => x.Component)
                                        .OrderByDescending(x => x.EntryDate).Take(10).ToListAsync(ct);
            var shipments = await db.Shipment.AsNoTracking().OrderByDescending(x => x.Date).Take(10).ToListAsync(ct);

            var weeklyShipment = await db.CompHistory.Where(x => (x.state == "출고" || x.state == "폐기") && x.Date >= weekAgo).SumAsync(x => x.Quantity, ct);
            var weeklyReceipt = await db.CompHistory.Where(x => x.state == "입고" && x.Date >= weekAgo).SumAsync(x => x.Quantity, ct);
            var weeklyProduction = await db.ChipUsage.Where(x => x.Division == "제작" && x.UseDate >= weekAgo).SumAsync(x => x.Production, ct);
            var weeklyChipUsage = await db.ChipUsage.Where(x => x.Division == "납품" && x.UseDate >= weekAgo).SumAsync(x => x.Production, ct);
            var shortage = await db.Component.CountAsync(x => x.Stock < x.Quantity, ct);
            var cyroRequests = await db.CyropreservedRequest.CountAsync(x => x.Status == RequestStatusEnum.Pending, ct);
            var chipRequests = await db.ChipUsageRequest.CountAsync(x => x.Status == RequestStatusEnum.Pending, ct);

            return new DashboardSnapshot(cyropreserved, reagents, equipment, chipUsages, compHistories, shipments,
                weeklyShipment, weeklyReceipt, weeklyProduction, weeklyChipUsage, shortage, cyroRequests, chipRequests);
        }
    }
}
