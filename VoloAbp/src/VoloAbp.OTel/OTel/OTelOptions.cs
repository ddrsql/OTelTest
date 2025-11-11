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
        /// OTel上报地址
        /// </summary>
        public string Endpoint { get; set; }
    }
}
