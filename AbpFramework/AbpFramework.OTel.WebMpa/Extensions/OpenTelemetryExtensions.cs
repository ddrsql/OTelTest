using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AbpFramework.OTel.WebMpa.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static void AddOpenTelemetry()
        {
            //Environment.SetEnvironmentVariable("OTEL_RESOURCE_ATTRIBUTES", "service.name=AbpFrameworkOTel,service.version=1.0.0,deployment.environment=local");

            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService("AbpFrameworkOTel", serviceNamespace: "ddrsql", serviceVersion: "1.0.0")
                .AddAttributes(new[]
                {
                    new KeyValuePair<string, object>("deployment.environment", "local")
                });

            var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddSource(OTelModule.AspNetSourceName)
                .AddAspNetInstrumentation()
                .AddHttpClientInstrumentation()
                .SetResourceBuilder(resourceBuilder)
                //.AddSource("MyCompany.MyProduct.MyLibrary", "OpenTelemetry.Instrumentation.AspNet.Telemetry")  //设置 OpenTelemetry SDK 时，告诉它要监听哪些 ActivitySource 的事件。
                .AddConsoleExporter()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri("http://localhost:4318/v1/traces");
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                })
                .Build();

            var meterProvider = Sdk.CreateMeterProviderBuilder()
                .AddAspNetInstrumentation()
                .AddHttpClientInstrumentation()
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri("http://localhost:4318/v1/metrics");
                })
                .AddConsoleExporter()
                .Build();

            var resource = ResourceBuilder.CreateDefault().Build();
            var attrStr = string.Empty;
            foreach (var attribute in resource.Attributes)
            {
                attrStr += $"{attribute.Key} = {attribute.Value}；";
                Console.WriteLine("OTEL环境变量：" + attrStr);
            }
        }
    }
}