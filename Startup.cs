/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MovieWatching.Database;
using MovieWatching.Models;

namespace MovieWatching;

public class Startup(IConfiguration configuration, IHostEnvironment hostingEnvironment)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseSqlite(configuration["Database:ConnectionString"]);
            options.EnableSensitiveDataLogging();
        });
        
        services.AddRouting();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })

        .AddCookie(options =>
        {
            options.LoginPath = "/signin";
            options.LogoutPath = "/signout";
        })
        .AddDiscord(options =>
        {
            options.ClientId = configuration["Discord:ClientId"] ?? string.Empty;
            options.ClientSecret = configuration["Discord:ClientSecret"] ?? string.Empty;
            options.Scope.Add("identify");
        });

        services.AddMvc();

        //services.AddScoped<MainModel>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        });
        
        app.Use((context, next) =>
        {
            context.Request.Scheme = "https";
            return next(context);
        });
        
        // Required to serve files with no extension in the .well-known folder
        var options = new StaticFileOptions()
        {
            ServeUnknownFileTypes = true,
        };

        app.UseStaticFiles(options);

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}
