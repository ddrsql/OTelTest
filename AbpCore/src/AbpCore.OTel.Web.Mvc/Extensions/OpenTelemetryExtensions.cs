using Abp.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.AspNetCore.Extensions;

public static class OpenTelemetryExtension
{
    /// <summary>
    /// 配置 OpenTelemetry
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAbpOpenTelemetry(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (!configuration.GetValue<bool>("OTelOptions:Enabled", false))
        {
            return services;
        }
        var endpoint = new Uri(configuration.GetValue<string>("OTelOptions:Endpoint", ""));
        var ratioSampler = configuration.GetValue<double>("OTelOptions:RatioSampler", 1.0);
        // 将日志集成到 OpenTelemetry 管道中，并导出到后端（如 OTLP Collector）
        //services.AddOpenTelemetry(loggerOptions =>
        //{
        //    loggerOptions.IncludeScopes = true;  //启用日志作用域（Scopes），将上下文信息（如请求 ID、用户信息）包含在日志中。
        //    loggerOptions.ParseStateValues = true;  //将结构化日志参数（如 {UserId}）解析为独立字段，而不是嵌入在文本中。
        //    loggerOptions.IncludeFormattedMessage = true;  //格式化后的完整日志消息包含在导出的日志中
        //    loggerOptions.AddOtlpExporter(options =>
        //    {
        //        options.Endpoint = new Uri(endpoint, "/v1/logs");
        //        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
        //    });
        //    //loggerOptions.AddConsoleExporter();
        //});

        var applicationName = Environment.GetEnvironmentVariable(EnvironmentConsts.ASPNETCORE_APPLICATION);
        if (applicationName.IsNullOrWhiteSpace())
        {
            applicationName = Assembly.GetEntryAssembly()?.GetName().Name;
        }
        // 配置分布式追踪（Tracing） 和 指标采集（Metrics） 的初始化逻辑
        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                    .AddService(applicationName, serviceVersion: "1.0.0")
                    .AddAttributes(new[]
                    {
                        new KeyValuePair<string, object>("deployment.environment", environment.EnvironmentName)
                    });
            })
            .WithTracing(tracerBuilder =>
            {
                tracerBuilder
                .SetSampler(new TraceIdRatioBasedSampler(ratioSampler))  // 设置采样率
                .AddEntityFrameworkCoreInstrumentation()
                .AddHangfireInstrumentation()
                .AddRedisInstrumentation(options => { options.SetVerboseDatabaseStatements = true; })
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.EnrichWithHttpResponse = (activity, response) =>
                    {
                        // 安全头列表
                        var allowedHeaders = new HashSet<string> { "Content-Type", "X-Trace-Id" };
                        foreach (var header in response.Headers)
                        {
                            //if (allowedHeaders.Contains(header.Key))
                            //{
                            activity.SetTag($"http.response.header.{header.Key}", header.Value);
                            //}
                        }
                    };
                    options.RecordException = true;  // 记录异常
                })
                .AddGrpcClientInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(endpoint, "/v1/traces");
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                });
                //tracerBuilder.AddConsoleExporter();
            })
            .WithMetrics(meterBuilder =>
            {
                meterBuilder
                .AddProcessInstrumentation()
                .AddRuntimeInstrumentation()
                // 使用 AddMeter 替代 AddAspNetCoreInstrumentation 以避免路由约束冲突
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                .AddMeter("Microsoft.AspNetCore.Http.Connections")
                .AddMeter("Microsoft.AspNetCore.Routing")
                .AddMeter("Microsoft.AspNetCore.Diagnostics")
                .AddMeter("Microsoft.AspNetCore.Mvc")
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(endpoint, "/v1/metrics");
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                });
                //meterBuilder.AddConsoleExporter();
            });

        var resource = ResourceBuilder.CreateDefault().Build();
        foreach (var attribute in resource.Attributes)
        {
            Console.WriteLine($"{attribute.Key} = {attribute.Value}");
        }
        return services;
    }
}
