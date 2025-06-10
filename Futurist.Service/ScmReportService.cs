using Futurist.Repository.Command.ScmReportCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Serilog;

namespace Futurist.Service;

public class ScmReportService : IScmReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly ILogger _logger = Log.ForContext<RofoService>();

    public ScmReportService(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetDomesticByProductCustomer(DateTime periodeDate)
    {
        try
        {
            var command = new GetDomesticByProductCustomerCommand
            {
                PeriodeDate = periodeDate
            };

            var result = await _unitOfWork.ScmReportRepository.GetDomesticByProductCustomer(command);

            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.DomesticByProductCustomerReportRetrieved
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetRofoRoomIdsAsync failed {@command}", e.Message);
            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ErrorRetrievingDomesticByProductCustomerReport
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetDomesticByProduct(DateTime periodeDate)
    {
        try
        {
            var command = new GetDomesticByProductCommand
            {
                PeriodeDate = periodeDate
            };

            var result = await _unitOfWork.ScmReportRepository.GetDomesticByProduct(command);

            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.DomesticByProductReportRetrieved
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetDomesticByProduct failed {@command}", e.Message);
            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ErrorRetrievingDomesticByProductReport
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetDomesticByCustomer(DateTime periodeDate)
    {
        try
        {
            var command = new GetDomesticByCustomerCommand
            {
                PeriodeDate = periodeDate
            };

            var result = await _unitOfWork.ScmReportRepository.GetDomesticByCustomer(command);

            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.DomesticByCustomerReportRetrieved
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetDomesticByCustomer failed {@command}", e.Message);
            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ErrorRetrievingDomesticByCustomerReport
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetDomesticRawData(DateTime periodeDate)
    {
        try
        {
            var command = new GetDomesticRawDataCommand
            {
                PeriodeDate = periodeDate
            };

            var result = await _unitOfWork.ScmReportRepository.GetDomesticRawData(command);

            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.DomesticRawDataRetrieved
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetDomesticRawData failed {@command}", e.Message);
            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ErrorRetrievingDomesticRawData
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetExportByProductCustomer(DateTime periodeDate)
    {
        try
        {
            var command = new GetExportByProductCustomerCommand
            {
                PeriodeDate = periodeDate
            };

            var result = await _unitOfWork.ScmReportRepository.GetExportByProductCustomer(command);

            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.ExportByProductCustomerReportRetrieved
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetExportByProductCustomer failed {@command}", e.Message);
            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ErrorRetrievingExportByProductCustomerReport
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetExportByProduct(DateTime periodeDate)
    {
        try
        {
            var command = new GetExportByProductCommand
            {
                PeriodeDate = periodeDate
            };

            var result = await _unitOfWork.ScmReportRepository.GetExportByProduct(command);

            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.ExportByProductReportRetrieved
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetExportByProduct failed {@command}", e.Message);
            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ErrorRetrievingExportByProductReport
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetExportByCustomer(DateTime periodeDate)
    {
        try
        {
            var command = new GetExportByCustomerCommand
            {
                PeriodeDate = periodeDate
            };

            var result = await _unitOfWork.ScmReportRepository.GetExportByCustomer(command);

            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.ExportByCustomerReportRetrieved
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetExportByCustomer failed {@command}", e.Message);
            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ErrorRetrievingExportByCustomerReport
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetExportRawData(DateTime periodeDate)
    {
        try
        {
            var command = new GetExportRawDataCommand
            {
                PeriodeDate = periodeDate
            };

            var result = await _unitOfWork.ScmReportRepository.GetExportRawData(command);

            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.ExportRawDataRetrieved
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetExportRawData failed {@command}", e.Message);
            return new ServiceResponse<IEnumerable<ScmReportDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ErrorRetrievingExportRawData
            };
        }
    }

    public async Task<IEnumerable<int>> GetYearsAsync()
    {
        try
        {
            return await _unitOfWork.ScmReportRepository.GetYearsAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetYearsAsync failed {@command}", e.Message);
            throw new Exception(ServiceMessageConstants.ErrorRetrievingYears);
        }
    }

    public async Task<IEnumerable<int>> GetMonthsAsync(int year)
    {
        try
        {
            return await _unitOfWork.ScmReportRepository.GetMonthsAsync(year);
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetMonthsAsync failed {@command}", e.Message);
            throw new Exception(ServiceMessageConstants.ErrorRetrievingMonths);
        }
    }
}