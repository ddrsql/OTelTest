using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace VoloAbp.OTel
{
    public class VoloAbpOTelModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            if (context.Services.GetConfiguration().GetValue("OTelOptions:Enabled", false))
                context.Services.OnRegistered(OTelActivityInterceptorRegistrar.RegisterIfNeeded);
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<OTelOptions>(configuration.GetSection(OTelOptions.Key));
        }
    }
}
