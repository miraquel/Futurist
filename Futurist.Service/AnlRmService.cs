using Futurist.Repository.Command.AnlRmCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Serilog;

namespace Futurist.Service;

public class AnlRmService : IAnlRmService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly ILogger _logger = Log.ForContext<AnlRmService>();

    public AnlRmService(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<AnlRmPriceDto>>> GetAnlRmpPrice(int room, int verId, int year, int month, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetAnlRmPriceCommand
            {
                Room = room,
                VerId = verId,
                Year = year,
                Month = month
            };

            var anlRmPrices = await _unitOfWork.AnlRmRepository.GetAnlRmpPrice(command, cancellationToken);

            return new ServiceResponse<IEnumerable<AnlRmPriceDto>>
            {
                Data = _mapper.MapToIEnumerableDto(anlRmPrices),
                Message = ServiceMessageConstants.AnlRmPriceFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "AnlRmPrice {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<AnlRmPriceDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.AnlRmPriceNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<AnlKursDto>>> GetAnlKursAsync(int version, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetAnlKursCommand
            {
                Version = version
            };

            var anlKursList = await _unitOfWork.AnlRmRepository.GetAnlKursAsync(command, cancellationToken);
            
            return new ServiceResponse<IEnumerable<AnlKursDto>>
            {
                Data = _mapper.MapToIEnumerableDto(anlKursList),
                Message = ServiceMessageConstants.AnlKursFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "AnlKurs {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<AnlKursDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.AnlKursNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<AnlFgPriceDto>>> GetAnlFgPriceAsync(int room, int verId, int year, int month, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetAnlFgPriceCommand
            {
                Room = room,
                VerId = verId,
                Year = year,
                Month = month
            };

            var anlFgPrices = await _unitOfWork.AnlRmRepository.GetAnlFgPriceAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<AnlFgPriceDto>>
            {
                Data = _mapper.MapToIEnumerableDto(anlFgPrices),
                Message = ServiceMessageConstants.AnlFgPriceFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "AnlFgPrice {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<AnlFgPriceDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.AnlFgPriceNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<AnlPmPriceDto>>> GetAnlPmPriceAsync(int room, int verId, int year, int month, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetAnlPmPriceCommand
            {
                Room = room,
                VerId = verId,
                Year = year,
                Month = month
            };

            var anlPmPrices = await _unitOfWork.AnlRmRepository.GetAnlPmPriceAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<AnlPmPriceDto>>
            {
                Data = _mapper.MapToIEnumerableDto(anlPmPrices),
                Message = ServiceMessageConstants.AnlPmPriceFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "AnlPmPrice {@command}", e.Message);
            return new ServiceResponse<IEnumerable<AnlPmPriceDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.AnlPmPriceNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetRoomIdsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetRoomIdsCommand();

            var roomIds = await _unitOfWork.AnlRmRepository.GetRoomIdsAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<int>>
            {
                Data = roomIds,
                Message = ServiceMessageConstants.AnlRmRoomIdsFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetRoomIds {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.AnlRmRoomIdsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetRofoVerIdsAsync(int room, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetRofoVerIdsCommand
            {
                Room = room
            };

            var verIds = await _unitOfWork.AnlRmRepository.GetRofoVerIdsAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<int>>
            {
                Data = verIds,
                Message = ServiceMessageConstants.AnlRmVerIdsFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetVerIds {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.AnlRmVerIdsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetYearsAsync(int room, int verId, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetYearsCommand
            {
                Room = room,
                VerId = verId
            };

            var years = await _unitOfWork.AnlRmRepository.GetYearsAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<int>>
            {
                Data = years,
                Message = ServiceMessageConstants.AnlRmYearsFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetYears {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.AnlRmYearsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetMonthsAsync(int room, int verId, int year, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetMonthsCommand
            {
                Room = room,
                VerId = verId,
                Year = year
            };

            var months = await _unitOfWork.AnlRmRepository.GetMonthsAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<int>>
            {
                Data = months,
                Message = ServiceMessageConstants.AnlRmMonthsFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetMonths {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.AnlRmMonthsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetVerIdsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetVerIdsCommand();

            var verIds = await _unitOfWork.AnlRmRepository.GetVerIdsAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<int>>
            {
                Data = verIds,
                Message = ServiceMessageConstants.AnlRmVerIdsFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetVerIds {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.AnlRmVerIdsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<AnlRmPriceGroupDto>>> GetAnlRmPriceGroupAsync(int room, int verId, int year, int month, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetAnlRmPriceGroupCommand
            {
                Room = room,
                VerId = verId,
                Year = year,
                Month = month
            };
            var result = await _unitOfWork.AnlRmRepository.GetAnlRmPriceGroupAsync(command, cancellationToken);
            return new ServiceResponse<IEnumerable<AnlRmPriceGroupDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = "AnlRmPriceGroup found"
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "AnlRmPriceGroup {@command}", e.Message);
            return new ServiceResponse<IEnumerable<AnlRmPriceGroupDto>>
            {
                Errors = [e.Message],
                Message = "AnlRmPriceGroup not found"
            };
        }
    }
}