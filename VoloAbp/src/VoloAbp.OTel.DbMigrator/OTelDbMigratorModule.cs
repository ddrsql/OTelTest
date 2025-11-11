using VoloAbp.OTel.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace VoloAbp.OTel.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(OTelEntityFrameworkCoreModule),
    typeof(OTelApplicationContractsModule)
)]
public class OTelDbMigratorModule : AbpModule
{
}
