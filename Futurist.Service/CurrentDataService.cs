using Futurist.Repository.Command.CurrentDataCommands;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;

namespace Futurist.Service;

public class CurrentDataService : ICurrentDataService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;

    public CurrentDataService(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<ItemOnHandDto>>> GetItemOnHandAsync(CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetItemOnHandCommand();
            var itemOnHand = await _unitOfWork.CurrentDataRepository.GetItemOnHandAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<ItemOnHandDto>>
            {
                Data = _mapper.MapToIEnumerableDto(itemOnHand),
                Message = ServiceMessageConstants.ItemOnHandFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<ItemOnHandDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ItemOnHandNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<ItemPoIntransitDto>>> GetItemPoIntransitAsync(CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetItemPoIntransitCommand();
            var items = await _unitOfWork.CurrentDataRepository.GetItemPoIntransitAsync(command, cancellationToken);
            return new ServiceResponse<IEnumerable<ItemPoIntransitDto>>
            {
                Data = _mapper.MapToIEnumerableDto(items),
                Message = ServiceMessageConstants.ItemPoIntransitFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<ItemPoIntransitDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ItemPoIntransitNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<ItemPagDto>>> GetItemPagAsync(CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetItemPagCommand();
            var items = await _unitOfWork.CurrentDataRepository.GetItemPagAsync(command, cancellationToken);
            return new ServiceResponse<IEnumerable<ItemPagDto>>
            {
                Data = _mapper.MapToIEnumerableDto(items),
                Message = ServiceMessageConstants.ItemPagFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<ItemPagDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ItemPagNotFound
            };
        }
    }
}