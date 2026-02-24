using Abp.AspNetCore.Dependency;
using Abp.Dependency;
using Abp.Extensions;
using Microsoft.AspNetCore.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace AbpCore.OTel.Web.Startup;

public class Program
{
    public static void Main(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();

        var otlpEnabled = configuration.GetValue<bool>("OTelOptions:Enabled", false);
        var otlpEndpoint = configuration.GetValue<string>("OTelOptions:Endpoint", "http://localhost:4318");
        var endpoint = new Uri(otlpEndpoint);

        var applicationName = Environment.GetEnvironmentVariable(EnvironmentConsts.ASPNETCORE_APPLICATION);
        if (applicationName.IsNullOrWhiteSpace())
        {
            applicationName = Assembly.GetEntryAssembly()?.GetName().Name;
        }

        Log.Logger = new LoggerConfiguration()
            // 将配置传给 Serilog 的提供程序
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = (new Uri(endpoint, "/v1/logs")).ToString();
                options.Protocol = OtlpProtocol.HttpProtobuf;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = applicationName,
                    ["host.name"] = Environment.MachineName,
                    ["deployment.environment"] = environmentName
                };
            })
            .CreateLogger();

        try
        {
            Log.Warning("Runtime Framework: {FrameworkDescription}.", RuntimeInformation.FrameworkDescription);
            Log.Warning("Runtime OSArchitecture: {OSArchitecture}.", RuntimeInformation.OSArchitecture);
            Log.Warning("Runtime OSDescription: {OSDescription}.", RuntimeInformation.OSDescription);
            Log.Warning("Runtime ProcessArchitecture: {ProcessArchitecture}.", RuntimeInformation.ProcessArchitecture);
            // Log.Warning("Runtime RuntimeIdentifier: {RuntimeIdentifier}.", RuntimeInformation.RuntimeIdentifier);


            Log.Warning("Environment: {EnvironmentName}.", environmentName);

            // 处理编码问题
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Log.Information("Starting web host.");

            // 读取HOSTNAME环境变量
            var hostname = Environment.GetEnvironmentVariable("HOSTNAME") ?? "Unknown";

            Log.Warning("HOSTNAME: {hostname}.", hostname);

            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    internal static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseCastleWindsor(IocManager.Instance.IocContainer);
}
