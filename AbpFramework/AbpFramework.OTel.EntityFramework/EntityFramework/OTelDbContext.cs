using Abp.DynamicEntityProperties;
using Abp.Zero.EntityFramework;
using AbpFramework.OTel.Authorization.Roles;
using AbpFramework.OTel.Authorization.Users;
using AbpFramework.OTel.MultiTenancy;
using AbpFramework.OTel.Tasks;
using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data.Entity;

namespace AbpFramework.OTel.EntityFramework
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class OTelDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...
        public DbSet<Task> Tasks { get; set; }

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public OTelDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in OTelDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of OTelDbContext since ABP automatically handles it.
         */
        public OTelDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public OTelDbContext(DbConnection existingConnection)
         : base(existingConnection, false)
        {

        }

        public OTelDbContext(DbConnection existingConnection, bool contextOwnsConnection)
         : base(existingConnection, contextOwnsConnection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            System.Data.Entity.Infrastructure.Interception.DbInterception.Add(new TaggedTraceidCommandInterceptor());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DynamicProperty>().Property(p => p.PropertyName).HasMaxLength(250);
            modelBuilder.Entity<DynamicEntityProperty>().Property(p => p.EntityFullName).HasMaxLength(250);
        }
    }
}
