using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Riok.Mapperly.Abstractions;

namespace Futurist.Service;

[Mapper]
public partial class MapperlyMapper
{
    // Common
    public partial PagedListRequest MapToPagedListRequest(PagedListRequestDto dto);
    public partial PagedListRequestDto MapToPagedListRequestDto(PagedListRequest entity);
    public partial ListRequest MapToListRequest(ListRequestDto dto);
    public partial ListRequestDto MapToListRequestDto(ListRequest entity);
    
    // Rofo
    public partial Rofo MapToEntity(RofoDto dto);
    public partial RofoDto MapToDto(Rofo entity);
    public partial IEnumerable<Rofo> MapToIEnumerable(IEnumerable<RofoDto> dtos);
    public partial IEnumerable<RofoDto> MapToIEnumerableDto(IEnumerable<Rofo> entities);
    public partial PagedList<Rofo> MapToPagedList(PagedListDto<RofoDto> dto);
    public partial PagedListDto<RofoDto> MapToPagedListDto(PagedList<Rofo> entity);
    
    // BomStd
    public partial BomStd MapToEntity(BomStdDto dto);
    public partial BomStdDto MapToDto(BomStd entity);
    public partial IEnumerable<BomStd> MapToIEnumerable(IEnumerable<BomStdDto> dtos);
    public partial IEnumerable<BomStdDto> MapToIEnumerableDto(IEnumerable<BomStd> entities);
    public partial PagedList<BomStd> MapToPagedList(PagedListDto<BomStdDto> dto);
    public partial PagedListDto<BomStdDto> MapToPagedListDto(PagedList<BomStd> entity);
    
    // MupSp
    public partial MupSp MapToEntity(MupSpDto dto);
    public partial MupSpDto MapToDto(MupSp entity);
    public partial IEnumerable<MupSp> MapToIEnumerable(IEnumerable<MupSpDto> dtos);
    public partial IEnumerable<MupSpDto> MapToIEnumerableDto(IEnumerable<MupSp> entities);
    public partial PagedList<MupSp> MapToPagedList(PagedListDto<MupSpDto> dto);
    public partial PagedListDto<MupSpDto> MapToPagedListDto(PagedList<MupSp> entity);
    
    // FgCostSp
    public partial FgCostSp MapToEntity(FgCostSpDto dto);
    public partial FgCostSpDto MapToDto(FgCostSp entity);
    public partial IEnumerable<FgCostSp> MapToIEnumerable(IEnumerable<FgCostSpDto> dtos);
    public partial IEnumerable<FgCostSpDto> MapToIEnumerableDto(IEnumerable<FgCostSp> entities);
    public partial PagedList<FgCostSp> MapToPagedList(PagedListDto<FgCostSpDto> dto);
    public partial PagedListDto<FgCostSpDto> MapToPagedListDto(PagedList<FgCostSp> entity);
    
    // FgCostDetailSp
    public partial FgCostDetailSp MapToEntity(FgCostDetailSpDto dto);
    public partial FgCostDetailSpDto MapToDto(FgCostDetailSp entity);
    public partial IEnumerable<FgCostDetailSp> MapToIEnumerable(IEnumerable<FgCostDetailSpDto> dtos);
    public partial IEnumerable<FgCostDetailSpDto> MapToIEnumerableDto(IEnumerable<FgCostDetailSp> entities);
    public partial PagedList<FgCostDetailSp> MapToPagedList(PagedListDto<FgCostDetailSpDto> dto);
    public partial PagedListDto<FgCostDetailSpDto> MapToPagedListDto(PagedList<FgCostDetailSp> entity);
    
    // Job Monitoring, ignore duration
    public partial JobMonitoring MapToEntity(JobMonitoringDto dto);
    public partial JobMonitoringDto MapToDto(JobMonitoring entity);
    public partial IEnumerable<JobMonitoring> MapToIEnumerable(IEnumerable<JobMonitoringDto> dtos);
    public partial IEnumerable<JobMonitoringDto> MapToIEnumerableDto(IEnumerable<JobMonitoring> entities);
    public partial PagedList<JobMonitoring> MapToPagedList(PagedListDto<JobMonitoringDto> dto);
    public partial PagedListDto<JobMonitoringDto> MapToPagedListDto(PagedList<JobMonitoring> entity);
    
