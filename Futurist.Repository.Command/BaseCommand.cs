using System.Data;

namespace Futurist.Repository.Command;

public abstract class BaseCommand
{
    public IDbTransaction? DbTransaction { get; init; }
}