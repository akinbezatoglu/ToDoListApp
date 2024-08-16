﻿using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.Server.Authentication.Services;
using ToDoListApp.Server.Common.Api;
using ToDoListApp.Server.Common.Api.Extensions;
using ToDoListApp.Server.Data;

namespace ToDoListApp.Server.Authentication.Endpoints
{
    public class Login : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPost("/login", Handle)
            .WithSummary("Logs in a user")
            .WithRequestValidation<Request>();

        public record Request(string Username, string Password);
        public record Response(string Token);
        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(Request request, ApplicationDbContext database, Jwt jwt, CancellationToken cancellationToken)
        {
            var user = await database.Users.SingleOrDefaultAsync(x => x.Username == request.Username && x.Password == request.Password, cancellationToken);

            if (user is null || user.Password != request.Password)
            {
                return TypedResults.Unauthorized();
            }

            var token = jwt.GenerateToken(user);
            var response = new Response(token);
            return TypedResults.Ok(response);
        }

    }
}