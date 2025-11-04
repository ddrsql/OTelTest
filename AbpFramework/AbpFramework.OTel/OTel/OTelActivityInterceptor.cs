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
        public void Intercept(IInvocation invocation)
        {
            _logger.Error($"方法调用前：{invocation.Method?.DeclaringType?.Name}.{invocation.Method?.Name}");

            // 判断是否启用
            var oTelEnabled = ConfigurationManager.AppSettings["OTel_Enabled"] ?? "false";
            bool.TryParse(oTelEnabled, out bool oTelEnabledBool);
            if (!oTelEnabledBool)
            {
                invocation.Proceed();
                return;
            }

            MethodInfo method;
            try
            {
                method = invocation.MethodInvocationTarget;
            }
            catch
            {
                method = invocation.GetConcreteMethod();
            }


            // 执行过程判断是否需要跟踪
            //if (!OTelActivityHelper.IsOTelActivityMethod(invocation.Method, out var oTelActivityAttribute))
            //{
            //    invocation.Proceed();
            //    return;
            //}

            // https://opentelemetry.io/docs/languages/dotnet/traces/best-practices/
            Activity activity = null;
            try
            {
                activity = _activitySource.StartActivity(invocation.Method.DeclaringType?.Name + "." + invocation.Method.Name);
                if (activity != null && activity.IsAllDataRequested == true)
                {
                    var oTelMethodParameters = ConfigurationManager.AppSettings["OTel_Method_Parameters"] ?? "false";
                    bool.TryParse(oTelMethodParameters, out bool oTelMethodParametersBool);
                    if (oTelMethodParametersBool)
                    {
                        var parameters = invocation.Method.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            // 参数名
                            var name = parameters[i].Name;
                            // 类型
                            //var type = parameters[i].ParameterType;
                            // 值
                            var value = invocation.GetArgumentValue(i);
                            var valueStr = JsonConvert.SerializeObject(value);
                            activity?.SetTag(name, valueStr);
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

            _logger.Error($"方法调用后：{invocation.Method?.DeclaringType?.Name}.{invocation.Method?.Name}");
        }
    }
}