    // Exchange Rate Sp
    public partial ExchangeRateSp MapToEntity(ExchangeRateSpDto dto);
    public partial ExchangeRateSpDto MapToDto(ExchangeRateSp entity);
    public partial IEnumerable<ExchangeRateSp> MapToIEnumerable(IEnumerable<ExchangeRateSpDto> dtos);
    public partial IEnumerable<ExchangeRateSpDto> MapToIEnumerableDto(IEnumerable<ExchangeRateSp> entities);
    public partial PagedList<ExchangeRateSp> MapToPagedList(PagedListDto<ExchangeRateSpDto> dto);
    public partial PagedListDto<ExchangeRateSpDto> MapToPagedListDto(PagedList<ExchangeRateSp> entity);
    
    // SpTask
    public partial SpTask MapToEntity(SpTaskDto dto);
    public partial SpTaskDto MapToDto(SpTask entity);
    public partial IEnumerable<SpTask> MapToIEnumerable(IEnumerable<SpTaskDto> dtos);
    public partial IEnumerable<SpTaskDto> MapToIEnumerableDto(IEnumerable<SpTask> entities);
    public partial PagedList<SpTask> MapToPagedList(PagedListDto<SpTaskDto> dto);
    public partial PagedListDto<SpTaskDto> MapToPagedListDto(PagedList<SpTask> entity);
    
    // Item Adjustment
    public partial ItemAdjustment MapToEntity(ItemAdjustmentDto dto);
    public partial ItemAdjustmentDto MapToDto(ItemAdjustment entity);
    public partial IEnumerable<ItemAdjustment> MapToIEnumerable(IEnumerable<ItemAdjustmentDto> dtos);
    public partial IEnumerable<ItemAdjustmentDto> MapToIEnumerableDto(IEnumerable<ItemAdjustment> entities);
    public partial PagedList<ItemAdjustment> MapToPagedList(PagedListDto<ItemAdjustmentDto> dto);
    public partial PagedListDto<ItemAdjustmentDto> MapToPagedListDto(PagedList<ItemAdjustment> entity);
    
    // FgCostVerSp
    public partial FgCostVerSp MapToEntity(FgCostVerSpDto dto);
    public partial FgCostVerSpDto MapToDto(FgCostVerSp entity);
    public partial IEnumerable<FgCostVerSp> MapToIEnumerable(IEnumerable<FgCostVerSpDto> dtos);
    public partial IEnumerable<FgCostVerSpDto> MapToIEnumerableDto(IEnumerable<FgCostVerSp> entities);
    public partial PagedList<FgCostVerSp> MapToPagedList(PagedListDto<FgCostVerSpDto> dto);
    public partial PagedListDto<FgCostVerSpDto> MapToPagedListDto(PagedList<FgCostVerSp> entity);
    
    // ItemForecastSp
    public partial ItemForecastSp MapToEntity(ItemForecastSpDto dto);
    public partial ItemForecastSpDto MapToDto(ItemForecastSp entity);
    public partial IEnumerable<ItemForecastSp> MapToIEnumerable(IEnumerable<ItemForecastSpDto> dtos);
    public partial IEnumerable<ItemForecastSpDto> MapToIEnumerableDto(IEnumerable<ItemForecastSp> entities);
    public partial PagedList<ItemForecastSp> MapToPagedList(PagedListDto<ItemForecastSpDto> dto);
    public partial PagedListDto<ItemForecastSpDto> MapToPagedListDto(PagedList<ItemForecastSp> entity);
    
