using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Claims;
using ToDoListApp.Server.Common.Api;
using ToDoListApp.Server.Common.Api.Extensions;
using ToDoListApp.Server.Data;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Lists.Endpoints
{
    public class UpdateList : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPut("/", Handle)
            .WithSummary("Updates a todo list")
            .WithRequestValidation<Request>()
            .WithEnsureUserOwnsEntity<TodoList, Request>(x => x.Id);

        public record Request(int Id, string Title, string? Content);
        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Id).GreaterThan(0);
                RuleFor(x => x.Title)
                    .NotEmpty()
                    .MaximumLength(100);
            }
        }

        private static async Task<Ok> Handle(Request request, ApplicationDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
        {
            var post = await database.Todolists.SingleAsync(x => x.Id == request.Id, cancellationToken);
            post.Title = request.Title;
            post.Content = request.Content;
            post.UpdatedAtUtc = DateTime.UtcNow;
            await database.SaveChangesAsync(cancellationToken);

            // TODO: Publish post updated event

            return TypedResults.Ok();
        }
    }
}