using Volo.Abp.Modularity;

namespace VoloAbp.OTel;

public abstract class OTelApplicationTestBase<TStartupModule> : OTelTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
