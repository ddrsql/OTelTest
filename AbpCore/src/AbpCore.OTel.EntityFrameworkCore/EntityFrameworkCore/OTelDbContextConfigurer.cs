using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace AbpCore.OTel.EntityFrameworkCore;

public static class OTelDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<OTelDbContext> builder, string connectionString)
    {
        //builder.UseSqlServer(connectionString);
        builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    public static void Configure(DbContextOptionsBuilder<OTelDbContext> builder, DbConnection connection)
    {
        //builder.UseSqlServer(connection);
        builder.UseMySql(connection, ServerVersion.AutoDetect(connection.ConnectionString));
    }
}
