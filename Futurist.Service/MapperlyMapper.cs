using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Riok.Mapperly.Abstractions;

namespace Futurist.Service;

[Mapper]
public partial class MapperlyMapper
{
    // Rofo
    public partial Rofo MapToEntity(RofoDto dto);
    public partial RofoDto MapToDto(Rofo entity);
    public partial IEnumerable<Rofo> MapToIEnumerable(IEnumerable<RofoDto> dtos);
    public partial IEnumerable<RofoDto> MapToIEnumerableDto(IEnumerable<Rofo> entities);
    public partial PagedListRequest<Rofo> MapToPagedListRequest(PagedListRequestDto<RofoDto> dto);
    public partial PagedListRequestDto<RofoDto> MapToPagedListRequestDto(PagedListRequest<Rofo> entity);
    public partial PagedList<Rofo> MapToPagedList(PagedListDto<RofoDto> dto);
    public partial PagedListDto<RofoDto> MapToPagedListDto(PagedList<Rofo> entity);
    
    // BomStd
    public partial BomStd MapToEntity(BomStdDto dto);
    public partial BomStdDto MapToDto(BomStd entity);
    public partial IEnumerable<BomStd> MapToIEnumerable(IEnumerable<BomStdDto> dtos);
    public partial IEnumerable<BomStdDto> MapToIEnumerableDto(IEnumerable<BomStd> entities);
    public partial PagedListRequest<BomStd> MapToPagedListRequest(PagedListRequestDto<BomStdDto> dto);
    public partial PagedListRequestDto<BomStdDto> MapToPagedListRequestDto(PagedListRequest<BomStd> entity);
    public partial PagedList<BomStd> MapToPagedList(PagedListDto<BomStdDto> dto);
    public partial PagedListDto<BomStdDto> MapToPagedListDto(PagedList<BomStd> entity);
    
    // MupSp
    public partial MupSp MapToEntity(MupSpDto dto);
    public partial MupSpDto MapToDto(MupSp entity);
    public partial IEnumerable<MupSp> MapToIEnumerable(IEnumerable<MupSpDto> dtos);
    public partial IEnumerable<MupSpDto> MapToIEnumerableDto(IEnumerable<MupSp> entities);
    public partial PagedListRequest<MupSp> MapToPagedListRequest(PagedListRequestDto<MupSpDto> dto);
    public partial PagedListRequestDto<MupSpDto> MapToPagedListRequestDto(PagedListRequest<MupSp> entity);
    public partial PagedList<MupSp> MapToPagedList(PagedListDto<MupSpDto> dto);
    public partial PagedListDto<MupSpDto> MapToPagedListDto(PagedList<MupSp> entity);
    
    // FgCostSp
    public partial FgCostSp MapToEntity(FgCostSpDto dto);
    public partial FgCostSpDto MapToDto(FgCostSp entity);
    public partial IEnumerable<FgCostSp> MapToIEnumerable(IEnumerable<FgCostSpDto> dtos);
    public partial IEnumerable<FgCostSpDto> MapToIEnumerableDto(IEnumerable<FgCostSp> entities);
    public partial PagedListRequest<FgCostSp> MapToPagedListRequest(PagedListRequestDto<FgCostSpDto> dto);
    public partial PagedListRequestDto<FgCostSpDto> MapToPagedListRequestDto(PagedListRequest<FgCostSp> entity);
    public partial PagedList<FgCostSp> MapToPagedList(PagedListDto<FgCostSpDto> dto);
    public partial PagedListDto<FgCostSpDto> MapToPagedListDto(PagedList<FgCostSp> entity);
    
    // FgCostDetailSp
    public partial FgCostDetailSp MapToEntity(FgCostDetailSpDto dto);
    public partial FgCostDetailSpDto MapToDto(FgCostDetailSp entity);
    public partial IEnumerable<FgCostDetailSp> MapToIEnumerable(IEnumerable<FgCostDetailSpDto> dtos);
    public partial IEnumerable<FgCostDetailSpDto> MapToIEnumerableDto(IEnumerable<FgCostDetailSp> entities);
    public partial PagedListRequest<FgCostDetailSp> MapToPagedListRequest(PagedListRequestDto<FgCostDetailSpDto> dto);
    public partial PagedListRequestDto<FgCostDetailSpDto> MapToPagedListRequestDto(PagedListRequest<FgCostDetailSp> entity);
    public partial PagedList<FgCostDetailSp> MapToPagedList(PagedListDto<FgCostDetailSpDto> dto);
    public partial PagedListDto<FgCostDetailSpDto> MapToPagedListDto(PagedList<FgCostDetailSp> entity);
}