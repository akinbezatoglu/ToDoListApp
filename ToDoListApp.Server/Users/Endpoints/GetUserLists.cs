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

        public record Request(int Id, int? Page, int? PageSize) : IPagedRequest;
        public class RequestValidator : PagedRequestValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }
        }
        public record Response(int Id, string Title, string? Content, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);

        public static async Task<PagedList<Response>> Handle([AsParameters] Request request, ApplicationDbContext database, CancellationToken cancellationToken)
        {
            return await database.Todolists
                .Where(x => x.UserId == request.Id)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new Response
                (
                    x.Id,
                    x.Title,
                    x.Content,
                    x.CreatedAtUtc,
                    x.UpdatedAtUtc
                ))
                .ToPagedListAsync(request, cancellationToken);
        }
    }
}