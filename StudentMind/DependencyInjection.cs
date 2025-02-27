using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentMind.Infracstructure;
using StudentMind.Infrastructure.Context;
using System.Text;

namespace StudentMind
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigRoute();
            services.AddDatabase(configuration);
            services.AddAuthenticationAndAuthorization(configuration);
            services.AddRepositoryLayer();
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

        public static void AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // Set to true in production
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,  // Removed Issuer validation
                        ValidateAudience = false, // Removed Audience validation
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero // Optional: Adjust for token expiry precision
                    };
                });

            // Add role-based authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
            });

            //// Apply authentication globally to all controllers
            //services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //        .RequireAuthenticatedUser()
            //        .Build();
            //    options.Filters.Add(new AuthorizeFilter(policy));
            //});

        }

        public static void AddServices(this IServiceCollection services)
        {
            // Add additional services here
        }

        public static void AddMapping(this IServiceCollection services)
        {
            // Register AutoMapper with all profiles in a single call
        }
    }
}
