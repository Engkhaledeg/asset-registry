using Microsoft.OpenApi.Models;

namespace AssetRegistry.Api.Configuration;

public static class OpenApiConfiguration
{
    private const string SecuritySchemeName = "EntraId";

    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = EntraIdSettings.From(configuration);

        return services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Asset Registry API", Version = "v1" });

            options.AddSecurityDefinition(SecuritySchemeName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = settings.AuthorizationUrl,
                        TokenUrl = settings.TokenUrl,
                        Scopes = new Dictionary<string, string>
                        {
                            [settings.ApiScope] = "Read and write assets"
                        }
                    }
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = SecuritySchemeName }
                }] = new[] { settings.ApiScope }
            });
        });
    }

    public static IApplicationBuilder UseOpenApiDocumentation(this WebApplication app)
    {
        var settings = EntraIdSettings.From(app.Configuration);

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.OAuthClientId(settings.SwaggerClientId);
            options.OAuthScopes(settings.ApiScope);
            options.OAuthUsePkce();
        });

        return app;
    }

    private sealed record EntraIdSettings(
        Uri AuthorizationUrl,
        Uri TokenUrl,
        string ApiScope,
        string SwaggerClientId)
    {
        public static EntraIdSettings From(IConfiguration configuration)
        {
            var tenantId = configuration["AzureAd:TenantId"];
            var clientId = configuration["AzureAd:ClientId"];
            var scopeName = configuration["AzureAd:Scopes"] ?? "access_as_user";

            return new EntraIdSettings(
                new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),
                new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),
                $"api://{clientId}/{scopeName}",
                configuration["Swagger:ClientId"] ?? clientId ?? string.Empty);
        }
    }
}
