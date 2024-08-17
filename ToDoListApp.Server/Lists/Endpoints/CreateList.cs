using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoListApp.Server.Authentication;
using ToDoListApp.Server.Common.Api;
using ToDoListApp.Server.Common.Api.Extensions;
using ToDoListApp.Server.Data;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Lists.Endpoints
{
    public class CreateList : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPost("/", Handle)
            .WithSummary("Creates a new todo list")
            .WithRequestValidation<Request>();

        public record Request(string Title, string? Content);
        public record Response(Guid Id);
        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Title)
                    .NotEmpty()
                    .MaximumLength(250);

                RuleFor(x => x.Content)
                    .NotEmpty()
                    .MaximumLength(750);
            }
        }

        private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(Request request, ApplicationDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
        {
            var user = await database.Users
                .Where(u => u.ReferenceId == claimsPrincipal.GetUserReferenceId())
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return TypedResults.Unauthorized();
            }

            var todolist = new TodoList
            {
                Title = request.Title,
                Content = request.Content,
                UserId = user.Id,
            };

            await database.Todolists.AddAsync(todolist, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            var response = new Response(todolist.ReferenceId);
            return TypedResults.Ok(response);
        }
    }
}