using Abp.Dependency;
using Castle.Core;
using System.Reflection;

namespace AbpCore.OTel
{
    /// <summary>
    /// This class is used to register interceptor for needed classes for OTelActivity mechanism.
    /// </summary>
    public static class OTelActivityInterceptorRegistrar
    {
        /// <summary>
        /// Initializes the registerer.
        /// </summary>
        /// <param name="iocManager">IOC manager</param>
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                var implementationType = handler.ComponentModel.Implementation.GetTypeInfo();
                if (OTelActivityHelper.IsOTelActivityType(implementationType))
                {
                    // 将OTelActivityInterceptor添加到处理程序的拦截器中
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(OTelActivityInterceptor)));
                }
                //HandleTypesWithOTelActivityAttribute(implementationType, handler);
                //HandleConventionalUnitOfWorkTypes(iocManager, implementationType, handler);
            };
        }
        //public static void Initialize(IIocManager iocManager)
        //{
        //    iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
        //    {
        //        // 检查实现类型是否为 IApplicationService 或 IDomainService
        //        if (typeof(IApplicationService).IsAssignableFrom(handler.ComponentModel.Implementation)
        //            || typeof(IDomainService).IsAssignableFrom(handler.ComponentModel.Implementation))
        //        {
        //            // 将 OTelActivityInterceptor 添加到组件的拦截器中
        //            handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(OTelActivityInterceptor)));
        //        }
        //    };
        //}

        ///// <summary>
        ///// 处理带有 OTelActivity 特性的类型。
        ///// </summary>
        ///// <param name="implementationType">要检查的实现类型。</param>
        ///// <param name="handler">要添加拦截器的处理程序。</param>
        //private static void HandleTypesWithOTelActivityAttribute(TypeInfo implementationType, IHandler handler)
        //{
        //    // 检查类型或其方法是否具有 OTelActivity 特性
        //    if (OTelActivityHelper.IsOTelActivityType(implementationType) || OTelActivityHelper.AnyMethodHasOTelActivity(implementationType))
        //    {
        //        // 将OTelActivityInterceptor添加到处理程序的拦截器中
        //        handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(OTelActivityInterceptor)));
        //    }
        //}


        ///// <summary>
        ///// 检查给定的实现类型是否具有OTelActivity特性。
        ///// </summary>
        ///// <param name="implementationType">要检查OTelActivity特性的类型。</param>
        ///// <returns>如果类型具有OTelActivity特性，则返回true，否则返回false。</returns>
        //private static bool IsOTelActivityType(TypeInfo implementationType)
        //{
        //    return OTelActivityHelper.HasOTelActivityAttribute(implementationType);
        //}

        ///// <summary>
        ///// 检查给定的实现类型是否有任何方法具有OTelActivity特性。
        ///// </summary>
        ///// <param name="implementationType">要检查的类型。</param>
        ///// <returns>如果任何方法具有OTelActivity特性，则返回true，否则返回false。</returns>
        //private static bool AnyMethodHasOTelActivityAttribute(TypeInfo implementationType)
        //{
        //    // 获取实现类型的所有实例、公共和非公共方法
        //    // 检查这些方法中是否有任何具有OTelActivity特性的方法
        //    return implementationType
        //        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        //        .Any(OTelActivityHelper.HasOTelActivityAttribute);
        //}
    }
}
