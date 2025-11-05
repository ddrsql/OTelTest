using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpCore.OTel.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AbpCore.OTel.Web.Host.Startup
{
    [DependsOn(
       typeof(OTelWebCoreModule))]
    public class OTelWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public OTelWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(OTelWebHostModule).GetAssembly());
        }
    }
}
