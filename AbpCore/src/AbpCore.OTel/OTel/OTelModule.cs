using Abp.Modules;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AbpCore.OTel
{
    public class OTelModule : AbpModule
    {
        public const string AspNetSourceName = "OpenTelemetry.Instrumentation.AspNet.Telemetry";
        public override void PreInitialize()
        {
            var configuration = IocManager.Resolve<IConfiguration>();
            var options = new OTelOptions();
            configuration.GetSection(OTelOptions.Key).Bind(options);

            IocManager.IocContainer.Register(
                Component.For<IOptions<OTelOptions>>()
                    .Instance(Options.Create(options))
                    .LifestyleSingleton()
            );

            if (options.Enabled)
            {
                Configuration.IocManager.IocContainer.Register(Component.For<OTelActivityInterceptor>());
                OTelActivityInterceptorRegistrar.Initialize(this.IocManager);
            }
        }
    }
}
