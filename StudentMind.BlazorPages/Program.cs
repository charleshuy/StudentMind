using Microsoft.EntityFrameworkCore;
using StudentMind.BlazorPages.Components;
using StudentMind.Infracstructure;
using StudentMind.Infrastructure.Context;
using StudentMind.Services;
using System.Text.Json.Serialization;

namespace StudentMind.BlazorPages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ Register HttpClient
            builder.Services.AddHttpClient();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddDbContextFactory<DatabaseContext>(options =>
             {
                 options.UseSqlServer(builder.Configuration.GetConnectionString("StudentMindDb"));
             });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowGoogleAuth", policy =>
                {
                    policy.WithOrigins("https://accounts.google.com")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });



            builder.Services.AddRepositoryLayer(builder.Configuration);
            builder.Services.AddServiceLayer(builder.Configuration);
            builder.Services.AddQuickGridEntityFrameworkAdapter();
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseMigrationsEndPoint();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseCors("AllowGoogleAuth");
            app.Use(async (context, next) =>
            {
                context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin-allow-popups";
                context.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
                await next();
            });

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();

        }
    }
}
