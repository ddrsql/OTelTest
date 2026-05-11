using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using AbpCore.OTel.EntityFrameworkCore.Seed;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace AbpCore.OTel.EntityFrameworkCore;

[DependsOn(
    typeof(OTelCoreModule),
    typeof(AbpZeroCoreEntityFrameworkCoreModule))]
public class OTelEntityFrameworkModule : AbpModule
{
    /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
    public bool SkipDbContextRegistration { get; set; }

    public bool SkipDbSeed { get; set; }

    public override void PreInitialize()
    {
        if (!SkipDbContextRegistration)
        {
            Configuration.Modules.AbpEfCore().AddDbContext<OTelDbContext>(options =>
            {
                if (options.ExistingConnection != null)
                {
                    OTelDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                }
                else
                {
                    OTelDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                }
                var interceptor = IocManager.Resolve<TaggedTraceidCommandInterceptor>();
                options.DbContextOptions.AddInterceptors(interceptor);
            });
        }
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(OTelEntityFrameworkModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        if (!SkipDbSeed)
        {
            SeedHelper.SeedHostDb(IocManager);
        }
    }
}
