using StudentMind.Infracstructure;
using StudentMind.Services;

namespace StudentMind.Razor
{
    public static class DependencyInjection
    {

        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigRoute();
            services.AddRepositoryLayer(configuration);
            services.AddServiceLayer(configuration);
            services.AddHttpContextAccessor();
            services.AddSignalR();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;


            });
        }
        public static void ConfigRoute(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
        }
    }
}
