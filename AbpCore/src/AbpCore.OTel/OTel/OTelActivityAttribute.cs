using System;

namespace AbpCore.OTel
{
    /// <summary>
    /// Activity (Span) 
    /// Span 表示跟踪中的单个操作。Span 可以嵌套，形成跟踪树。
    /// 每个跟踪包含一个根 Span，通常描述整个操作，并且（可选）还包含一个或多个用于描述其子操作的子 Span。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
    public class OTelActivityAttribute : Attribute
    {
        public OTelActivityAttribute()
        {
        }

        public OTelActivityAttribute(bool isDisabled)
        {
            IsDisabled = isDisabled;
        }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled { get; set; }
    }
}
