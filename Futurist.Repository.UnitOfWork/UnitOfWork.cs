using System.Data;
using System.Data.Common;
using Futurist.Repository.Interface;

namespace Futurist.Repository.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public UnitOfWork(
        IDbTransaction dbTransaction, 
        IDbConnection dbConnection, 
        IRofoRepository rofoRepository, 
        ICommonRepository commonRepository, 
        IBomStdRepository bomStdRepository, 
        IMupRepository mupRepository)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
        RofoRepository = rofoRepository;
        CommonRepository = commonRepository;
        BomStdRepository = bomStdRepository;
        MupRepository = mupRepository;
    }

    public IRofoRepository RofoRepository { get; }
    public ICommonRepository CommonRepository { get; }
    public IBomStdRepository BomStdRepository { get; }
    public IMupRepository MupRepository { get; }

    public async Task Commit()
    {
        if (_dbTransaction is DbTransaction dbTransaction)
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
                _dbTransaction.Commit();
                _dbConnection.BeginTransaction();
            }
            catch (Exception)
            {
                _dbTransaction.Rollback();
            }
        }
    }

    public async Task Rollback()
    {
        if (_dbTransaction is DbTransaction dbTransaction)
        {
            await dbTransaction.RollbackAsync();
        }
        else
        {
            _dbTransaction.Rollback();
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
                _dbTransaction.Dispose();
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
        await CastAndDispose(_dbTransaction);

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