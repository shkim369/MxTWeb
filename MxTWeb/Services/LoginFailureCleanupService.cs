using Microsoft.EntityFrameworkCore;
using MxTWeb.Data;

namespace MxTWeb.Services
{
    public class LoginFailureCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LoginFailureCleanupService> _logger;

        public LoginFailureCleanupService(IServiceScopeFactory scopeFactory, ILogger<LoginFailureCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupOldRecords();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "로그인 실패 기록 정리 중 오류 발생");
                }

                // 24시간마다 실행
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task CleanupOldRecords()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var cutoffDate = DateTime.Now.AddDays(-30);
            var oldRecords = await dbContext.LoginFailureRecords
                .Where(r => r.AttemptedAt < cutoffDate)
                .ToListAsync();

            if (oldRecords.Any())
            {
                dbContext.LoginFailureRecords.RemoveRange(oldRecords);
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("30일 이상 된 로그인 실패 기록 {Count}건 삭제 완료", oldRecords.Count);
            }
        }
    }
}
