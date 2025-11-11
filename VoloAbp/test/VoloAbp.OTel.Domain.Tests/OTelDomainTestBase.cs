using Volo.Abp.Modularity;

namespace VoloAbp.OTel;

/* Inherit from this class for your domain layer tests. */
public abstract class OTelDomainTestBase<TStartupModule> : OTelTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
