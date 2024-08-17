using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.Server.Authentication.Services;
using ToDoListApp.Server.Authentication.Services.PasswordHasher;
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

        public record Request(string Email, string Password, string Name);
        public record Response(string Token);
        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .MaximumLength(250);

                RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(250);

                RuleFor(x => x.Password)
                    .NotEmpty()
                    .MaximumLength(128);
                
            }
        }

        private static async Task<Results<Ok<Response>, ValidationError>> Handle(IPasswordHasher passwordHasher, Request request, ApplicationDbContext database, Jwt jwt, CancellationToken cancellationToken)
        {
            var isEmailExist = await database.Users
                .AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (isEmailExist)
            {
                return new ValidationError("Email is already registered");
            }

            var user = new User
            {
                Email = request.Email,
                Password = passwordHasher.Hash(request.Password),
                Fullname = request.Name
            };

            await database.Users.AddAsync(user, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            var token = jwt.GenerateToken(user);
            var response = new Response(token);
            return TypedResults.Ok(response);
        }
    }
}
