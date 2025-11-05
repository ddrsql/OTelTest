using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace AbpCore.OTel.EntityFrameworkCore;

public static class OTelDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<OTelDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<OTelDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
