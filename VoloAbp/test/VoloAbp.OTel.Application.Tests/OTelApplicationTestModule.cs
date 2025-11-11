using Volo.Abp.Modularity;

namespace VoloAbp.OTel;

[DependsOn(
    typeof(OTelApplicationModule),
    typeof(OTelDomainTestModule)
)]
public class OTelApplicationTestModule : AbpModule
{

}
