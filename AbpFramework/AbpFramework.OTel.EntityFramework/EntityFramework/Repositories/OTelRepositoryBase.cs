using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace AbpFramework.OTel.EntityFramework.Repositories
{
    public abstract class OTelRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<OTelDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected OTelRepositoryBase(IDbContextProvider<OTelDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class OTelRepositoryBase<TEntity> : OTelRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected OTelRepositoryBase(IDbContextProvider<OTelDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
