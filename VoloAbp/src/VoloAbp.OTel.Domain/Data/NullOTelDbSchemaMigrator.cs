using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VoloAbp.OTel.Data;

/* This is used if database provider does't define
 * IOTelDbSchemaMigrator implementation.
 */
public class NullOTelDbSchemaMigrator : IOTelDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
