using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VoloAbp.OTel.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class OTelDbContextFactory : IDesignTimeDbContextFactory<OTelDbContext>
{
    public OTelDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        OTelEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<OTelDbContext>()
            .UseMySQL(configuration.GetConnectionString("Default"));
        
        return new OTelDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../VoloAbp.OTel.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
