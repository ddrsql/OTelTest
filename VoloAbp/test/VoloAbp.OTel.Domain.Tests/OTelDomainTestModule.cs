using Volo.Abp.Modularity;

namespace VoloAbp.OTel;

[DependsOn(
    typeof(OTelDomainModule),
    typeof(OTelTestBaseModule)
)]
public class OTelDomainTestModule : AbpModule
{

}
