using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Castle.Logging.Log4Net;
using AbpCore.OTel.Authentication.JwtBearer;
using AbpCore.OTel.Configuration;
using AbpCore.OTel.Identity;
using AbpCore.OTel.Web.Resources;
using Castle.Facilities.Logging;
using Castle.Services.Logging.SerilogIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace AbpCore.OTel.Web.Startup;

public class Startup
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IConfigurationRoot _appConfiguration;

    public Startup(IWebHostEnvironment env)
    {
        _hostingEnvironment = env;
        _appConfiguration = env.GetAppConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // 集成 OpenTelemetry
        services.AddAbpOpenTelemetry(_appConfiguration, _hostingEnvironment);

        // MVC
        services.AddControllersWithViews(
                options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
                }
            );

        IdentityRegistrar.Register(services);
        AuthConfigurer.Configure(services, _appConfiguration);

        services.Configure<WebEncoderOptions>(options =>
        {
            options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
        });

        services.AddScoped<IWebResourceManager, WebResourceManager>();

        services.AddSignalR();

        // 配置 Swagger
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "AbpCore.OTel API", Version = "v1" });
            options.DocInclusionPredicate((docName, description) =>
            {
                // 排除没有显式 HTTP 方法的 Action (通常是 MVC Controller 的 View 方法，如 Account/Error403)
                if (description.HttpMethod == null)
                {
                    return false;
                }
                return true;
            });

            // 添加 JWT 认证支持
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        //// Configure Abp and Dependency Injection
        //services.AddAbpWithoutCreatingServiceProvider<OTelWebMvcModule>(
        //    // Configure Log4Net logging
        //    options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
        //        f => f.UseAbpLog4Net().WithConfig(
        //            _hostingEnvironment.IsDevelopment()
        //                ? "log4net.config"
        //                : "log4net.Production.config"
        //            )
        //    )
        //);
        services.AddAbpWithoutCreatingServiceProvider<OTelWebMvcModule>(options =>
        {
            options.IocManager.IocContainer.AddFacility<LoggingFacility>(f =>
                f.LogUsing(new SerilogFactory(Log.Logger)));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseAbp(); // Initializes ABP framework.

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseJwtTokenMiddleware();

        app.UseAuthorization();

        // 启用 Swagger 中间件
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "AbpCore.OTel API V1");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<AbpCommonHub>("/signalr");
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
        });
    }
}
