using System;
using System.Collections.Generic;
using System.Text;

namespace VoloAbp.OTel
{
    /// <summary>
    /// OTel配置
    /// </summary>
    public class OTelOptions
    {
        public const string Key = "OTelOptions";

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 采样率 0.1 - 1.0
        /// </summary>
        public double RatioSampler { get; set; }

        /// <summary>
        /// 是否记录 SQL 参数到 Span Tag
        /// </summary>
        public bool RecordSqlParameter { get; set; }

        /// <summary>
        /// 是否记录方法参数到 Span Tag
        /// </summary>
        public bool RecordMethodParam { get; set; }

        /// <summary>
        /// 忽略path
        /// </summary>
        public List<string> IgnorePaths { get; set; }
    }
}
