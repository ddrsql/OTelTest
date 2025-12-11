using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace VoloAbp.OTel.Web;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        // 启动阶段临时日志器，应用最早执行，用来捕获启动日志 仅在启动阶段使用
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host
                .AddAppSettingsSecretsJson()
                .UseAutofac();

            //.UseSerilog((context, services, loggerConfiguration) =>
            //{
            //    loggerConfiguration
            //    #if DEBUG
            //        .MinimumLevel.Debug()
            //    #else
            //        .MinimumLevel.Information()
            //    #endif
            //        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            //        .Enrich.FromLogContext()
            //        .WriteTo.Async(c => c.File("Logs/logs.txt"))
            //        .WriteTo.Async(c => c.Console())
            //        .WriteTo.Async(c => c.AbpStudio(services));
            //});
            builder.Logging.ClearProviders(); // 清空默认日志提供程序
            //Add support to logging with SERILOG
            // 应用运行时正式日志器 Host 构建时，替换临时日志器
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            }, writeToProviders: true);

            if (builder.Services.GetConfiguration().GetValue("OTelOptions:Enabled", false))
            {
                builder.ConfigureOpenTelemetry();
            }
            await builder.AddApplicationAsync<OTelWebModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
