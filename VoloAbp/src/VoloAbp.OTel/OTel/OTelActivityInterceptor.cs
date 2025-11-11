using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Json;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace VoloAbp.OTel
{
    public class OTelActivityInterceptor : AbpInterceptor, ITransientDependency
    {
        private readonly ActivitySource _activitySource;
        //private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger<OTelActivityInterceptor> _logger;
        private readonly IConfiguration _configuration;
        private readonly OTelOptions _oTelOptions;

        public OTelActivityInterceptor(
            ActivitySource activitySource,
            //IJsonSerializer jsonSerializer, 
            ILogger<OTelActivityInterceptor> logger,
            IConfiguration configuration,
            IOptionsSnapshot<OTelOptions> oTelOptions
            )
        {
            _activitySource = activitySource;
            //_jsonSerializer = jsonSerializer;
            _logger = logger;
            _configuration = configuration;
            _oTelOptions = oTelOptions.Value;
        }

        public override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            _logger.LogDebug($"方法调用前：{invocation.Method.DeclaringType?.Name}.{invocation.Method.Name}");
            if (!_oTelOptions.Enabled)
            {
                await invocation.ProceedAsync();
                return;
            }

            if (!OTelActivityHelper.IsOTelActivityMethod(invocation.Method, out var oTelActivityAttribute))
            {
                await invocation.ProceedAsync();
                return;
            }

            // https://opentelemetry.io/docs/languages/dotnet/traces/best-practices/
            Activity activity = null;
            try
            {
                activity = _activitySource.StartActivity(invocation.Method.DeclaringType?.Name + "." + invocation.Method.Name);
                //if (activity != null && activity.IsAllDataRequested == true)
                //{
                //    activity.SetTag("", "");
                //}
            }
            finally
            {
                await invocation.ProceedAsync();
                activity?.Stop();
                activity?.Dispose();
            }

            _logger.LogDebug($"方法调用后：{invocation.Method.DeclaringType?.Name}.{invocation.Method.Name}");
        }
    }
}
