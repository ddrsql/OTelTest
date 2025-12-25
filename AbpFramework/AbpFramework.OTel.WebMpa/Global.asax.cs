using Abp.Castle.Logging.Log4Net;
using Abp.Web;
using Abp.WebApi.Validation;
using AbpFramework.OTel.WebMpa.Extensions;
using Castle.Facilities.Logging;
using log4net;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web;

namespace AbpFramework.OTel.WebMpa
{
    public class MvcApplication : AbpWebApplication<OTelWebModule>
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            var serviceName = ConfigurationManager.AppSettings["OTel_ServiceName"] ?? typeof(OpenTelemetryExtensions).Namespace;
            var environment = ConfigurationManager.AppSettings["OTel_Environment"];
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(serviceName, serviceNamespace: "ddrsql", serviceVersion: "1.0.0")
                .AddAttributes(new[]
                {
                    new KeyValuePair<string, object>("deployment.environment", environment)
                });

            var otlpEndpoint = new Uri(ConfigurationManager.AppSettings["OTel_Endpoint"]);
            // create an instance for the logger
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddOpenTelemetry(logging =>
                {
                    logging.AddConsoleExporter();
                    logging.SetResourceBuilder(resourceBuilder)
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint, "/v1/logs");
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
                    // ... add other options if you'd like
                });
            });
            LogManager.GetRepository().Properties["ILoggerFactory"] = loggerFactory;
#if DEBUG
            string folderPath = Server.MapPath("~/App_Data/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            // 创建日志文件路径
            string logPath = Server.MapPath("~/App_Data/otel_console.log");
            // 重定向控制台输出到文件
            FileStream fileStream = new FileStream(logPath, FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream) { AutoFlush = true };
            Console.SetOut(writer);
#endif


#if DEBUG
            AbpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseAbpLog4Net().WithConfig(Server.MapPath("log4net.config"))
            );
#else
            AbpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseAbpLog4Net().WithConfig(Server.MapPath("log4net.Production.config"))
            );
#endif

            base.Application_Start(sender, e);
        }
    }
}
