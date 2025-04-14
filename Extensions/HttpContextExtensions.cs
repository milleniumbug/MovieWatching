/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MovieWatching.Database;

namespace MovieWatching.Extensions;

public static class HttpContextExtensions
{
    public static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        return (from scheme in await schemes.GetAllSchemesAsync()
                where !string.IsNullOrEmpty(scheme.DisplayName)
                select scheme).ToArray();
    }

    public static async Task<bool> IsProviderSupportedAsync(this HttpContext context, string provider)
    {
        ArgumentNullException.ThrowIfNull(context);

        return (from scheme in await context.GetExternalProvidersAsync()
                where string.Equals(scheme.Name, provider, StringComparison.OrdinalIgnoreCase)
                select scheme).Any();
    }
    
    public static async Task<User?> EnsureUser(this HttpContext context, DatabaseContext databaseContext)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(databaseContext);
        
        #if DEBUG
        await Task.Delay(TimeSpan.FromSeconds(1));
        #endif

        if (!context.IsAuthenticated())
        {
            return null;
        }

        string? nameIdentifier = null;
        string? userName = null;

        foreach (var claim in context.User.Claims)
        {
            switch (claim.Type)
            {
                case ClaimTypes.NameIdentifier:
                    nameIdentifier = claim.Value;
                    break;
                case ClaimTypes.Name:
                    userName = claim.Value;
                    break;
            }
        }
        
        if (nameIdentifier == null || !ulong.TryParse(nameIdentifier, out ulong userId))
        {
            return null;
        }
        
        return await databaseContext.EnsureUser(userId, userName!);
    }

    public static bool IsAuthenticated(this HttpContext context)
    {
        return context.User.Identity?.IsAuthenticated ?? false;
    }
}
