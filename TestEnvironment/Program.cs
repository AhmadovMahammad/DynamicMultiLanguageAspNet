using MultiLanguageProvider.AppCode.Providers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TestEnvironment.Models.DataContext;

internal class Program
{
    public static string[]? principals { get; private set; }
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Add services to the container (to All controllers)
        builder.Services.AddControllersWithViews(cfg =>
        {
            //It is used for adding authentication for all controllers
            AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
            cfg.Filters.Add(new AuthorizeFilter(policy));
        });

        //Configure lowercase routing
        builder.Services.AddRouting(cfg => cfg.LowercaseUrls = true);

        //Configure Sql connection
        builder.Services.AddDbContext<LanguageProviderDbContext>(cfg =>
        {
            cfg.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        }, ServiceLifetime.Scoped);

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();

        //Configure
        builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        //Add mediatR
        builder.Services.AddMediatR(Assembly.GetCallingAssembly());

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseRequestLocalization(cfg =>
        {
            cfg.AddSupportedCultures(new[] { "az", "en" });
            cfg.AddSupportedUICultures(new[] { "az", "en" });

            cfg.RequestCultureProviders.Clear();
            cfg.RequestCultureProviders.Add(new CultureProvider());
        });

        app.UseAuthentication();
        app.UseAuthorization();

        //Configure endpoints, for users or other roles
        app.UseEndpoints(endpoints =>
        {
            #region Admin Area
            endpoints.MapControllerRoute(
                name: "Areas-with-language",
                pattern: "{lang}/{area:exists}/{controller=Dashboard}/{action=index}/{id?}",
                constraints: new
                {
                    lang = "az|en"
                });

            endpoints.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
              );
            #endregion

            endpoints.MapControllerRoute(
                     name: "default-with-language",
                     pattern: "{lang}/{controller=home}/{action=index}/{id?}",
                     constraints: new
                     {
                         lang = "az|en"
                     });

            endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=home}/{action=index}/{id?}"
                  );
        });

        app.Run();
    }
}