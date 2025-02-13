using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentMind.Infracstructure.Context;

namespace StudentMind
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigRoute();
            services.AddDatabase(configuration);
            services.AddServices();
            services.AddMapping();
            services.AddHttpContextAccessor();
        }
        public static void ConfigRoute(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("StudentMindDb"));
            });
        }

        public static void AddServices(this IServiceCollection services)
        {
            // Add services here
            
        }
        public static void AddMapping(this IServiceCollection services)
        {
            // Register AutoMapper with all profiles in a single call
            
            
        }
    }
}
