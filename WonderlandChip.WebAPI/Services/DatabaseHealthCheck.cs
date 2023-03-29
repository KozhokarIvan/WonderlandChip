using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WonderlandChip.Database.DbContexts;

namespace WonderlandChip.WebAPI.Services
{
    internal class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ChipizationDbContext _dbContext;
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            bool isHealthy = await IsDatabaseConnectionEstablished();
            return isHealthy ?
                HealthCheckResult.Healthy() :
                HealthCheckResult.Unhealthy();
        }
        public DatabaseHealthCheck(ChipizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        private async Task<bool> IsDatabaseConnectionEstablished() =>
            await _dbContext.Database.CanConnectAsync();
    }
}
