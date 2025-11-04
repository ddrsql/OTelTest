using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using AbpFramework.OTel.EntityFramework;

namespace AbpFramework.OTel.Migrator
{
    [DependsOn(typeof(OTelDataModule))]
    public class OTelMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<OTelDbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}