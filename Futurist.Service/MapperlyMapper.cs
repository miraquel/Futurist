﻿using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Hangfire.Storage.Monitoring;
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
}