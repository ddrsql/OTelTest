using Microsoft.Extensions.Localization;
using Volo.Abp.Domain.Services;
using VoloAbp.OTel.Localization;

namespace VoloAbp.OTel;

public abstract class OTelDomainService : DomainService
{
    protected IStringLocalizer<OTelResource> L => LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<OTelResource>>();
    protected OTelDomainService()
    {
    }
}
