using Abp.MultiTenancy;
using Abp.Zero.EntityFramework;
using AbpFramework.OTel.Migrations.SeedData;
using EntityFramework.DynamicFilters;
using MySql.Data.EntityFramework;
using System.Data.Entity.Migrations;

namespace AbpFramework.OTel.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<OTel.EntityFramework.OTelDbContext>, IMultiTenantSeed
    {
        public AbpTenantBase Tenant { get; set; }

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "OTel";
            
            //SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator());
        }

        protected override void Seed(OTel.EntityFramework.OTelDbContext context)
        {
            context.DisableAllFilters();

            if (Tenant == null)
            {
                //Host seed
                new InitialHostDbBuilder(context).Create();

                //Default tenant seed (in host database).
                new DefaultTenantCreator(context).Create();
                new TenantRoleAndUserBuilder(context, 1).Create();
            }
            else
            {
                //You can add seed for tenant databases and use Tenant property...
            }

            context.SaveChanges();
        }
    }
}
