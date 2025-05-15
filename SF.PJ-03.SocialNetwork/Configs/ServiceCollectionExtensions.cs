using SF.PJ_03.SocialNetwork.Data.Repository;

namespace SF.PJ_03.SocialNetwork.Configs
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomRepository<TEntity, TRepository>(this IServiceCollection services)
            where TEntity : class
            where TRepository : class, IRepository<TEntity>
        {
            services.AddScoped<IRepository<TEntity>, TRepository>();

            return services;
        }

        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            return services;
        }
    }
}
