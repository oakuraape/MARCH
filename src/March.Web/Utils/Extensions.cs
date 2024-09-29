﻿

namespace March.Web.Utils;

public static class Extensions
{
    public static IEndpointRouteBuilder AddPublicFeature(this RouteGroupBuilder routeBuilder) =>  routeBuilder
        .AllowAnonymous();

    public static IEndpointRouteBuilder AddProtectedFeature(this RouteGroupBuilder routeBuilder) => routeBuilder
        .RequireAuthorization();

    public static RouteHandlerBuilder WithValidation<TValidator>(this RouteHandlerBuilder routeBuilder) where TValidator : IEndpointFilter
    {
        return routeBuilder.AddEndpointFilter<TValidator>();
    }

    public static RouteHandlerBuilder WithFeatureFlags(this RouteHandlerBuilder routeBuilder, params FeatureFlag[] featureFlags)
    {
        return routeBuilder.AddEndpointFilter(async (invocationContext, next) =>
        {
            var featureFlagService = invocationContext.HttpContext.RequestServices.GetRequiredService<FeatureFlagService>();
            var logger = invocationContext.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

            foreach (var featureFlag in featureFlags)
            {
                if (featureFlagService.IsFeatureDisabled(featureFlag))
                {
                    logger.LogWarning("{Feature} feature is disabled", featureFlag);
                    return Results.Problem($"{featureFlag} feature is disabled");
                }
            }

            return await next(invocationContext);
        });
    }

    public static RouteHandlerBuilder WithRoutePath(
        this IEndpointRouteBuilder routeBuilder,
        HTTP httpMethod,
        string routePath,
        Delegate endpointHandler)
    {
        return httpMethod switch
        {
            HTTP.GET => routeBuilder.MapGet(routePath, endpointHandler),
            HTTP.POST => routeBuilder.MapPost(routePath, endpointHandler),
            HTTP.PUT => routeBuilder.MapPut(routePath, endpointHandler),
            HTTP.DELETE => routeBuilder.MapDelete(routePath, endpointHandler),
            _ => throw new NotSupportedException($"Http method '{httpMethod}' is not supported")
        };
    }

    public static RazorComponentResult Component<TComponent, TModel>(TModel? model = default) where TComponent : IComponent
    {
        return model is null
            ? new RazorComponentResult<TComponent>()
            : new RazorComponentResult<TComponent>(new { Model = model });
    }

    public static RazorComponentResult Component<TComponent>() where TComponent : IComponent
    {
        return new RazorComponentResult<TComponent>();
    }
}
