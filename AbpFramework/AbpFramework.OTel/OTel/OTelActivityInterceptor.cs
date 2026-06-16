using Abp.Domain.Uow;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using OpenTelemetry.Instrumentation.AspNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;

namespace AbpFramework.OTel
{
    public class OTelActivityInterceptor : IInterceptor
    {
        private static readonly ActivitySource _activitySource = new ActivitySource(OTelModule.AspNetSourceName);

        private readonly ILogger _logger;
        public OTelActivityInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        private static MethodInfo GetMethodInfo(IInvocation invocation)
        {
            MethodInfo method;
            try
            {
                method = invocation.MethodInvocationTarget;
            }
            catch
            {
                method = invocation.GetConcreteMethod();
            }

            return method;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = GetMethodInfo(invocation);

            // 判断是否启用
            var oTelEnabled = ConfigurationManager.AppSettings["OTel_Enabled"] ?? "false";
            bool.TryParse(oTelEnabled, out bool oTelEnabledBool);
            if (!oTelEnabledBool)
            {
                invocation.Proceed();
                return;
            }

            // 执行过程判断是否需要跟踪
            if (!OTelActivityHelper.IsOTelActivityMethod(method, out var oTelActivityAttribute))
            {
                invocation.Proceed();
                return;
            }

            // https://opentelemetry.io/docs/languages/dotnet/traces/best-practices/
            Activity activity = null;
            try
            {
                activity = _activitySource.StartActivity(method.DeclaringType?.Name + "." + method.Name);
                if (activity != null && activity.IsAllDataRequested == true)
                {
                    var oTelMethodParameters = ConfigurationManager.AppSettings["OTel_RecordMethodParam"] ?? "false";
                    bool.TryParse(oTelMethodParameters, out bool oTelMethodParametersBool);
                    if (oTelMethodParametersBool)
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length > 0)
                        {
                            var args = new Dictionary<string, object>();
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                // 参数名、值
                                args[parameters[i].Name] = invocation.GetArgumentValue(i);
                            }
                            if (args.Count > 0)
                            {
                                try
                                {
                                    activity?.SetTag("arguments", JsonConvert.SerializeObject(args));
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error(ex.ToString());
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                invocation.Proceed(); // 执行原方法
                activity?.Stop();
                activity?.Dispose();
            }
        }
    }
}
