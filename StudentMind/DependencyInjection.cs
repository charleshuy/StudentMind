using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentMind.Infracstructure;
using StudentMind.Infrastructure.Context;
using System.Security.Claims;
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
                        ValidateIssuer = false,  // No need to validate issuer
                        ValidateAudience = false, // No need to validate audience
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero // Reduce token expiration delays
                    };

                    // Extract JWT from cookies if not in headers
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.ContainsKey("AuthToken"))
                            {
                                context.Token = context.Request.Cookies["AuthToken"];
                            }
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                            if (claimsIdentity == null) return Task.CompletedTask;

                            // Ensure ASP.NET Core recognizes "role" claim as ClaimTypes.Role
                            var roleClaim = claimsIdentity.FindFirst("role");
                            if (roleClaim != null)
                            {
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            // ✅ Use role-based authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
            });
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
