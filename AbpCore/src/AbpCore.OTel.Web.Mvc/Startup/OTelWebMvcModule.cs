using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpCore.OTel.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AbpCore.OTel.Web.Startup;

[DependsOn(typeof(OTelWebCoreModule))]
[DependsOn(typeof(OTelModule))]
public class OTelWebMvcModule : AbpModule
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfigurationRoot _appConfiguration;

    public OTelWebMvcModule(IWebHostEnvironment env)
    {
        _env = env;
        _appConfiguration = env.GetAppConfiguration();
    }

    public override void PreInitialize()
    {
        //启用、禁用审计日志
        Configuration.Auditing.IsEnabled = false;
        Configuration.Navigation.Providers.Add<OTelNavigationProvider>();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(OTelWebMvcModule).GetAssembly());
    }
}
