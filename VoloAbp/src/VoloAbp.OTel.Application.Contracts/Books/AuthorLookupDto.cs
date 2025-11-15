using System;
using Volo.Abp.Application.Dtos;

namespace VoloAbp.OTel.Books;

public class AuthorLookupDto : EntityDto<Guid>
{
    public string Name { get; set; }
}