    // ScmReport
    public partial ScmReport MapToEntity(ScmReportDto dto);
    public partial ScmReportDto MapToDto(ScmReport entity);
    public partial IEnumerable<ScmReport> MapToIEnumerable(IEnumerable<ScmReportDto> dtos);
    public partial IEnumerable<ScmReportDto> MapToIEnumerableDto(IEnumerable<ScmReport> entities);
    public partial PagedList<ScmReport> MapToPagedList(PagedListDto<ScmReportDto> dto);
    public partial PagedListDto<ScmReportDto> MapToPagedListDto(PagedList<ScmReport> entity);
    
    // Versions
    public partial Versions MapToEntity(VersionsDto dto);
    public partial VersionsDto MapToDto(Versions entity);
    public partial IEnumerable<Versions> MapToIEnumerable(IEnumerable<VersionsDto> dtos);
    public partial IEnumerable<VersionsDto> MapToIEnumerableDto(IEnumerable<Versions> entities);
    public partial PagedList<Versions> MapToPagedList(PagedListDto<VersionsDto> dto);
    public partial PagedListDto<VersionsDto> MapToPagedListDto(PagedList<Versions> entity);
    
    // MupVerSp
    public partial MupVerSp MapToEntity(MupVerSpDto dto);
    public partial MupVerSpDto MapToDto(MupVerSp entity);
    public partial IEnumerable<MupVerSp> MapToIEnumerable(IEnumerable<MupVerSpDto> dtos);
    public partial IEnumerable<MupVerSpDto> MapToIEnumerableDto(IEnumerable<MupVerSp> entities);
    public partial PagedList<MupVerSp> MapToPagedList(PagedListDto<MupVerSpDto> dto);
    public partial PagedListDto<MupVerSpDto> MapToPagedListDto(PagedList<MupVerSp> entity);
    
    // FgCostVerDetailSp
    public partial FgCostVerDetailSp MapToEntity(FgCostVerDetailSpDto dto);
    public partial FgCostVerDetailSpDto MapToDto(FgCostVerDetailSp entity);
    public partial IEnumerable<FgCostVerDetailSp> MapToIEnumerable(IEnumerable<FgCostVerDetailSpDto> dtos);
    public partial IEnumerable<FgCostVerDetailSpDto> MapToIEnumerableDto(IEnumerable<FgCostVerDetailSp> entities);
    public partial PagedList<FgCostVerDetailSp> MapToPagedList(PagedListDto<FgCostVerDetailSpDto> dto);
    public partial PagedListDto<FgCostVerDetailSpDto> MapToPagedListDto(PagedList<FgCostVerDetailSp> entity);
    
    // AnlRmPrice
    public partial AnlRmPrice MapToEntity(AnlRmPriceDto dto);
    public partial AnlRmPriceDto MapToDto(AnlRmPrice entity);
    public partial IEnumerable<AnlRmPrice> MapToIEnumerable(IEnumerable<AnlRmPriceDto> dtos);
    public partial IEnumerable<AnlRmPriceDto> MapToIEnumerableDto(IEnumerable<AnlRmPrice> entities);
    public partial PagedList<AnlRmPrice> MapToPagedList(PagedListDto<AnlRmPriceDto> dto);
    public partial PagedListDto<AnlRmPriceDto> MapToPagedListDto(PagedList<AnlRmPrice> entity);
    
    // AnlKurs
    public partial AnlKurs MapToEntity(AnlKursDto dto);
    public partial AnlKursDto MapToDto(AnlKurs entity);
    public partial IEnumerable<AnlKurs> MapToIEnumerable(IEnumerable<AnlKursDto> dtos);
    public partial IEnumerable<AnlKursDto> MapToIEnumerableDto(IEnumerable<AnlKurs> entities);
    public partial PagedList<AnlKurs> MapToPagedList(PagedListDto<AnlKursDto> dto);
    public partial PagedListDto<AnlKursDto> MapToPagedListDto(PagedList<AnlKurs> entity);
    
