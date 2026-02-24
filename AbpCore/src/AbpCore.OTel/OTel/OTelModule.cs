using Abp.Modules;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace AbpCore.OTel
{
    public class OTelModule : AbpModule
    {
        public const string AspNetSourceName = "OpenTelemetry.Instrumentation.AspNet.Telemetry";
        public override void PreInitialize()
        {
            var configuration = IocManager.Resolve<IConfiguration>();
            var oTelEnabled = configuration.GetValue<bool>("OTelOptions:Enabled");
            if (oTelEnabled)
            {
                Configuration.IocManager.IocContainer.Register(Component.For<OTelActivityInterceptor>());
                OTelActivityInterceptorRegistrar.Initialize(this.IocManager);
            }
        }
    }
}
