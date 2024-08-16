using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.Server.Common.Api;
using ToDoListApp.Server.Common.Api.Extensions;
using ToDoListApp.Server.Data;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Lists.Endpoints
{
    public class GetListById : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapGet("/{id}", Handle)
            .WithSummary("Gets a todo list by id")
            .WithRequestValidation<Request>()
            .WithEnsureUserOwnsEntity<TodoList, Request>(x => x.Id);

        public record Request(int Id);
        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }
        }
        public record Response(
            int Id,
            string Title,
            string? Content,
            int UserId,
            string Username,
            string Displayname,
            DateTime CreateAtUtc,
            DateTime? UpdatedAtUtc
        );

        private static async Task<Results<Ok<Response>, NotFound>> Handle([AsParameters] Request request, ApplicationDbContext database, CancellationToken cancellationToken)
        {
            var post = await database.Todolists
                .Where(x => x.Id == request.Id)
                .Select(x => new Response
                (
                    x.Id,
                    x.Title,
                    x.Content,
                    x.UserId,
                    x.User.Username,
                    x.User.Displayname,
                    x.CreatedAtUtc,
                    x.UpdatedAtUtc
                ))
                .SingleOrDefaultAsync(cancellationToken);

            return post is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(post);
        }
    }
}
