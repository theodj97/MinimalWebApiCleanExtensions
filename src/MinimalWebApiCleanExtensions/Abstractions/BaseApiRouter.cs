using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalWebApiCleanExtensions.Interfaces;

namespace MinimalWebApiCleanExtensions.Abstractions;

public abstract class BaseApiRouter : IGroupMap
{
    protected readonly IHttpContextAccessor contextAccessor;
    public string UserName => contextAccessor.HttpContext!.User.Claims.First(x => x.Type == ClaimTypes.Name).Value;
    public string UserEmail => contextAccessor.HttpContext!.User.Claims.First(x => x.Type == ClaimTypes.Email).Value;
    public string UserRole => contextAccessor.HttpContext!.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;
    public required string RouteName { get; init; }
    protected const string ROUTE_TERMINATION_NAME = "Routes";

    public BaseApiRouter(IHttpContextAccessor contextAccessor)
    {
        this.contextAccessor = contextAccessor;
        var className = GetType().Name;

        RouteName = className.EndsWith(ROUTE_TERMINATION_NAME)
            ? className[..^ROUTE_TERMINATION_NAME.Length]
            : throw new ArgumentException($"Route name doesn't match syntax finishing in {ROUTE_TERMINATION_NAME}");
    }

    /// <summary>
    /// Maps the API routes for the group.
    /// </summary>
    /// <param name="app"></param>
    public abstract void MapGroup(IEndpointRouteBuilder app);

    /// <summary>
    /// Create a group of api routes with authorization, and fluentValidationFilter.
    /// </summary>
    /// <param name="authPolicy"></param>
    /// <param name="addOpenApiMetadata"></param>
    /// <param name="openApiParameters"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    internal RouteGroupBuilder CreateAuthorizedRouteGroupBuilder(IEndpointRouteBuilder app,
                                                                 string[]? authPolicy = null,
                                                                 bool addOpenApiMetadata = true,
                                                                 OpenApiParameter[]? openApiParameters = null)
    {
        RouteGroupBuilder routeGroupBuilder = CreateBaseRouteGroupBuilder(app, addOpenApiMetadata, openApiParameters);

        if (addOpenApiMetadata is true)
            routeGroupBuilder.ProducesProblem((int)HttpStatusCode.Unauthorized);

        if (authPolicy is not null && authPolicy.Length != 0)
            routeGroupBuilder.RequireAuthorization(authPolicy);
        else
            routeGroupBuilder.RequireAuthorization();

        return routeGroupBuilder;
    }

    /// <summary>
    /// Create a group of api routes with fluentValidationFilter.
    /// </summary>
    /// <param name="authPolicy"></param>
    /// <param name="addOpenApiMetadata"></param>
    /// <param name="openApiParameters"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    internal RouteGroupBuilder CreateBaseRouteGroupBuilder(IEndpointRouteBuilder app,
                                                           bool addOpenApiMetadata = true,
                                                           OpenApiParameter[]? openApiParameters = null)
    {
        RouteGroupBuilder routeGroupBuilder = app.MapGroup($"/api/{RouteName}")
            .WithTags(RouteName);

        if (addOpenApiMetadata is false)
            routeGroupBuilder.WithMetadata(new ExcludeFromDescriptionAttribute());
        else
        {
            routeGroupBuilder.ProducesProblem((int)HttpStatusCode.InternalServerError);

            if (addOpenApiMetadata && openApiParameters is not null && openApiParameters.Length != 0)
            {
                routeGroupBuilder.WithOpenApi(operation =>
                {
                    foreach (var parameter in openApiParameters)
                        operation.Parameters.Add(parameter);
                    return operation;
                });
            }
        }

        return routeGroupBuilder;
    }
}
