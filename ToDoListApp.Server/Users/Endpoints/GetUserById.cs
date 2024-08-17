using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;
using System.Security.Claims;
using ToDoListApp.Server.Authentication;
using ToDoListApp.Server.Common.Api;
using ToDoListApp.Server.Common.Api.Extensions;
using ToDoListApp.Server.Data;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Users.Endpoints
{
    public class GetUserById : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapGet("/{id}", Handle)
            .WithSummary("Gets a user by id")
            .WithRequestValidation<Request>()
            .WithEnsureUserClaims<Request>(x => x.Id)
            .WithEnsureEntityExists<User, Request>(x => x.Id);

        public record Request(Guid Id);
        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }
        public record Response(
            Guid Id,
            string Username,
            string Password,
            string Displayname,
            DateTime CreatedAtUtc,
            int TodolistsCount
        );

        private static async Task<Results<Ok<Response>, NotFound>> Handle([AsParameters] Request request, ApplicationDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
        {
            var user = await database.Users
                .Where(x => x.ReferenceId == request.Id)
                .Select(x => new Response
                (
                    x.ReferenceId,
                    x.Email,
                    x.Password,
                    x.Fullname,
                    x.CreatedAtUtc,
                    x.Todolists.Count
                ))
                .SingleOrDefaultAsync(cancellationToken);

            return user is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(user);
        }
    }
}