    // AnlFgPrice
    public partial AnlFgPrice MapToEntity(AnlFgPriceDto dto);
    public partial AnlFgPriceDto MapToDto(AnlFgPrice entity);
    public partial IEnumerable<AnlFgPrice> MapToIEnumerable(IEnumerable<AnlFgPriceDto> dtos);
    public partial IEnumerable<AnlFgPriceDto> MapToIEnumerableDto(IEnumerable<AnlFgPrice> entities);
    public partial PagedList<AnlFgPrice> MapToPagedList(PagedListDto<AnlFgPriceDto> dto);
    
    // AnlFgPrice
    public partial PagedListDto<AnlFgPriceDto> MapToPagedListDto(PagedList<AnlFgPrice> entity);
    public partial AnlPmPrice MapToEntity(AnlPmPriceDto dto);
    public partial AnlPmPriceDto MapToDto(AnlPmPrice entity);
    public partial IEnumerable<AnlPmPrice> MapToIEnumerable(IEnumerable<AnlPmPriceDto> dtos);
    public partial IEnumerable<AnlPmPriceDto> MapToIEnumerableDto(IEnumerable<AnlPmPrice> entities);
    public partial PagedList<AnlPmPrice> MapToPagedList(PagedListDto<AnlPmPriceDto> dto);
    
    // ItemOnHand
    public partial ItemOnHand MapToEntity(ItemOnHandDto dto);
    public partial ItemOnHandDto MapToDto(ItemOnHand entity);
    public partial IEnumerable<ItemOnHand> MapToIEnumerable(IEnumerable<ItemOnHandDto> dtos);
    public partial IEnumerable<ItemOnHandDto> MapToIEnumerableDto(IEnumerable<ItemOnHand> entities);
    public partial PagedList<ItemOnHand> MapToPagedList(PagedListDto<ItemOnHandDto> dto);
    public partial PagedListDto<ItemOnHandDto> MapToPagedListDto(PagedList<ItemOnHand> entity);
    
    // ItemPoIntransit
    public partial ItemPoIntransit MapToEntity(ItemPoIntransitDto dto);
    public partial ItemPoIntransitDto MapToDto(ItemPoIntransit entity);
    public partial IEnumerable<ItemPoIntransit> MapToIEnumerable(IEnumerable<ItemPoIntransitDto> dtos);
    public partial IEnumerable<ItemPoIntransitDto> MapToIEnumerableDto(IEnumerable<ItemPoIntransit> entities);
    public partial PagedList<ItemPoIntransit> MapToPagedList(PagedListDto<ItemPoIntransitDto> dto);
    public partial PagedListDto<ItemPoIntransitDto> MapToPagedListDto(PagedList<ItemPoIntransit> entity);
    
    // ItemPag
    public partial ItemPag MapToEntity(ItemPagDto dto);
    public partial ItemPagDto MapToDto(ItemPag entity);
    public partial IEnumerable<ItemPag> MapToIEnumerable(IEnumerable<ItemPagDto> dtos);
    public partial IEnumerable<ItemPagDto> MapToIEnumerableDto(IEnumerable<ItemPag> entities);
    public partial PagedList<ItemPag> MapToPagedList(PagedListDto<ItemPagDto> dto);
    public partial PagedListDto<ItemPagDto> MapToPagedListDto(PagedList<ItemPag> entity);
    
    // AnlRmPriceGroup
    public partial AnlRmPriceGroup MapToEntity(AnlRmPriceGroupDto dto);
    public partial AnlRmPriceGroupDto MapToDto(AnlRmPriceGroup entity);
    public partial IEnumerable<AnlRmPriceGroup> MapToIEnumerable(IEnumerable<AnlRmPriceGroupDto> dtos);
    public partial IEnumerable<AnlRmPriceGroupDto> MapToIEnumerableDto(IEnumerable<AnlRmPriceGroup> entities);
    public partial PagedList<AnlRmPriceGroup> MapToPagedList(PagedListDto<AnlRmPriceGroupDto> dto);
    public partial PagedListDto<AnlRmPriceGroupDto> MapToPagedListDto(PagedList<AnlRmPriceGroup> entity);
}