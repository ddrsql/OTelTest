using AbpFramework.OTel.EntityFramework;
using EntityFramework.DynamicFilters;

namespace AbpFramework.OTel.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly OTelDbContext _context;

        public InitialHostDbBuilder(OTelDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new DefaultEditionsCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
        }
    }
}
