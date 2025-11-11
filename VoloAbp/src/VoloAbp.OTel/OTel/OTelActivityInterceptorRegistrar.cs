using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Uow;

namespace VoloAbp.OTel
{
    public static class OTelActivityInterceptorRegistrar
    {
        public static void RegisterIfNeeded(IOnServiceRegistredContext context)
        {
            if (ShouldIntercept(context.ImplementationType))
            {
                context.Interceptors.TryAdd<OTelActivityInterceptor>();
            }
        }

        /// <summary>
        /// 确定指定类型是否应该被拦截。
        /// </summary>
        /// <param name="type">要检查的类型。</param>
        /// <returns>如果类型应该被拦截，则为 true；否则为 false。</returns>
        private static bool ShouldIntercept(Type type)
        {
            // DynamicProxyIgnoreTypes 判断某个类型是否在“动态代理忽略列表”中
            // 判断某个类型是否需要被拦截
            // 1. 检查类型是否在“动态代理忽略列表”中
            // 2. 检查类型是否是 OTel 活动类型
            return !DynamicProxyIgnoreTypes.Contains(type) && OTelActivityHelper.IsOTelActivityType(type.GetTypeInfo());
        }
    }
}
