using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpCore.OTel.EntityFrameworkCore;
using AbpCore.OTel.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace AbpCore.OTel.Web.Tests;

[DependsOn(
    typeof(OTelWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class OTelWebTestModule : AbpModule
{
    public OTelWebTestModule(OTelEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(OTelWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(OTelWebMvcModule).Assembly);
    }
}