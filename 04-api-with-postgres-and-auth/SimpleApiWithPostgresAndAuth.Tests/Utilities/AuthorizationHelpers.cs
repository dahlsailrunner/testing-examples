using System.Security.Claims;

namespace SimpleApiWithPostgresAndAuth.Tests.Utilities;

internal class AutoAuthorizeMiddleware(RequestDelegate rd)
{
    public async Task Invoke(HttpContext httpContext)
    {
        var identity = new ClaimsIdentity("Bearer");

        identity.AddClaim(new Claim("sub", "9e3163b9-1ae6-4652-9dc6-7898ab7b7a00"));
        identity.AddClaim(new Claim(ClaimTypes.Name, "test-name"));

        identity.AddClaims(GetClaimsBasedOnHttpHeaders(httpContext));

        httpContext.User.AddIdentity(identity);
        await rd.Invoke(httpContext);
    }

    private static IEnumerable<Claim> GetClaimsBasedOnHttpHeaders(HttpContext context)
    {
        const string headerPrefix = "X-Test-";

        var claims = new List<Claim>();

        var claimHeaders = context.Request.Headers.Keys.Where(k => k.StartsWith(headerPrefix));
        foreach (var header in claimHeaders)
        {
            var value = context.Request.Headers[header];
            var claimType = header[headerPrefix.Length..];
            if (!string.IsNullOrEmpty(value))
            {
                claims.Add(new Claim(claimType == "role" ? ClaimTypes.Role : claimType, value!));
            }
        }

        return claims;
    }
}

internal class AutoAuthorizeStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            builder.UseMiddleware<AutoAuthorizeMiddleware>();
            next(builder);
        };
    }
}
