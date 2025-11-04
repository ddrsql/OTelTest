using Abp.Application.Services;
using Abp.Dependency;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Castle.Core;
using Castle.MicroKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AbpFramework.OTel
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
            };
        }
    }
}
