using AbpFramework.OTel.Migrations;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Configuration;
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

            var otlpEndpoint = new Uri(ConfigurationManager.AppSettings["OTel_Endpoint"]);
            var oTelRatioSampler = 1.0;
            double.TryParse(ConfigurationManager.AppSettings["OTel_RatioSampler"], out oTelRatioSampler);
            var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetSampler(new TraceIdRatioBasedSampler(oTelRatioSampler))  // 设置采样率
                .SetResourceBuilder(resourceBuilder)
                .AddSource(OTelModule.AspNetSourceName)
                //.AddAspNetInstrumentation()
                .AddAspNetInstrumentation(options =>
                {
                    options.EnrichWithHttpRequest = (activity, rawObject) =>
                    {
                        if (rawObject is HttpRequestBase request)
                        {
                            activity.DisplayName = $"{request.HttpMethod} {request.Url.AbsolutePath}";
                            activity.SetTag("http.route", request.Url.AbsolutePath);
                            activity.SetTag("name", request.Url.AbsolutePath);
                            activity.SetTag("test", request.Url.AbsolutePath);
                        }
                    };
                    options.EnrichWithHttpResponse = (activity, rawObject) =>
                    {
                        if (rawObject is HttpResponseBase response)
                        {
                            var httpMethod = activity.GetTagItem("http.request.method");
                            var urlPath = activity.GetTagItem("url.path");
                            activity.DisplayName = httpMethod + " " + urlPath;
                        }
                    };
                })
                .AddHttpClientInstrumentation()
                //.AddSqlClientInstrumentation()  // MSSQL
                //.AddSource("MyCompany.MyProduct.MyLibrary", "OpenTelemetry.Instrumentation.AspNet.Telemetry")  //设置 OpenTelemetry SDK 时，告诉它要监听哪些 ActivitySource 的事件。
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint, "/v1/traces");
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                })
                .AddConsoleExporter()
                .Build();

            var meterProvider = Sdk.CreateMeterProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetInstrumentation()
                .AddHttpClientInstrumentation()
                //.AddSqlClientInstrumentation()  // MSSQL
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint, "/v1/metrics");
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
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