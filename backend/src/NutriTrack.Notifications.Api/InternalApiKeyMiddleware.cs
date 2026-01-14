namespace NutriTrack.Notifications.Api;

public sealed class InternalApiKeyMiddleware : IMiddleware
{
    private const string HeaderName = "X-Internal-Api-Key";
    private readonly string _apiKey;

    public InternalApiKeyMiddleware(IConfiguration config)
    {
        _apiKey = config["InternalAuth:ApiKey"] ?? "";
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Path.StartsWithSegments("/internal"))
        {
            await next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Internal API key is not configured.");
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var provided) ||
            !string.Equals(provided.ToString(), _apiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized.");
            return;
        }

        await next(context);
    }
}