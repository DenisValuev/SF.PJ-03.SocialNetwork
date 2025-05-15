using Microsoft.EntityFrameworkCore.Infrastructure;
using SF.PJ_03.SocialNetwork.Data.Repository;

namespace SF.PJ_03.SocialNetwork.Data.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _appContext;

        private Dictionary<Type, object> _reposirories;

        public UnitOfWork(ApplicationDbContext app)
        {
            this._appContext = app;
        }
        public void Dispose()
        {

        }

        public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class
        {
            if (_reposirories == null)
            {
                _reposirories = new Dictionary<Type, object>();
            }

            if (hasCustomRepository)
            {
                var customRepo = _appContext.GetService<IRepository<TEntity>>();
                if (customRepo != null)
                {
                    return customRepo;
                }
            }

            var type = typeof(TEntity);
            if (!_reposirories.ContainsKey(type))
            {
                _reposirories[type] = new Repository<TEntity>(_appContext);
            }

            return (IRepository<TEntity>)_reposirories[type];
        }

        public int SaveChahges(bool ensureAutoHistory = false)
        {
            throw new NotImplementedException();
        }
    }
}
