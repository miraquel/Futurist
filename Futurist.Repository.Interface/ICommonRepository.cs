namespace Futurist.Repository.Interface;

public interface ICommonRepository
{
    Task<int> GetLastInsertedIdAsync();
}