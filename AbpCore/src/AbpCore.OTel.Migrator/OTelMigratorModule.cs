using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpCore.OTel.Configuration;
using AbpCore.OTel.EntityFrameworkCore;
using AbpCore.OTel.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace AbpCore.OTel.Migrator;

[DependsOn(typeof(OTelEntityFrameworkModule))]
public class OTelMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public OTelMigratorModule(OTelEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(OTelMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            OTelConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(OTelMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
