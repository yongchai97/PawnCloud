using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PawnCloud.AuditLogs;

public class AuditLogCleanupWorker : AsyncPeriodicBackgroundWorkerBase
{
    private const int CleanupBatchSize = 1000;
    private const int CleanupPeriodMilliseconds = 5 * 60 * 1000;

    private readonly IRepository<AuditLog, long> _repository;

    public AuditLogCleanupWorker(
        AbpAsyncTimer timer,
        IRepository<AuditLog, long> repository)
        : base(timer)
    {
        _repository = repository;
        Timer.Period = CleanupPeriodMilliseconds;
        Timer.RunOnStart = false;
    }

    protected override async Task DoWorkAsync()
    {
        try
        {
            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var cutoff = DateTime.UtcNow.AddYears(-1);
                    var ids = await _repository.GetAll()
                        .Where(log => log.ExecutionTime <= cutoff)
                        .OrderBy(log => log.ExecutionTime)
                        .ThenBy(log => log.Id)
                        .Select(log => log.Id)
                        .Take(CleanupBatchSize)
                        .ToListAsync();

                    if (ids.Count > 0)
                    {
                        await _repository.DeleteAsync(log => ids.Contains(log.Id));
                    }
                }

                await unitOfWork.CompleteAsync();
            }
        }
        catch (Exception exception)
        {
            Logger.Error("Audit log cleanup failed.", exception);
        }
    }
}