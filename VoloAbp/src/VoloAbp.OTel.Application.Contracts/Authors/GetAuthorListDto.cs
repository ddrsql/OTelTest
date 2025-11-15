using Volo.Abp.Application.Dtos;

namespace VoloAbp.OTel.Authors;

public class GetAuthorListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
