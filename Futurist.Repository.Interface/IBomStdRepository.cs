﻿using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.BomStdCommand;

namespace Futurist.Repository.Interface;

public interface IBomStdRepository
{
    Task<SpTask?> ProcessBomStdAsync(ProcessBomStdCommand command);
    Task<IEnumerable<BomStd>> BomErrorCheckAsync(BomErrorCheckCommand command);
    Task<PagedList<BomStd>> BomErrorCheckPagedListAsync(BomErrorCheckPagedListCommand command);
    Task<IEnumerable<int>> GetBomStdRoomIdsAsync(GetBomStdRoomIdsCommand command);
}