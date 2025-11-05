using System;
using System.Collections.Generic;
using System.Text;

namespace AbpCore.OTel
{
    public class OTelOptions
    {
        public const string Key = "OTelOptions";

        public bool Enabled { get; set; }
    }
}
