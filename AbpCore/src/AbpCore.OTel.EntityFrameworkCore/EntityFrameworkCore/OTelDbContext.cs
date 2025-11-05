using Abp.Zero.EntityFrameworkCore;
using AbpCore.OTel.Authorization.Roles;
using AbpCore.OTel.Authorization.Users;
using AbpCore.OTel.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace AbpCore.OTel.EntityFrameworkCore;

public class OTelDbContext : AbpZeroDbContext<Tenant, Role, User, OTelDbContext>
{
    /* Define a DbSet for each entity of the application */

    public OTelDbContext(DbContextOptions<OTelDbContext> options)
        : base(options)
    {
    }
}
