using Abp.Modules;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace AbpFramework.OTel
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
