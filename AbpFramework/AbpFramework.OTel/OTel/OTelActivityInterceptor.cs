using Abp.Domain.Uow;
using AbpFramework.OTel.OTel;
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
using System.Threading;

namespace AbpFramework.OTel
{
    public class OTelActivityInterceptor : IInterceptor
    {
        private static readonly ActivitySource _activitySource = new ActivitySource(OTelModule.AspNetSourceName);

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MaxDepth = 3, // 限制序列化深度，防止大对象/深层对象爆内存
            ContractResolver = new IgnoreEFDynamicProxiesResolver() // 防止EF 动态代理属性触发复杂延迟加载
        };

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
            var methodName = string.Empty;
            try
            {
                methodName = method.DeclaringType?.Name + "." + method.Name;
                activity = _activitySource.StartActivity(methodName);
                if (activity != null && activity.IsAllDataRequested == true)
                {
                    var oTelMethodParameters = ConfigurationManager.AppSettings["OTel_RecordMethodParam"] ?? "false";
                    bool.TryParse(oTelMethodParameters, out bool oTelMethodParametersBool);
                    if (!oTelMethodParametersBool)  //不记录参数
                        return;

                    var oTelRecordOnlyControllerParam = ConfigurationManager.AppSettings["OTel_RecordOnlyControllerParam"] ?? "true";
                    bool.TryParse(oTelRecordOnlyControllerParam, out bool oTelRecordOnlyControllerParamBool);
                    bool isController = methodName.IndexOf("Controller", StringComparison.OrdinalIgnoreCase) >= 0;
                    if (oTelRecordOnlyControllerParamBool && !isController)  //只记录控制器参数，当前非控制器
                        return;

                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                        return;

                    if (parameters.Length > 0)
                    {
                        var args = new Dictionary<string, object>();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (invocation.Arguments[i] is CancellationToken)
                                continue;
                            // 参数名、值
                            args[parameters[i].Name] = invocation.GetArgumentValue(i);
                        }
                        if (args.Count > 0)
                        {
                            activity?.SetTag("arguments", JsonConvert.SerializeObject(args, _jsonSettings));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"方法：{methodName}，异常：{ex.Message}", ex);
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
