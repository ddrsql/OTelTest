using Abp.Modules;
using Castle.MicroKernel.Registration;
using System.Configuration;

namespace AbpCore.OTel
{
    public class OTelModule : AbpModule
    {
        public const string AspNetSourceName = "OpenTelemetry.Instrumentation.AspNet.Telemetry";
        public override void PreInitialize()
        {
            var oTelEnabled = ConfigurationManager.AppSettings["OTel_Enabled"] ?? "false";
            bool.TryParse(oTelEnabled, out bool oTelEnabledBool);
            if (oTelEnabledBool)
            {
                Configuration.IocManager.IocContainer.Register(Component.For<OTelActivityInterceptor>());
                OTelActivityInterceptorRegistrar.Initialize(this.IocManager);
            }
        }
    }
}
