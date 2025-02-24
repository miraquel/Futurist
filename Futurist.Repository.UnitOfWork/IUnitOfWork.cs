using Futurist.Repository.Interface;

namespace Futurist.Repository.UnitOfWork;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IRofoRepository RofoRepository { get; }
    ICommonRepository CommonRepository { get; }
    IBomStdRepository BomStdRepository { get; }
    IMupRepository MupRepository { get; }
    Task Commit();
    Task Rollback();
}