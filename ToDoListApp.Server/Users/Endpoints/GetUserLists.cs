using FluentValidation;
using ToDoListApp.Server.Common.Api;
using ToDoListApp.Server.Common.Api.Extensions;
using ToDoListApp.Server.Common.Api.Request;
using ToDoListApp.Server.Data;
using ToDoListApp.Server.Data.Types;

namespace ToDoListApp.Server.Users.Endpoints
{
    public class GetUserLists : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapGet("/{id}/lists", Handle)
            .WithSummary("Get a user's todo lists")
            .WithRequestValidation<Request>()
            .WithEnsureUserClaims<Request>(x => x.Id)
            .WithEnsureEntityExists<User, Request>(x => x.Id);

        public record Request(Guid Id, int? Page, int? PageSize) : IPagedRequest;
        public class RequestValidator : PagedRequestValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }
        public record Response(Guid Id, string Title, string? Content, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);

        public static async Task<PagedList<Response>> Handle([AsParameters] Request request, ApplicationDbContext database, CancellationToken cancellationToken)
        {
            return await database.Users
                .Where(u => u.ReferenceId == request.Id)
                .SelectMany(u => u.Todolists)
                .OrderByDescending(l => l.CreatedAtUtc)
                .Select(x => new Response
                (
                    x.ReferenceId,
                    x.Title,
                    x.Content,
                    x.CreatedAtUtc,
                    x.UpdatedAtUtc
                ))
                .ToPagedListAsync(request, cancellationToken);
        }
    }
}