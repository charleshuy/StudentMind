using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentMind.Core.Interfaces;
using StudentMind.Infracstructure.Repo;
using StudentMind.Infrastructure.Context;

namespace StudentMind.Infracstructure
{
    public static class DependencyInjection
    {
        public static void AddRepositoryLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddDbContextFactory<DatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("StudentMindDb"));
            });
        }
    }
}
