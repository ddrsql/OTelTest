using Abp.MultiTenancy;
using AbpCore.OTel.Editions;
using AbpCore.OTel.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AbpCore.OTel.EntityFrameworkCore.Seed.Tenants;

public class DefaultTenantBuilder
{
    private readonly OTelDbContext _context;

    public DefaultTenantBuilder(OTelDbContext context)
    {
        _context = context;
    }

    public void Create()
    {
        CreateDefaultTenant();
    }

    private void CreateDefaultTenant()
    {
        // Default tenant

        var defaultTenant = _context.Tenants.IgnoreQueryFilters().FirstOrDefault(t => t.TenancyName == AbpTenantBase.DefaultTenantName);
        if (defaultTenant == null)
        {
            defaultTenant = new Tenant(AbpTenantBase.DefaultTenantName, AbpTenantBase.DefaultTenantName);

            var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                defaultTenant.EditionId = defaultEdition.Id;
            }

            _context.Tenants.Add(defaultTenant);
            _context.SaveChanges();
        }
    }
}
