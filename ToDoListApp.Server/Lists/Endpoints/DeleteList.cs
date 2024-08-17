using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoListApp.Server.Common.Api;
using ToDoListApp.Server.Common.Api.Extensions;
using ToDoListApp.Server.Data;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Lists.Endpoints
{
    public class DeleteList : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapDelete("/{id}", Handle)
            .WithSummary("Deletes a todo list")
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

        private static async Task<Results<Ok, NotFound>> Handle([AsParameters] Request request, ApplicationDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
        {
            var rowsDeleted = await database.Todolists
                .Where(x => x.ReferenceId == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            return rowsDeleted == 1
                ? TypedResults.Ok()
                : TypedResults.NotFound();
        }
    }
}