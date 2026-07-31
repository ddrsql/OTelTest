using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AbpFramework.OTel.OTel
{
    /// <summary>
    /// 自定义 Json 解析器：忽略 EF 生成的动态代理属性和延迟加载集合，防止序列化触发 EF 报错
    /// </summary>
    public class IgnoreEFDynamicProxiesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            // 1. 过滤 EF 内部代理属性 (例如 EntityKey, EntityState 等)
            if (property.DeclaringType.FullName.StartsWith("System.Data.Entity") ||
                property.DeclaringType.FullName.StartsWith("DynamicProxies"))
            {
                property.ShouldSerialize = _ => false;
                return property;
            }

            return property;
        }
    }
}
