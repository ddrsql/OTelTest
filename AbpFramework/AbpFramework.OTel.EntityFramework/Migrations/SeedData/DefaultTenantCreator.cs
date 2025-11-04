using System.Linq;
using AbpFramework.OTel.EntityFramework;
using AbpFramework.OTel.MultiTenancy;

namespace AbpFramework.OTel.Migrations.SeedData
{
    public class DefaultTenantCreator
    {
        private readonly OTelDbContext _context;

        public DefaultTenantCreator(OTelDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateUserAndRoles();
        }

        private void CreateUserAndRoles()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                _context.Tenants.Add(new Tenant {TenancyName = Tenant.DefaultTenantName, Name = Tenant.DefaultTenantName});
                _context.SaveChanges();
            }
        }
    }
}
