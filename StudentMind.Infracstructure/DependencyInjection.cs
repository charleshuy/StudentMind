using Microsoft.Extensions.DependencyInjection;
using StudentMind.Infracstructure.Interfaces;
using StudentMind.Infracstructure.Repo;

namespace StudentMind.Infracstructure
{
    public static class DependencyInjection
    {
        public static void AddRepositoryLayer(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }
    }
}
