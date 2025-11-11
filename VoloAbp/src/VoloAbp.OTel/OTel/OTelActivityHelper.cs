using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Volo.Abp;
using Volo.Abp.Uow;

namespace VoloAbp.OTel
{
    public static class OTelActivityHelper
    {
        /// <summary>
        /// 确定给定类型是否为OpenTelemetry活动类型。
        /// </summary>
        /// <param name="implementationType">要检查的类型。</param>
        /// <returns>如果类型为OpenTelemetry活动类型，则返回true；否则返回false。</returns>
        public static bool IsOTelActivityType(TypeInfo implementationType)
        {
            //Explicitly defined OTelActivityAttribute
            // 检查类型是否具有OTelActivityAttribute或其方法之一具有该属性
            if (HasOTelActivityAttribute(implementationType) || AnyMethodHasOTelActivityAttribute(implementationType))
            {
                Console.WriteLine("YYYYYYYYYYYYYYY=====" + implementationType.FullName);
                return true;
            }

            //Conventional classes
            // 检查类型是否实现IOTelActivityEnabled接口
            if (typeof(IOTelActivityEnabled).GetTypeInfo().IsAssignableFrom(implementationType))
            {
                Console.WriteLine("YYYYYYYYYYYYYYY=====" + implementationType.FullName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 确定指定方法是否为OTel活动方法。
        /// </summary>
        /// <param name="methodInfo">要检查的方法。</param>
        /// <param name="oTelActivityAttribute">与方法关联的OTel活动属性，若未找到则为null。</param>
        /// <returns>如果方法是OTel活动方法，则为true；否则为false。</returns>
        public static bool IsOTelActivityMethod([NotNull] MethodInfo methodInfo, out OTelActivityAttribute oTelActivityAttribute)
        {
            Check.NotNull(methodInfo, nameof(methodInfo));

            //Method declaration
            // 检查方法声明上的OTelActivityAttribute
            var attrs = methodInfo.GetCustomAttributes(true).OfType<OTelActivityAttribute>().ToArray();
            if (attrs.Any())
            {
                oTelActivityAttribute = attrs.First();
                return !oTelActivityAttribute.IsDisabled;
            }

            // 如果方法有声明类型，则检查类声明上的OTelActivityAttribute
            if (methodInfo.DeclaringType != null)
            {
                //Class declaration
                attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<OTelActivityAttribute>().ToArray();
                if (attrs.Any())
                {
                    oTelActivityAttribute = attrs.First();
                    return !oTelActivityAttribute.IsDisabled;
                }

                //Conventional classes
                if (typeof(IOTelActivityEnabled).GetTypeInfo().IsAssignableFrom(methodInfo.DeclaringType))
                {
                    oTelActivityAttribute = null;
                    return true;
                }
            }

            oTelActivityAttribute = null;
            return false;
        }

        private static bool AnyMethodHasOTelActivityAttribute(TypeInfo implementationType)
        {
            if (implementationType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(HasOTelActivityAttribute))
            {
                var methods = implementationType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(HasOTelActivityAttribute);
                Console.WriteLine("YYYYYYYYYYYYYYY=====methods " + string.Join(" , ", methods.Select(m => m.DeclaringType?.Name + "." + m.Name)));
            }
            return implementationType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(HasOTelActivityAttribute);
        }

        private static bool HasOTelActivityAttribute(MemberInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(OTelActivityAttribute), true);
        }
    }
}
