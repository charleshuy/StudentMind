using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StudentMind.Core.Base;
using StudentMind.Services.Interfaces;
using StudentMind.Services.Services;
using System.Security.Claims;
using System.Text;

namespace StudentMind.Services
{
    public static class DependencyInjection
    {
        public static void AddServiceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(configuration);
            services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();
            services.AddScoped<IUserService, UserService>();
        }

        private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                string adminSdkRelativePath = configuration["Firebase:AdminSDKPath"]
                    ?? throw new BaseException.CoreException("config", "Missing Firebase:AdminSDKPath");

                string adminSdkFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, adminSdkRelativePath);

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(adminSdkFullPath)
                });
            }

            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]
                ?? throw new BaseException.CoreException("config", "Missing Jwt:Key")));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Firebase Authentication
            .AddJwtBearer("Firebase", options =>
            {
                options.Authority = $"https://securetoken.google.com/{configuration["Firebase:ProjectId"]}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{configuration["Firebase:ProjectId"]}",
                    ValidateAudience = true,
                    ValidAudience = configuration["Firebase:ProjectId"],
                    ValidateLifetime = true
                };
            })
            // Custom JWT Authentication
            .AddJwtBearer("Jwt", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // No Issuer validation
                    ValidateAudience = false, // No Audience validation
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtKey,
                    RoleClaimType = ClaimTypes.Role
                };
            });
        }
    }

}
