using Castle.Core.Logging;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace AbpCore.OTel
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
        }
    }
}
