using SF.PJ_03.SocialNetwork.Data.Repository;

namespace SF.PJ_03.SocialNetwork.Data.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChahges(bool ensureAutoHistory = false);
        IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class;
    }
}
