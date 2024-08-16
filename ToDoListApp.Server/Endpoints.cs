using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using ToDoListApp.Server.Authentication;
using ToDoListApp.Server.Authentication.Endpoints;
using ToDoListApp.Server.Common.Api;
using ToDoListApp.Server.Common.Api.Filters;
using ToDoListApp.Server.Lists.Endpoints;
using ToDoListApp.Server.Users.Endpoints;

namespace ToDoListApp.Server
{
    public static class Endpoints
    {
        private static readonly OpenApiSecurityScheme securityScheme = new()
        {
            Type = SecuritySchemeType.Http,
            Name = JwtBearerDefaults.AuthenticationScheme,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Reference = new()
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
        };

        public static void MapEndpoints(this WebApplication app)
        {
            var endpoints = app.MapGroup("")
                .AddEndpointFilter<RequestLoggingFilter>()
                .WithOpenApi();

            endpoints.MapAuthenticationEndpoints();
            endpoints.MapListEndpoints();
            endpoints.MapUserEndpoints();
        }

        private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app.MapGroup("/auth")
                .WithTags("Authentication");

            endpoints.MapPublicGroup()
                .MapEndpoint<Signup>()
                .MapEndpoint<Login>();
        }

        private static void MapListEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app.MapGroup("/lists")
                .WithTags("TodoLists");

            endpoints.MapAuthorizedGroup()
            .MapEndpoint<CreateList>()
            .MapEndpoint<DeleteList>()
            .MapEndpoint<GetListById>()
            .MapEndpoint<UpdateList>();
        }

        private static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app.MapGroup("/users")
                .WithTags("Users");

            endpoints.MapAuthorizedGroup()
                .MapEndpoint<GetUserById>()
                .MapEndpoint<GetUserLists>();
        }

        private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
        {
            return app.MapGroup(prefix ?? string.Empty)
                .AllowAnonymous();
        }

        private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder app, string? prefix = null)
        {
            return app.MapGroup(prefix ?? string.Empty)
                .RequireAuthorization()
                .WithOpenApi(x => new(x)
                {
                    Security = [new() { [securityScheme] = [] }],
                });
        }

        private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
        {
            TEndpoint.Map(app);
            return app;
        }
    }
}
