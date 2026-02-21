using Abp.AspNetCore.Dependency;
using Abp.Dependency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AbpCore.OTel.Web.Startup;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 集成 Castle Windsor (ABP)
        builder.Host.UseCastleWindsor(IocManager.Instance.IocContainer);

        // 注册 OpenTelemetry
        builder.ConfigureOpenTelemetry();

        // 手动初始化 Startup 以适配 Minimal API
        var startup = new Startup(builder.Environment);
        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app, app.Environment, app.Services.GetRequiredService<ILoggerFactory>());

        app.Run();
    }
}
