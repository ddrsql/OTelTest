using Abp.MultiTenancy;
using AbpFramework.OTel.Authorization.Users;

namespace AbpFramework.OTel.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {
            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}