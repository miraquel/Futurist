using System.Data;
using Futurist.Repository.Command.CommonCommand;

namespace Futurist.Repository.Interface;

public interface ICommonRepository
{
    Task<int> GetLastInsertedIdAsync(GetLastInsertedIdCommand command);
}