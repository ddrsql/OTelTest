using Abp.MultiTenancy;
using AbpCore.OTel.Authorization.Users;

namespace AbpCore.OTel.MultiTenancy;

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
