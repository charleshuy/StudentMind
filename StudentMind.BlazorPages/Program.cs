using Microsoft.EntityFrameworkCore;
using StudentMind.BlazorPages.Components;
using StudentMind.Infracstructure;
using StudentMind.Infrastructure.Context;

namespace StudentMind.BlazorPages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddDbContextFactory<DatabaseContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("StudentMindDb"));
            });
            builder.Services.AddRepositoryLayer();
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

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();

        }
    }
}
