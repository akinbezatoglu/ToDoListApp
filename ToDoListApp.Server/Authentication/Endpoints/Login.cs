using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.Server.Authentication.Services;
using ToDoListApp.Server.Authentication.Services.PasswordHasher;
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

        public record Request(string Email, string Password);
        public record Response(string Token);
        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(IPasswordHasher passwordHasher, Request request, ApplicationDbContext database, Jwt jwt, CancellationToken cancellationToken)
        {
            var user = await database.Users.SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

            if (user is null || !passwordHasher.Verify(request.Password, user.Password))
            {
                return TypedResults.Unauthorized();
            }

            var token = jwt.GenerateToken(user);
            var response = new Response(token);
            return TypedResults.Ok(response);
        }

    }
}