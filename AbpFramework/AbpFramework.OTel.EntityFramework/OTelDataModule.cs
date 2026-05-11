using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFramework;
using AbpFramework.OTel.EntityFramework;

namespace AbpFramework.OTel
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(OTelCoreModule))]
    public class OTelDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<OTelDbContext>());

            Configuration.DefaultNameOrConnectionString = "Default";

            DbInterception.Add(new TaggedTraceidCommandInterceptor());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
