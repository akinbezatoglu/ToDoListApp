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
            string Title,
            string? Content,
            DateTime CreateAtUtc,
            DateTime? UpdatedAtUtc
        );

        private static async Task<Results<Ok<Response>, NotFound>> Handle([AsParameters] Request request, ApplicationDbContext database, CancellationToken cancellationToken)
        {
            var list = await database.Todolists
                .Where(x => x.ReferenceId == request.Id)
                .Select(x => new Response
                (
                    x.ReferenceId,
                    x.Title,
                    x.Content,
                    x.CreatedAtUtc,
                    x.UpdatedAtUtc
                ))
                .SingleOrDefaultAsync(cancellationToken);

            return list is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(list);
        }
    }
}
