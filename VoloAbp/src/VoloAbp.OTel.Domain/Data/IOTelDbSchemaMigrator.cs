using System.Threading.Tasks;

namespace VoloAbp.OTel.Data;

public interface IOTelDbSchemaMigrator
{
    Task MigrateAsync();
}
