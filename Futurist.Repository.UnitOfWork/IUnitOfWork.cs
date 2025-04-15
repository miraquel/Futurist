using System.Data;
using Futurist.Repository.Interface;

namespace Futurist.Repository.UnitOfWork;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IRofoRepository RofoRepository { get; }
    ICommonRepository CommonRepository { get; }
    IBomStdRepository BomStdRepository { get; }
    IMupRepository MupRepository { get; }
    IFgCostRepository FgCostRepository { get; }
    IJobMonitoringRepository JobMonitoringRepository { get; }
    IExchangeRateRepository ExchangeRateRepository { get; }
    IItemAdjustmentRepository ItemAdjustmentRepository { get; }
    IDbTransaction? CurrentTransaction { get; }
    IDbTransaction BeginTransaction();
    Task CommitAsync();
    Task RollbackAsync();
}