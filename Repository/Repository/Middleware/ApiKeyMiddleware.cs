using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Configuration;
using Repository.Controllers;

namespace Repository.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string apiKeyName = "X-API-Key";
        private readonly ILogger<UsersController> _logger;

        public ApiKeyMiddleware(RequestDelegate next, ILogger<UsersController> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Try to get API key value, check if more than 1 API key is provided
            if (!context.Request.Headers.TryGetValue(apiKeyName, out var extractedApiKey) || extractedApiKey.Count != 1)
            {
                _logger.LogInformation("No or more than 1 API keys provided.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("No API key or more than 1 API keys provided.");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

            var apiKey = appSettings.GetValue<string>(apiKeyName);

            // Check if API key value matches the one in the appsettings.json
            if (!apiKey.Equals(extractedApiKey))
            {
                _logger.LogInformation("Unauthorized client.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await _next(context);
        }
    }
}
