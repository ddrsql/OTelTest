using Microsoft.AspNetCore.Builder;
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

namespace Microsoft.AspNetCore.Extensions;

public static class OpenTelemetryExtensions
{
    /// <summary>
    /// 配置 OpenTelemetry
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder ConfigureOpenTelemetry(this WebApplicationBuilder builder)
    {
        var endpoint = new Uri(builder.Services.GetConfiguration().GetValue<string>("OTelOptions:Endpoint", ""));
        var ratioSampler = builder.Services.GetConfiguration().GetValue<double>("OTelOptions:RatioSampler", 1.0);
        // 将日志集成到 OpenTelemetry 管道中，并导出到后端（如 OTLP Collector）
        builder.Logging.AddOpenTelemetry(loggerOptions =>
        {
            loggerOptions.IncludeScopes = true;  //启用日志作用域（Scopes），将上下文信息（如请求 ID、用户信息）包含在日志中。
            loggerOptions.ParseStateValues = true;  //将结构化日志参数（如 {UserId}）解析为独立字段，而不是嵌入在文本中。
            loggerOptions.IncludeFormattedMessage = true;  //格式化后的完整日志消息包含在导出的日志中
            loggerOptions.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(endpoint, "/v1/logs");
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            });
            //loggerOptions.AddConsoleExporter();
        });

        var applicationName = Environment.GetEnvironmentVariable(EnvironmentConsts.ASPNETCORE_APPLICATION);
        if (applicationName.IsNullOrWhiteSpace())
        {
            applicationName = builder.Environment.ApplicationName;
        }
        // 配置分布式追踪（Tracing） 和 指标采集（Metrics） 的初始化逻辑
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                    .AddService(applicationName, serviceVersion: "1.0.0")
                    .AddAttributes(new[]
                    {
                        new KeyValuePair<string, object>("deployment.environment", builder.Environment.EnvironmentName)
                    })
                    .AddHostDetector()  //检测运行环境主机信息
                    .AddContainerDetector();  //检测运行环境容器信息
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
                .AddAspNetCoreInstrumentation()
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
        return builder;
    }
}
