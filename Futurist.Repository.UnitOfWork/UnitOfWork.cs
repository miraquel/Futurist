using System.Data;
using System.Data.Common;
using Futurist.Repository.Interface;

namespace Futurist.Repository.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _dbConnection;
    private readonly Func<IDbTransaction> _transactionFactory;

    public UnitOfWork(
        IDbConnection dbConnection, 
        IRofoRepository rofoRepository, 
        ICommonRepository commonRepository, 
        IBomStdRepository bomStdRepository, 
        IMupRepository mupRepository,
        IFgCostRepository fgCostRepository,
        IJobMonitoringRepository jobMonitoringRepository,
        IExchangeRateRepository exchangeRateRepository,
        IItemAdjustmentRepository itemAdjustmentRepository,
        Func<IDbTransaction> transactionFactory)
    {
        _dbConnection = dbConnection;
        RofoRepository = rofoRepository;
        CommonRepository = commonRepository;
        BomStdRepository = bomStdRepository;
        MupRepository = mupRepository;
        _transactionFactory = transactionFactory;
        ItemAdjustmentRepository = itemAdjustmentRepository;
        ExchangeRateRepository = exchangeRateRepository;
        JobMonitoringRepository = jobMonitoringRepository;
        FgCostRepository = fgCostRepository;
    }
    
    public IDbTransaction? CurrentTransaction { get; private set; }

    public IDbTransaction BeginTransaction()
    {
        CurrentTransaction ??= _transactionFactory();
        return CurrentTransaction;
    }

    public IRofoRepository RofoRepository { get; }
    public ICommonRepository CommonRepository { get; }
    public IBomStdRepository BomStdRepository { get; }
    public IMupRepository MupRepository { get; }
    public IFgCostRepository FgCostRepository { get; }
    public IJobMonitoringRepository JobMonitoringRepository { get; }
    public IExchangeRateRepository ExchangeRateRepository { get; }
    public IItemAdjustmentRepository ItemAdjustmentRepository { get; }

    public async Task CommitAsync()
    {
        if (CurrentTransaction is DbTransaction dbTransaction)
        {
            try
            {
                await dbTransaction.CommitAsync();
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
            }
        }
        else
        {
            try
            {
                CurrentTransaction?.Commit();
                _dbConnection.BeginTransaction();
            }
            catch (Exception)
            {
                CurrentTransaction?.Rollback();
            }
        }
    }

    public async Task RollbackAsync()
    {
        if (CurrentTransaction is DbTransaction dbTransaction)
        {
            await dbTransaction.RollbackAsync();
        }
        else
        {
            CurrentTransaction?.Rollback();
        }
    }
    
    private bool _disposed;

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                //Close the SQL Connection and dispose the objects
                _dbConnection.Close();
                _dbConnection.Dispose();
                CurrentTransaction?.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbConnection is DbConnection dbConnection)
        {
            await dbConnection.CloseAsync();
        }
        else
        {
            _dbConnection.Close();
        }
        
        await CastAndDispose(_dbConnection);
        if (CurrentTransaction != null) await CastAndDispose(CurrentTransaction);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync();
            else
                resource.Dispose();
        }
    }
}