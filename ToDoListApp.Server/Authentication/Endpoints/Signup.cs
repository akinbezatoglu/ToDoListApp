using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.Server.Authentication.Services;
using ToDoListApp.Server.Common.Api;
using ToDoListApp.Server.Common.Api.Extensions;
using ToDoListApp.Server.Common.Api.Results;
using ToDoListApp.Server.Data;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Authentication.Endpoints
{
    public class Signup : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPost("/signup", Handle)
            .WithSummary("Creates a new user account")
            .WithRequestValidation<Request>();

        public record Request(string Username, string Password, string Name);
        public record Response(string Token);
        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Name).NotEmpty();
            }
        }

        private static async Task<Results<Ok<Response>, ValidationError>> Handle(Request request, ApplicationDbContext database, Jwt jwt, CancellationToken cancellationToken)
        {
            var isUsernameTaken = await database.Users
                .AnyAsync(x => x.Username == request.Username, cancellationToken);

            if (isUsernameTaken)
            {
                return new ValidationError("Username is already taken");
            }

            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
                Displayname = request.Name
            };

            await database.Users.AddAsync(user, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            var token = jwt.GenerateToken(user);
            var response = new Response(token);
            return TypedResults.Ok(response);
        }
    }
}
